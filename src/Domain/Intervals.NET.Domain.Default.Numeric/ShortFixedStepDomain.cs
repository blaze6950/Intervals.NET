using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.Numeric;

/// <summary>
/// Provides a fixed-step domain implementation for <see cref="short"/> (Int16) values with a step size of 1.
/// </summary>
/// <remarks>
/// <para>
/// This domain treats short integers as discrete values with uniform step sizes of 1.
/// All operations are O(1) and allocation-free.
/// </para>
/// 
/// <para><strong>Performance:</strong></para>
/// <list type="bullet">
/// <item><description>Floor/Ceiling: O(1) - returns value itself</description></item>
/// <item><description>Distance: O(1) - simple subtraction</description></item>
/// <item><description>Add: O(1) - addition with overflow check</description></item>
/// </list>
/// 
/// <para><strong>Usage:</strong></para>
/// <code>
/// var domain = new ShortFixedStepDomain();
/// var range = Range.Closed((short)10, (short)100);
/// var span = range.Span(domain); // Returns 91
/// </code>
/// </remarks>
public readonly struct ShortFixedStepDomain : IFixedStepDomain<short>
{
    /// <summary>
    /// Returns the largest short value less than or equal to the specified value.
    /// For short integers, this is the value itself.
    /// </summary>
    /// <param name="value">The value to floor.</param>
    /// <returns>The value itself, as short integers are already discrete.</returns>
    public short Floor(short value) => value;

    /// <summary>
    /// Returns the smallest short value greater than or equal to the specified value.
    /// For short integers, this is the value itself.
    /// </summary>
    /// <param name="value">The value to ceiling.</param>
    /// <returns>The value itself, as short integers are already discrete.</returns>
    public short Ceiling(short value) => value;

    /// <summary>
    /// Calculates the distance between two short values.
    /// </summary>
    /// <param name="start">The start value.</param>
    /// <param name="end">The end value.</param>
    /// <returns>The distance as a long integer (end - start).</returns>
    public long Distance(short start, short end) => end - start;

    /// <summary>
    /// Adds the specified offset to the given short value.
    /// </summary>
    /// <param name="value">The base value.</param>
    /// <param name="offset">The offset to add (can be negative).</param>
    /// <returns>The result of value + offset.</returns>
    /// <exception cref="OverflowException">Thrown when the result would overflow short bounds.</exception>
    public short Add(short value, long offset)
    {
        var result = value + offset;
        
        if (result < short.MinValue || result > short.MaxValue)
        {
            throw new OverflowException($"Adding {offset} to {value} would overflow short range [{short.MinValue}, {short.MaxValue}].");
        }
        
        return (short)result;
    }
}
