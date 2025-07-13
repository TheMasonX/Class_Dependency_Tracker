using System;
using System.Collections.Generic;
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
    public int Credits { get; set; }
    public Semester Semester { get; set; }
    public string? Notes { get; set; }

    #endregion Properties

    public override string ToString()
    {
        string id = ID.HasValue ? ID.Value.ToString() : "NULL";
        string notes = !Notes.IsNullOrEmpty() ? $"\"{Notes}\"" : "NULL";
        return $"({id}, \"{Name}\", {Credits}, {(int)Semester}, {notes})";
    }

    private const string _columns = "ID, Name, Credits, Semester, Notes";
    private const string _select = $"Select * from Classes"; //Use * instead of columns for backwards compatibility
    private const string _insert = $"Insert into Classes ({ _columns}) Values";

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
        SQLExtensions.ExecuteReader(connectionString, _select, rowReader);

        Log.Logger.Information("Read {ClassCount} from {FilePath}", classes.Count, filePath);

        return classes;
    }

    private static bool TryReadRow(SqliteDataReader reader, [NotNullWhen(true)] out DBClassModel? model)
    {
        try
        {
            int index = 0;
            model = new DBClassModel
            {
                ID = reader.SafeGetInt(index++),
                Name = reader.GetString(index++),
                Credits = reader.SafeGetInt(index++, 0).Value,
                Semester = (Semester)reader.SafeGetInt(index++, 0).Value,
                Notes = reader.SafeGetString(index++),
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
        if (!classes.Any())
        {
            return true;
        }

        string connectionString = SQLExtensions.GetConnectionString(filePath);
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(_insert);

        sb.AppendJoin(",\n", classes);
        int? changes = SQLExtensions.ExecuteNonQuery(connectionString, sb.ToString());
        Log.Logger.Information("Saved {ClassCount} classes to {FilePath} with {Changes} changes", classes.Count(), filePath, changes);

        return changes == classes.Count();
    }

    #endregion
}
