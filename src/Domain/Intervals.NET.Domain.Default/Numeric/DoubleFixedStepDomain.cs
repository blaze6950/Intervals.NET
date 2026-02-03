using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.Numeric;

/// <summary>
/// Provides a fixed-step domain for double-precision floating-point numbers.
/// Steps are of size 1.0.
/// Note: Due to floating-point precision limitations, this domain is best suited
/// for integer-like double values. For precise decimal arithmetic, use DecimalFixedStepDomain.
/// </summary>
public readonly record struct DoubleFixedStepDomain : IFixedStepDomain<double>
{
    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double Floor(double value) => Math.Floor(value);

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double Ceiling(double value) => Math.Ceiling(value);

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(double start, double end) => (long)(Floor(end) - Floor(start));

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double Add(double value, long offset) => value + offset;

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double Subtract(double value, long offset) => value - offset;
}