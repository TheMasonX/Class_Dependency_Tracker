using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using ClassDependencyTracker.Utils.Extensions;

using Microsoft.Data.Sqlite;

using Serilog;

namespace ClassDependencyTracker.Models.DB;

[Flags]
public enum Semester
{
    Unknown = 0x0,
    Spring  = 0x1,
    Summer  = 0x2,
    Fall    = 0x4,
    Any     = 0xF,
}

public class DBClassModel
{
    #region Properties

    public int? ID { get; set; }
    public string Name { get; set; } = "";
    public string? URL { get; set; }
    public int Credits { get; set; }
    public Semester Semester { get; set; }

    #endregion Properties

    public override string ToString()
    {
        string id = ID.HasValue ? ID.Value.ToString() : "NULL";
        string url = !URL.IsNullOrEmpty() ? $"\"{URL}\"" : "NULL";
        return $"({id}, \"{Name}\", {url}, {Credits}, {(int)Semester})";
    }

    #region Static Methods

    public static List<DBClassModel> Read(string filePath)
    {
        List<DBClassModel> classes = [];

        string connectionString = SQLExtensions.GetConnectionString(filePath, true);
        void rowReader(SqliteDataReader reader)
        {
            if (TryReadRow(reader, out DBClassModel? model))
                classes.Add(model);
        }
        SQLExtensions.ExecuteReader(connectionString, "Select * from Classes", rowReader);

        Log.Logger.Information("Read {ClassCount} from {FilePath}", classes.Count, filePath);

        return classes;
    }

    private static bool TryReadRow(SqliteDataReader reader, [NotNullWhen(true)] out DBClassModel? model)
    {
        try
        {
            model = new DBClassModel
            {
                ID = reader.SafeGetInt(0),
                Name = reader.GetString(1),
                URL = reader.SafeGetString(2),
                Credits = reader.SafeGetInt(3, 0).Value,
                Semester = (Semester)reader.SafeGetInt(4, 0).Value,
            };
            return true;
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "DBClassModel.ReadRow() error");
            model = null;
            return false;
        }
    }

    public static bool Save(string filePath, IEnumerable<DBClassModel> classes)
    {
        string connectionString = SQLExtensions.GetConnectionString(filePath);
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Insert into Classes (ID, Name, URL, Credits, Semester) Values");

        sb.AppendJoin(",\n", classes);
        int? changes = SQLExtensions.ExecuteNonQuery(connectionString, sb.ToString());
        Log.Logger.Information("Saved {ClassCount} classes to {FilePath} with {Changes} changes", classes.Count(), filePath, changes);

        return changes == classes.Count();
    }

    #endregion
}
