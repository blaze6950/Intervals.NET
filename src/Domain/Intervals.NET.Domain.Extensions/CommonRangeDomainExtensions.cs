using Intervals.NET.Domain.Abstractions;
using RangeFactory = Intervals.NET.Factories.Range;

namespace Intervals.NET.Domain.Extensions;

/// <summary>
/// Common extension methods that work with any range domain (<see cref="IRangeDomain{T}"/>).
/// <para>
/// These operations are <strong>performance-agnostic</strong> and work uniformly across both
/// fixed-step and variable-step domains.
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// This class contains operations that don't require distance calculations and work
/// uniformly across all domain types. These methods delegate boundary manipulation
/// to the underlying domain without measuring or iterating the range.
/// </para>
/// 
/// <para><strong>Usage:</strong></para>
/// <code>
/// using Intervals.NET.Domain.Extensions;  // Common operations
/// 
/// var range = Range.Closed(10, 100);
/// var domain = new IntegerFixedStepDomain();
/// 
/// // Works with any domain type:
/// var shifted = range.Shift(domain, 5);        // Move by 5 steps
/// var expanded = range.Expand(domain, 2, 3);   // Expand by fixed amounts
/// </code>
/// 
/// <para><strong>Operations:</strong></para>
/// <list type="bullet">
/// <item><description><c>Shift</c> - Moves range boundaries by a fixed step count (preserves inclusivity)</description></item>
/// <item><description><c>Expand</c> - Expands or contracts range by fixed step counts on each side</description></item>
/// </list>
/// 
/// <para><strong>Why These Are Separate:</strong></para>
/// <para>
/// These methods accept <see cref="IRangeDomain{T}"/> (the base interface) rather than
/// specific fixed or variable-step interfaces. This makes them usable with any domain
/// type without importing performance-specific namespaces.
/// </para>
/// 
/// <para>
/// Unlike <c>Span()</c> or <c>ExpandByRatio()</c>, these operations don't measure the
/// range - they simply add/subtract steps from boundaries. Therefore, their performance
/// depends only on the domain's <c>Add()</c> operation, which is typically O(1).
/// </para>
/// 
/// <para><strong>See Also:</strong></para>
/// <list type="bullet">
/// <item><description>Intervals.NET.Domain.Extensions.Fixed - For O(1) fixed-step operations with span calculations</description></item>
/// <item><description>Intervals.NET.Domain.Extensions.Variable - For variable-step operations with span calculations</description></item>
/// </list>
/// </remarks>
public static class CommonRangeDomainExtensions
{
    /// <summary>
    /// Shifts the given range by the specified offset using the provided domain.
    /// <para>
    /// Moves both boundaries by the same number of steps, preserving the range's inclusivity flags.
    /// </para>
    /// </summary>
    /// <param name="range">The range to be shifted.</param>
    /// <param name="domain">The domain that defines how to add an offset to values of type T.</param>
    /// <param name="offset">
    /// The offset by which to shift the range. Positive values shift the range forward,
    /// negative values shift it backward.
    /// </param>
    /// <typeparam name="TRangeValue">The type of the values in the range. Must implement IComparable&lt;T&gt;.</typeparam>
    /// <typeparam name="TDomain">The type of the domain that implements IRangeDomain&lt;TRangeValue&gt;.</typeparam>
    /// <returns>A new <see cref="Range{T}"/> instance representing the shifted range with the same inclusivity.</returns>
    /// <remarks>
    /// <para>
    /// This operation preserves:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Range inclusivity flags (both start and end)</description></item>
    /// <item><description>Infinite boundaries (infinity + offset = infinity)</description></item>
    /// <item><description>Relative distance between boundaries</description></item>
    /// </list>
    /// 
    /// <para><strong>Examples:</strong></para>
    /// <code>
    /// var range = Range.Closed(10, 20);  // [10, 20]
    /// var domain = new IntegerFixedStepDomain();
    /// 
    /// var shifted = range.Shift(domain, 5);    // [15, 25]
    /// var shiftedBack = range.Shift(domain, -3);  // [7, 17]
    /// </code>
    /// 
    /// <para><strong>Performance:</strong></para>
    /// <para>
    /// Typically O(1) for most domains, as it only calls the domain's <c>Add()</c> method twice.
    /// </para>
    /// </remarks>
    public static Range<TRangeValue> Shift<TRangeValue, TDomain>(
        this Range<TRangeValue> range,
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
    /// Expands the given range by the specified amounts on the left and right sides using the provided domain.
    /// <para>
    /// Adjusts boundaries independently by fixed step counts, preserving inclusivity.
    /// </para>
    /// </summary>
    /// <param name="range">The range to be expanded.</param>
    /// <param name="domain">The domain that defines how to add an offset to values of type T.</param>
    /// <param name="left">
    /// The amount to expand the range on the left side. Positive values expand the range to the left
    /// (move start boundary backward), while negative values contract it (move start forward).
    /// </param>
    /// <param name="right">
    /// The amount to expand the range on the right side. Positive values expand the range to the right
    /// (move end boundary forward), while negative values contract it (move end backward).
    /// </param>
    /// <typeparam name="TRangeValue">The type of the values in the range. Must implement IComparable&lt;T&gt;.</typeparam>
    /// <typeparam name="TDomain">The type of the domain that implements IRangeDomain&lt;TRangeValue&gt;.</typeparam>
    /// <returns>A new <see cref="Range{T}"/> instance representing the expanded range.</returns>
    /// <remarks>
    /// <para>
    /// This operation allows asymmetric expansion - you can expand different amounts on each side.
    /// Negative values cause contraction instead of expansion.
    /// </para>
    /// 
    /// <para><strong>Examples:</strong></para>
    /// <code>
    /// var range = Range.Closed(10, 20);  // [10, 20]
    /// var domain = new IntegerFixedStepDomain();
    /// 
    /// var expanded = range.Expand(domain, left: 2, right: 3);    // [8, 23]
    /// var contracted = range.Expand(domain, left: -2, right: -3); // [12, 17]
    /// var asymmetric = range.Expand(domain, left: 5, right: 0);  // [5, 20]
    /// </code>
    /// 
    /// <para><strong>Performance:</strong></para>
    /// <para>
    /// Typically O(1) for most domains, as it only calls the domain's <c>Add()</c> method twice.
    /// </para>
    /// 
    /// <para><strong>See Also:</strong></para>
    /// <list type="bullet">
    /// <item><description><c>ExpandByRatio</c> in Fixed/Variable namespaces - For proportional expansion based on range span</description></item>
    /// </list>
    /// </remarks>
    public static Range<TRangeValue> Expand<TRangeValue, TDomain>(
        this Range<TRangeValue> range,
        TDomain domain,
        long left = 0,
        long right = 0
    )
        where TRangeValue : IComparable<TRangeValue>
        where TDomain : IRangeDomain<TRangeValue>
    {
        var newStart = range.Start.IsFinite ? domain.Add(range.Start.Value, -left) : range.Start;
        var newEnd = range.End.IsFinite ? domain.Add(range.End.Value, right) : range.End;

        return RangeFactory.Create(newStart, newEnd, range.IsStartInclusive, range.IsEndInclusive);
    }
}