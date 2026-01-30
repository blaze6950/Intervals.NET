using BenchmarkDotNet.Attributes;
using Intervals.NET.Domain.Default.Numeric;
using Intervals.NET.Domain.Default.DateTime;
using Intervals.NET.Domain.Default.Calendar;
using Intervals.NET.Domain.Extensions;
using Intervals.NET.Domain.Extensions.Fixed;
using Intervals.NET.Domain.Extensions.Variable;
using Range = Intervals.NET.Factories.Range;

namespace Intervals.NET.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks hot-path scenarios with repeated domain operations in tight loops.
/// 
/// RATIONALE:
/// - Real applications call domain operations thousands of times
/// - Struct-based domains should show zero allocations in loops
/// - Cache-friendliness matters for tight loops
/// - GC pressure comparison: struct (zero) vs class-based alternatives
/// 
/// KEY INSIGHT:
/// - Microbenchmarks hide cumulative allocation costs
/// - 1000+ iterations expose memory allocation patterns
/// - Struct domains: stack-allocated, no GC pressure
/// - Hot loops benefit from inlining and register allocation
/// 
/// SCENARIOS:
/// 1. Sequential Add operations (simulate date progression)
/// 2. Repeated Distance calculations (simulate range analysis)
/// 3. Repeated Span calculations (simulate validation loops)
/// 4. Mixed operations (realistic usage patterns)
/// 
/// FOCUS:
/// - Zero allocations for fixed-step domains
/// - Minimal allocations for variable-step domains (struct itself)
/// - Cache locality benefits of contiguous struct arrays
/// </summary>
[MemoryDiagnoser]
[ThreadingDiagnoser]
public class DomainHotPathScenariosBenchmarks
{
    private const int IterationCount = 1000;

    private readonly IntegerFixedStepDomain _intDomain = new();
    private readonly DateTimeDayFixedStepDomain _dateTimeDayDomain = new();
    private readonly DateTimeHourFixedStepDomain _dateTimeHourDomain = new();
    private readonly StandardDateTimeBusinessDaysVariableStepDomain _businessDayDomain = new();

