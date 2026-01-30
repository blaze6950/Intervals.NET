using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.DateTime;

/// <summary>
/// Provides a fixed-step domain implementation for <see cref="TimeOnly"/> with a step size of 1 second.
/// </summary>
public readonly struct TimeOnlySecondFixedStepDomain : IFixedStepDomain<TimeOnly>
{
    private const long TicksPerSecond = global::System.TimeSpan.TicksPerSecond;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly Floor(TimeOnly value) =>
        new((value.Ticks / TicksPerSecond) * TicksPerSecond);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly Ceiling(TimeOnly value)
    {
        var ticks = value.Ticks;
        var remainder = ticks % TicksPerSecond;
        return remainder == 0 ? value : new TimeOnly(((ticks / TicksPerSecond) + 1) * TicksPerSecond);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(TimeOnly start, TimeOnly end) =>
        (end.Ticks - start.Ticks) / TicksPerSecond;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly Add(TimeOnly value, long offset) =>
        new(value.Ticks + (offset * TicksPerSecond));

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly Subtract(TimeOnly value, long offset) =>
        new(value.Ticks - (offset * TicksPerSecond));
}