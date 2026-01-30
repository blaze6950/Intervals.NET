using BenchmarkDotNet.Attributes;
using Intervals.NET.Domain.Default.DateTime;

namespace Intervals.NET.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks boundary alignment operations across DateTime domains with different step granularities.
/// 
/// RATIONALE:
/// - Different step sizes affect boundary alignment complexity
/// - Floor/Ceiling operations remove finer granularity components
/// - Coarser granularities (month, year) have more complex alignment logic
/// - All operations remain O(1) but absolute performance varies
/// 
/// KEY INSIGHT:
/// - Tick/Microsecond: Trivial (no-op or simple arithmetic)
/// - Second/Minute/Hour: Tick arithmetic with division/modulo
/// - Day: DateTime.Date property (optimized by runtime)
/// - Month: Month arithmetic with edge cases (last day of month)
/// - Year: Simple year arithmetic
/// 
/// FOCUS:
/// - Distance calculation across granularities
/// - Floor operation (remove finer components)
/// - Ceiling operation (round up to next boundary)
/// - Impact of non-boundary vs boundary values
/// </summary>
[MemoryDiagnoser]
[ThreadingDiagnoser]
public class CrossGranularityDomainBenchmarks
{
    // Test values
    private readonly DateTime _dateTimeOnBoundary = new(2025, 6, 15, 0, 0, 0, 0); // Midnight, on multiple boundaries
    private readonly DateTime _dateTimeOffBoundary = new(2025, 6, 15, 14, 37, 42, 123); // Off all boundaries
    private readonly DateTime _dateTime2 = new(2025, 12, 31, 23, 59, 59, 999);

    // Domain instances - finest to coarsest
    private readonly DateTimeTicksFixedStepDomain _tickDomain = new();
    private readonly DateTimeMicrosecondFixedStepDomain _microsecondDomain = new();
    private readonly DateTimeMillisecondFixedStepDomain _millisecondDomain = new();
    private readonly DateTimeSecondFixedStepDomain _secondDomain = new();
    private readonly DateTimeMinuteFixedStepDomain _minuteDomain = new();
    private readonly DateTimeHourFixedStepDomain _hourDomain = new();
    private readonly DateTimeDayFixedStepDomain _dayDomain = new();
    private readonly DateTimeMonthFixedStepDomain _monthDomain = new();
    private readonly DateTimeYearFixedStepDomain _yearDomain = new();

    #region Distance Operations - O(1) Across All Granularities

    /// <summary>
    /// Baseline: Tick-level distance (finest granularity).
    /// Expected: O(1), simple tick subtraction, zero allocations.
    /// </summary>
    [Benchmark(Baseline = true)]
    public long TickDomain_Distance()
    {
        return _tickDomain.Distance(_dateTimeOnBoundary, _dateTime2);
    }

    /// <summary>
    /// Microsecond-level distance.
    /// Expected: O(1), tick subtraction + division by 10.
    /// </summary>
    [Benchmark]
    public long MicrosecondDomain_Distance()
    {
        return _microsecondDomain.Distance(_dateTimeOnBoundary, _dateTime2);
    }

    /// <summary>
    /// Millisecond-level distance.
    /// Expected: O(1), tick subtraction + division by 10,000.
    /// </summary>
    [Benchmark]
    public long MillisecondDomain_Distance()
    {
        return _millisecondDomain.Distance(_dateTimeOnBoundary, _dateTime2);
    }

    /// <summary>
    /// Second-level distance.
    /// Expected: O(1), tick subtraction + division by 10,000,000.
    /// </summary>
    [Benchmark]
    public long SecondDomain_Distance()
    {
        return _secondDomain.Distance(_dateTimeOnBoundary, _dateTime2);
    }

    /// <summary>
    /// Minute-level distance.
    /// Expected: O(1), tick subtraction + division.
    /// </summary>
    [Benchmark]
    public long MinuteDomain_Distance()
    {
        return _minuteDomain.Distance(_dateTimeOnBoundary, _dateTime2);
    }

    /// <summary>
    /// Hour-level distance.
    /// Expected: O(1), tick subtraction + division.
    /// </summary>
    [Benchmark]
    public long HourDomain_Distance()
    {
        return _hourDomain.Distance(_dateTimeOnBoundary, _dateTime2);
    }