    private int[] _intValues = null!;
    private DateTime[] _dateTimeValues = null!;
    private Range<int>[] _intRanges = null!;
    private Range<DateTime>[] _dateTimeRanges = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);

        // Integer values for sequential operations
        _intValues = new int[IterationCount];
        for (int i = 0; i < IterationCount; i++)
        {
            _intValues[i] = random.Next(0, 100000);
        }

        // DateTime values for sequential operations
        _dateTimeValues = new DateTime[IterationCount];
        var baseDate = new DateTime(2024, 1, 1);
        for (int i = 0; i < IterationCount; i++)
        {
            _dateTimeValues[i] = baseDate.AddDays(random.Next(0, 3650));
        }

        // Integer ranges for Span calculations
        _intRanges = new Range<int>[IterationCount];
        for (int i = 0; i < IterationCount; i++)
        {
            int start = random.Next(0, 1000);
            int end = start + random.Next(10, 1000);
            _intRanges[i] = Range.Closed<int>(start, end);
        }

        // DateTime ranges for Span calculations
        _dateTimeRanges = new Range<DateTime>[IterationCount];
        for (int i = 0; i < IterationCount; i++)
        {
            var start = baseDate.AddDays(random.Next(0, 1000));
            var end = start.AddDays(random.Next(10, 365));
            _dateTimeRanges[i] = Range.Closed<DateTime>(start, end);
        }
    }

    #region Sequential Add Operations in Hot Loop

    /// <summary>
    /// Baseline: 1000 sequential Add operations with integer domain.
    /// Expected: Zero allocations, tight loop, excellent cache behavior.
    /// </summary>
    [Benchmark(Baseline = true)]
    public int IntegerDomain_HotLoop_SequentialAdd()
    {
        int result = 0;
        for (int i = 0; i < IterationCount; i++)
        {
            result = _intDomain.Add(_intValues[i], i);
        }
        return result;
    }

    /// <summary>
    /// 1000 sequential Add operations with DateTime day domain.
    /// Expected: Zero allocations, struct-based domain on stack.
    /// </summary>
    [Benchmark]
    public DateTime DateTimeDayDomain_HotLoop_SequentialAdd()
    {
        DateTime result = _dateTimeValues[0];
        for (int i = 0; i < IterationCount; i++)
        {
            result = _dateTimeDayDomain.Add(_dateTimeValues[i], i);
        }
        return result;
    }

    /// <summary>
    /// 1000 sequential Add operations with DateTime hour domain.
    /// Expected: Zero allocations, similar to day domain.
    /// </summary>
    [Benchmark]
    public DateTime DateTimeHourDomain_HotLoop_SequentialAdd()
    {
        DateTime result = _dateTimeValues[0];
        for (int i = 0; i < IterationCount; i++)
        {
            result = _dateTimeHourDomain.Add(_dateTimeValues[i], i);
        }
        return result;
    }

    /// <summary>
    /// 1000 sequential Add operations with business day domain (variable-step).
    /// Expected: Minimal allocations (struct domain), but slower due to O(N) per Add.
    /// Demonstrates performance difference between fixed and variable-step domains.
    /// </summary>
    [Benchmark]
    public DateTime BusinessDayDomain_HotLoop_SequentialAdd()
    {
        DateTime result = _dateTimeValues[0];
        for (int i = 0; i < IterationCount; i++)
        {
            // Adding small steps (1-5 days) to keep reasonable runtime
            result = _businessDayDomain.Add(_dateTimeValues[i], i % 5 + 1);
        }
        return result;
    }

    #endregion

    #region Repeated Distance Calculations

    /// <summary>
    /// 1000 Distance calculations with integer domain.
    /// Expected: Zero allocations, O(1) per calculation.
    /// </summary>
    [Benchmark]
    public long IntegerDomain_HotLoop_Distance()
    {
        long totalDistance = 0;
        for (int i = 0; i < IterationCount - 1; i++)
        {
            totalDistance += _intDomain.Distance(_intValues[i], _intValues[i + 1]);
        }
        return totalDistance;
    }

    /// <summary>
    /// 1000 Distance calculations with DateTime day domain.
    /// Expected: Zero allocations, O(1) per calculation.
    /// </summary>
    [Benchmark]
    public long DateTimeDayDomain_HotLoop_Distance()
    {
        long totalDistance = 0;
        for (int i = 0; i < IterationCount - 1; i++)
        {
            totalDistance += _dateTimeDayDomain.Distance(_dateTimeValues[i], _dateTimeValues[i + 1]);
        }
        return totalDistance;
    }

    /// <summary>
    /// 100 Distance calculations with business day domain (reduced iterations due to O(N)).
    /// Expected: Minimal allocations, but O(N) per calculation makes this much slower.
    /// </summary>
    [Benchmark]
    public double BusinessDayDomain_HotLoop_Distance_Reduced()
    {
        double totalDistance = 0;
        // Only 100 iterations due to O(N) cost per Distance call
        for (int i = 0; i < 100; i++)
        {
            totalDistance += _businessDayDomain.Distance(_dateTimeValues[i], _dateTimeValues[i + 1]);
        }
        return totalDistance;
    }

    #endregion

    #region Repeated Span Calculations

    /// <summary>
    /// 1000 Span calculations with integer domain (fixed-step).
    /// Expected: Zero allocations, O(1) per Span calculation.
    /// </summary>
    [Benchmark]
    public long IntegerDomain_HotLoop_Span()
    {
        long totalSpan = 0;
        for (int i = 0; i < IterationCount; i++)
        {
            totalSpan += _intRanges[i].Span(_intDomain).Value;
        }
        return totalSpan;
    }

    /// <summary>
    /// 1000 Span calculations with DateTime day domain (fixed-step).
    /// Expected: Zero allocations, O(1) per Span calculation.
    /// </summary>
    [Benchmark]
    public long DateTimeDayDomain_HotLoop_Span()
    {
        long totalSpan = 0;
        for (int i = 0; i < IterationCount; i++)
        {
            totalSpan += _dateTimeRanges[i].Span(_dateTimeDayDomain).Value;
        }
        return totalSpan;
    }

    /// <summary>
    /// 100 Span calculations with business day domain (variable-step, reduced iterations).
    /// Expected: Minimal allocations, but O(N) per Span makes this significantly slower.
    /// </summary>
    [Benchmark]
    public double BusinessDayDomain_HotLoop_Span_Reduced()
    {
        double totalSpan = 0;
        // Only 100 iterations due to O(N) cost per Span call
        for (int i = 0; i < 100; i++)
        {
            totalSpan += _dateTimeRanges[i].Span(_businessDayDomain).Value;
        }
        return totalSpan;
    }

    #endregion

    #region Mixed Operations - Realistic Scenarios

    /// <summary>
    /// Mixed operations: Add, Distance, Floor, Ceiling in sequence.
    /// Expected: Zero allocations, demonstrates combined operation overhead.
    /// Simulates realistic range manipulation workflow.
    /// </summary>
    [Benchmark]
    public long IntegerDomain_HotLoop_MixedOperations()
    {
        long result = 0;
        for (int i = 0; i < IterationCount; i++)
        {
            var value = _intValues[i];
            var added = _intDomain.Add(value, 10);
            var subtracted = _intDomain.Subtract(added, 5);
            var floored = _intDomain.Floor(subtracted);
            var ceiled = _intDomain.Ceiling(floored);
            result += _intDomain.Distance(value, ceiled);
        }
        return result;
    }

    /// <summary>
    /// Mixed operations with DateTime day domain.
    /// Expected: Zero allocations, demonstrates DateTime-specific operation costs.
    /// </summary>
    [Benchmark]
    public long DateTimeDayDomain_HotLoop_MixedOperations()
    {
        long result = 0;
        for (int i = 0; i < IterationCount; i++)
        {
            var value = _dateTimeValues[i];
            var added = _dateTimeDayDomain.Add(value, 10);
            var subtracted = _dateTimeDayDomain.Subtract(added, 5);
            var floored = _dateTimeDayDomain.Floor(subtracted);
            var ceiled = _dateTimeDayDomain.Ceiling(floored);
            result += _dateTimeDayDomain.Distance(value, ceiled);
        }
        return result;
    }

    #endregion

    #region Range Manipulation in Loops

    /// <summary>
    /// Repeated range shifts in loop (common in sliding window algorithms).
    /// Expected: Allocates Range structs but on stack, no heap pressure.
    /// </summary>
    [Benchmark]
    public Range<int> IntegerDomain_HotLoop_RangeShift()
    {
        var range = Range.Closed<int>(0, 100);
        for (int i = 0; i < IterationCount; i++)
        {
            range = range.Shift(_intDomain, 1);
        }
        return range;
    }

    /// <summary>
    /// Repeated range expansions in loop.
    /// Expected: Stack-allocated Range structs, zero heap allocations.
    /// </summary>
    [Benchmark]
    public Range<int> IntegerDomain_HotLoop_RangeExpand()
    {
        var range = Range.Closed<int>(100, 200);
        for (int i = 0; i < IterationCount; i++)
        {
            range = range.Expand(_intDomain, left: 1, right: 1);
        }
        return range;
    }

    /// <summary>
    /// Repeated ExpandByRatio operations (requires Span calculation each time).
    /// Expected: Zero allocations, but multiple O(1) operations per iteration.
    /// </summary>
    [Benchmark]
    public Range<int> IntegerDomain_HotLoop_RangeExpandByRatio()
    {
        var range = Range.Closed<int>(100, 200);
        for (int i = 0; i < 100; i++) // Reduced iterations due to Span cost
        {
            range = range.ExpandByRatio(_intDomain, leftRatio: 0.1, rightRatio: 0.1);
        }
        return range;
    }

    #endregion

    #region Array Processing with Domains

    /// <summary>
    /// Process array of values with domain operations (simulates batch processing).
    /// Expected: Zero allocations, excellent cache locality with contiguous array.
    /// </summary>
    [Benchmark]
    public int[] IntegerDomain_HotLoop_ArrayProcessing()
    {
        var results = new int[IterationCount];
        for (int i = 0; i < IterationCount; i++)
        {
            results[i] = _intDomain.Add(_intValues[i], i % 100);
        }
        return results;
    }

    /// <summary>
    /// Process array of DateTime values with day domain.
    /// Expected: Zero allocations (domain is struct), DateTime values copied by value.
    /// </summary>
    [Benchmark]
    public DateTime[] DateTimeDayDomain_HotLoop_ArrayProcessing()
    {
        var results = new DateTime[IterationCount];
        for (int i = 0; i < IterationCount; i++)
        {
            results[i] = _dateTimeDayDomain.Add(_dateTimeValues[i], i % 100);
        }
        return results;
    }

    #endregion
}
