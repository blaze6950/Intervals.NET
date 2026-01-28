using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.Calendar;

/// <summary>
/// Provides a variable-step domain for <see cref="DateOnly"/> that treats business days as steps.
/// <para>
/// ⚠️ <strong>Standard Configuration:</strong> This domain uses the standard business week definition:
/// Monday through Friday are business days; Saturday and Sunday are weekends.
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// This is a <strong>standard implementation</strong> that follows the most common business calendar:
/// </para>
/// <list type="bullet">
/// <item><description><strong>Business Days:</strong> Monday, Tuesday, Wednesday, Thursday, Friday</description></item>
/// <item><description><strong>Weekends:</strong> Saturday, Sunday</description></item>
/// <item><description><strong>No Holidays:</strong> Does not account for public holidays or custom non-working days</description></item>
/// </list>
/// 
/// <para><strong>Performance:</strong> O(N) - Operations require iteration through calendar days.</para>
/// 
/// <para><strong>Behavior:</strong></para>
/// <list type="bullet">
/// <item><description><c>Floor()</c> - Moves weekend dates back to the previous Friday</description></item>
/// <item><description><c>Ceiling()</c> - Moves weekend dates forward to the next Monday</description></item>
/// <item><description><c>Add()/Subtract()</c> - Skips weekends when counting steps</description></item>
/// <item><description><c>Distance()</c> - Counts only business days between dates</description></item>
/// </list>
/// 
/// <para><strong>Custom Business Week Requirements:</strong></para>
/// <para>
/// If you need a different business week configuration (e.g., Sunday-Thursday, or custom holidays),
/// you <strong>must implement a custom domain</strong>. This implementation cannot be configured
/// and is intentionally kept simple and performant for the standard use case.
/// </para>
/// 
/// <para><strong>Examples:</strong></para>
/// <code>
/// var domain = new StandardDateOnlyBusinessDaysVariableStepDomain();
/// 
/// // Friday, January 3, 2025
/// var friday = new DateOnly(2025, 1, 3);
/// // Monday, January 6, 2025
/// var monday = new DateOnly(2025, 1, 6);
/// 
/// // Distance skips weekend: Friday + 1 business day = Monday
/// var distance = domain.Distance(friday, monday); // Returns 2.0 (Friday and Monday)
/// 
/// // Add skips weekend
/// var nextDay = domain.Add(friday, 1); // Returns Monday, Jan 6
/// 
/// // Floor weekend to Friday
/// var saturday = new DateOnly(2025, 1, 4);
/// var floored = domain.Floor(saturday); // Returns Friday, Jan 3
/// </code>
/// 
/// <para><strong>See Also:</strong></para>
/// <list type="bullet">
/// <item><description><see cref="StandardDateTimeBusinessDaysVariableStepDomain"/> - DateTime version</description></item>
/// <item><description><see cref="IVariableStepDomain{T}"/> - Base interface for variable-step domains</description></item>
/// </list>
/// </remarks>
public readonly struct StandardDateOnlyBusinessDaysVariableStepDomain : IVariableStepDomain<DateOnly>
{
    /// <summary>
    /// Adds the specified number of business days to the given date.
    /// Weekends (Saturday and Sunday) are skipped.
    /// </summary>
    /// <param name="value">The starting date.</param>
    /// <param name="steps">The number of business days to add (can be negative to subtract).</param>
    /// <returns>The resulting date after adding the specified business days.</returns>
    [Pure]
    public DateOnly Add(DateOnly value, long steps)
    {
        var current = value;
        var remaining = Math.Abs(steps);
        var forward = steps > 0;

        while (remaining > 0)
        {
            current = current.AddDays(forward ? 1 : -1);
            if (IsBusinessDay(current))
            {
                remaining--;
            }
        }

        return current;
    }

    /// <summary>
    /// Subtracts the specified number of business days from the given date.
    /// Weekends (Saturday and Sunday) are skipped.
    /// </summary>
    /// <param name="value">The starting date.</param>
    /// <param name="steps">The number of business days to subtract (can be negative to add).</param>
    /// <returns>The resulting date after subtracting the specified business days.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateOnly Subtract(DateOnly value, long steps) => Add(value, -steps);

    /// <summary>
    /// Returns the largest business day less than or equal to the specified date.
    /// If the date falls on a weekend, returns the previous Friday.
    /// If the date is a business day, returns it unchanged.
    /// </summary>
    /// <param name="value">The date to floor.</param>
    /// <returns>The date floored to the nearest business day.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateOnly Floor(DateOnly value)
    {
        return value.DayOfWeek switch
        {
            DayOfWeek.Saturday => value.AddDays(-1), // Move to Friday
            DayOfWeek.Sunday => value.AddDays(-2),   // Move to Friday
            _ => value // Already a business day
        };
    }

    /// <summary>
    /// Returns the smallest business day greater than or equal to the specified date.
    /// If the date falls on a weekend, returns the next Monday.
    /// If the date is already a business day, returns it unchanged.
    /// </summary>
    /// <param name="value">The date to ceiling.</param>
    /// <returns>The date ceiling to the nearest business day.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateOnly Ceiling(DateOnly value)
    {
        // If already a business day, return as-is
        if (IsBusinessDay(value))
        {
            return value;
        }

        return value.DayOfWeek switch
        {
            DayOfWeek.Saturday => value.AddDays(2), // Move to Monday
            DayOfWeek.Sunday => value.AddDays(1),   // Move to Monday
            _ => value // Already a business day (should not reach here)
        };
    }

    /// <summary>
    /// Calculates the number of business days between two dates.
    /// Only counts Monday through Friday; weekends are excluded.
    /// </summary>
    /// <param name="start">The start date.</param>
    /// <param name="end">The end date.</param>
    /// <returns>The number of business days between the dates (can be negative if end is before start).</returns>
    /// <remarks>
    /// <para>⚠️ <strong>Performance:</strong> O(N) - Iterates through each day in the range.</para>
    /// </remarks>
    [Pure]
    public double Distance(DateOnly start, DateOnly end)
    {
        var current = Floor(start);
        var target = Floor(end);

        if (current == target)
            return 0.0; // Same date = 0 steps needed

        double count = 0;

        if (current < target)
        {
            while (current < target)
            {
                current = Add(current, 1);
                count++;
            }
        }
        else
        {
            while (current > target)
            {
                current = Subtract(current, 1);
                count--;
            }
        }

        return count;
    }

    /// <summary>
    /// Determines whether the specified date falls on a business day (Monday through Friday).
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <returns>True if the date is Monday through Friday; false if Saturday or Sunday.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsBusinessDay(DateOnly date) =>
        date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
}
