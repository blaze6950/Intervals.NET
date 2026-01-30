using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.TimeSpan;

/// <summary>
/// Provides a fixed-step domain implementation for <see cref="global::System.TimeSpan"/> with a step size of 1 microsecond (10 ticks).
/// </summary>
public readonly struct TimeSpanMicrosecondFixedStepDomain : IFixedStepDomain<global::System.TimeSpan>
{
    private const long TicksPerMicrosecond = 10;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public global::System.TimeSpan Floor(global::System.TimeSpan value) =>
        new((value.Ticks / TicksPerMicrosecond) * TicksPerMicrosecond);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public global::System.TimeSpan Ceiling(global::System.TimeSpan value)
    {
        var ticks = value.Ticks;
        var remainder = ticks % TicksPerMicrosecond;
        return remainder == 0
            ? value
            : new global::System.TimeSpan(((ticks / TicksPerMicrosecond) + 1) * TicksPerMicrosecond);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(global::System.TimeSpan start, global::System.TimeSpan end) =>
        (end - start).Ticks / TicksPerMicrosecond;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public global::System.TimeSpan Add(global::System.TimeSpan value, long offset) =>
        value + new global::System.TimeSpan(offset * TicksPerMicrosecond);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public global::System.TimeSpan Subtract(global::System.TimeSpan value, long offset) =>
        value - new global::System.TimeSpan(offset * TicksPerMicrosecond);
}