using BenchmarkDotNet.Attributes;
using Intervals.NET.Domain.Default.Calendar;
using Intervals.NET.Domain.Extensions.Variable;
using Range = Intervals.NET.Factories.Range;

namespace Intervals.NET.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks variable-step domain operations to demonstrate O(N) characteristics.
/// 
/// RATIONALE:
/// - Variable-step domains have non-uniform step sizes (business days skip weekends)
/// - Operations MUST iterate through range, resulting in O(N) complexity
/// - Performance SCALES with range size (contrast with fixed-step O(1))
/// - Critical to show performance difference vs fixed-step domains
/// 
/// KEY INSIGHT:
/// - 5-day range: Fast (minimal iterations)
/// - 30-day range: ~6x slower (6x more days to check)
/// - 365-day range: ~73x slower (73x more days to check)
/// - This proves O(N) scaling behavior
/// 
/// FOCUS:
/// - Add/Subtract operations (skip weekends)
/// - Distance calculation (count business days)
/// - Floor/Ceiling operations (weekend alignment)
/// - Span calculation (O(N) extension method)
/// - ExpandByRatio (requires O(N) Span calculation)
/// 
/// DOMAINS TESTED:
/// - StandardDateTimeBusinessDaysVariableStepDomain
/// - StandardDateOnlyBusinessDaysVariableStepDomain
/// </summary>
[MemoryDiagnoser]
[ThreadingDiagnoser]
public class VariableStepDomainBenchmarks
{
    private readonly StandardDateTimeBusinessDaysVariableStepDomain _dateTimeDomain = new();
    private readonly StandardDateOnlyBusinessDaysVariableStepDomain _dateOnlyDomain = new();

    // Test dates (starting on Monday for consistency)
    private DateTime _mondayDateTime;
    private DateTime _fridayDateTime;
    private DateTime _dateTimeAfter5Days;
    private DateTime _dateTimeAfter30Days;
    private DateTime _dateTimeAfter365Days;

    private DateOnly _mondayDateOnly;
    private DateOnly _dateOnlyAfter5Days;
    private DateOnly _dateOnlyAfter30Days;
    private DateOnly _dateOnlyAfter365Days;

    // Ranges for Span/ExpandByRatio tests
    private Range<DateTime> _dateTimeRange5Days;
    private Range<DateTime> _dateTimeRange30Days;
    private Range<DateTime> _dateTimeRange365Days;

    private Range<DateOnly> _dateOnlyRange5Days;
    private Range<DateOnly> _dateOnlyRange30Days;
    private Range<DateOnly> _dateOnlyRange365Days;

    [GlobalSetup]
    public void Setup()
    {
        // Start on Monday, January 6, 2025
        _mondayDateTime = new DateTime(2025, 1, 6); // Monday
        _fridayDateTime = new DateTime(2025, 1, 10); // Friday same week
        _dateTimeAfter5Days = _mondayDateTime.AddDays(5);
        _dateTimeAfter30Days = _mondayDateTime.AddDays(30);
        _dateTimeAfter365Days = _mondayDateTime.AddDays(365);

        _mondayDateOnly = new DateOnly(2025, 1, 6);
        _dateOnlyAfter5Days = _mondayDateOnly.AddDays(5);
        _dateOnlyAfter30Days = _mondayDateOnly.AddDays(30);
        _dateOnlyAfter365Days = _mondayDateOnly.AddDays(365);

        // Create ranges
        _dateTimeRange5Days = Range.Closed<DateTime>(_mondayDateTime, _dateTimeAfter5Days);
        _dateTimeRange30Days = Range.Closed<DateTime>(_mondayDateTime, _dateTimeAfter30Days);
        _dateTimeRange365Days = Range.Closed<DateTime>(_mondayDateTime, _dateTimeAfter365Days);

        _dateOnlyRange5Days = Range.Closed<DateOnly>(_mondayDateOnly, _dateOnlyAfter5Days);
        _dateOnlyRange30Days = Range.Closed<DateOnly>(_mondayDateOnly, _dateOnlyAfter30Days);
        _dateOnlyRange365Days = Range.Closed<DateOnly>(_mondayDateOnly, _dateOnlyAfter365Days);
    }

    #region Add Operations - O(N) Scaling Expected

