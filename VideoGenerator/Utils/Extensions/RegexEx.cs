using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VideoGenerator.Utils.Extensions;

public static partial class RegexEx
{
    [GeneratedRegex(@"(\.png)|(\.bmp)|(\.jpg)")]
    public static partial Regex GetImageFileRegex();
    public static readonly Regex ImageFileRegex = GetImageFileRegex();
}
