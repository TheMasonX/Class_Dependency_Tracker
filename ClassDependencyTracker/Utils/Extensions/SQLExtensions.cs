using System.Diagnostics.CodeAnalysis;

using Microsoft.Data.Sqlite;

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

    public static int ExecuteNonQuery(string connectionString, string commandString, Action<SqliteCommand>? commandAction = null)
    {
        using SqliteConnection connection = new SqliteConnection(connectionString);
        connection.Open();

        using SqliteTransaction transaction = connection.BeginTransaction();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = commandString;
        commandAction?.Invoke(command);

        int changes = command.ExecuteNonQuery();
        transaction.Commit();
        return changes;
    }

    [return: NotNullIfNotNull(nameof(@default))]
    public static T? SafeGet<T>(this SqliteDataReader reader, Func<SqliteDataReader, int, T> func, int ordinal, T? @default = default)
    {
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
