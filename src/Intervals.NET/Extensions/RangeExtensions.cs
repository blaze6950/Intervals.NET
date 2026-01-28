using System.Runtime.CompilerServices;

namespace Intervals.NET.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Range{T}"/>.
/// </summary>
public static class RangeExtensions
{
    /// <summary>
    /// Determines whether this range overlaps with another range.
    /// Two ranges overlap if they share at least one common point.
    /// </summary>
    /// <typeparam name="T">The type of values in the range.</typeparam>
    /// <param name="range">The first range.</param>
    /// <param name="other">The second range to check for overlap.</param>
    /// <returns>True if the ranges overlap; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Overlaps<T>(this Range<T> range, Range<T> other)
        where T : IComparable<T>
    {
        // Check if ranges are disjoint
        // Range1 is completely before Range2
        var endStartComparison = RangeValue<T>.Compare(range.End, other.Start);
        if (endStartComparison < 0)
        {
            return false;
        }

        if (endStartComparison == 0 && (!range.IsEndInclusive || !other.IsStartInclusive))
        {
            return false;
        }

        // Range1 is completely after Range2
        var startEndComparison = RangeValue<T>.Compare(range.Start, other.End);
        if (startEndComparison > 0)
        {
            return false;
        }

        if (startEndComparison == 0 && (!range.IsStartInclusive || !other.IsEndInclusive))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Determines whether this range contains the specified value.
    /// </summary>
    /// <typeparam name="T">The type of values in the range.</typeparam>
    /// <param name="range">The range to check.</param>
    /// <param name="value">The value to check for containment.</param>
    /// <returns>True if the range contains the value; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains<T>(this Range<T> range, T value)
        where T : IComparable<T>
    {
        // Check start boundary
        if (range.Start.IsFinite)
        {
            var startComparison = value.CompareTo(range.Start.Value);
            if (startComparison < 0)
            {
                return false;
            }

            if (startComparison == 0 && !range.IsStartInclusive)
            {
                return false;
            }
        }
        // else: start is -infinity, so value is always >= start

        // Check end boundary
        if (range.End.IsFinite)
        {
            var endComparison = value.CompareTo(range.End.Value);
            if (endComparison > 0)
            {
                return false;
            }

            if (endComparison == 0 && !range.IsEndInclusive)
            {
                return false;
            }
        }
        // else: end is +infinity, so value is always <= end

        return true;
    }

    /// <summary>
    /// Determines whether this range completely contains another range.
    /// </summary>
    /// <typeparam name="T">The type of values in the range.</typeparam>
    /// <param name="range">The outer range.</param>
    /// <param name="other">The inner range to check.</param>
    /// <returns>True if this range contains the other range; otherwise, false.</returns>
    public static bool Contains<T>(this Range<T> range, Range<T> other)
        where T : IComparable<T>
    {
        // Check start boundary
        if (other.Start.IsNegativeInfinity)
        {
            // other has infinite start, so range must also have infinite start
            if (!range.Start.IsNegativeInfinity)
            {
                return false;
            }
            // Both have infinite start - check inclusivity
            if (other.IsStartInclusive && !range.IsStartInclusive)
            {
                return false;
            }
        }
        else if (!range.Start.IsNegativeInfinity)
        {
            // Both are finite, compare them
            var comparison = RangeValue<T>.Compare(range.Start, other.Start);
            if (comparison > 0)
            {
                return false;
            }

            if (comparison == 0)
            {
                // If boundaries are equal, range must be inclusive if other is inclusive
                if (other.IsStartInclusive && !range.IsStartInclusive)
                {
                    return false;
                }
            }
        }
        // else: range has infinite start, which contains other's finite start

        // Check end boundary
        if (other.End.IsPositiveInfinity)
        {
            // other has infinite end, so range must also have infinite end
            if (!range.End.IsPositiveInfinity)
            {
                return false;
            }
            // Both have infinite end - check inclusivity
            if (other.IsEndInclusive && !range.IsEndInclusive)
            {
                return false;
            }
        }
        else if (!range.End.IsPositiveInfinity)
        {
            // Both are finite, compare them
            var comparison = RangeValue<T>.Compare(range.End, other.End);
            if (comparison < 0)
            {
                return false;
            }

            if (comparison == 0)
            {
                // If boundaries are equal, range must be inclusive if other is inclusive
                if (other.IsEndInclusive && !range.IsEndInclusive)
                {
                    return false;
                }
            }
        }
        // else: range has infinite end, which contains other's finite end

        return true;
    }

    /// <summary>
    /// Computes the intersection of two ranges.
    /// </summary>
    /// <typeparam name="T">The type of values in the range.</typeparam>
    /// <param name="range">The first range.</param>
    /// <param name="other">The second range.</param>
    /// <returns>
    /// A new range representing the intersection, or null if the ranges do not overlap.
    /// </returns>
    public static Range<T>? Intersect<T>(this Range<T> range, Range<T> other)
        where T : IComparable<T>
    {
        if (!range.Overlaps(other))
        {
            return null;
        }

        // Determine the start of intersection
        RangeValue<T> intersectionStart;
        bool intersectionStartInclusive;

        var startComparison = RangeValue<T>.Compare(range.Start, other.Start);
        if (startComparison > 0)
        {
            intersectionStart = range.Start;
            intersectionStartInclusive = range.IsStartInclusive;
        }
        else if (startComparison < 0)
        {
            intersectionStart = other.Start;
            intersectionStartInclusive = other.IsStartInclusive;
        }
        else
        {
            // Starts are equal
            intersectionStart = range.Start;
            // Both must be inclusive for intersection to be inclusive
            intersectionStartInclusive = range.IsStartInclusive && other.IsStartInclusive;
        }

        // Determine the end of intersection
        RangeValue<T> intersectionEnd;
        bool intersectionEndInclusive;

        var endComparison = RangeValue<T>.Compare(range.End, other.End);
        if (endComparison < 0)
        {
            intersectionEnd = range.End;
            intersectionEndInclusive = range.IsEndInclusive;
        }
        else if (endComparison > 0)
        {
            intersectionEnd = other.End;
            intersectionEndInclusive = other.IsEndInclusive;
        }
        else
        {
            // Ends are equal
            intersectionEnd = range.End;
            // Both must be inclusive for intersection to be inclusive
            intersectionEndInclusive = range.IsEndInclusive && other.IsEndInclusive;
        }

        return new Range<T>(intersectionStart, intersectionEnd, intersectionStartInclusive, intersectionEndInclusive);
    }

    /// <summary>
    /// Computes the union of two ranges if they overlap or are adjacent.
    /// </summary>
    /// <typeparam name="T">The type of values in the range.</typeparam>
    /// <param name="range">The first range.</param>
    /// <param name="other">The second range.</param>
    /// <returns>
    /// A new range representing the union, or null if the ranges do not overlap and are not adjacent.
    /// </returns>
    public static Range<T>? Union<T>(this Range<T> range, Range<T> other)
        where T : IComparable<T>
    {
        // Check if ranges can be merged (overlapping or adjacent)
        if (!range.Overlaps(other) && !range.IsAdjacent(other))
        {
            return null;
        }

        // Determine the start of union
        RangeValue<T> unionStart;
        bool unionStartInclusive;

        var startComparison = RangeValue<T>.Compare(range.Start, other.Start);
        if (startComparison < 0)
        {
            unionStart = range.Start;
            unionStartInclusive = range.IsStartInclusive;
        }
        else if (startComparison > 0)
        {
            unionStart = other.Start;
            unionStartInclusive = other.IsStartInclusive;
        }
        else
        {
            // Starts are equal
            unionStart = range.Start;
            // Either can be inclusive for union to be inclusive
            unionStartInclusive = range.IsStartInclusive || other.IsStartInclusive;
        }

        // Determine the end of union
        RangeValue<T> unionEnd;
        bool unionEndInclusive;

        var endComparison = RangeValue<T>.Compare(range.End, other.End);
        if (endComparison > 0)
        {
            unionEnd = range.End;
            unionEndInclusive = range.IsEndInclusive;
        }
        else if (endComparison < 0)
        {
            unionEnd = other.End;
            unionEndInclusive = other.IsEndInclusive;
        }
        else
        {
            // Ends are equal
            unionEnd = range.End;
            // Either can be inclusive for union to be inclusive
            unionEndInclusive = range.IsEndInclusive || other.IsEndInclusive;
        }

        return new Range<T>(unionStart, unionEnd, unionStartInclusive, unionEndInclusive);
    }

    /// <summary>
    /// Determines whether this range is adjacent to another range.
    /// Two ranges are adjacent if they touch at exactly one point but do not overlap.
    /// </summary>
    /// <typeparam name="T">The type of values in the range.</typeparam>
    /// <param name="range">The first range.</param>
    /// <param name="other">The second range.</param>
    /// <returns>True if the ranges are adjacent; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAdjacent<T>(this Range<T> range, Range<T> other)
        where T : IComparable<T>
    {
        // Check if range.End touches other.Start
        if (range.End.IsFinite && other.Start.IsFinite)
        {
            var comparison = RangeValue<T>.Compare(range.End, other.Start);
            if (comparison == 0)
            {
                // Adjacent if exactly one is inclusive (they touch but don't overlap)
                if (range.IsEndInclusive != other.IsStartInclusive)
                {
                    return true;
                }
            }
        }

        // Check if other.End touches range.Start
        if (other.End.IsFinite && range.Start.IsFinite)
        {
            var comparison = RangeValue<T>.Compare(other.End, range.Start);
            if (comparison == 0)
            {
                // Adjacent if exactly one is inclusive (they touch but don't overlap)
                if (other.IsEndInclusive != range.IsStartInclusive)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether this range is completely before another range (no overlap).
    /// </summary>
    /// <typeparam name="T">The type of values in the range.</typeparam>
    /// <param name="range">The first range.</param>
    /// <param name="other">The second range.</param>
    /// <returns>True if this range is completely before the other range; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBefore<T>(this Range<T> range, Range<T> other)
        where T : IComparable<T>
    {
        if (range.End.IsPositiveInfinity || other.Start.IsNegativeInfinity)
        {
            return false;
        }

        var comparison = RangeValue<T>.Compare(range.End, other.Start);
        if (comparison < 0)
        {
            return true;
        }

        if (comparison == 0 && (!range.IsEndInclusive || !other.IsStartInclusive))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether this range is completely after another range (no overlap).
    /// </summary>
    /// <typeparam name="T">The type of values in the range.</typeparam>
    /// <param name="range">The first range.</param>
    /// <param name="other">The second range.</param>
    /// <returns>True if this range is completely after the other range; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAfter<T>(this Range<T> range, Range<T> other)
        where T : IComparable<T> => other.IsBefore(range);

    /// <summary>
    /// Determines whether this range is empty (contains no values).
    /// A range is empty if start > end, or if start == end with both boundaries exclusive.
    /// </summary>
    /// <typeparam name="T">The type of values in the range.</typeparam>
    /// <param name="range">The range to check.</param>
    /// <returns>True if the range is empty; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty<T>(this Range<T> range)
        where T : IComparable<T>
    {
        if (!range.Start.IsFinite || !range.End.IsFinite)
        {
            return false; // Infinite ranges are never empty
        }

        var comparison = RangeValue<T>.Compare(range.Start, range.End);
        if (comparison > 0)
        {
            return true;
        }

        if (comparison == 0 && range is { IsStartInclusive: false, IsEndInclusive: false })
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether this range is bounded (has both start and end values).
    /// </summary>
    /// <typeparam name="T">The type of values in the range.</typeparam>
    /// <param name="range">The range to check.</param>
    /// <returns>True if the range has both start and end values; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBounded<T>(this Range<T> range)
        where T : IComparable<T>
    {
        return range.Start.IsFinite && range.End.IsFinite;
    }

    /// <summary>
    /// Determines whether this range is unbounded (has infinite start or end).
    /// </summary>
    /// <typeparam name="T">The type of values in the range.</typeparam>
    /// <param name="range">The range to check.</param>
    /// <returns>True if the range has infinite start or end; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUnbounded<T>(this Range<T> range)
        where T : IComparable<T>
    {
        return !range.Start.IsFinite || !range.End.IsFinite;
    }

    /// <summary>
    /// Determines whether this range contains all values (infinite in both directions).
    /// </summary>
    /// <typeparam name="T">The type of values in the range.</typeparam>
    /// <param name="range">The range to check.</param>
    /// <returns>True if the range contains all values; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInfinite<T>(this Range<T> range)
        where T : IComparable<T>
    {
        return range.Start.IsNegativeInfinity && range.End.IsPositiveInfinity;
    }

    /// <summary>
    /// Enumerates all ranges that result from excluding the other range from this range.
    /// This can produce 0, 1, or 2 ranges depending on how the ranges overlap.
    /// </summary>
    /// <typeparam name="T">The type of values in the range.</typeparam>
    /// <param name="range">The range to subtract from.</param>
    /// <param name="other">The range to subtract.</param>
    /// <returns>An enumerable of ranges representing the difference.</returns>
    public static IEnumerable<Range<T>> Except<T>(this Range<T> range, Range<T> other)
        where T : IComparable<T>
    {
        if (!range.Overlaps(other))
        {
            // No overlap, return the original range
            yield return range;
            yield break;
        }

        // Check if there's a left portion before the overlap
        if (range.Start.IsNegativeInfinity)
        {
            // range starts at -infinity
            if (other.Start.IsFinite)
            {
                yield return new Range<T>(RangeValue<T>.NegativeInfinity, other.Start, true, !other.IsStartInclusive);
            }
        }
        else if (!other.Start.IsNegativeInfinity)
        {
            // Both are finite, compare them
            var comparison = RangeValue<T>.Compare(range.Start, other.Start);
            if (comparison < 0)
            {
                // There's a left portion
                yield return new Range<T>(range.Start, other.Start, range.IsStartInclusive,
                    !other.IsStartInclusive);
            }
            else if (comparison == 0 && range.IsStartInclusive && !other.IsStartInclusive)
            {
                // Single point at the start
                yield return new Range<T>(range.Start, range.Start, true, true);
            }
        }
        // else: other starts at -infinity, no left portion possible

        // Check if there's a right portion after the overlap
        if (range.End.IsPositiveInfinity)
        {
            // range ends at +infinity
            if (other.End.IsFinite)
            {
                yield return new Range<T>(other.End, RangeValue<T>.PositiveInfinity, !other.IsEndInclusive, true);
            }
        }
        else if (!other.End.IsPositiveInfinity)
        {
            // Both are finite, compare them
            var comparison = RangeValue<T>.Compare(range.End, other.End);
            if (comparison > 0)
            {
                // There's a right portion
                yield return new Range<T>(other.End, range.End, !other.IsEndInclusive, range.IsEndInclusive);
            }
            else if (comparison == 0 && range.IsEndInclusive && !other.IsEndInclusive)
            {
                // Single point at the end
                yield return new Range<T>(range.End, range.End, true, true);
            }
        }
        // else: other ends at +infinity, no right portion possible
    }
}