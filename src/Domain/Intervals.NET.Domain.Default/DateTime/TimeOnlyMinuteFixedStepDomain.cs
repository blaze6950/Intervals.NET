using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.DateTime;

/// <summary>
/// Provides a fixed-step domain implementation for <see cref="TimeOnly"/> with a step size of 1 minute.
/// </summary>
/// <remarks>
/// <para><strong>Requires:</strong> .NET 6.0 or greater.</para>
/// </remarks>
public readonly struct TimeOnlyMinuteFixedStepDomain : IFixedStepDomain<TimeOnly>
{
    private const long TicksPerMinute = global::System.TimeSpan.TicksPerMinute;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly Floor(TimeOnly value) =>
        new((value.Ticks / TicksPerMinute) * TicksPerMinute);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly Ceiling(TimeOnly value)
    {
        var ticks = value.Ticks;
        var remainder = ticks % TicksPerMinute;
        return remainder == 0 ? value : new TimeOnly(((ticks / TicksPerMinute) + 1) * TicksPerMinute);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(TimeOnly start, TimeOnly end) =>
        (end.Ticks - start.Ticks) / TicksPerMinute;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly Add(TimeOnly value, long offset) =>
        new(value.Ticks + (offset * TicksPerMinute));

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly Subtract(TimeOnly value, long offset) =>
        new(value.Ticks - (offset * TicksPerMinute));
}