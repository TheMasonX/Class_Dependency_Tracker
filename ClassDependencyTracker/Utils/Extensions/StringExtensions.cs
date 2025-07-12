using System;
using System.Diagnostics.CodeAnalysis;

namespace ClassDependencyTracker.Utils.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrEmpty ([NotNullWhen(false)] this string? value) => string.IsNullOrEmpty(value);
    public static bool IsNullOrWhiteSpace ([NotNullWhen(false)] this string? value) => string.IsNullOrWhiteSpace(value);
    public static int Compare (this string? value, string? other, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase) => string.Compare(value, other, comparison);
    public static bool EqualsInsensitive (this string? value, string? other) => string.Equals(value, other, StringComparison.CurrentCultureIgnoreCase);
}