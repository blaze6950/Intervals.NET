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
    /// The number of domain steps contained within the range boundaries, or infinity if the range is unbounded.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Counts the number of discrete domain steps that fall within the range boundaries, respecting inclusivity.
    /// Variable-step domains use <see cref="IRangeDomain{T}.Distance"/> which returns a long (discrete step count),
    /// not fractional distances.
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
    public static RangeValue<long> Span<TRangeValue, TDomain>(this Range<TRangeValue> range, TDomain domain)
        where TRangeValue : IComparable<TRangeValue>
        where TDomain : IVariableStepDomain<TRangeValue> => Internal.RangeDomainOperations.CalculateSpan(range, domain);

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
    /// The offset is calculated as <c>(long)(span * ratio)</c>, which truncates any fractional part
    /// from the ratio multiplication.
    /// </para>
    /// <para><strong>Example:</strong></para>
    /// <code>
    /// var range = Range.Closed(new DateTime(2026, 1, 20), new DateTime(2026, 1, 26));
    /// var domain = new StandardDateTimeBusinessDaysVariableStepDomain();
    /// var span = range.Span(domain);  // 5 business days (Mon-Fri)
    /// 
    /// // Expand by 40% on each side:
    /// var expanded = range.ExpandByRatio(domain, 0.4, 0.4);
    /// // Calculation: leftOffset = (long)(5 * 0.4) = (long)2.0 = 2
    /// // Result: expanded by 2 business days on each side
    /// </code>
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
        return Internal.RangeDomainOperations.ExpandByRatio(range, domain, leftRatio, rightRatio);
    }

    /// <summary>
    /// Shifts the given range by the specified offset using the provided variable-step domain.
    /// <para>
    /// ⚠️ <strong>Performance:</strong> May be O(N) - The domain's Add() method may require iteration.
    /// </para>
    /// </summary>
    /// <param name="range">The range to be shifted.</param>
    /// <param name="domain">The variable-step domain that defines how to add an offset to values of type T.</param>
    /// <param name="offset">
    /// The offset by which to shift the range. Positive values shift the range forward,
    /// negative values shift it backward.
    /// </param>
    /// <typeparam name="TRangeValue">The type of the values in the range. Must implement IComparable&lt;T&gt;.</typeparam>
    /// <typeparam name="TDomain">The type of the domain that implements IVariableStepDomain&lt;TRangeValue&gt;.</typeparam>
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
    /// var range = Range.Closed(new DateTime(2026, 1, 20), new DateTime(2026, 1, 24));  // Tue-Fri
    /// var domain = new StandardDateTimeBusinessDaysVariableStepDomain();
    /// 
    /// // Shift forward by 3 business days - may iterate through calendar
    /// var shifted = range.Shift(domain, 3);  // Fri-Tue (skips weekend)
    /// </code>
    /// 
    /// <para><strong>Performance:</strong></para>
    /// <para>
    /// May be O(N) - Variable-step domains may need to iterate through steps to perform Add().
    /// For example, business day domains must iterate through calendar days, checking each for weekends/holidays.
    /// </para>
    /// </remarks>
    public static Range<TRangeValue> Shift<TRangeValue, TDomain>(
        this Range<TRangeValue> range,
        TDomain domain,
        long offset)
        where TRangeValue : IComparable<TRangeValue>
        where TDomain : IVariableStepDomain<TRangeValue> =>
        Internal.RangeDomainOperations.Shift(range, domain, offset);

    /// <summary>
    /// Expands the given range by the specified amounts on the left and right sides using the provided variable-step domain.
    /// <para>
    /// ⚠️ <strong>Performance:</strong> May be O(N) - The domain's Add() method may require iteration.
    /// </para>
    /// </summary>
    /// <param name="range">The range to be expanded.</param>
    /// <param name="domain">The variable-step domain that defines how to add an offset to values of type T.</param>
    /// <param name="left">
    /// The amount to expand the range on the left side. Positive values expand the range to the left
    /// (move start boundary backward), while negative values contract it (move start forward).
    /// </param>
    /// <param name="right">
    /// The amount to expand the range on the right side. Positive values expand the range to the right
    /// (move end boundary forward), while negative values contract it (move end backward).
    /// </param>
    /// <typeparam name="TRangeValue">The type of the values in the range. Must implement IComparable&lt;T&gt;.</typeparam>
    /// <typeparam name="TDomain">The type of the domain that implements IVariableStepDomain&lt;TRangeValue&gt;.</typeparam>
    /// <returns>A new <see cref="Range{T}"/> instance representing the expanded range.</returns>
    /// <remarks>
    /// <para>
    /// This operation allows asymmetric expansion - you can expand different amounts on each side.
    /// Negative values cause contraction instead of expansion.
    /// </para>
    /// 
    /// <para><strong>Examples:</strong></para>
    /// <code>
    /// var range = Range.Closed(new DateTime(2026, 1, 20), new DateTime(2026, 1, 24));  // Tue-Fri
    /// var domain = new StandardDateTimeBusinessDaysVariableStepDomain();
    /// 
    /// // Expand by 2 business days on left, 3 on right - may iterate through calendar
    /// var expanded = range.Expand(domain, left: 2, right: 3);  // Previous Fri - Next Wed
    /// </code>
    /// 
    /// <para><strong>Performance:</strong></para>
    /// <para>
    /// May be O(N) - Variable-step domains may need to iterate through steps to perform Add().
    /// For example, business day domains must iterate through calendar days, checking each for weekends/holidays.
    /// The operation calls Add() twice (once for each boundary), so total complexity depends on the offset sizes.
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
        long right = 0)
        where TRangeValue : IComparable<TRangeValue>
        where TDomain : IVariableStepDomain<TRangeValue> =>
        Internal.RangeDomainOperations.Expand(range, domain, left, right);
}