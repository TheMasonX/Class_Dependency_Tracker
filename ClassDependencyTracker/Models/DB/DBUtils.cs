using System.IO;

using ClassDependencyTracker.Utils.Extensions;

using Microsoft.Data.Sqlite;

using Serilog;

namespace ClassDependencyTracker.Models.DB;

public static class DBUtils
{
    /// <summary>Created using DB Browser for SQLite</summary>
    private const string DBSchema =
        """
        CREATE TABLE IF NOT EXISTS "Classes" (
        	"ID"	INTEGER NOT NULL,
        	"Name"	TEXT NOT NULL,
        	"URL"	TEXT,
        	"Credits"	INTEGER NOT NULL DEFAULT 0,
        	PRIMARY KEY("ID" AUTOINCREMENT)
        );
        CREATE TABLE IF NOT EXISTS "Dependencies" (
        	"ID"	INTEGER NOT NULL UNIQUE,
        	"SourceID"	INTEGER NOT NULL CHECK("SourceID" <> "RequiredID"),
        	"RequiredID"	INTEGER NOT NULL,
        	PRIMARY KEY("ID" AUTOINCREMENT),
        	UNIQUE("SourceID","RequiredID"),
        	FOREIGN KEY("RequiredID") REFERENCES "Classes"("ID"),
        	FOREIGN KEY("SourceID") REFERENCES "Classes"("ID")
        );
        """;

    public static void DeleteDBFile(string filePath)
    {
        if (!File.Exists(filePath))
            return;

        SqliteConnection.ClearAllPools();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        try
        {
            File.Delete(filePath);
            Log.Logger.Information("Deleted existing DB file at {FilePath}", filePath);
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "Couldn't delete existing DB file at {FilePath}", filePath);
        }
    }

    public static void CreateDB(string filePath, bool overwrite = true)
    {
        if (overwrite)
        {
            DeleteDBFile(filePath);
        }

        string connectionString = SQLExtensions.GetConnectionString(filePath);
        int changes = SQLExtensions.ExecuteNonQuery(connectionString, DBSchema);
        Log.Logger.Information("Executed the DBSchema command with {Changes} changes to {FilePath}", changes, filePath);
    }

    public static void SaveToFile(string filePath, ClassModel[] classes)
    {
        //Save the classes to the DB so we can get the DB IDs
        IEnumerable<DBClassModel> dbClasses = classes.Select(x => x.ToDBModel());
        if (!DBClassModel.Save(filePath, dbClasses))
        {
            Log.Logger.Error("Error saving classes to {FilePath}", filePath);
        }

        List <(ClassModel model, DBClassModel dbModel)> pairs = [];
        List<DBClassModel> readDBClasses = DBClassModel.Read(filePath);
        List<DBDependencyModel> dependencyModels = [];
        foreach (ClassModel model in classes)
        {
            IEnumerable<DBDependencyModel> parsedDependencies = model.Requirements.ToArray()
                .Select(x => x.ToDBModel(readDBClasses))
                .Where(x => x is not null)!;
            dependencyModels.AddRange(parsedDependencies);
        }

        if (!DBDependencyModel.Save(filePath, dependencyModels))
        {
            Log.Logger.Error("Error saving dependencies to {FilePath}", filePath);
        }
    }

    public static ClassModel[] LoadFromFile(string filePath)
    {
        List<DBClassModel> dbClasses = DBClassModel.Read(filePath);
        ClassModel[] classes = dbClasses.Select(ClassModel.ParseDBModel).ToArray();
        List<DBDependencyModel> dbDependencies = DBDependencyModel.Read(filePath);
        IEnumerable<DependencyModel> dependencies = dbDependencies.Select(x => DependencyModel.ParseDBModel(x, classes)).Where(x => x is not null)!;


        foreach (DependencyModel dependencyModel in dependencies)
        {
            dependencyModel.SourceClass.AddRequirement(dependencyModel.RequiredClass);
        }

        return classes;
    }
}
