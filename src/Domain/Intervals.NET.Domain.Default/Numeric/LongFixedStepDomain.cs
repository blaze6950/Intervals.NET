using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.Numeric;

/// <summary>
/// Provides a fixed-step domain for 64-bit integers. Steps are of size 1.
/// </summary>
public readonly struct LongFixedStepDomain : IFixedStepDomain<long>
{
    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Add(long value, long steps) => checked(value + steps);

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Subtract(long value, long steps) => checked(value - steps);

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(long start, long end) => checked(end - start);

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Floor(long value) => value;

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Ceiling(long value) => value;
}