    /// <summary>
    /// Day-level distance.
    /// Expected: O(1), tick subtraction + division by TicksPerDay.
    /// </summary>
    [Benchmark]
    public long DayDomain_Distance()
    {
        return _dayDomain.Distance(_dateTimeOnBoundary, _dateTime2);
    }

    /// <summary>
    /// Month-level distance (most complex).
    /// Expected: O(1), year/month arithmetic (more complex than tick-based).
    /// </summary>
    [Benchmark]
    public long MonthDomain_Distance()
    {
        return _monthDomain.Distance(_dateTimeOnBoundary, _dateTime2);
    }

    /// <summary>
    /// Year-level distance.
    /// Expected: O(1), simple year subtraction.
    /// </summary>
    [Benchmark]
    public long YearDomain_Distance()
    {
        return _yearDomain.Distance(_dateTimeOnBoundary, _dateTime2);
    }

    #endregion

    #region Floor Operations - On Boundary Values

    /// <summary>
    /// Tick domain Floor on boundary value (no-op).
    /// Expected: O(1), returns value as-is, fastest possible.
    /// </summary>
    [Benchmark]
    public DateTime TickDomain_Floor_OnBoundary()
    {
        return _tickDomain.Floor(_dateTimeOnBoundary);
    }

    /// <summary>
    /// Second domain Floor on boundary value.
    /// Expected: O(1), tick arithmetic (truncate to second).
    /// </summary>
    [Benchmark]
    public DateTime SecondDomain_Floor_OnBoundary()
    {
        return _secondDomain.Floor(_dateTimeOnBoundary);
    }

    /// <summary>
    /// Minute domain Floor on boundary value.
    /// Expected: O(1), tick arithmetic (truncate to minute).
    /// </summary>
    [Benchmark]
    public DateTime MinuteDomain_Floor_OnBoundary()
    {
        return _minuteDomain.Floor(_dateTimeOnBoundary);
    }

    /// <summary>
    /// Hour domain Floor on boundary value.
    /// Expected: O(1), tick arithmetic (truncate to hour).
    /// </summary>
    [Benchmark]
    public DateTime HourDomain_Floor_OnBoundary()
    {
        return _hourDomain.Floor(_dateTimeOnBoundary);
    }

    /// <summary>
    /// Day domain Floor on boundary value.
    /// Expected: O(1), DateTime.Date property (runtime optimized).
    /// </summary>
    [Benchmark]
    public DateTime DayDomain_Floor_OnBoundary()
    {
        return _dayDomain.Floor(_dateTimeOnBoundary);
    }

    /// <summary>
    /// Month domain Floor on boundary value.
    /// Expected: O(1), new DateTime with day=1 (simple constructor).
    /// </summary>
    [Benchmark]
    public DateTime MonthDomain_Floor_OnBoundary()
    {
        return _monthDomain.Floor(_dateTimeOnBoundary);
    }

    /// <summary>
    /// Year domain Floor on boundary value.
    /// Expected: O(1), new DateTime with month=1, day=1.
    /// </summary>
    [Benchmark]
    public DateTime YearDomain_Floor_OnBoundary()
    {
        return _yearDomain.Floor(_dateTimeOnBoundary);
    }

    #endregion

    #region Floor Operations - Off Boundary Values

    /// <summary>
    /// Second domain Floor off boundary (has milliseconds).
    /// Expected: O(1), truncates milliseconds via tick arithmetic.
    /// </summary>
    [Benchmark]
    public DateTime SecondDomain_Floor_OffBoundary()
    {
        return _secondDomain.Floor(_dateTimeOffBoundary);
    }

    /// <summary>
    /// Minute domain Floor off boundary (has seconds).
    /// Expected: O(1), truncates seconds/milliseconds.
    /// </summary>
    [Benchmark]
    public DateTime MinuteDomain_Floor_OffBoundary()
    {
        return _minuteDomain.Floor(_dateTimeOffBoundary);
    }

    /// <summary>
    /// Hour domain Floor off boundary (has minutes).
    /// Expected: O(1), truncates minutes/seconds/milliseconds.
    /// </summary>
    [Benchmark]
    public DateTime HourDomain_Floor_OffBoundary()
    {
        return _hourDomain.Floor(_dateTimeOffBoundary);
    }

