using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using ClassDependencyTracker.Utils.Extensions;

using Microsoft.Data.Sqlite;

using Serilog;

namespace ClassDependencyTracker.Models.DB;

public class DBDependencyModel
{
    #region Properties

    public int? ID { get; set; }
    public int SourceID { get; set; }
    public int RequiredID { get; set; }

    #endregion Properties

    public override string ToString()
    {
        string id = ID.HasValue ? ID.Value.ToString() : "NULL";
        return $"({id}, {SourceID}, {RequiredID})";
    }

    #region Static Methods

    public static List<DBDependencyModel> Read(string filePath)
    {
        List<DBDependencyModel> dependencies = [];

        string connectionString = SQLExtensions.GetConnectionString(filePath, true);
        void rowReader(SqliteDataReader reader)
        {
            if (TryReadRow(reader, out DBDependencyModel? model))
                dependencies.Add(model);
        }
        SQLExtensions.ExecuteReader(connectionString, "Select * from Dependencies", rowReader);

        return dependencies;
    }

    private static bool TryReadRow(SqliteDataReader reader, [NotNullWhen(true)] out DBDependencyModel? model)
    {
        try
        {
            model = new DBDependencyModel
            {
                ID = reader.SafeGetInt(0),
                SourceID = reader.GetInt32(1),
                RequiredID = reader.GetInt32(2),
            };
            return true;
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "DBDependencyModel.ReadRow() error");
            model = null;
            return false;
        }
    }

    public static bool Save(string filePath, IEnumerable<DBDependencyModel> requirements)
    {
        if (!requirements.Any()) //Valid to save without any requirements
            return true;

        string connectionString = SQLExtensions.GetConnectionString(filePath);
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Insert into Dependencies (ID, SourceID, RequiredID) Values");

        sb.AppendJoin(",\n", requirements);
        int? changes = SQLExtensions.ExecuteNonQuery(connectionString, sb.ToString());
        return changes == requirements.Count();
    }

    #endregion
}
