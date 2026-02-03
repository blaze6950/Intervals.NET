using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.DateTime;

/// <summary>
/// Provides a fixed-step domain implementation for <see cref="TimeOnly"/> with a step size of 1 microsecond (10 ticks).
/// </summary>
public readonly record struct TimeOnlyMicrosecondFixedStepDomain : IFixedStepDomain<TimeOnly>
{
    private const long TicksPerMicrosecond = 10;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly Floor(TimeOnly value) =>
        new((value.Ticks / TicksPerMicrosecond) * TicksPerMicrosecond);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly Ceiling(TimeOnly value)
    {
        var ticks = value.Ticks;
        var remainder = ticks % TicksPerMicrosecond;
        return remainder == 0 ? value : new TimeOnly(((ticks / TicksPerMicrosecond) + 1) * TicksPerMicrosecond);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(TimeOnly start, TimeOnly end) =>
        (end.Ticks - start.Ticks) / TicksPerMicrosecond;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly Add(TimeOnly value, long offset) =>
        new(value.Ticks + (offset * TicksPerMicrosecond));

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly Subtract(TimeOnly value, long offset) =>
        new(value.Ticks - (offset * TicksPerMicrosecond));
}