using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.DateTime;

/// <summary>
/// Fixed step domain for DateTime with month steps. Steps are added or subtracted in whole months.
/// </summary>
public readonly struct DateTimeMonthFixedStepDomain : IFixedStepDomain<System.DateTime>
{
    /// <inheritdoc/>
    [Pure]
    public System.DateTime Add(System.DateTime value, long offset)
    {
        if (offset > int.MaxValue || offset < int.MinValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(offset),
                offset,
                $"Month offset must be between {int.MinValue:N0} and {int.MaxValue:N0}. Received: {offset:N0}");
        }
        return value.AddMonths((int)offset);
    }

    /// <inheritdoc/>
    [Pure]
    public System.DateTime Subtract(System.DateTime value, long offset)
    {
        if (offset > int.MaxValue || offset < int.MinValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(offset),
                offset,
                $"Month offset must be between {int.MinValue:N0} and {int.MaxValue:N0}. Received: {offset:N0}");
        }
        return value.AddMonths(-(int)offset);
    }

    /// <inheritdoc/>
    [Pure]
    public long Distance(System.DateTime start, System.DateTime end)
    {
        // Calculate the distance in months between the floored boundaries
        // This represents the number of month steps between the two dates
        // Works correctly for both forward and reverse ranges (negative distances)
        var startFloor = Floor(start);
        var endFloor = Floor(end);

        var yearDifference = endFloor.Year - startFloor.Year;
        var monthDifference = endFloor.Month - startFloor.Month;

        return yearDifference * 12 + monthDifference;
    }

    /// <inheritdoc/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public System.DateTime Floor(System.DateTime value) => new(value.Year, value.Month, 1);

    /// <inheritdoc/>
    [Pure]
    public System.DateTime Ceiling(System.DateTime value)
    {
        var floored = Floor(value);
        // If already on boundary, return as-is
        return floored == value ? value : floored.AddMonths(1);
    }
}