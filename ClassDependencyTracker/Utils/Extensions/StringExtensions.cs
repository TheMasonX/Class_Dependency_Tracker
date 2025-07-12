using System;
using System.Diagnostics.CodeAnalysis;

namespace ClassDependencyTracker.Utils.Extensions;

public enum StringFilterType
{
    Equals,
    Contains,
    StartsWith,
    EndsWith,
}

public static class StringExtensions
{
    public static bool IsNullOrEmpty ([NotNullWhen(false)] this string? value) => string.IsNullOrEmpty(value);
    public static bool IsNullOrWhiteSpace ([NotNullWhen(false)] this string? value) => string.IsNullOrWhiteSpace(value);
    public static int Compare (this string? value, string? other, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase) => string.Compare(value, other, comparison);
    public static bool EqualsInsensitive (this string? value, string? other) => string.Equals(value, other, StringComparison.CurrentCultureIgnoreCase);

    public static bool ApplyFilter(string input, string filter, StringFilterType filterType, bool caseSensitive, bool @default = false)
    {
        StringComparison sc = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        return filterType switch
        {
            StringFilterType.Equals => input.Equals(filter, sc),
            StringFilterType.Contains => input.Contains(filter, sc),
            StringFilterType.StartsWith => input.StartsWith(filter, sc),
            StringFilterType.EndsWith => input.EndsWith(filter, sc),
            _ => @default,
        };
    }
}