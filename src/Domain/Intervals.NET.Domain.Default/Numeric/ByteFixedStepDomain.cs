using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.Numeric;

/// <summary>
/// Provides a fixed-step domain for bytes (Byte). Steps are of size 1.
/// </summary>
public readonly struct ByteFixedStepDomain : IFixedStepDomain<byte>
{
    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte Floor(byte value) => value;

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte Ceiling(byte value) => value;

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(byte start, byte end) => end - start;

    /// <inheritdoc/>
    [Pure]
    public byte Add(byte value, long offset)
    {
        var result = value + offset;
        if (result < byte.MinValue || result > byte.MaxValue)
        {
            throw new OverflowException($"Adding {offset} to {value} would overflow byte range [0, 255].");
        }
        return (byte)result;
    }

    /// <inheritdoc/>
    [Pure]
    public byte Subtract(byte value, long offset) => Add(value, -offset);
}