using System.Runtime.CompilerServices;

namespace Intervals.NET.Parsers;

/// <summary>
/// Provides methods to parse ranges using interpolated string syntax.
/// The interpolated string handler avoids allocating the final string - values are parsed on-the-fly.
/// </summary>
public static class RangeInterpolatedStringParser
{
    /// <summary>
    /// Parses a range from an interpolated string handler. Throws an exception if parsing fails.
    /// The expected format is:
    /// $"{openBracket}{start}, {end}{closeBracket}" or $"[{start}, {end}]"
    /// where brackets are '[' or '(' and ']' or ')'.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the values in the range. Must implement IComparable&lt;T&gt; and ISpanParsable&lt;T&gt;.
    /// </typeparam>
    /// <param name="handler">
    /// The interpolated string handler that processes the range format.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="Range{T}"/> representing the parsed range.
    /// </returns>
    /// <exception cref="FormatException">
    /// Thrown when the interpolated string format is invalid.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Range<T> Parse<T>(
        RangeInterpolatedStringHandler<T> handler
    ) where T : IComparable<T>, ISpanParsable<T> => handler.GetRange();

    /// <summary>
    /// Attempts to parse a range from an interpolated string handler.
    /// The expected format is:
    /// $"{openBracket}{start}, {end}{closeBracket}" or $"[{start}, {end}]"
    /// where brackets are '[' or '(' and ']' or ')'.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the values in the range. Must implement IComparable&lt;T&gt; and ISpanParsable&lt;T&gt;.
    /// </typeparam>
    /// <param name="handler">
    /// The interpolated string handler that processes the range format.
    /// </param>
    /// <param name="range">
    /// When this method returns, contains the parsed range if successful; otherwise, the default value.
    /// </param>
    /// <returns>
    /// <c>true</c> if the range was parsed successfully; otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse<T>(
        RangeInterpolatedStringHandler<T> handler,
        out Range<T> range
    ) where T : IComparable<T>, ISpanParsable<T> => handler.TryGetRange(out range);
}