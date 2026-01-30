using BenchmarkDotNet.Attributes;
using Intervals.NET.Benchmarks.Competitors;
using Intervals.NET.Extensions;
using Range = Intervals.NET.Factories.Range;

namespace Intervals.NET.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks real-world usage patterns: sliding windows, validation loops, range checking.
/// 
/// RATIONALE:
/// - Microbenchmarks don't show cache effects, loop overhead, allocation pressure
/// - Real code operates on collections of ranges, sequential checks
/// - Struct vs class makes biggest difference in aggregate
/// 
/// SCENARIOS:
/// 1. Sliding window: Check N values against same range (hot loop)
/// 2. Sequential validation: Check values against sequence of ranges
/// 3. Range filtering: Filter collection based on overlap
/// 4. Time slot booking: Check availability across multiple time ranges
/// 
/// ALLOCATION FOCUS:
/// - Struct: zero allocations in loops (stack-allocated)
/// - Class: N allocations for N ranges, plus GC pressure
/// - Cache effects: struct array is contiguous, class array is pointer indirection
/// </summary>
[MemoryDiagnoser]
[ThreadingDiagnoser]
public class RealWorldScenariosBenchmarks
{
    private const int IterationCount = 1000;
    private const int RangeCount = 100;

    private int[] _values = null!;
    private NaiveInterval[] _naiveRanges = null!;
    private Range<int>[] _modernRanges = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42); // Deterministic seed

        // Generate test values
        _values = new int[IterationCount];
        for (int i = 0; i < IterationCount; i++)
        {
            _values[i] = random.Next(0, 1000);
        }

        // Generate ranges for filtering/validation scenarios
        _naiveRanges = new NaiveInterval[RangeCount];
        _modernRanges = new Range<int>[RangeCount];

        for (int i = 0; i < RangeCount; i++)
        {
            int start = i * 10;
            int end = start + 50;
            _naiveRanges[i] = NaiveInterval.Closed(start, end);
            _modernRanges[i] = Range.Closed<int>(start, end);
        }
    }

    #region Sliding Window - Single Range, Many Values

    /// <summary>
    /// Hot loop: Check 1000 values against the same range
    /// Simulates validation logic: "Is this value in acceptable range?"
    /// 
    /// Cache behavior:
    /// - Struct: range lives in register/L1 cache
    /// - Class: pointer dereference every access
    /// </summary>
    [Benchmark(Baseline = true)]
    public int Naive_SlidingWindow_SingleRange()
    {
        var range = NaiveInterval.Closed(100, 500);
        int count = 0;

        for (int i = 0; i < IterationCount; i++)
        {
            if (range.Contains(_values[i]))
            {
                count++;
            }
        }

        return count;
    }

    [Benchmark]
    public int IntervalsNet_SlidingWindow_SingleRange()
    {
        var range = Range.Closed<int>(100, 500);
        int count = 0;

        for (int i = 0; i < IterationCount; i++)
        {
            if (range.Contains(_values[i]))
            {
                count++;
            }
        }

        return count;
    }

    #endregion

    #region Sequential Validation - Many Ranges

    /// <summary>
    /// For each value, find which range (if any) contains it
    /// Simulates: routing, categorization, time slot assignment
    /// 
    /// Allocation analysis:
    /// - Naive: array of 100 class instances = 100 heap allocations
    /// - Modern: array of 100 structs = 1 array allocation, structs inline
    /// </summary>
    [Benchmark]
    public int Naive_SequentialValidation()
    {
        int matchCount = 0;

        for (int i = 0; i < _values.Length; i++)
        {
            for (int j = 0; j < _naiveRanges.Length; j++)
            {
                if (_naiveRanges[j].Contains(_values[i]))
                {
                    matchCount++;
                    break; // Found match, move to next value
                }
            }
        }

        return matchCount;
    }

    [Benchmark]
    public int IntervalsNet_SequentialValidation()
    {
        int matchCount = 0;

        for (int i = 0; i < _values.Length; i++)
        {
            for (int j = 0; j < _modernRanges.Length; j++)
            {
                if (_modernRanges[j].Contains(_values[i]))
                {
                    matchCount++;
                    break;
                }
            }
        }

        return matchCount;
    }

    #endregion

    #region Range Overlap Checking - Scheduling Scenario

    /// <summary>
    /// Check all pairs of ranges for overlaps
    /// Simulates: schedule conflict detection, resource booking
    /// 
    /// Complexity: O(n²) comparisons
    /// Tests: method call overhead, struct passing vs reference passing
    /// </summary>
    [Benchmark]
    public int Naive_OverlapDetection()
    {
        int overlapCount = 0;

        for (int i = 0; i < _naiveRanges.Length; i++)
        {
            for (int j = i + 1; j < _naiveRanges.Length; j++)
            {
                if (_naiveRanges[i].Overlaps(_naiveRanges[j]))
                {
                    overlapCount++;
                }
            }
        }

        return overlapCount;
    }

    [Benchmark]
    public int IntervalsNet_OverlapDetection()
    {
        int overlapCount = 0;

        for (int i = 0; i < _modernRanges.Length; i++)
        {
            for (int j = i + 1; j < _modernRanges.Length; j++)
            {
                if (_modernRanges[i].Overlaps(_modernRanges[j]))
                {
                    overlapCount++;
                }
            }
        }

        return overlapCount;
    }

    #endregion

    #region Range Intersection - Pure Operation

    /// <summary>
    /// Compute all pairwise intersections without collecting results
    /// Tests allocation behavior of intersection operation itself
    /// 
    /// Allocation impact:
    /// - Naive: Creates new class instance for each intersection result
    /// - Modern: Returns nullable struct (no heap allocation for the range itself)
    /// 
    /// Note: We count intersections rather than collecting them to isolate
    /// the allocation behavior of the Range/Interval types themselves,
    /// not the collection overhead.
    /// </summary>
    [Benchmark]
    public int Naive_ComputeIntersections()
    {
        int intersectionCount = 0;

        for (int i = 0; i < _naiveRanges.Length; i++)
        {
            for (int j = i + 1; j < _naiveRanges.Length; j++)
            {
                var intersection = _naiveRanges[i].Intersect(_naiveRanges[j]);
                if (intersection != null)
                {
                    intersectionCount++;
                    // Consume the result to prevent dead code elimination
                    _ = intersection.Start;
                }
            }
        }

        return intersectionCount;
    }

    [Benchmark]
    public int IntervalsNet_ComputeIntersections()
    {
        int intersectionCount = 0;

        for (int i = 0; i < _modernRanges.Length; i++)
        {
            for (int j = i + 1; j < _modernRanges.Length; j++)
            {
                var intersection = _modernRanges[i].Intersect(_modernRanges[j]);
                if (intersection.HasValue)
                {
                    intersectionCount++;
                    // Consume the result to prevent dead code elimination
                    _ = intersection.Value.Start;
                }
            }
        }

        return intersectionCount;
    }

    #endregion

    #region LINQ-Style Filtering

    /// <summary>
    /// Filter ranges using LINQ-like patterns
    /// Tests: boxing avoidance, allocation in functional pipelines
    /// </summary>
    [Benchmark]
    public int Naive_LINQ_FilterByValue()
    {
        int targetValue = 250;
        return _naiveRanges.Count(r => r.Contains(targetValue));
    }

    [Benchmark]
    public int IntervalsNet_LINQ_FilterByValue()
    {
        int targetValue = 250;
        return _modernRanges.Count(r => r.Contains(targetValue));
    }

    #endregion

    #region Batch Construction - Configuration Loading

    /// <summary>
    /// Simulate loading many ranges from configuration and storing them
    /// Tests: cumulative allocation overhead at scale
    /// 
    /// Real-world analogue: Loading business hours, price tiers, availability slots
    /// 
    /// Allocation behavior:
    /// - Naive: array (800 bytes) + 100 class instances (100 × ~32 bytes = 3200 bytes) = ~4000 bytes total
    /// - Modern: array (800 bytes) + structs inline (0 bytes) = ~800 bytes total
    /// 
    /// Note: We use a field to store results to ensure allocation is measured,
    /// then copy to local variable to prevent the field write from dominating the benchmark.
    /// </summary>
    private NaiveInterval[] _tempNaive = null!;
    private Range<int>[] _tempModern = null!;

    [Benchmark]
    public int Naive_BatchConstruction()
    {
        _tempNaive = new NaiveInterval[RangeCount];

        for (int i = 0; i < RangeCount; i++)
        {
            _tempNaive[i] = NaiveInterval.Closed(i * 10, i * 10 + 50);
        }

        // Return count to prevent dead code elimination
        return _tempNaive.Length;
    }

    [Benchmark]
    public int IntervalsNet_BatchConstruction()
    {
        _tempModern = new Range<int>[RangeCount];

        for (int i = 0; i < RangeCount; i++)
        {
            _tempModern[i] = Range.Closed<int>(i * 10, i * 10 + 50);
        }

        // Return count to prevent dead code elimination
        return _tempModern.Length;
    }

    #endregion
}
