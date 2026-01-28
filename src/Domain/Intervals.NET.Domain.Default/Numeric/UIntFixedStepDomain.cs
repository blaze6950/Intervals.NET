using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.Numeric;

/// <summary>
/// Provides a fixed-step domain for unsigned integers (UInt32). Steps are of size 1.
/// </summary>
public readonly struct UIntFixedStepDomain : IFixedStepDomain<uint>
{
    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Floor(uint value) => value;

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Ceiling(uint value) => value;

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(uint start, uint end) => (long)end - (long)start;

    /// <inheritdoc/>
    [Pure]
    public uint Add(uint value, long offset)
    {
        var result = (long)value + offset;
        if (result < uint.MinValue || result > uint.MaxValue)
        {
            throw new OverflowException($"Adding {offset} to {value} would overflow uint range [0, {uint.MaxValue}].");
        }
        return (uint)result;
    }

    /// <inheritdoc/>
    [Pure]
    public uint Subtract(uint value, long offset) => Add(value, -offset);
}