    /// <summary>
    /// Baseline: Add 5 business days (skips weekend).
    /// Expected: O(N), iterates through ~7 calendar days (5 business + 2 weekend).
    /// </summary>
    [Benchmark(Baseline = true)]
    public DateTime DateTimeDomain_Add_5BusinessDays()
    {
        return _dateTimeDomain.Add(_mondayDateTime, 5);
    }

    /// <summary>
    /// Add 20 business days (skips multiple weekends).
    /// Expected: O(N), ~4x slower than 5-day baseline (28 calendar days).
    /// </summary>
    [Benchmark]
    public DateTime DateTimeDomain_Add_20BusinessDays()
    {
        return _dateTimeDomain.Add(_mondayDateTime, 20);
    }

    /// <summary>
    /// Add 100 business days (skips many weekends).
    /// Expected: O(N), ~20x slower than 5-day baseline (140 calendar days).
    /// </summary>
    [Benchmark]
    public DateTime DateTimeDomain_Add_100BusinessDays()
    {
        return _dateTimeDomain.Add(_mondayDateTime, 100);
    }

    /// <summary>
    /// DateOnly domain: Add 5 business days.
    /// Expected: O(N), similar to DateTime version.
    /// </summary>
    [Benchmark]
    public DateOnly DateOnlyDomain_Add_5BusinessDays()
    {
        return _dateOnlyDomain.Add(_mondayDateOnly, 5);
    }

    /// <summary>
    /// DateOnly domain: Add 20 business days.
    /// Expected: O(N), scales linearly with step count.
    /// </summary>
    [Benchmark]
    public DateOnly DateOnlyDomain_Add_20BusinessDays()
    {
        return _dateOnlyDomain.Add(_mondayDateOnly, 20);
    }

    #endregion

    #region Distance Operations - O(N) Scaling Expected

    /// <summary>
    /// Distance over 5 calendar days (Mon-Fri).
    /// Expected: O(N), fast, ~5 business days.
    /// </summary>
    [Benchmark]
    public double DateTimeDomain_Distance_5Days()
    {
        return _dateTimeDomain.Distance(_mondayDateTime, _dateTimeAfter5Days);
    }

    /// <summary>
    /// Distance over 30 calendar days.
    /// Expected: O(N), ~6x slower than 5-day (must check all 30 days).
    /// </summary>
    [Benchmark]
    public double DateTimeDomain_Distance_30Days()
    {
        return _dateTimeDomain.Distance(_mondayDateTime, _dateTimeAfter30Days);
    }

    /// <summary>
    /// Distance over 365 calendar days (full year).
    /// Expected: O(N), ~73x slower than 5-day (must check all 365 days).
    /// Demonstrates linear scaling with range size.
    /// </summary>
    [Benchmark]
    public double DateTimeDomain_Distance_365Days()
    {
        return _dateTimeDomain.Distance(_mondayDateTime, _dateTimeAfter365Days);
    }

    /// <summary>
    /// DateOnly domain: Distance over 30 days.
    /// Expected: O(N), similar performance to DateTime.
    /// </summary>
    [Benchmark]
    public double DateOnlyDomain_Distance_30Days()
    {
        return _dateOnlyDomain.Distance(_mondayDateOnly, _dateOnlyAfter30Days);
    }

    #endregion

    #region Floor Operations - O(1) Expected

    /// <summary>
    /// Floor operation on Monday (no change needed).
    /// Expected: O(1), simple DayOfWeek check, zero allocations.
    /// </summary>
    [Benchmark]
    public DateTime DateTimeDomain_Floor_Monday()
    {
        return _dateTimeDomain.Floor(_mondayDateTime);
    }

    /// <summary>
    /// Floor operation on Saturday (moves to Friday).
    /// Expected: O(1), DayOfWeek check + AddDays(-1).
    /// </summary>
    [Benchmark]
    public DateTime DateTimeDomain_Floor_Saturday()
    {
        var saturday = new DateTime(2025, 1, 11); // Saturday
        return _dateTimeDomain.Floor(saturday);
    }

    /// <summary>
    /// DateOnly domain: Floor on Monday.
    /// Expected: O(1), similar to DateTime version.
    /// </summary>
    [Benchmark]
    public DateOnly DateOnlyDomain_Floor_Monday()
    {
        return _dateOnlyDomain.Floor(_mondayDateOnly);
    }

