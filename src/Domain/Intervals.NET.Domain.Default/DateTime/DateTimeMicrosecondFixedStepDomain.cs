using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.DateTime;

/// <summary>
/// Fixed step domain for DateTime with microsecond steps. Steps are added or subtracted in whole microseconds.
/// </summary>
public readonly struct DateTimeMicrosecondFixedStepDomain : IFixedStepDomain<System.DateTime>
{
    private const long TicksPerMicrosecond = 10;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public System.DateTime Floor(System.DateTime value) =>
        new(value.Ticks / TicksPerMicrosecond * TicksPerMicrosecond);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public System.DateTime Ceiling(System.DateTime value)
    {
        var floored = Floor(value);
        return floored == value ? value : floored.AddMicroseconds(1);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(System.DateTime start, System.DateTime end) =>
        (Floor(end) - Floor(start)).Ticks / TicksPerMicrosecond;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public System.DateTime Add(System.DateTime value, long offset) =>
        new(value.Ticks + (offset * TicksPerMicrosecond));

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public System.DateTime Subtract(System.DateTime value, long offset) =>
        new(value.Ticks - (offset * TicksPerMicrosecond));
}