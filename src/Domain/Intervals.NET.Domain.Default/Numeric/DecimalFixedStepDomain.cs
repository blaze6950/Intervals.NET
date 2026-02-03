using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.Numeric;

/// <summary>
/// Provides a fixed-step domain for decimal numbers. Steps are of size 1.
/// This domain provides precise decimal arithmetic without floating-point errors.
/// </summary>
public readonly record struct DecimalFixedStepDomain : IFixedStepDomain<decimal>
{
    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public decimal Floor(decimal value) => Math.Floor(value);

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public decimal Ceiling(decimal value) => Math.Ceiling(value);

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(decimal start, decimal end) => (long)(Floor(end) - Floor(start));

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public decimal Add(decimal value, long offset) => value + offset;

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public decimal Subtract(decimal value, long offset) => value - offset;
}