using ClassDependencyTracker.Models.DB;

using NUnit.Framework.Internal;

namespace ClassDependencyTracker.Tests;

[TestFixture]
public class DBTests
{
    const string TempFile = @".\Temp.db";

    [TearDown]
    public void TearDown()
    {
        //Would throw if the file had been saved to
        //if (File.Exists(TempFile))
        //    File.Delete(TempFile);
    }

    [Test]
    [TestCase(@".\Test.db")]
    public void ReadClasses(string filePath)
    {
        Assert.That(File.Exists(filePath));

        List<DBClassModel> models = DBClassModel.ReadClasses(filePath);
        Console.WriteLine($"Found {models.Count} classes in DB file {filePath}");
        foreach (DBClassModel model in models)
        {
            Console.WriteLine(model);
        }
    }

    [Test]
    [TestCase(@".\Test.db")]
    public void WriteClasses(string filePath)
    {
        Assert.That(File.Exists(filePath));
        File.Copy(filePath, TempFile, true);

        DBClassModel[] classes =
        [
            new DBClassModel { Name = "More Classes", },
            new DBClassModel { Name = "All The Classes", URL = "www.college.com", Credits = 4},
        ];

        bool res = DBClassModel.SaveClasses(TempFile, classes);
        Assert.That(res, "Failed to save the correct number of classes");

        List<DBClassModel> models = DBClassModel.ReadClasses(TempFile);
        Console.WriteLine($"Found {models.Count} classes in DB file {TempFile}");
        foreach (DBClassModel model in models)
        {
            Console.WriteLine(model);
        }
    }

    [Test]
    [TestCase(@".\Test.db")]
    public void ReadDependencies(string filePath)
    {
        Assert.That(File.Exists(filePath));

        List<DBDependencyModel> models = DBDependencyModel.ReadDependencies(filePath);
        Console.WriteLine($"Found {models.Count} classes in DB file {filePath}");
        foreach (DBDependencyModel model in models)
        {
            Console.WriteLine(model);
        }
    }

    [Test]
    [TestCase(@".\Test.db")]
    public void WriteDependencies(string filePath)
    {
        Assert.That(File.Exists(filePath));
        File.Copy(filePath, TempFile, true);

        DBDependencyModel[] dependencies =
        [
            new DBDependencyModel { SourceID = 1, RequiredID = 2 },
            new DBDependencyModel { SourceID = 1, RequiredID = 3 },
        ];

        bool res = DBDependencyModel.SaveDependencies(TempFile, dependencies);
        Assert.That(res, "Failed to save the correct number of dependencies");

        List<DBDependencyModel> models = DBDependencyModel.ReadDependencies(TempFile);
        Console.WriteLine($"Found {models.Count} dependencies in DB file {TempFile}");
        foreach (DBDependencyModel model in models)
        {
            Console.WriteLine(model);
        }
    }
}
