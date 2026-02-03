using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.TimeSpan;

/// <summary>
/// Provides a fixed-step domain implementation for <see cref="global::System.TimeSpan"/> with a step size of 1 minute.
/// </summary>
public readonly record struct TimeSpanMinuteFixedStepDomain : IFixedStepDomain<global::System.TimeSpan>
{
    private const long TicksPerMinute = global::System.TimeSpan.TicksPerMinute;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public global::System.TimeSpan Floor(global::System.TimeSpan value) =>
        new((value.Ticks / TicksPerMinute) * TicksPerMinute);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public global::System.TimeSpan Ceiling(global::System.TimeSpan value)
    {
        var ticks = value.Ticks;
        var remainder = ticks % TicksPerMinute;
        return remainder == 0
            ? value
            : new global::System.TimeSpan(((ticks / TicksPerMinute) + 1) * TicksPerMinute);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(global::System.TimeSpan start, global::System.TimeSpan end) =>
        (end - start).Ticks / TicksPerMinute;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public global::System.TimeSpan Add(global::System.TimeSpan value, long offset) =>
        value + global::System.TimeSpan.FromMinutes(offset);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public global::System.TimeSpan Subtract(global::System.TimeSpan value, long offset) =>
        value - global::System.TimeSpan.FromMinutes(offset);
}