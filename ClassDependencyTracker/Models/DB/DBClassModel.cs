using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using ClassDependencyTracker.Utils.Extensions;

using Microsoft.Data.Sqlite;

using Serilog;

namespace ClassDependencyTracker.Models.DB;

public class DBClassModel
{
    #region Properties

    public int? ID { get; set; }

    public string Name { get; set; } = "";

    public string? URL { get; set; }

    public int Credits { get; set; }

    #endregion Properties

    public override string ToString()
    {
        string id = ID.HasValue ? ID.Value.ToString() : "NULL";
        string url = !URL.IsNullOrEmpty() ? $"\"{URL}\"" : "NULL";
        return $"({id}, \"{Name}\", {url}, {Credits})";
    }

    #region Static Methods

    public static List<DBClassModel> ReadClasses(string filePath)
    {
        List<DBClassModel> classes = [];

        string connectionString = SQLExtensions.GetConnectionString(filePath, true);
        void rowReader(SqliteDataReader reader)
        {
            if (TryReadRow(reader, out DBClassModel? model))
                classes.Add(model);
        }
        SQLExtensions.ExecuteReader(connectionString, "Select * from Classes", rowReader);

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

    public static bool SaveClasses(string filePath, IEnumerable<DBClassModel> classes)
    {
        string connectionString = SQLExtensions.GetConnectionString(filePath);
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Insert into Classes (ID, Name, URL, Credits) Values");

        //IEnumerable<string> values = classes.Select(x => $"({x.ID.ToString() ?? "NULL"}, {x.Name}, {x.URL ??  "NULL"}, {Credits})");
        sb.AppendJoin(",\n", classes);
        int changes = SQLExtensions.ExecuteNonQuery(connectionString, sb.ToString());
        return changes == classes.Count();
    }

    #endregion
}
