using Intervals.NET.Domain.Abstractions;
using RangeFactory = Intervals.NET.Factories.Range;

namespace Intervals.NET.Domain.Extensions.Internal;

/// <summary>
/// Internal helper class containing shared implementation logic for range domain operations.
/// <para>
/// This class eliminates code duplication between Fixed and Variable extension classes
/// while maintaining explicit performance semantics at the public API level.
/// </para>
/// </summary>
internal static class RangeDomainOperations
{
    /// <summary>
    /// Calculates the span (distance) of a range using any domain type.
    /// Returns the number of discrete steps or infinity if the range is unbounded.
    /// </summary>
    /// <remarks>
    /// Performance depends on the domain's Distance() implementation:
    /// - Fixed-step domains: O(1)
    /// - Variable-step domains: May be O(N)
    /// </remarks>
    public static RangeValue<long> CalculateSpan<TRangeValue, TDomain>(
        Range<TRangeValue> range,
        TDomain domain
    )
        where TRangeValue : IComparable<TRangeValue>
        where TDomain : IRangeDomain<TRangeValue>
    {
        // If either boundary is unbounded in the direction that expands the range, span is infinite
        if (range.Start.IsNegativeInfinity || range.End.IsPositiveInfinity)
        {
            return RangeValue<long>.PositiveInfinity;
        }

        var firstStep = CalculateFirstStep(range, domain);
        var lastStep = CalculateLastStep(range, domain);

        // After domain alignment, boundaries can cross (e.g., open range smaller than one step)
        if (firstStep.CompareTo(lastStep) > 0)
        {
            return 0;
        }

        if (firstStep.CompareTo(lastStep) == 0)
        {
            return HandleSingleStepCase(range, domain);
        }

        var distance = domain.Distance(firstStep, lastStep);
        return distance + 1;
    }

    /// <summary>
    /// Shifts a range by the specified offset using any domain type.
    /// </summary>
    /// <remarks>
    /// Performance depends on the domain's Add() implementation:
    /// - Fixed-step domains: O(1)
    /// - Variable-step domains: May be O(N)
    /// </remarks>
    public static Range<TRangeValue> Shift<TRangeValue, TDomain>(
        Range<TRangeValue> range,
        TDomain domain,
        long offset
    )
        where TRangeValue : IComparable<TRangeValue>
        where TDomain : IRangeDomain<TRangeValue>
    {
        var newStart = range.Start.IsFinite ? domain.Add(range.Start.Value, offset) : range.Start;
        var newEnd = range.End.IsFinite ? domain.Add(range.End.Value, offset) : range.End;

        return RangeFactory.Create(newStart, newEnd, range.IsStartInclusive, range.IsEndInclusive);
    }

    /// <summary>
    /// Expands a range by the specified amounts on the left and right sides using any domain type.
    /// </summary>
    /// <remarks>
    /// Performance depends on the domain's Add() implementation:
    /// - Fixed-step domains: O(1)
    /// - Variable-step domains: May be O(N)
    /// </remarks>
    public static Range<TRangeValue> Expand<TRangeValue, TDomain>(
        Range<TRangeValue> range,
        TDomain domain,
        long left,
        long right
    )
        where TRangeValue : IComparable<TRangeValue>
        where TDomain : IRangeDomain<TRangeValue>
    {
        var newStart = range.Start.IsFinite ? domain.Add(range.Start.Value, -left) : range.Start;
        var newEnd = range.End.IsFinite ? domain.Add(range.End.Value, right) : range.End;

        return RangeFactory.Create(newStart, newEnd, range.IsStartInclusive, range.IsEndInclusive);
    }

    /// <summary>
    /// Expands a range by specified ratios on the left and right sides using any domain type.
    /// </summary>
    /// <remarks>
    /// Performance depends on the domain's Span() and Add() implementations:
    /// - Fixed-step domains: O(1)
    /// - Variable-step domains: May be O(N)
    /// </remarks>
    public static Range<TRangeValue> ExpandByRatio<TRangeValue, TDomain>(
        Range<TRangeValue> range,
        TDomain domain,
        double leftRatio,
        double rightRatio
    )
        where TRangeValue : IComparable<TRangeValue>
        where TDomain : IRangeDomain<TRangeValue>
    {
        var distance = CalculateSpan(range, domain);

        if (!distance.IsFinite)
        {
            throw new ArgumentException("Cannot expand range by ratio when span is infinite.", nameof(range));
        }

        var leftOffset = (long)(distance.Value * leftRatio);
        var rightOffset = (long)(distance.Value * rightRatio);

        return Expand(range, domain, leftOffset, rightOffset);
    }

    // Private helper methods for Span calculation

    private static TRangeValue CalculateFirstStep<TRangeValue, TDomain>(
        Range<TRangeValue> range,
        TDomain domain
    )
        where TRangeValue : IComparable<TRangeValue>
        where TDomain : IRangeDomain<TRangeValue>
    {
        if (range.IsStartInclusive)
        {
            // Include boundary: use floor to include the step we're on/in
            return domain.Floor(range.Start.Value);
        }

        // Exclude boundary: floor to get the boundary, then add 1 to skip it
        var flooredStart = domain.Floor(range.Start.Value);
        return domain.Add(flooredStart, 1);
    }

    private static TRangeValue CalculateLastStep<TRangeValue, TDomain>(
        Range<TRangeValue> range,
        TDomain domain
    )
        where TRangeValue : IComparable<TRangeValue>
        where TDomain : IRangeDomain<TRangeValue>
    {
        if (range.IsEndInclusive)
        {
            // Include boundary: use floor to include the step we're on/in
            return domain.Floor(range.End.Value);
        }

        // Exclude boundary: floor to get the boundary, then subtract 1 to exclude it
        var flooredEnd = domain.Floor(range.End.Value);
        return domain.Add(flooredEnd, -1);
    }

    private static long HandleSingleStepCase<TRangeValue, TDomain>(
        Range<TRangeValue> range,
        TDomain domain
    )
        where TRangeValue : IComparable<TRangeValue>
        where TDomain : IRangeDomain<TRangeValue>
    {
        // If both floor to the same step, check if either bound is actually ON that step
        var startIsOnBoundary = domain.Floor(range.Start.Value).CompareTo(range.Start.Value) == 0;
        var endIsOnBoundary = domain.Floor(range.End.Value).CompareTo(range.End.Value) == 0;

        if (range is { IsStartInclusive: true, IsEndInclusive: true } && (startIsOnBoundary || endIsOnBoundary))
        {
            return 1;
        }

        // Otherwise, they're in between domain steps, return 0
        return 0;
    }
}
