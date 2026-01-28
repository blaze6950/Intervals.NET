using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.DateTime;

/// <summary>
/// Fixed step domain for DateTime with tick steps. Steps are added or subtracted in ticks.
/// A tick is 100 nanoseconds, the smallest unit of time in .NET DateTime.
/// </summary>
public readonly struct DateTimeTicksFixedStepDomain : IFixedStepDomain<System.DateTime>
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public System.DateTime Floor(System.DateTime value) => value;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public System.DateTime Ceiling(System.DateTime value) => value;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Distance(System.DateTime start, System.DateTime end) => (end - start).Ticks;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public System.DateTime Add(System.DateTime value, long offset) => value.AddTicks(offset);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public System.DateTime Subtract(System.DateTime value, long offset) => value.AddTicks(-offset);
}