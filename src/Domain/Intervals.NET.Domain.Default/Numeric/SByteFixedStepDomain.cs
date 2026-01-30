using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.Numeric;

/// <summary>
/// Provides a fixed-step domain for signed bytes (SByte). Steps are of size 1.
/// </summary>
public readonly struct SByteFixedStepDomain : IFixedStepDomain<sbyte>
{
    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte Floor(sbyte value) => value;

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte Ceiling(sbyte value) => value;

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(sbyte start, sbyte end) => end - start;

    /// <inheritdoc/>
    [Pure]
    public sbyte Add(sbyte value, long offset)
    {
        var result = value + offset;
        if (result < sbyte.MinValue || result > sbyte.MaxValue)
        {
            throw new OverflowException($"Adding {offset} to {value} would overflow sbyte range [-128, 127].");
        }
        return (sbyte)result;
    }

    /// <inheritdoc/>
    [Pure]
    public sbyte Subtract(sbyte value, long offset) => Add(value, -offset);
}