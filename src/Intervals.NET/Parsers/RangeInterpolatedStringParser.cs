namespace Intervals.NET.Parsers;

/// <summary>
/// Provides methods to parse ranges using interpolated string syntax.
/// The interpolated string handler avoids allocating the final string - values are parsed on-the-fly.
/// </summary>
public static class RangeInterpolatedStringParser
{
    /// <summary>
    /// Parses a range using interpolated string syntax.
    /// Expected format: $"[{start}, {end}]" or $"{bracket}{start}, {end}{bracket}"
    /// The string is never materialized - values are parsed directly from the interpolated string.
    /// </summary>
    /// <param name="handler">The interpolated string handler that performs zero-allocation parsing.</param>
    /// <typeparam name="T">The type of the values in the range.</typeparam>
    /// <returns>A new Range instance.</returns>
    /// <exception cref="FormatException">Thrown if the format is invalid.</exception>
    /// <example>
    /// <code>
    /// // Using literal brackets (recommended - cleanest syntax)
    /// var range1 = RangeInterpolatedStringParser.Parse&lt;int&gt;($"[{10}, {20}]");
    /// var range2 = RangeInterpolatedStringParser.Parse&lt;int&gt;($"({0}, {100})");
    /// var range3 = RangeInterpolatedStringParser.Parse&lt;double&gt;($"[{1.5}, {9.5})");
    /// 
    /// // Using variables
    /// int start = 10;
    /// int end = 20;
    /// var range4 = RangeInterpolatedStringParser.Parse&lt;int&gt;($"[{start}, {end}]");
    /// 
    /// // Using char bracket variables (when you need dynamic brackets)
    /// char openBracket = '[';
    /// char closeBracket = ')';
    /// var range5 = RangeInterpolatedStringParser.Parse($"{openBracket}{start}, {end}{closeBracket}");
    /// 
    /// // Using infinity
    /// var range6 = RangeInterpolatedStringParser.Parse($"[{RangeValue&lt;int&gt;.NegativeInfinity}, {100}]");
    /// 
    /// // NO STRING IS BUILT - values are parsed directly from the interpolation!
    /// </code>
    /// </example>
    public static Range<T> Parse<T>(
        RangeInterpolatedStringHandler<T> handler
    ) where T : IComparable<T>, ISpanParsable<T> => handler.GetRange();

    /// <summary>
    /// Attempts to parse a range using interpolated string syntax.
    /// Expected format: $"[{start}, {end}]" or $"{bracket}{start}, {end}{bracket}"
    /// The string is never materialized - values are parsed directly from the interpolated string.
    /// </summary>
    /// <param name="handler">The interpolated string handler that performs zero-allocation parsing.</param>
    /// <param name="range">The parsed range if successful.</param>
    /// <typeparam name="T">The type of the values in the range.</typeparam>
    /// <returns>True if parsing succeeded; otherwise false.</returns>
    /// <example>
    /// <code>
    /// // Using literal brackets (recommended)
    /// if (RangeInterpolatedStringParser.TryParse($"[{10}, {20}]", out Range&lt;int&gt; range))
    /// {
    ///     Console.WriteLine($"Parsed: {range}");
    /// }
    /// 
    /// // Using variables
    /// int start = 10;
    /// int end = 20;
    /// if (RangeInterpolatedStringParser.TryParse($"[{start}, {end})", out Range&lt;int&gt; range2))
    /// {
    ///     Console.WriteLine($"Parsed: {range2}");
    /// }
    /// 
    /// // Using char bracket variables
    /// char openBracket = '(';
    /// char closeBracket = ']';
    /// if (RangeInterpolatedStringParser.TryParse($"{openBracket}{start}, {end}{closeBracket}", out var range3))
    /// {
    ///     Console.WriteLine($"Parsed: {range3}");
    /// }
    /// 
    /// // NO STRING IS BUILT - values are parsed directly from the interpolation!
    /// </code>
    /// </example>
    public static bool TryParse<T>(
        RangeInterpolatedStringHandler<T> handler,
        out Range<T> range
    ) where T : IComparable<T>, ISpanParsable<T> => handler.TryGetRange(out range);
}