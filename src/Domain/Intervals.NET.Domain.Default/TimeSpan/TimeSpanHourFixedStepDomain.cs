using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.TimeSpan;

/// <summary>
/// Provides a fixed-step domain implementation for <see cref="global::System.TimeSpan"/> with a step size of 1 hour.
/// </summary>
public readonly struct TimeSpanHourFixedStepDomain : IFixedStepDomain<global::System.TimeSpan>
{
    private const long TicksPerHour = global::System.TimeSpan.TicksPerHour;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public global::System.TimeSpan Floor(global::System.TimeSpan value) =>
        new((value.Ticks / TicksPerHour) * TicksPerHour);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public global::System.TimeSpan Ceiling(global::System.TimeSpan value)
    {
        var ticks = value.Ticks;
        var remainder = ticks % TicksPerHour;
        return remainder == 0
            ? value
            : new global::System.TimeSpan(((ticks / TicksPerHour) + 1) * TicksPerHour);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(global::System.TimeSpan start, global::System.TimeSpan end) =>
        (end - start).Ticks / TicksPerHour;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public global::System.TimeSpan Add(global::System.TimeSpan value, long offset) =>
        value + global::System.TimeSpan.FromHours(offset);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public global::System.TimeSpan Subtract(global::System.TimeSpan value, long offset) =>
        value - global::System.TimeSpan.FromHours(offset);
}