    #endregion

    #region Ceiling Operations - O(1) Expected

    /// <summary>
    /// Ceiling operation on Monday at midnight (no change).
    /// Expected: O(1), simple checks, zero allocations.
    /// </summary>
    [Benchmark]
    public DateTime DateTimeDomain_Ceiling_Monday()
    {
        return _dateTimeDomain.Ceiling(_mondayDateTime);
    }

    /// <summary>
    /// Ceiling operation on Friday with time component (moves to next Monday).
    /// Expected: O(1), conditional logic with AddDays.
    /// </summary>
    [Benchmark]
    public DateTime DateTimeDomain_Ceiling_FridayWithTime()
    {
        var fridayWithTime = new DateTime(2025, 1, 10, 15, 30, 0); // Friday 3:30 PM
        return _dateTimeDomain.Ceiling(fridayWithTime);
    }

    /// <summary>
    /// Ceiling operation on Sunday (moves to Monday).
    /// Expected: O(1), DayOfWeek check + AddDays(1).
    /// </summary>
    [Benchmark]
    public DateTime DateTimeDomain_Ceiling_Sunday()
    {
        var sunday = new DateTime(2025, 1, 12); // Sunday
        return _dateTimeDomain.Ceiling(sunday);
    }

    #endregion

    #region Span Operations - O(N) Scaling Expected

    /// <summary>
    /// Span calculation for 5-day range.
    /// Expected: O(N), requires Distance calculation (iterates days).
    /// </summary>
    [Benchmark]
    public double DateTimeDomain_Span_5Days()
    {
        return _dateTimeRange5Days.Span(_dateTimeDomain).Value;
    }

    /// <summary>
    /// Span calculation for 30-day range.
    /// Expected: O(N), ~6x slower than 5-day range.
    /// </summary>
    [Benchmark]
    public double DateTimeDomain_Span_30Days()
    {
        return _dateTimeRange30Days.Span(_dateTimeDomain).Value;
    }

    /// <summary>
    /// Span calculation for 365-day range (full year).
    /// Expected: O(N), ~73x slower than 5-day range.
    /// Clearly demonstrates O(N) vs fixed-step O(1).
    /// </summary>
    [Benchmark]
    public double DateTimeDomain_Span_365Days()
    {
        return _dateTimeRange365Days.Span(_dateTimeDomain).Value;
    }

    /// <summary>
    /// DateOnly domain: Span for 30-day range.
    /// Expected: O(N), similar scaling to DateTime.
    /// </summary>
    [Benchmark]
    public double DateOnlyDomain_Span_30Days()
    {
        return _dateOnlyRange30Days.Span(_dateOnlyDomain).Value;
    }

    #endregion

    #region ExpandByRatio Operations - O(N) Scaling Expected

    /// <summary>
    /// ExpandByRatio for 30-day range (20% on each side).
    /// Expected: O(N), requires Span calculation first (O(N)), then Expand (O(N)).
    /// Total: O(N) for span + O(N) for expansion = O(N) overall.
    /// </summary>
    [Benchmark]
    public Range<DateTime> DateTimeDomain_ExpandByRatio_30Days()
    {
        return _dateTimeRange30Days.ExpandByRatio(_dateTimeDomain, leftRatio: 0.2, rightRatio: 0.2);
    }

    /// <summary>
    /// ExpandByRatio for 365-day range (20% on each side).
    /// Expected: O(N), significantly slower than 30-day version.
    /// Demonstrates compound O(N) cost.
    /// </summary>
    [Benchmark]
    public Range<DateTime> DateTimeDomain_ExpandByRatio_365Days()
    {
        return _dateTimeRange365Days.ExpandByRatio(_dateTimeDomain, leftRatio: 0.2, rightRatio: 0.2);
    }

    /// <summary>
    /// DateOnly domain: ExpandByRatio for 30-day range.
    /// Expected: O(N), similar to DateTime version.
    /// </summary>
    [Benchmark]
    public Range<DateOnly> DateOnlyDomain_ExpandByRatio_30Days()
    {
        return _dateOnlyRange30Days.ExpandByRatio(_dateOnlyDomain, leftRatio: 0.2, rightRatio: 0.2);
    }

    #endregion
}
