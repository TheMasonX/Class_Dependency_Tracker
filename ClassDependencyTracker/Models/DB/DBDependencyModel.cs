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

    private const string _columns = "ID, SourceID, RequiredID";
    private const string _select = "Select * from Dependencies"; //Use * instead of columns for backwards compatibility
    private const string _insert = $"Insert into Dependencies ({_columns}) Values";

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

        SQLExtensions.ExecuteReader(connectionString, _select, rowReader);
        return dependencies;
    }

    private static bool TryReadRow(SqliteDataReader reader, [NotNullWhen(true)] out DBDependencyModel? model)
    {
        try
        {
            int index = 0;
            model = new DBDependencyModel
            {
                ID = reader.SafeGetInt(index++),
                SourceID = reader.GetInt32(index++),
                RequiredID = reader.GetInt32(index++),
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
        sb.AppendLine(_insert);

        sb.AppendJoin(",\n", requirements);
        int? changes = SQLExtensions.ExecuteNonQuery(connectionString, sb.ToString());
        return changes == requirements.Count();
    }

    #endregion
}
