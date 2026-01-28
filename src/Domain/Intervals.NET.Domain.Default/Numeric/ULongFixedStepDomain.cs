using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.Numeric;

/// <summary>
/// Provides a fixed-step domain for unsigned long integers (UInt64). Steps are of size 1.
/// </summary>
/// <remarks>
/// Distance calculation may not be accurate for ranges larger than long.MaxValue.
/// In such cases, the distance is clamped to long.MaxValue.
/// </remarks>
public readonly struct ULongFixedStepDomain : IFixedStepDomain<ulong>
{
    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong Floor(ulong value) => value;

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong Ceiling(ulong value) => value;

    /// <inheritdoc/>
    [Pure]
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

    /// <inheritdoc/>
    [Pure]
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

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong Subtract(ulong value, long offset) => Add(value, -offset);
}