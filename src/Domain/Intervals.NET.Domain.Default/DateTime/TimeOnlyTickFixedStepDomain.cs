using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.DateTime;

/// <summary>
/// Provides a fixed-step domain implementation for <see cref="TimeOnly"/> with a step size of 1 tick (100 nanoseconds).
/// </summary>
/// <remarks>
/// <para>This is the finest granularity TimeOnly domain.</para>
/// </remarks>
public readonly record struct TimeOnlyTickFixedStepDomain : IFixedStepDomain<TimeOnly>
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly Floor(TimeOnly value) => value;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly Ceiling(TimeOnly value) => value;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(TimeOnly start, TimeOnly end) => end.Ticks - start.Ticks;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly Add(TimeOnly value, long offset) => new(value.Ticks + offset);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly Subtract(TimeOnly value, long offset) => new(value.Ticks - offset);
}