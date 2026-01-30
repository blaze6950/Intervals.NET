using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.Numeric;

/// <summary>
/// Provides a fixed-step domain for unsigned short integers (UInt16). Steps are of size 1.
/// </summary>
public readonly struct UShortFixedStepDomain : IFixedStepDomain<ushort>
{
    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort Floor(ushort value) => value;

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort Ceiling(ushort value) => value;

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(ushort start, ushort end) => end - start;

    /// <inheritdoc/>
    [Pure]
    public ushort Add(ushort value, long offset)
    {
        var result = value + offset;
        if (result < ushort.MinValue || result > ushort.MaxValue)
        {
            throw new OverflowException($"Adding {offset} to {value} would overflow ushort range [{ushort.MinValue}, {ushort.MaxValue}].");
        }
        return (ushort)result;
    }

    /// <inheritdoc/>
    [Pure]
    public ushort Subtract(ushort value, long offset) => Add(value, -offset);
}