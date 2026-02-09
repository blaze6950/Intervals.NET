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
        where TDomain : IFixedStepDomain<TRangeValue> => Internal.RangeDomainOperations.CalculateSpan(range, domain);

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
    /// <para>
    /// Note: the supplied "ratio" values are coefficients, not percentages. You can convert a coefficient to a
    /// percentage by multiplying by 100 (for example: 1 -> 100%, 0.5 -> 50%, 0 -> 0%). Conceptually, think of the
    /// coefficients as discrete counts of domain steps proportional to the current span. Negative coefficients behave
    /// like negative offsets in the default <c>Expand</c> method (they move the boundary inward rather than outward).
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
    /// 
    /// // Negative ratios contract the range (behave like negative offsets):
    /// var contracted = range.ExpandByRatio(domain, -0.2, -0.2);
    /// // Calculation: leftOffset = (long)(11 * -0.2) = (long)-2.2 = -2
    /// // Result: [12, 18] (contracted inward by 2 steps on each side)
    /// </code>
    /// </remarks>
    public static Range<TRangeValue> ExpandByRatio<TRangeValue, TDomain>(
        this Range<TRangeValue> range,
        TDomain domain,
        double leftRatio,
        double rightRatio
    )
        where TRangeValue : IComparable<TRangeValue>
        where TDomain : IFixedStepDomain<TRangeValue> =>
        Internal.RangeDomainOperations.ExpandByRatio(range, domain, leftRatio, rightRatio);

    /// <summary>
    /// Shifts the given range by the specified offset using the provided fixed-step domain.
    /// <para>
    /// ⚡ <strong>Performance:</strong> O(1) - Constant time.
    /// </para>
    /// </summary>
    /// <param name="range">The range to be shifted.</param>
    /// <param name="domain">The fixed-step domain that defines how to add an offset to values of type T.</param>
    /// <param name="offset">
    /// The offset by which to shift the range. Positive values shift the range forward,
    /// negative values shift it backward.
    /// </param>
    /// <typeparam name="TRangeValue">The type of the values in the range. Must implement IComparable&lt;T&gt;.</typeparam>
    /// <typeparam name="TDomain">The type of the domain that implements IFixedStepDomain&lt;TRangeValue&gt;.</typeparam>
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
    /// var shifted = range.Shift(domain, 5);       // [15, 25] - O(1)
    /// var shiftedBack = range.Shift(domain, -3);  // [7, 17] - O(1)
    /// </code>
    /// 
    /// <para><strong>Performance:</strong></para>
    /// <para>
    /// O(1) - Fixed-step domains use arithmetic for Add(), ensuring constant-time performance.
    /// </para>
    /// </remarks>
    public static Range<TRangeValue> Shift<TRangeValue, TDomain>(
        this Range<TRangeValue> range,
        TDomain domain,
        long offset
    )
        where TRangeValue : IComparable<TRangeValue>
        where TDomain : IFixedStepDomain<TRangeValue> => Internal.RangeDomainOperations.Shift(range, domain, offset);

    /// <summary>
    /// Expands the given range by the specified amounts on the left and right sides using the provided fixed-step domain.
    /// <para>
    /// ⚡ <strong>Performance:</strong> O(1) - Constant time.
    /// </para>
    /// </summary>
    /// <param name="range">The range to be expanded.</param>
    /// <param name="domain">The fixed-step domain that defines how to add an offset to values of type T.</param>
    /// <param name="left">
    /// The amount to expand the range on the left side. Positive values expand the range to the left
    /// (move start boundary backward), while negative values contract it (move start forward).
    /// </param>
    /// <param name="right">
    /// The amount to expand the range on the right side. Positive values expand the range to the right
    /// (move end boundary forward), while negative values contract it (move end backward).
    /// </param>
    /// <typeparam name="TRangeValue">The type of the values in the range. Must implement IComparable&lt;T&gt;.</typeparam>
    /// <typeparam name="TDomain">The type of the domain that implements IFixedStepDomain&lt;TRangeValue&gt;.</typeparam>
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
    /// var expanded = range.Expand(domain, left: 2, right: 3);    // [8, 23] - O(1)
    /// var contracted = range.Expand(domain, left: -2, right: -3); // [12, 17] - O(1)
    /// var asymmetric = range.Expand(domain, left: 5, right: 0);  // [5, 20] - O(1)
    /// </code>
    /// 
    /// <para><strong>Performance:</strong></para>
    /// <para>
    /// O(1) - Fixed-step domains use arithmetic for Add(), ensuring constant-time performance.
    /// </para>
    /// 
    /// <para><strong>See Also:</strong></para>
    /// <list type="bullet">
    /// <item><description><c>ExpandByRatio</c> - For proportional expansion based on range span</description></item>
    /// </list>
    /// </remarks>
    public static Range<TRangeValue> Expand<TRangeValue, TDomain>(
        this Range<TRangeValue> range,
        TDomain domain,
        long left = 0,
        long right = 0
    )
        where TRangeValue : IComparable<TRangeValue>
        where TDomain : IFixedStepDomain<TRangeValue> =>
        Internal.RangeDomainOperations.Expand(range, domain, left, right);
}