using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.DateTime;

/// <summary>
/// Fixed step domain for DateTime with minute steps. Steps are added or subtracted in whole minutes.
/// </summary>
public readonly record struct DateTimeMinuteFixedStepDomain : IFixedStepDomain<System.DateTime>
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public System.DateTime Floor(System.DateTime value) =>
        new(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public System.DateTime Ceiling(System.DateTime value)
    {
        var floored = Floor(value);
        return floored == value ? value : floored.AddMinutes(1);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(System.DateTime start, System.DateTime end) =>
        (long)(Floor(end) - Floor(start)).TotalMinutes;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public System.DateTime Add(System.DateTime value, long offset) => value.AddMinutes(offset);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public System.DateTime Subtract(System.DateTime value, long offset) => value.AddMinutes(-offset);
}