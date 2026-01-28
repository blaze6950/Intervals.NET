using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.Numeric;

/// <summary>
/// Provides a fixed-step domain implementation for <see cref="byte"/> values with a step size of 1.
/// </summary>
public readonly struct ByteFixedStepDomain : IFixedStepDomain<byte>
{
    public byte Floor(byte value) => value;
    public byte Ceiling(byte value) => value;
    public long Distance(byte start, byte end) => end - start;
    
    public byte Add(byte value, long offset)
    {
        var result = value + offset;
        if (result < byte.MinValue || result > byte.MaxValue)
        {
            throw new OverflowException($"Adding {offset} to {value} would overflow byte range [0, 255].");
        }
        return (byte)result;
    }
}
