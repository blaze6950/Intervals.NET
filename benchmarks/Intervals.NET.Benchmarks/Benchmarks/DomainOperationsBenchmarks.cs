using BenchmarkDotNet.Attributes;
using Intervals.NET.Domain.Abstractions;
using Intervals.NET.Domain.Default.Numeric;
using Intervals.NET.Domain.Default.DateTime;
using Intervals.NET.Domain.Default.TimeSpan;

namespace Intervals.NET.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks core domain operations: Add, Subtract, Distance, Floor, Ceiling.
/// 
/// RATIONALE:
/// - Domain operations are fundamental building blocks for range calculations
/// - O(1) constant-time performance is critical for fixed-step domains
/// - Zero allocations expected for struct-based domain implementations
/// - AggressiveInlining should enable near-native performance
/// 
/// FOCUS:
/// - Numeric domains: int, long, decimal, double
/// - DateTime domains: day, hour granularities
/// - TimeSpan domains: day granularity
/// - All operations should show zero allocations
/// 
/// EXPECTATIONS:
/// - All fixed-step domain operations are O(1)
/// - Struct-based domains produce zero heap allocations
/// - Performance should be consistent regardless of value magnitude
/// </summary>
[MemoryDiagnoser]
[ThreadingDiagnoser]
public class DomainOperationsBenchmarks
{
    private const long Steps = 100;
    
    // Numeric test values
    private const int IntValue = 1000;
    private const long LongValue = 1000L;
    private const decimal DecimalValue = 1000m;
    private const double DoubleValue = 1000.0;
    
    // DateTime test values
    private readonly DateTime _dateTime = new(2024, 6, 15, 10, 30, 0);
    private readonly DateTime _dateTime2 = new(2024, 12, 31, 18, 45, 0);
    
    // TimeSpan test values
    private readonly TimeSpan _timeSpan = TimeSpan.FromDays(100);
    private readonly TimeSpan _timeSpan2 = TimeSpan.FromDays(200);
    
    // Domain instances
    private readonly IntegerFixedStepDomain _intDomain = new();
    private readonly LongFixedStepDomain _longDomain = new();
    private readonly DecimalFixedStepDomain _decimalDomain = new();
    private readonly DoubleFixedStepDomain _doubleDomain = new();
    private readonly DateTimeDayFixedStepDomain _dateTimeDayDomain = new();
    private readonly DateTimeHourFixedStepDomain _dateTimeHourDomain = new();
    private readonly TimeSpanDayFixedStepDomain _timeSpanDayDomain = new();

    #region Add Operations - O(1) Expected

    /// <summary>
    /// Baseline: Integer domain Add operation.
    /// Expected: O(1), zero allocations, AggressiveInlining optimization.
    /// </summary>
    [Benchmark(Baseline = true)]
    public int IntegerDomain_Add()
    {
        return _intDomain.Add(IntValue, Steps);
    }

    /// <summary>
    /// Long domain Add operation.
    /// Expected: O(1), zero allocations, similar performance to integer.
    /// </summary>
    [Benchmark]
    public long LongDomain_Add()
    {
        return _longDomain.Add(LongValue, Steps);
    }

    /// <summary>
    /// Decimal domain Add operation.
    /// Expected: O(1), zero allocations, potentially slower due to decimal arithmetic.
    /// </summary>
    [Benchmark]
    public decimal DecimalDomain_Add()
    {
        return _decimalDomain.Add(DecimalValue, Steps);
    }

    /// <summary>
    /// Double domain Add operation.
    /// Expected: O(1), zero allocations, floating-point arithmetic.
    /// </summary>
    [Benchmark]
    public double DoubleDomain_Add()
    {
        return _doubleDomain.Add(DoubleValue, Steps);
    }

    /// <summary>
    /// DateTime day domain Add operation.
    /// Expected: O(1), zero allocations, DateTime.AddDays internal call.
    /// </summary>
    [Benchmark]
    public DateTime DateTimeDayDomain_Add()
    {
        return _dateTimeDayDomain.Add(_dateTime, Steps);
    }

    /// <summary>
    /// DateTime hour domain Add operation.
    /// Expected: O(1), zero allocations, DateTime.AddHours internal call.
    /// </summary>
    [Benchmark]
    public DateTime DateTimeHourDomain_Add()
    {
        return _dateTimeHourDomain.Add(_dateTime, Steps);
    }

    /// <summary>
    /// TimeSpan day domain Add operation.
    /// Expected: O(1), zero allocations, TimeSpan arithmetic.
    /// </summary>
    [Benchmark]
    public TimeSpan TimeSpanDayDomain_Add()
    {
        return _timeSpanDayDomain.Add(_timeSpan, Steps);
    }

    #endregion

    #region Distance Operations - O(1) Expected

    /// <summary>
    /// Integer domain Distance calculation.
    /// Expected: O(1), zero allocations, simple subtraction.
    /// </summary>
    [Benchmark]
    public long IntegerDomain_Distance()
    {
        return _intDomain.Distance(IntValue, IntValue + 1000);
    }

