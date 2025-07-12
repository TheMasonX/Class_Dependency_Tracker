using ClassDependencyTracker.Models;
using ClassDependencyTracker.Models.DB;
using ClassDependencyTracker.Utils.Extensions;
using ClassDependencyTracker.ViewModels;

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
        DBUtils.DeleteDBFile(TempFile);
    }

    [Test]
    [TestCase(@".\Test.db")]
    public void ReadClasses(string filePath)
    {
        Assert.That(File.Exists(filePath));

        List<DBClassModel> models = DBClassModel.Read(filePath);
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

        bool res = DBClassModel.Save(TempFile, classes);
        Assert.That(res, "Failed to save the correct number of classes");

        List<DBClassModel> models = DBClassModel.Read(TempFile);
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

        List<DBDependencyModel> models = DBDependencyModel.Read(filePath);
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

        bool res = DBDependencyModel.Save(TempFile, dependencies);
        Assert.That(res, "Failed to save the correct number of dependencies");

        List<DBDependencyModel> models = DBDependencyModel.Read(TempFile);
        Console.WriteLine($"Found {models.Count} dependencies in DB file {TempFile}");
        foreach (DBDependencyModel model in models)
        {
            Console.WriteLine(model);
        }
    }

    [Test]
    public void CreateDB()
    {
        DBUtils.CreateDB(TempFile);
        Assert.That(File.Exists(TempFile));
    }

    [Test]
    public void CreateAndSaveToDB()
    {
        DBUtils.CreateDB(TempFile);
        Assert.That(File.Exists(TempFile));

        ClassModel[] classes =
        [
            new ClassModel("Class 1") { URL ="www.college.com", Credits = 2 },
            new ClassModel("Other Class") { Credits = 3 },
            new ClassModel("Hard Class") { Credits = 4 },
        ];

        //TODO: Singleton pattern made this hard to mock
        MainWindowVM mainWindowVM = new MainWindowVM();
        foreach (ClassModel model in classes)
        {
            mainWindowVM.Classes.Add(model);
        }

        classes[1].AddRequirement(classes[0]);
        classes[2].AddRequirement(classes[0]);
        classes[2].AddRequirement(classes[1]);

        DBUtils.TrySaveToFile(TempFile, classes);
        List<DBClassModel> dbClasses = DBClassModel.Read(TempFile);
        List<DBDependencyModel> dbDependencies = DBDependencyModel.Read(TempFile);

        Assert.That(dbClasses.Count, Is.EqualTo(classes.Length));
        Assert.That(dbDependencies.Count, Is.EqualTo(3));
    }
}
