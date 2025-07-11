using System.Drawing;
using System.Text.RegularExpressions;

using BenchmarkDotNet.Attributes;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using VideoGenerator.Utils.Extensions;

namespace VideoGenerator.Benchmarks;

//[SimpleJob(runtimeMoniker: RuntimeMoniker.Net60)]
//[SimpleJob(runtimeMoniker: RuntimeMoniker.Net80)]
[MemoryDiagnoser]
//[TestClass]
public class Benchmarks
{
    string[] Files = [];

    [Params(5, 10, 50, 100)]
    public int FileCount = 30;
    

    [GlobalSetup]
    //[TestInitialize]
    public void Setup()
    {
        Files = Enumerable.Repeat(@".\uv_test.png", FileCount).ToArray();
    }

    //[Benchmark]
    //public void GetImages ()
    //{
    //    var fileCount = Files.Select(f => Image.FromFile(f)).Count();
    //}

    //[Benchmark]
    //public void GetImages_Parallel ()
    //{
    //    var fileCount = Files.AsParallel().Select(f => Image.FromFile(f)).Count();
    //}

    //[Benchmark]
    //public void GetImages_Parallel_Ordered ()
    //{
    //    var fileCount = Files.AsParallel().AsOrdered().Select(f => Image.FromFile(f)).Count();
    //}

    //[Benchmark]
    //public void GetImages_Array ()
    //{
    //    var fileCount = Files.Select(f => Image.FromFile(f)).ToArray();
    //}

    //[Benchmark]
    //public void GetImages_Parallel_Array ()
    //{
    //    var fileCount = Files.AsParallel().Select(f => Image.FromFile(f)).ToArray();
    //}

    //[Benchmark]
    //public void GetImages_Parallel_Ordered_Array ()
    //{
    //    var fileCount = Files.AsParallel().AsOrdered().Select(f => Image.FromFile(f)).ToArray();
    //}

    //[Benchmark]
    //[TestMethod]
    //public void GetSortedImages ()
    //{
    //    var files = Files.Order().Select(f => Image.FromFile(f)).ToArray();
    //}

    //[Benchmark]
    //[TestMethod]
    //public void GetSortedImages_BeforeParallel_Ordered ()
    //{
    //    var files = Files.Order().AsParallel().Select(f => Image.FromFile(f)).ToArray();
    //}

    //[Benchmark]
    //[TestMethod]
    //public void GetSortedImages_AfterParallel_Ordered ()
    //{
    //    var files = Files.AsParallel().Order().Select(f => Image.FromFile(f)).ToArray();
    //}

    #region Regex
    //[Params(5, 10, 50, 100, 1_000, 100_000, 1_000_000)]
    //public int FileCount = 30;

    //[GlobalSetup]
    //public void Setup ()
    //{
    //    //LocalRegex = new(regex);
    //    //LocalCompiledRegex = new(regex, RegexOptions.Compiled);
    //    Files = Enumerable.Repeat(@".\uv_test.png", FileCount).ToArray();
    //}

    //public Regex LocalRegex = new("");
    //public volatile string volatileRegex = @"(\.png)|(\.bmp)|(\.jpg)";
    //public string regex = @"(\.png)|(\.bmp)|(\.jpg)";
    //public Regex LocalCompiledRegex = new("");
    //Static Compiled Method
    //[Benchmark]
    //public void RegexMatchFiles()
    //{
    //    var fileCount = Files.Where(f => RegexEx.GetImageFileRegex().IsMatch(f)).Count();
    //}

    //[Benchmark]
    //public void RegexMatchFiles_Parallel ()
    //{
    //    var fileCount = Files.AsParallel().Where(f => RegexEx.GetImageFileRegex().IsMatch(f)).Count();
    //}


    //Static Compiled Variable
    //[Benchmark]
    //public void RegexMatchFiles ()
    //{
    //    var fileCount = Files.Where(f => RegexEx.ImageFileRegex.IsMatch(f)).Count();
    //}

    //[Benchmark]
    //public void RegexMatchFiles_Parallel ()
    //{
    //    var fileCount = Files.AsParallel().Where(f => RegexEx.ImageFileRegex.IsMatch(f)).Count();
    //}

    //Literal String
    //[Benchmark]
    //public void RegexMatchFiles ()
    //{
    //    var fileCount = Files.Where(f => Regex.IsMatch(f, @"(\.png)|(\.bmp)|(\.jpg)")).Count();
    //}

    //[Benchmark]
    //public void RegexMatchFiles_Parallel ()
    //{
    //    var fileCount = Files.AsParallel().Where(f => Regex.IsMatch(f, @"(\.png)|(\.bmp)|(\.jpg)")).Count();
    //}

    ////Member Variable
    //[Benchmark]
    //public void RegexMatchFiles ()
    //{
    //    var fileCount = Files.Where(f => Regex.IsMatch(f, regex)).Count();
    //}

    //[Benchmark]
    //public void RegexMatchFiles_Parallel ()
    //{
    //    var fileCount = Files.AsParallel().Where(f => Regex.IsMatch(f, regex)).Count();
    //}

    ////Volatile Member Variable
    //[Benchmark]
    //public void RegexMatchFiles ()
    //{
    //    var fileCount = Files.Where(f => Regex.IsMatch(f, volatileRegex)).Count();
    //}

    //[Benchmark]
    //public void RegexMatchFiles_Parallel ()
    //{
    //    var fileCount = Files.AsParallel().Where(f => Regex.IsMatch(f, volatileRegex)).Count();
    //}
    #endregion Regex
}