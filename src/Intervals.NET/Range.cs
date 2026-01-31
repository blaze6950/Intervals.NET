using System.Runtime.CompilerServices;

namespace Intervals.NET;

/// <summary>
/// Represents a range of values of type T.
/// </summary>
/// <typeparam name="T">
/// The type of the values in the range. Must implement IComparable&lt;T&gt;.
/// </typeparam>
public readonly record struct Range<T> where T : IComparable<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Range{T}"/> struct.
    /// </summary>
    /// <param name="start">
    /// The start value of the range.
    /// Use RangeValue&lt;T&gt;.NegativeInfinity for unbounded start.
    /// </param>
    /// <param name="end">
    /// The end value of the range.
    /// Use RangeValue&lt;T&gt;.PositiveInfinity for unbounded end.
    /// </param>
    /// <param name="isStartInclusive">
    /// Indicates whether the start value is inclusive.
    /// </param>
    /// <param name="isEndInclusive">
    /// Indicates whether the end value is inclusive.
    /// </param>
    internal Range(RangeValue<T> start, RangeValue<T> end, bool isStartInclusive = true, bool isEndInclusive = false)
    {
        // Validate that start <= end when both are finite
        if (!TryValidateBounds(start, end, isStartInclusive, isEndInclusive, out var message))
        {
            throw new ArgumentException(message, nameof(start));
        }

        Start = start;
        End = end;
        IsStartInclusive = isStartInclusive;
        IsEndInclusive = isEndInclusive;
    }

    /// <summary>
    /// Checks whether the provided bounds form a valid range. Returns false and an explanatory message when invalid.
    /// </summary>
    /// <param name="start">Start bound.</param>
    /// <param name="end">End bound.</param>
    /// <param name="isStartInclusive">Whether start is inclusive.</param>
    /// <param name="isEndInclusive">Whether end is inclusive.</param>
    /// <param name="message">Optional error message when validation fails.</param>
    internal static bool TryValidateBounds(RangeValue<T> start, RangeValue<T> end, bool isStartInclusive, bool isEndInclusive, out string? message)
    {
        message = null;

        // Only validate ordering when both bounds are finite
        if (start.IsFinite && end.IsFinite)
        {
            var comparison = RangeValue<T>.Compare(start, end);
            if (comparison > 0)
            {
                message = "Start value cannot be greater than end value.";
                return false;
            }

            // If start == end, at least one bound must be inclusive for the range to contain any values
            if (comparison == 0 && !isStartInclusive && !isEndInclusive)
            {
                message = "When start equals end, at least one bound must be inclusive.";
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Internal constructor that skips validation for performance.
    /// Use only when values are already validated (e.g., from parser).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Range(RangeValue<T> start, RangeValue<T> end, bool isStartInclusive, bool isEndInclusive, bool skipValidation)
    {
        if (!skipValidation && !TryValidateBounds(start, end, isStartInclusive, isEndInclusive, out var message))
        {
            throw new ArgumentException(message, nameof(start));
        }

        Start = start;
        End = end;
        IsStartInclusive = isStartInclusive;
        IsEndInclusive = isEndInclusive;
    }

    /// <summary>
    /// The start value of the range.
    /// Can be finite, negative infinity, or positive infinity.
    /// </summary>
    public RangeValue<T> Start { get; }

    /// <summary>
    /// The end value of the range.
    /// Can be finite, negative infinity, or positive infinity.
    /// </summary>
    public RangeValue<T> End { get; }

    /// <summary>
    /// Indicates whether the start value is inclusive.
    /// Meaning the range includes the start value: [start, ...
    /// </summary>
    public bool IsStartInclusive { get; }

    /// <summary>
    /// Indicates whether the end value is inclusive.
    /// Meaning the range includes the end value: ..., end]
    /// </summary>
    public bool IsEndInclusive { get; }

    /// <summary>
    /// Returns true when this Range's bounds and inclusivity form a valid range.
    /// This is computed on-demand.
    /// </summary>
    public bool IsValid => TryValidateBounds(Start, End, IsStartInclusive, IsEndInclusive, out _);

    /// <summary>
    /// Returns a string representation of the range.
    /// Example: [start, end), (start, end], etc.
    /// </summary>
    public override string ToString()
    {
        var startBracket = IsStartInclusive ? "[" : "(";
        var endBracket = IsEndInclusive ? "]" : ")";

        var startValue = Start.ToString();
        var endValue = End.ToString();

        return $"{startBracket}{startValue}, {endValue}{endBracket}";
    }

    /// <summary>
    /// Computes the intersection of two ranges using the &amp; operator.
    /// </summary>
    /// <param name="left">The first range.</param>
    /// <param name="right">The second range.</param>
    /// <returns>
    /// A new range representing the intersection, or null if the ranges do not overlap.
    /// </returns>
    /// <remarks>
    /// This operator uses the <see cref="Extensions.RangeExtensions.Intersect{T}"/> extension method.
    /// </remarks>
    public static Range<T>? operator &(Range<T> left, Range<T> right) =>
        Extensions.RangeExtensions.Intersect(left, right);

    /// <summary>
    /// Computes the union of two ranges using the | operator.
    /// </summary>
    /// <param name="left">The first range.</param>
    /// <param name="right">The second range.</param>
    /// <returns>
    /// A new range representing the union, or null if the ranges do not overlap and are not adjacent.
    /// </returns>
    /// <remarks>
    /// This operator uses the <see cref="Extensions.RangeExtensions.Union{T}"/> extension method.
    /// The ranges must overlap or be adjacent for the union to be valid.
    /// </remarks>
    public static Range<T>? operator |(Range<T> left, Range<T> right) =>
        Extensions.RangeExtensions.Union(left, right);
}