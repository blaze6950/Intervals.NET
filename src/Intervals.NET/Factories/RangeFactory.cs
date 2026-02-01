using System.Runtime.CompilerServices;
using Intervals.NET.Parsers;

namespace Intervals.NET.Factories;

/// <summary>
/// Provides factory methods for creating ranges with different inclusivity options.
/// </summary>
public static class Range
{
    /// <summary>
    /// Creates a closed range [start, end].
    /// </summary>
    /// <param name="start">
    /// The start value of the range.
    /// Use RangeValue&lt;T&gt;.NegativeInfinity for unbounded start.
    /// </param>
    /// <param name="end">
    /// The end value of the range.
    /// Use RangeValue&lt;T&gt;.PositiveInfinity for unbounded end.
    /// </param>
    /// <typeparam name="T">
    /// The type of the values in the range. Must implement IComparable&lt;T&gt;.
    /// </typeparam>
    /// <returns>
    /// A new instance of <see cref="Range{T}"/> representing the closed range [start, end].
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Range<T> Closed<T>(RangeValue<T> start, RangeValue<T> end) where T : IComparable<T>
        => new(start, end, true, true);

    /// <summary>
    /// Creates an open range (start, end).
    /// </summary>
    /// <param name="start">
    /// The start value of the range.
    /// Use RangeValue&lt;T&gt;.NegativeInfinity for unbounded start.
    /// </param>
    /// <param name="end">
    /// The end value of the range.
    /// Use RangeValue&lt;T&gt;.PositiveInfinity for unbounded end.
    /// </param>
    /// <typeparam name="T">
    /// The type of the values in the range. Must implement IComparable&lt;T&gt;.
    /// </typeparam>
    /// <returns>
    /// A new instance of <see cref="Range{T}"/> representing the open range (start, end).
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Range<T> Open<T>(RangeValue<T> start, RangeValue<T> end) where T : IComparable<T>
        => new(start, end, false, false);

    /// <summary>
    /// Creates an open range (start, end).
    /// </summary>
    /// <param name="start">The start value of the range.</param>
    /// <param name="end">The end value of the range.</param>
    /// <typeparam name="T">
    /// The type of the values in the range. Must implement IComparable&lt;T&gt;.
    /// </typeparam>
    /// <returns>
    /// A new instance of <see cref="Range{T}"/> representing the open range (start, end).
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Range<T> Open<T>(T start, T end) where T : IComparable<T>
        => new(start, end, false, false);

    /// <summary>
    /// Creates a half-open range (start, end].
    /// </summary>
    /// <param name="start">
    /// The start value of the range.
    /// Use RangeValue&lt;T&gt;.NegativeInfinity for unbounded start.
    /// </param>
    /// <param name="end">
    /// The end value of the range.
    /// Use RangeValue&lt;T&gt;.PositiveInfinity for unbounded end.
    /// </param>
    /// <typeparam name="T">
    /// The type of the values in the range. Must implement IComparable&lt;T&gt;.
    /// </typeparam>
    /// <returns>
    /// A new instance of <see cref="Range{T}"/> representing the half-open range (start, end].
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Range<T> OpenClosed<T>(RangeValue<T> start, RangeValue<T> end) where T : IComparable<T>
        => new(start, end, false, true);

    /// <summary>
    /// Creates a half-open range [start, end).
    /// </summary>
    /// <param name="start">
    /// The start value of the range.
    /// Use RangeValue&lt;T&gt;.NegativeInfinity for unbounded start.
    /// </param>
    /// <param name="end">
    /// The end value of the range.
    /// Use RangeValue&lt;T&gt;.PositiveInfinity for unbounded end.
    /// </param>
    /// <typeparam name="T">
    /// The type of the values in the range. Must implement IComparable&lt;T&gt;.
    /// </typeparam>
    /// <returns>
    /// A new instance of <see cref="Range{T}"/> representing the half-open range [start, end).
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Range<T> ClosedOpen<T>(RangeValue<T> start, RangeValue<T> end) where T : IComparable<T>
        => new(start, end, true, false);

    /// <summary>
    /// Creates a half-open range [start, end).
    /// </summary>
    /// <param name="start">The start value of the range.</param>
    /// <param name="end">The end value of the range.</param>
    /// <typeparam name="T">
    /// The type of the values in the range. Must implement IComparable&lt;T&gt;.
    /// </typeparam>
    /// <returns>
    /// A new instance of <see cref="Range{T}"/> representing the half-open range [start, end).
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Range<T> ClosedOpen<T>(T start, T end) where T : IComparable<T>
        => new(start, end, true, false);

