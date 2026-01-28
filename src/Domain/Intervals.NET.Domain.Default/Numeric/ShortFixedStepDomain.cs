using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.Numeric;

/// <summary>
/// Provides a fixed-step domain for short integers (Int16). Steps are of size 1.
/// </summary>
public readonly struct ShortFixedStepDomain : IFixedStepDomain<short>
{
    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short Floor(short value) => value;

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short Ceiling(short value) => value;

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(short start, short end) => end - start;

    /// <inheritdoc/>
    [Pure]
    public short Add(short value, long offset)
    {
        var result = value + offset;
        if (result < short.MinValue || result > short.MaxValue)
        {
            throw new OverflowException($"Adding {offset} to {value} would overflow short range [{short.MinValue}, {short.MaxValue}].");
        }
        return (short)result;
    }

    /// <inheritdoc/>
    [Pure]
    public short Subtract(short value, long offset) => Add(value, -offset);
}