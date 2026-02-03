using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.TimeSpan;

/// <summary>
/// Provides a fixed-step domain implementation for <see cref="global::System.TimeSpan"/> with a step size of 1 tick (100 nanoseconds).
/// </summary>
/// <remarks>
/// <para>This is the finest granularity TimeSpan domain, operating at the tick level (100ns precision).</para>
/// </remarks>
public readonly record struct TimeSpanTickFixedStepDomain : IFixedStepDomain<global::System.TimeSpan>
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public global::System.TimeSpan Floor(global::System.TimeSpan value) => value;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public global::System.TimeSpan Ceiling(global::System.TimeSpan value) => value;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(global::System.TimeSpan start, global::System.TimeSpan end) =>
        (end - start).Ticks;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public global::System.TimeSpan Add(global::System.TimeSpan value, long offset) =>
        value + global::System.TimeSpan.FromTicks(offset);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public global::System.TimeSpan Subtract(global::System.TimeSpan value, long offset) =>
        value - global::System.TimeSpan.FromTicks(offset);
}