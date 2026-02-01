using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Extensions.Variable;

/// <summary>
/// Extension methods for ranges with <strong>variable-step domains</strong>.
/// <para>
/// ⚠️ <strong>Performance Warning:</strong> Methods in this namespace may be <strong>O(N)</strong> or worse,
/// depending on the domain implementation.
/// </para>
/// /// </summary>
/// <remarks>
/// <para>
/// Variable-step domains have non-uniform step sizes (e.g., months with varying days,
/// business calendars with holidays, irregular sequences), requiring potentially expensive
/// distance calculations that may iterate through the range.
/// </para>
/// 
/// <para><strong>Usage:</strong></para>
/// <code>
/// using Intervals.NET.Domain.Extensions.Variable;  // ⚠️ Potentially O(N)
/// 
/// var startDate = new DateTime(2024, 1, 1);
/// var endDate = new DateTime(2024, 12, 31);
/// var range = Range.Closed(startDate, endDate);
/// var domain = new BusinessDayDomain();  // Example variable-step domain
/// 
/// // May require iteration through the range:
/// var span = range.Span(domain);  // Counts business days (skips weekends/holidays)
/// </code>
/// 
/// <para><strong>Performance Characteristics:</strong></para>
/// <para>
/// The actual performance depends on the specific domain implementation. Common scenarios:
/// </para>
/// <list type="bullet">
/// <item><description><strong>Business day calendars:</strong> O(N) - Must check each day</description></item>
/// <item><description><strong>Custom sequences:</strong> O(N) or worse - May require computation per step</description></item>
/// <item><description><strong>Variable-length periods:</strong> Depends on calculation method</description></item>
/// </list>
/// <para>
/// Always consult the specific domain's documentation for its complexity guarantees.
/// </para>
/// 
/// <para><strong>When to Use:</strong></para>
/// <para>
/// Use this namespace when working with irregular step sizes where constant-time
/// distance calculation is impossible or impractical. Examples include:
/// </para>
/// <list type="bullet">
/// <item><description>Business days (accounting for weekends and holidays)</description></item>
/// <item><description>Custom calendars with variable-length periods</description></item>
/// <item><description>Domain-specific sequences with irregular spacing</description></item>
/// </list>
/// 
/// <para><strong>Performance Tips:</strong></para>
/// <list type="bullet">
/// <item><description>Cache span calculations if used multiple times</description></item>
/// <item><description>Consider using fixed-step domains when possible</description></item>
/// <item><description>Profile your specific use case with realistic data</description></item>
/// </list>
/// 
/// <para><strong>See Also:</strong></para>
/// <list type="bullet">
/// <item><description>Intervals.NET.Domain.Extensions.Fixed - For O(1) fixed-step operations</description></item>
/// <item><description>Intervals.NET.Domain.Extensions - Common performance-agnostic operations</description></item>
/// </list>
/// </remarks>
public static class RangeDomainExtensions
{
    /// <summary>
    /// Calculates the span (distance) of the given range using the specified variable-step domain.
    /// <para>
    /// ⚠️ <strong>Performance:</strong> May be O(N) or worse, depending on the domain implementation.
    /// </para>
    /// </summary>
    /// <param name="range">The range for which to calculate the span.</param>
    /// <param name="domain">The variable-step domain that defines how to calculate the distance between two values of type T.</param>
    /// <typeparam name="TRangeValue">The type of the values in the range. Must implement IComparable&lt;T&gt;.</typeparam>
    /// <typeparam name="TDomain">The type of the domain that implements IVariableStepDomain&lt;TRangeValue&gt;.</typeparam>
    /// <returns>
    /// The span (distance) of the range as a double, potentially including fractional steps,
    /// or infinity if the range is unbounded.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Counts the number of domain steps that fall within the range boundaries, respecting inclusivity.
    /// Unlike fixed-step domains, this may return fractional values to account for partial steps.
    /// </para>
    /// 
    /// <para>
    /// The complexity depends entirely on the specific domain implementation. Some domains may
    /// require iterating through every step in the range to calculate the distance.
    /// </para>
    /// 
    /// <para><strong>Warning:</strong></para>
    /// <para>
    /// For large ranges with expensive step calculations, this operation may be slow.
    /// Consider caching results if the span will be used multiple times.
    /// </para>
    /// </remarks>
    public static RangeValue<double> Span<TRangeValue, TDomain>(this Range<TRangeValue> range, TDomain domain)
        where TRangeValue : IComparable<TRangeValue>
        where TDomain : IVariableStepDomain<TRangeValue>
    {
        // If either boundary is unbounded in the direction that expands the range, span is infinite
        if (range.Start.IsNegativeInfinity || range.End.IsPositiveInfinity)
        {
            return RangeValue<double>.PositiveInfinity;
        }

        var firstStep = CalculateFirstStep(range, domain);
        var lastStep = CalculateLastStep(range, domain);

        switch (firstStep.CompareTo(lastStep))
        {
            // After domain alignment, boundaries can cross (e.g., open range smaller than one step)
            // Example: (Jan 1 00:00, Jan 1 00:01) with day domain -> firstStep=Jan 2, lastStep=Dec 31
            case > 0:
                return 0.0;
            case 0:
                return HandleSingleStepCase(range, domain);
        }

        // Note: IRangeDomain.Distance returns long, but for variable-step domains we interpret
        // this as the number of complete steps and add 1.0 to get the span including boundaries
        var distance = (double)domain.Distance(firstStep, lastStep);
        return distance + 1.0;

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

        static double HandleSingleStepCase(Range<TRangeValue> r, TDomain d)
        {
            // If both floor to the same step, check if either bound is actually ON that step
            var startIsOnBoundary = d.Floor(r.Start.Value).CompareTo(r.Start.Value) == 0;
            var endIsOnBoundary = d.Floor(r.End.Value).CompareTo(r.End.Value) == 0;

            if (r is { IsStartInclusive: true, IsEndInclusive: true } && (startIsOnBoundary || endIsOnBoundary))
            {
                return 1.0;
            }

            // Otherwise, they're in between domain steps, return 0
            return 0.0;
        }
    }

