using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Extensions.Fixed;

/// <summary>
/// Extension methods for ranges with <strong>fixed-step domains</strong>.
/// <para>
/// ⚡ <strong>Performance Guarantee:</strong> All methods in this namespace are <strong>O(1)</strong> - constant time.
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// Fixed-step domains have uniform step sizes (e.g., integers, days, hours), allowing
/// constant-time distance calculations and range operations.
/// </para>
/// 
/// <para><strong>Usage:</strong></para>
/// <code>
/// using Intervals.NET.Domain.Extensions.Fixed;  // ⚡ O(1) operations only
/// using Intervals.NET.Domain.Numeric;
/// 
/// var range = Range.Closed(10, 100);
/// var domain = new IntegerFixedStepDomain();
/// 
/// // All operations are O(1):
/// var span = range.Span(domain);                          // Count steps in range
/// var shifted = range.Shift(domain, 5);                   // Shift by offset
/// var expanded = range.ExpandByRatio(domain, 0.2, 0.2);   // Expand proportionally
/// </code>
/// 
/// <para><strong>Applicable Domains:</strong></para>
/// <list type="bullet">
/// <item><description>Numeric: IntegerFixedStepDomain, LongFixedStepDomain, DoubleFixedStepDomain, DecimalFixedStepDomain</description></item>
/// <item><description>DateTime: DateTimeDayFixedStepDomain, DateTimeHourFixedStepDomain, DateTimeMinuteFixedStepDomain, etc.</description></item>
/// </list>
/// 
/// <para><strong>When to Use:</strong></para>
/// <para>
/// Use this namespace when working with domains that have constant step sizes and you
/// need guaranteed constant-time performance for range operations.
/// </para>
/// 
/// <para><strong>See Also:</strong></para>
/// <list type="bullet">
/// <item><description>Intervals.NET.Domain.Extensions.Variable - For variable-step domains (O(N) operations)</description></item>
/// <item><description>Intervals.NET.Domain.Extensions - Common performance-agnostic operations</description></item>
/// </list>
/// </remarks>
public static class RangeDomainExtensions
{
    /// <summary>
    /// Calculates the span (distance) of the given range using the specified fixed-step domain.
    /// <para>
    /// ⚡ <strong>Performance:</strong> O(1) - Constant time.
    /// </para>
    /// </summary>
    /// <param name="range">The range for which to calculate the span.</param>
    /// <param name="domain">The fixed-step domain that defines how to calculate the distance between two values of type T.</param>
    /// <typeparam name="TRangeValue">The type of the values in the range. Must implement IComparable&lt;T&gt;.</typeparam>
    /// <typeparam name="TDomain">The type of the domain that implements IFixedStepDomain&lt;TRangeValue&gt;.</typeparam>
    /// <returns>The number of domain steps contained within the range boundaries, or infinity if the range is unbounded.</returns>
    /// <remarks>
    /// <para>
    /// Counts the number of domain steps that fall within the range boundaries, respecting inclusivity.
    /// Inclusive boundaries include the boundary step, exclusive boundaries exclude it.
    /// </para>
    /// 
    /// <para><strong>Examples with integer domain:</strong></para>
    /// <list type="bullet">
    /// <item><description>[10, 20] returns 11 (includes 10 through 20)</description></item>
    /// <item><description>(10, 20) returns 9 (includes 11 through 19)</description></item>
    /// <item><description>[10, 20) returns 10 (includes 10 through 19)</description></item>
    /// <item><description>(10, 20] returns 10 (includes 11 through 20)</description></item>
    /// </list>
    /// 
    /// <para><strong>Examples with DateTime day domain:</strong></para>
    /// <list type="bullet">
    /// <item><description>[Jan 1, Jan 5] returns 5 (includes 5 complete days)</description></item>
    /// <item><description>[Jan 1 10:00, Jan 1 15:00] returns 0 (both times within same day, no complete day boundary)</description></item>
    /// </list>
    /// </remarks>
    public static RangeValue<long> Span<TRangeValue, TDomain>(this Range<TRangeValue> range, TDomain domain)
        where TRangeValue : IComparable<TRangeValue>
        where TDomain : IFixedStepDomain<TRangeValue>
    {
        // If either boundary is unbounded in the direction that expands the range, span is infinite
        if (range.Start.IsNegativeInfinity || range.End.IsPositiveInfinity)
        {
            return RangeValue<long>.PositiveInfinity;
        }

        var firstStep = CalculateFirstStep(range, domain);
        var lastStep = CalculateLastStep(range, domain);

        // After domain alignment, boundaries can cross (e.g., open range smaller than one step)
        // Example: (Jan 1 00:00, Jan 1 00:01) with day domain -> firstStep=Jan 2, lastStep=Dec 31
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

        // Local functions
        static TRangeValue CalculateFirstStep(Range<TRangeValue> r, TDomain d)
        {
            if (r.IsStartInclusive)
            {
                // Include boundary: use floor to include the step we're on/in
                return d.Floor(r.Start.Value);
            }

            // Exclude boundary: floor to get the boundary, then add 1 to skip it
            var flooredStart = d.Floor(r.Start.Value);
            return d.Add(flooredStart, 1);
        }

        static TRangeValue CalculateLastStep(Range<TRangeValue> r, TDomain d)
        {
            if (r.IsEndInclusive)
            {
                // Include boundary: use floor to include the step we're on/in
                return d.Floor(r.End.Value);
            }

            // Exclude boundary: floor to get the boundary, then subtract 1 to exclude it
            var flooredEnd = d.Floor(r.End.Value);
            return d.Add(flooredEnd, -1);
        }

        static long HandleSingleStepCase(Range<TRangeValue> r, TDomain d)
        {
            // If both floor to the same step, check if either bound is actually ON that step
            var startIsOnBoundary = d.Floor(r.Start.Value).CompareTo(r.Start.Value) == 0;
            var endIsOnBoundary = d.Floor(r.End.Value).CompareTo(r.End.Value) == 0;

            if (r is { IsStartInclusive: true, IsEndInclusive: true } && (startIsOnBoundary || endIsOnBoundary))
            {
                return 1;
            }

            // Otherwise, they're in between domain steps, return 0
            return 0;
        }
    }

