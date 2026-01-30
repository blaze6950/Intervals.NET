using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.Numeric;

/// <summary>
/// Provides a fixed-step domain implementation for <see cref="uint"/> (UInt32) values with a step size of 1.
/// </summary>
public readonly struct UIntFixedStepDomain : IFixedStepDomain<uint>
{
    public uint Floor(uint value) => value;
    public uint Ceiling(uint value) => value;
    public long Distance(uint start, uint end) => (long)end - start;
    
    public uint Add(uint value, long offset)
    {
        var result = (long)value + offset;
        if (result < uint.MinValue || result > uint.MaxValue)
        {
            throw new OverflowException($"Adding {offset} to {value} would overflow uint range [0, {uint.MaxValue}].");
        }
        return (uint)result;
    }
    
    public uint Subtract(uint value, long offset) => Add(value, -offset);
}
