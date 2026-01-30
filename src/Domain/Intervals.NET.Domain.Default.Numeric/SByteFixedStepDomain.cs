using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.Numeric;

/// <summary>
/// Provides a fixed-step domain implementation for <see cref="sbyte"/> values with a step size of 1.
/// </summary>
public readonly struct SByteFixedStepDomain : IFixedStepDomain<sbyte>
{
    public sbyte Floor(sbyte value) => value;
    public sbyte Ceiling(sbyte value) => value;
    public long Distance(sbyte start, sbyte end) => end - start;
    
    public sbyte Add(sbyte value, long offset)
    {
        var result = value + offset;
        if (result < sbyte.MinValue || result > sbyte.MaxValue)
        {
            throw new OverflowException($"Adding {offset} to {value} would overflow sbyte range [-128, 127].");
        }
        return (sbyte)result;
    }
    
    public sbyte Subtract(sbyte value, long offset) => Add(value, -offset);
}
