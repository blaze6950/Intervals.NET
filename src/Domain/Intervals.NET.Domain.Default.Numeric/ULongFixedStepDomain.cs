using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.Numeric;

/// <summary>
/// Provides a fixed-step domain implementation for <see cref="ulong"/> (UInt64) values with a step size of 1.
/// </summary>
/// <remarks>
/// <para><strong>Note:</strong> Distance calculation may not be accurate for ranges larger than long.MaxValue.</para>
/// </remarks>
public readonly struct ULongFixedStepDomain : IFixedStepDomain<ulong>
{
    public ulong Floor(ulong value) => value;
    public ulong Ceiling(ulong value) => value;
    
    public long Distance(ulong start, ulong end)
    {
        if (end >= start)
        {
            var distance = end - start;
            if (distance > (ulong)long.MaxValue)
            {
                return long.MaxValue; // Clamp to max representable distance
            }
            return (long)distance;
        }
        else
        {
            // Negative distance
            var distance = start - end;
            if (distance > (ulong)long.MaxValue)
            {
                return long.MinValue; // Clamp to min representable distance
            }
            return -(long)distance;
        }
    }
    
    public ulong Add(ulong value, long offset)
    {
        if (offset >= 0)
        {
            var uoffset = (ulong)offset;
            if (value > ulong.MaxValue - uoffset)
            {
                throw new OverflowException($"Adding {offset} to {value} would overflow ulong range.");
            }
            return value + uoffset;
        }
        else
        {
            var uoffset = (ulong)(-offset);
            if (value < uoffset)
            {
                throw new OverflowException($"Adding {offset} to {value} would underflow ulong range.");
            }
            return value - uoffset;
        }
    }
    
    public ulong Subtract(ulong value, long offset) => Add(value, -offset);
}
