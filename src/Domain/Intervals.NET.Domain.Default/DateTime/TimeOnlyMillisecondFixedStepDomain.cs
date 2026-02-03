using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.DateTime;

/// <summary>
/// Provides a fixed-step domain implementation for <see cref="TimeOnly"/> with a step size of 1 millisecond.
/// </summary>
/// <remarks>
/// <para><strong>Requires:</strong> .NET 6.0 or greater.</para>
/// </remarks>
public readonly record struct TimeOnlyMillisecondFixedStepDomain : IFixedStepDomain<TimeOnly>
{
    private const long TicksPerMillisecond = global::System.TimeSpan.TicksPerMillisecond;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly Floor(TimeOnly value) =>
        new((value.Ticks / TicksPerMillisecond) * TicksPerMillisecond);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly Ceiling(TimeOnly value)
    {
        var ticks = value.Ticks;
        var remainder = ticks % TicksPerMillisecond;
        return remainder == 0 ? value : new TimeOnly(((ticks / TicksPerMillisecond) + 1) * TicksPerMillisecond);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(TimeOnly start, TimeOnly end) =>
        (end.Ticks - start.Ticks) / TicksPerMillisecond;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly Add(TimeOnly value, long offset) =>
        new(value.Ticks + (offset * TicksPerMillisecond));

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly Subtract(TimeOnly value, long offset) =>
        new(value.Ticks - (offset * TicksPerMillisecond));
}