    /// <summary>
    /// Day domain Floor off boundary (has time component).
    /// Expected: O(1), DateTime.Date property.
    /// </summary>
    [Benchmark]
    public DateTime DayDomain_Floor_OffBoundary()
    {
        return _dayDomain.Floor(_dateTimeOffBoundary);
    }

    /// <summary>
    /// Month domain Floor off boundary (mid-month).
    /// Expected: O(1), sets day to 1, preserves year/month.
    /// </summary>
    [Benchmark]
    public DateTime MonthDomain_Floor_OffBoundary()
    {
        return _monthDomain.Floor(_dateTimeOffBoundary);
    }

    #endregion

    #region Ceiling Operations - Off Boundary Values

    /// <summary>
    /// Second domain Ceiling off boundary.
    /// Expected: O(1), rounds up to next second.
    /// </summary>
    [Benchmark]
    public DateTime SecondDomain_Ceiling_OffBoundary()
    {
        return _secondDomain.Ceiling(_dateTimeOffBoundary);
    }

    /// <summary>
    /// Minute domain Ceiling off boundary.
    /// Expected: O(1), rounds up to next minute.
    /// </summary>
    [Benchmark]
    public DateTime MinuteDomain_Ceiling_OffBoundary()
    {
        return _minuteDomain.Ceiling(_dateTimeOffBoundary);
    }

    /// <summary>
    /// Hour domain Ceiling off boundary.
    /// Expected: O(1), rounds up to next hour.
    /// </summary>
    [Benchmark]
    public DateTime HourDomain_Ceiling_OffBoundary()
    {
        return _hourDomain.Ceiling(_dateTimeOffBoundary);
    }

    /// <summary>
    /// Day domain Ceiling off boundary.
    /// Expected: O(1), rounds up to next day (midnight).
    /// </summary>
    [Benchmark]
    public DateTime DayDomain_Ceiling_OffBoundary()
    {
        return _dayDomain.Ceiling(_dateTimeOffBoundary);
    }

    /// <summary>
    /// Month domain Ceiling off boundary (mid-month).
    /// Expected: O(1), rounds up to first day of next month.
    /// More complex due to AddMonths call.
    /// </summary>
    [Benchmark]
    public DateTime MonthDomain_Ceiling_OffBoundary()
    {
        return _monthDomain.Ceiling(_dateTimeOffBoundary);
    }

    /// <summary>
    /// Year domain Ceiling off boundary (mid-year).
    /// Expected: O(1), rounds up to January 1st of next year.
    /// </summary>
    [Benchmark]
    public DateTime YearDomain_Ceiling_OffBoundary()
    {
        return _yearDomain.Ceiling(_dateTimeOffBoundary);
    }

    #endregion

    #region Add Operations - Cross-Granularity Comparison

    /// <summary>
    /// Add 1000 ticks.
    /// Expected: O(1), simple tick addition.
    /// </summary>
    [Benchmark]
    public DateTime TickDomain_Add()
    {
        return _tickDomain.Add(_dateTimeOnBoundary, 1000);
    }

    /// <summary>
    /// Add 1000 seconds.
    /// Expected: O(1), DateTime.AddSeconds call.
    /// </summary>
    [Benchmark]
    public DateTime SecondDomain_Add()
    {
        return _secondDomain.Add(_dateTimeOnBoundary, 1000);
    }

    /// <summary>
    /// Add 100 hours.
    /// Expected: O(1), DateTime.AddHours call.
    /// </summary>
    [Benchmark]
    public DateTime HourDomain_Add()
    {
        return _hourDomain.Add(_dateTimeOnBoundary, 100);
    }

    /// <summary>
    /// Add 100 days.
    /// Expected: O(1), DateTime.AddDays call.
    /// </summary>
    [Benchmark]
    public DateTime DayDomain_Add()
    {
        return _dayDomain.Add(_dateTimeOnBoundary, 100);
    }

    /// <summary>
    /// Add 12 months.
    /// Expected: O(1), DateTime.AddMonths call (most complex due to month-length handling).
    /// </summary>
    [Benchmark]
    public DateTime MonthDomain_Add()
    {
        return _monthDomain.Add(_dateTimeOnBoundary, 12);
    }

    /// <summary>
    /// Add 5 years.
    /// Expected: O(1), DateTime.AddYears call.
    /// </summary>
    [Benchmark]
    public DateTime YearDomain_Add()
    {
        return _yearDomain.Add(_dateTimeOnBoundary, 5);
    }

    #endregion
}
