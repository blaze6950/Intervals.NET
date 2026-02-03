using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.DateTime;

/// <summary>
/// Provides a fixed-step domain implementation for <see cref="TimeOnly"/> with a step size of 1 hour.
/// </summary>
/// <remarks>
/// <para><strong>Requires:</strong> .NET 6.0 or greater.</para>
/// </remarks>
public readonly record struct TimeOnlyHourFixedStepDomain : IFixedStepDomain<TimeOnly>
{
    private const long TicksPerHour = global::System.TimeSpan.TicksPerHour;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly Floor(TimeOnly value) =>
        new((value.Ticks / TicksPerHour) * TicksPerHour);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly Ceiling(TimeOnly value)
    {
        var ticks = value.Ticks;
        var remainder = ticks % TicksPerHour;
        return remainder == 0 ? value : new TimeOnly(((ticks / TicksPerHour) + 1) * TicksPerHour);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(TimeOnly start, TimeOnly end) =>
        (end.Ticks - start.Ticks) / TicksPerHour;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly Add(TimeOnly value, long offset) =>
        new(value.Ticks + (offset * TicksPerHour));

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly Subtract(TimeOnly value, long offset) =>
        new(value.Ticks - (offset * TicksPerHour));
}