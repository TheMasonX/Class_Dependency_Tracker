using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDependencyTracker.Utils.Extensions;

public static class FileUtils
{
    public const string AllFilesFilter = "All files (*.*)|*.*";
    public const string TextFilter = "Text file (*.txt)|*.txt";
    public const string ImageFilter = "Image files (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png";
    public const string DBFilter = "SQLite Database files (*.db)|*.db";
    public const string SQLFilter = "SQL script files (*.sql)|*.sql";

    public const string TextExtension = ".txt";
    public const string BMPExtension = ".bmp";
    public const string JPGExtension = ".jpg";
    public const string PNGExtension = ".png";
    public const string DBExtension = ".db";
    public const string SQLExtension = ".sql";

    public static string FormatFileSize(int size) => FormatFileSize((ulong)size);

    public static string FormatFileSize(ulong size)
    {
        const double kb = 1024;
        const double mb = 1024 * 1024;
        const double gb = 1024 * 1024 * 1024;

        if (size < kb)
        {
            return $"{size:N0}B";
        }

        if (size < mb)
        {
            return $"{(size / kb):N1}KB";
        }

        if (size < gb)
        {
            return $"{(size / mb):N1}MB";
        }

        return $"{(size / gb):N1}GB";
    }
}
