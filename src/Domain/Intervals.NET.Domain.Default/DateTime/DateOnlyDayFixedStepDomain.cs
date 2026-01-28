using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.DateTime;

/// <summary>
/// Provides a fixed-step domain implementation for <see cref="DateOnly"/> with a step size of 1 day.
/// </summary>
/// <remarks>
/// <para>DateOnly represents dates without time components and is naturally aligned to day boundaries.</para>
/// <para><strong>Requires:</strong> .NET 6.0 or greater.</para>
/// </remarks>
public readonly struct DateOnlyDayFixedStepDomain : IFixedStepDomain<DateOnly>
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateOnly Floor(DateOnly value) => value;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateOnly Ceiling(DateOnly value) => value;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(DateOnly start, DateOnly end) => end.DayNumber - start.DayNumber;

    [Pure]
    public DateOnly Add(DateOnly value, long offset)
    {
        if (offset > int.MaxValue || offset < int.MinValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(offset),
                offset,
                $"Day offset must be between {int.MinValue:N0} and {int.MaxValue:N0}. Received: {offset:N0}");
        }
        return value.AddDays((int)offset);
    }

    [Pure]
    public DateOnly Subtract(DateOnly value, long offset)
    {
        if (offset > int.MaxValue || offset < int.MinValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(offset),
                offset,
                $"Day offset must be between {int.MinValue:N0} and {int.MaxValue:N0}. Received: {offset:N0}");
        }
        return value.AddDays(-(int)offset);
    }
}