    /// <summary>
    /// Expands the given range by specified ratios on the left and right sides using the provided variable-step domain.
    /// <para>
    /// ⚠️ <strong>Performance:</strong> May be O(N) or worse, depending on the domain implementation.
    /// </para>
    /// </summary>
    /// <param name="range">The range to be expanded.</param>
    /// <param name="domain">
    /// The variable-step domain that defines how to calculate the distance between two values of type T
    /// and how to add an offset to values of type T.
    /// </param>
    /// <param name="leftRatio">
    /// The ratio by which to expand the range on the left side. Positive values expand the range to the left,
    /// while negative values contract it.
    /// </param>
    /// <param name="rightRatio">
    /// The ratio by which to expand the range on the right side. Positive values expand the range to the right,
    /// while negative values contract it.
    /// </param>
    /// <typeparam name="TRangeValue">The type of the values in the range. Must implement IComparable&lt;T&gt;.</typeparam>
    /// <typeparam name="TDomain">The type of the domain that implements IVariableStepDomain&lt;TRangeValue&gt;.</typeparam>
    /// <returns>A new <see cref="Range{T}"/> instance representing the expanded range.</returns>
    /// <exception cref="ArgumentException">Thrown when the range span is infinite.</exception>
    /// <remarks>
    /// <para>
    /// This method first calculates the range's span (which may be O(N)), then applies
    /// the ratios to determine expansion amounts. The overall complexity depends on
    /// both the span calculation and the domain's Add operation.
    /// </para>
    /// 
    /// <para><strong>Truncation Behavior:</strong></para>
    /// <para>
    /// The offset is calculated as <c>(long)(span * ratio)</c>, which truncates any fractional part.
    /// For variable-step domains, span is a double that may include fractional steps, so truncation
    /// can result in precision loss.
    /// </para>
    /// <para><strong>Example:</strong></para>
    /// <code>
    /// // Variable-step domain might return fractional span
    /// var span = 10.7;  // e.g., business days with partial periods
    /// var ratio = 0.5;
    /// var offset = (long)(span * ratio);  // (long)5.35 = 5
    /// // The 0.35 fractional part is discarded
    /// </code>
    /// 
    /// <para><strong>Note:</strong></para>
    /// <para>
    /// If exact fractional expansion is required, consider using the Expand method directly
    /// with calculated offsets, or implement custom logic that handles fractional steps
    /// according to your domain's semantics.
    /// </para>
    /// </remarks>
    public static Range<TRangeValue> ExpandByRatio<TRangeValue, TDomain>(
        this Range<TRangeValue> range,
        TDomain domain,
        double leftRatio,
        double rightRatio
    )
        where TRangeValue : IComparable<TRangeValue>
        where TDomain : IVariableStepDomain<TRangeValue>
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