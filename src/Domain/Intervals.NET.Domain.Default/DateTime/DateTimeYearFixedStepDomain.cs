using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.DateTime;

/// <summary>
/// Fixed step domain for DateTime with year steps. Steps are added or subtracted in whole years.
/// </summary>
public readonly record struct DateTimeYearFixedStepDomain : IFixedStepDomain<System.DateTime>
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public System.DateTime Floor(System.DateTime value) => new(value.Year, 1, 1);

    [Pure]
    public System.DateTime Ceiling(System.DateTime value)
    {
        var floored = Floor(value);
        return floored == value ? value : floored.AddYears(1);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(System.DateTime start, System.DateTime end) =>
        Floor(end).Year - Floor(start).Year;

    [Pure]
    public System.DateTime Add(System.DateTime value, long offset)
    {
        if (offset > int.MaxValue || offset < int.MinValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(offset),
                offset,
                $"Year offset must be between {int.MinValue:N0} and {int.MaxValue:N0}. Received: {offset:N0}");
        }
        return value.AddYears((int)offset);
    }

    [Pure]
    public System.DateTime Subtract(System.DateTime value, long offset)
    {
        if (offset > int.MaxValue || offset < int.MinValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(offset),
                offset,
                $"Year offset must be between {int.MinValue:N0} and {int.MaxValue:N0}. Received: {offset:N0}");
        }
        return value.AddYears(-(int)offset);
    }
}