using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.TimeSpan;

/// <summary>
/// Provides a fixed-step domain implementation for <see cref="global::System.TimeSpan"/> with a step size of 1 millisecond.
/// </summary>
public readonly struct TimeSpanMillisecondFixedStepDomain : IFixedStepDomain<global::System.TimeSpan>
{
    private const long TicksPerMillisecond = global::System.TimeSpan.TicksPerMillisecond;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public global::System.TimeSpan Floor(global::System.TimeSpan value) =>
        new((value.Ticks / TicksPerMillisecond) * TicksPerMillisecond);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public global::System.TimeSpan Ceiling(global::System.TimeSpan value)
    {
        var ticks = value.Ticks;
        var remainder = ticks % TicksPerMillisecond;
        return remainder == 0
            ? value
            : new global::System.TimeSpan(((ticks / TicksPerMillisecond) + 1) * TicksPerMillisecond);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(global::System.TimeSpan start, global::System.TimeSpan end) =>
        (end - start).Ticks / TicksPerMillisecond;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public global::System.TimeSpan Add(global::System.TimeSpan value, long offset) =>
        value + global::System.TimeSpan.FromMilliseconds(offset);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public global::System.TimeSpan Subtract(global::System.TimeSpan value, long offset) =>
        value - global::System.TimeSpan.FromMilliseconds(offset);
}