using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.Numeric;

/// <summary>
/// Provides a fixed-step domain for floats (Single). Steps are of size 1.0f.
/// </summary>
/// <remarks>
/// This domain treats float values as having discrete steps of 1.0f.
/// Due to floating-point precision limitations, results may not be exact for very large values.
/// </remarks>
public readonly record struct FloatFixedStepDomain : IFixedStepDomain<float>
{
    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Floor(float value) => MathF.Floor(value);

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Ceiling(float value) => MathF.Ceiling(value);

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(float start, float end) => (long)(Floor(end) - Floor(start));

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Add(float value, long offset) => value + (offset * 1.0f);

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Subtract(float value, long offset) => value - (offset * 1.0f);
}
