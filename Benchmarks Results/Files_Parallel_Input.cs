using System;
using System.Linq;
using System.Drawing;

class Program
{
    void Main ()
    {
        var Files = Enumerable.Repeat(@".\uv_test.png", 30).ToArray();
        var fileCount = Files.AsParallel().Select(f => Image.FromFile(f)).Count();
    }
}