    /// <summary>
    /// Long domain Distance calculation.
    /// Expected: O(1), zero allocations, simple subtraction.
    /// </summary>
    [Benchmark]
    public long LongDomain_Distance()
    {
        return _longDomain.Distance(LongValue, LongValue + 1000);
    }

    /// <summary>
    /// Decimal domain Distance calculation.
    /// Expected: O(1), zero allocations, Floor + subtraction.
    /// </summary>
    [Benchmark]
    public long DecimalDomain_Distance()
    {
        return _decimalDomain.Distance(DecimalValue, DecimalValue + 1000);
    }

    /// <summary>
    /// DateTime day domain Distance calculation.
    /// Expected: O(1), zero allocations, tick-based calculation.
    /// </summary>
    [Benchmark]
    public long DateTimeDayDomain_Distance()
    {
        return _dateTimeDayDomain.Distance(_dateTime, _dateTime2);
    }

    /// <summary>
    /// DateTime hour domain Distance calculation.
    /// Expected: O(1), zero allocations, tick-based calculation.
    /// </summary>
    [Benchmark]
    public long DateTimeHourDomain_Distance()
    {
        return _dateTimeHourDomain.Distance(_dateTime, _dateTime2);
    }

    /// <summary>
    /// TimeSpan day domain Distance calculation.
    /// Expected: O(1), zero allocations, tick division.
    /// </summary>
    [Benchmark]
    public long TimeSpanDayDomain_Distance()
    {
        return _timeSpanDayDomain.Distance(_timeSpan, _timeSpan2);
    }

    #endregion

    #region Floor Operations - O(1) Expected

    /// <summary>
    /// Decimal domain Floor operation (non-trivial).
    /// Expected: O(1), zero allocations, Math.Floor call.
    /// </summary>
    [Benchmark]
    public decimal DecimalDomain_Floor()
    {
        return _decimalDomain.Floor(DecimalValue + 0.75m);
    }

    /// <summary>
    /// Double domain Floor operation (non-trivial).
    /// Expected: O(1), zero allocations, Math.Floor call.
    /// </summary>
    [Benchmark]
    public double DoubleDomain_Floor()
    {
        return _doubleDomain.Floor(DoubleValue + 0.75);
    }

    /// <summary>
    /// DateTime day domain Floor operation (removes time component).
    /// Expected: O(1), zero allocations, DateTime.Date property.
    /// </summary>
    [Benchmark]
    public DateTime DateTimeDayDomain_Floor()
    {
        return _dateTimeDayDomain.Floor(_dateTime);
    }

    /// <summary>
    /// DateTime hour domain Floor operation (removes minutes/seconds).
    /// Expected: O(1), zero allocations, tick arithmetic.
    /// </summary>
    [Benchmark]
    public DateTime DateTimeHourDomain_Floor()
    {
        return _dateTimeHourDomain.Floor(_dateTime);
    }

    /// <summary>
    /// TimeSpan day domain Floor operation.
    /// Expected: O(1), zero allocations, tick arithmetic.
    /// </summary>
    [Benchmark]
    public TimeSpan TimeSpanDayDomain_Floor()
    {
        return _timeSpanDayDomain.Floor(_timeSpan);
    }

    #endregion

    #region Ceiling Operations - O(1) Expected

    /// <summary>
    /// Decimal domain Ceiling operation.
    /// Expected: O(1), zero allocations, Math.Ceiling call.
    /// </summary>
    [Benchmark]
    public decimal DecimalDomain_Ceiling()
    {
        return _decimalDomain.Ceiling(DecimalValue + 0.25m);
    }

    /// <summary>
    /// Double domain Ceiling operation.
    /// Expected: O(1), zero allocations, Math.Ceiling call.
    /// </summary>
    [Benchmark]
    public double DoubleDomain_Ceiling()
    {
        return _doubleDomain.Ceiling(DoubleValue + 0.25);
    }

    /// <summary>
    /// DateTime day domain Ceiling operation.
    /// Expected: O(1), zero allocations, conditional date arithmetic.
    /// </summary>
    [Benchmark]
    public DateTime DateTimeDayDomain_Ceiling()
    {
        return _dateTimeDayDomain.Ceiling(_dateTime);
    }

    /// <summary>
    /// DateTime hour domain Ceiling operation.
    /// Expected: O(1), zero allocations, tick arithmetic.
    /// </summary>
    [Benchmark]
    public DateTime DateTimeHourDomain_Ceiling()
    {
        return _dateTimeHourDomain.Ceiling(_dateTime);
    }

    /// <summary>
    /// TimeSpan day domain Ceiling operation.
    /// Expected: O(1), zero allocations, tick arithmetic with conditional.
    /// </summary>
    [Benchmark]
    public TimeSpan TimeSpanDayDomain_Ceiling()
    {
        return _timeSpanDayDomain.Ceiling(_timeSpan);
    }

    #endregion
}
