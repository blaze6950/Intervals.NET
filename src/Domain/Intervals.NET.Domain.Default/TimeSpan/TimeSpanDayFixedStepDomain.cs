using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.TimeSpan;

/// <summary>
/// Provides a fixed-step domain implementation for <see cref="global::System.TimeSpan"/> with a step size of 1 day (24 hours).
/// </summary>
public readonly record struct TimeSpanDayFixedStepDomain : IFixedStepDomain<global::System.TimeSpan>
{
    private const long TicksPerDay = global::System.TimeSpan.TicksPerDay;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public global::System.TimeSpan Floor(global::System.TimeSpan value) =>
        new((value.Ticks / TicksPerDay) * TicksPerDay);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public global::System.TimeSpan Ceiling(global::System.TimeSpan value)
    {
        var ticks = value.Ticks;
        var remainder = ticks % TicksPerDay;
        return remainder == 0
            ? value
            : new global::System.TimeSpan(((ticks / TicksPerDay) + 1) * TicksPerDay);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(global::System.TimeSpan start, global::System.TimeSpan end) =>
        (end.Ticks - start.Ticks) / TicksPerDay;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public global::System.TimeSpan Add(global::System.TimeSpan value, long offset) =>
        value.Add(global::System.TimeSpan.FromDays(offset));

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public global::System.TimeSpan Subtract(global::System.TimeSpan value, long offset) =>
        value.Subtract(global::System.TimeSpan.FromDays(offset));
}