    /// <summary>
    /// Expands the given range by specified ratios on the left and right sides using the provided fixed-step domain.
    /// <para>
    /// ⚡ <strong>Performance:</strong> O(1) - Constant time.
    /// </para>
    /// </summary>
    /// <param name="range">The range to be expanded.</param>
    /// <param name="domain">
    /// The fixed-step domain that defines how to calculate the distance between two values of type T
    /// and how to add an offset to values of type T.
    /// </param>
    /// <param name="leftRatio">
    /// The ratio by which to expand the range on the left side. Positive values expand the range to the left,
    /// while negative values contract it. For example, 0.5 means expand by 50% of the range's span.
    /// </param>
    /// <param name="rightRatio">
    /// The ratio by which to expand the range on the right side. Positive values expand the range to the right,
    /// while negative values contract it. For example, 0.5 means expand by 50% of the range's span.
    /// </param>
    /// <typeparam name="TRangeValue">The type of the values in the range. Must implement IComparable&lt;T&gt;.</typeparam>
    /// <typeparam name="TDomain">The type of the domain that implements IFixedStepDomain&lt;TRangeValue&gt;.</typeparam>
    /// <returns>A new <see cref="Range{T}"/> instance representing the expanded range.</returns>
    /// <exception cref="ArgumentException">Thrown when the range span is infinite.</exception>
    /// <remarks>
    /// <para>
    /// Expands (or contracts) the range proportionally based on its current span.
    /// The operation first calculates the range's span, then applies the ratios to determine
    /// expansion amounts on each side.
    /// </para>
    /// 
    /// <para><strong>Truncation Behavior:</strong></para>
    /// <para>
    /// The offset is calculated as <c>(long)(span * ratio)</c>, which truncates any fractional part.
    /// For fixed-step domains, span is always a long integer, so no precision loss occurs.
    /// </para>
    /// <para><strong>Example:</strong></para>
    /// <code>
    /// var range = Range.Closed(10, 20);  // span = 11
    /// var domain = new IntegerFixedStepDomain();
    /// 
    /// // Expand by 50% on each side:
    /// var expanded = range.ExpandByRatio(domain, 0.5, 0.5);
    /// // Calculation: leftOffset = (long)(11 * 0.5) = 5
    /// // Result: [5, 25] (expanded by 5 on each side)
    /// 
    /// // With fractional result:
    /// var expanded2 = range.ExpandByRatio(domain, 0.4, 0.4);
    /// // Calculation: leftOffset = (long)(11 * 0.4) = (long)4.4 = 4
    /// // Result: [6, 24] (truncates to 4 steps)
    /// </code>
    /// </remarks>
    public static Range<TRangeValue> ExpandByRatio<TRangeValue, TDomain>(
        this Range<TRangeValue> range,
        TDomain domain,
        double leftRatio,
        double rightRatio
    )
        where TRangeValue : IComparable<TRangeValue>
        where TDomain : IFixedStepDomain<TRangeValue>
    {
        var distance = range.Span(domain);

        if (!distance.IsFinite)
        {
            throw new ArgumentException("Cannot expand range by ratio when span is infinite.", nameof(range));
        }

        var leftOffset = (long)(distance.Value * leftRatio);
        var rightOffset = (long)(distance.Value * rightRatio);

        return range.Expand(domain, leftOffset, rightOffset);
    }
}