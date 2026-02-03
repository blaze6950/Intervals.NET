using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.DateTime;

/// <summary>
/// Fixed step domain for DateTime with second steps. Steps are added or subtracted in whole seconds.
/// </summary>
public readonly record struct DateTimeSecondFixedStepDomain : IFixedStepDomain<System.DateTime>
{
    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public System.DateTime Add(System.DateTime value, long offset) => value.AddSeconds(offset);

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public System.DateTime Subtract(System.DateTime value, long offset) => value.AddSeconds(-offset);

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(System.DateTime start, System.DateTime end) =>
        (long)(Floor(end) - Floor(start)).TotalSeconds;

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public System.DateTime Floor(System.DateTime value) =>
        new(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public System.DateTime Ceiling(System.DateTime value)
    {
        var floored = Floor(value);
        return floored == value ? value : floored.AddSeconds(1);
    }
}