    /// <summary>
    /// Creates a range with explicit inclusivity settings.
    /// This is a general-purpose factory for cases where inclusivity needs to be preserved or specified explicitly.
    /// Useful for domain operations and transformations that maintain the original range's boundary semantics.
    /// </summary>
    /// <param name="start">
    /// The start value of the range.
    /// Use RangeValue&lt;T&gt;.NegativeInfinity for unbounded start.
    /// </param>
    /// <param name="end">
    /// The end value of the range.
    /// Use RangeValue&lt;T&gt;.PositiveInfinity for unbounded end.
    /// </param>
    /// <param name="isStartInclusive">Whether the start boundary is inclusive.</param>
    /// <param name="isEndInclusive">Whether the end boundary is inclusive.</param>
    /// <typeparam name="T">The type of values in the range. Must implement IComparable&lt;T&gt;.</typeparam>
    /// <returns>A new validated Range&lt;T&gt; instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Range<T> Create<T>(RangeValue<T> start, RangeValue<T> end, bool isStartInclusive, bool isEndInclusive)
        where T : IComparable<T>
        => new(start, end, isStartInclusive, isEndInclusive);


    /// <summary>
    /// Attempts to create a range with explicit inclusivity settings.
    /// Returns a boolean indicating whether the created range is valid.
    /// </summary>
    /// <param name="start">The starting value of the range.</param>
    /// <param name="end">The ending value of the range.</param>
    /// <param name="isStartInclusive">True if the start value is inclusive; false if exclusive.</param>
    /// <param name="isEndInclusive">True if the end value is inclusive; false if exclusive.</param>
    /// <param name="range">The resulting range when creation succeeds; default when it fails.</param>
    /// <param name="message">An optional message describing why creation failed.</param>
    /// <typeparam name="T">The type of values in the range. Must implement IComparable&lt;T&gt;.</typeparam>
    /// <returns>True when creation succeeded and range is valid; false otherwise.</returns>
    public static bool TryCreate<T>(RangeValue<T> start, RangeValue<T> end, bool isStartInclusive,
        bool isEndInclusive, out Range<T> range, out string? message)
        where T : IComparable<T>
    {
        // Validate bounds first without throwing
        if (Range<T>.TryValidateBounds(start, end, isStartInclusive, isEndInclusive, out message))
        {
            // Construct range without re-validating (skipValidation = true)
            range = new Range<T>(start, end, isStartInclusive, isEndInclusive, true);
            return true;
        }

        range = default;
        return false;
    }

    /// <summary>
    /// Parses a range from the given input string.
    /// The expected format is:
    /// [start,end], (start,end), [start,end), or (start,end]
    /// where start and end are values of type T, or empty for infinite bounds (+infinity/-infinity).
    /// </summary>
    /// <param name="input">
    /// The input string representing the range.
    /// </param>
    /// <param name="formatProvider">
    /// An optional format provider for parsing the boundary values.
    /// The default is CultureInfo.InvariantCulture.
    /// </param>
    /// <typeparam name="T">
    /// The type of the values in the range. Must implement IComparable&lt;T&gt; and ISpanParsable&lt;T&gt;.
    /// </typeparam>
    /// <returns>
    /// A new instance of <see cref="Range{T}"/> representing the parsed range.
    /// </returns>
    public static Range<T> FromString<T>(ReadOnlySpan<char> input, IFormatProvider? formatProvider = null)
        where T : IComparable<T>, ISpanParsable<T>
        => RangeStringParser.Parse<T>(input, formatProvider);

    /// <summary>
    /// Parses a range from an interpolated string with zero allocations.
    /// The expected format is: $"[{start}, {end}]" or $"{bracket}{start}, {end}{bracket}"
    /// Values are parsed directly from the interpolation without building the final string.
    /// This overload is automatically selected by the compiler when using interpolated strings.
    /// </summary>
    /// <param name="handler">The interpolated string handler that performs zero-allocation parsing.</param>
    /// <typeparam name="T">
    /// The type of the values in the range. Must implement IComparable&lt;T&gt; and ISpanParsable&lt;T&gt;.
    /// </typeparam>
    /// <returns>
    /// A new instance of <see cref="Range{T}"/> representing the parsed range.
    /// </returns>
    /// <exception cref="FormatException">Thrown if the interpolated string format is invalid.</exception>
    /// <example>
    /// <code>
    /// // Compiler automatically uses the handler overload for interpolated strings
    /// // Using literal brackets (cleanest and most readable)
    /// var range1 = Range.FromString&lt;int&gt;($"[{10}, {20}]");
    /// var range2 = Range.FromString&lt;int&gt;($"({0}, {100})");
    /// var range3 = Range.FromString&lt;double&gt;($"[{1.5}, {9.5})");
    /// 
    /// // Using variables - zero allocations!
    /// int start = 10;
    /// int end = 20;
    /// var range4 = Range.FromString&lt;int&gt;($"[{start}, {end}]");
    /// 
    /// // Using char bracket variables (when brackets need to be dynamic)
    /// char openBracket = '[';
    /// char closeBracket = ')';
    /// var range5 = Range.FromString&lt;int&gt;($"{openBracket}{start}, {end}{closeBracket}");
    /// 
    /// // Using infinity
    /// var range6 = Range.FromString&lt;int&gt;($"[{RangeValue&lt;int&gt;.NegativeInfinity}, {100}]");
    /// 
    /// // For regular strings, use the string overload:
    /// var range7 = Range.FromString&lt;int&gt;("[10, 20]");
    /// </code>
    /// </example>
    public static Range<T> FromString<T>(RangeInterpolatedStringHandler<T> handler)
        where T : IComparable<T>, ISpanParsable<T>
        => handler.GetRange();
}