using System.Diagnostics.CodeAnalysis;

using Microsoft.Data.Sqlite;

using Serilog;
using Serilog.Core;

namespace ClassDependencyTracker.Utils.Extensions;

public static class SQLExtensions
{
    public static string GetConnectionString(string filePath, bool isReadonly = false)
    {
        return new SqliteConnectionStringBuilder()
        {
            DataSource = filePath,
            ForeignKeys = true,
            Mode = isReadonly ? SqliteOpenMode.ReadOnly : SqliteOpenMode.ReadWriteCreate,
        }.ToString();
    }

    public static void ExecuteReader(string connectionString, string commandString, Action<SqliteDataReader> readAction)
    {
        using SqliteConnection connection = new SqliteConnection(connectionString);
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = commandString;

        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            readAction(reader);
        }
    }

    public static int? ExecuteNonQuery(string connectionString, string commandString, Action<SqliteCommand>? commandAction = null)
    {
        using SqliteConnection connection = new SqliteConnection(connectionString);
        connection.Open();

        using SqliteTransaction transaction = connection.BeginTransaction();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = commandString;
        commandAction?.Invoke(command);

        int? changes;
        try
        {
            changes = command.ExecuteNonQuery();
            transaction.Commit();
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "Error executing a non-query on database {DataSource}. Command:\n{Command}", connection.DataSource, commandString);
            changes = null;
        }
        return changes;
    }

    [return: NotNullIfNotNull(nameof(@default))]
    public static T? SafeGet<T>(this SqliteDataReader reader, Func<SqliteDataReader, int, T> func, int ordinal, T? @default = default)
    {
        if (reader.FieldCount <= ordinal)
        {
            Log.Logger.Warning("Trying to read field #{FieldNum} of reader, but it only has {FieldCount} available. Returning default value {Default}", ordinal, reader.FieldCount, @default);
            return @default;
        }
        return reader.IsDBNull(ordinal) ? @default : func(reader, ordinal);
    }

    [return: NotNullIfNotNull(nameof(@default))]
    public static int? SafeGetInt(this SqliteDataReader reader, int ordinal, int? @default = default)
    {
        return SafeGet(reader, (reader, ordinal) => reader.GetInt32(ordinal), ordinal, @default);
    }

    [return: NotNullIfNotNull(nameof(@default))]
    public static string? SafeGetString(this SqliteDataReader reader, int ordinal, string? @default = default)
    {
        return SafeGet(reader, (reader, ordinal) => reader.GetString(ordinal), ordinal, @default);
    }


}
