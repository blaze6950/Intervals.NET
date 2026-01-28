using BenchmarkDotNet.Attributes;
using Intervals.NET.Benchmarks.Competitors;
using Intervals.NET.Extensions;
using NodaTime;
using Range = Intervals.NET.Factories.Range;

namespace Intervals.NET.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks containment checks - one of the most common operations.
/// 
/// RATIONALE:
/// - Contains() is typically in hot paths (validation, filtering)
/// - Branch misprediction matters here
/// - Struct vs class affects cache locality
/// 
/// SCENARIOS:
/// - Value inside range (true path)
/// - Value outside range (false path)  
/// - Value at boundary (inclusive/exclusive edge cases)
/// - Range containment
/// 
/// ANTI-CONSTANT-FOLDING:
/// - Values come from fields to prevent compile-time optimization
/// </summary>
[MemoryDiagnoser]
[ThreadingDiagnoser]
public class ContainmentBenchmarks
{
    private NaiveInterval _naiveRange = null!;
    private Range<int> _modernRange;
    private Interval _nodaInterval;

    // Values designed to prevent constant folding
    private int _valueInside;
    private int _valueOutside;
    private int _valueBoundary;

    private NaiveInterval _naiveInner = null!;
    private Range<int> _modernInner;

    [GlobalSetup]
    public void Setup()
    {
        _naiveRange = NaiveInterval.Closed(10, 100);
        _modernRange = Range.Closed<int>(10, 100);

        var start = Instant.FromUnixTimeSeconds(10);
        var end = Instant.FromUnixTimeSeconds(100);
        _nodaInterval = new Interval(start, end);

        _valueInside = 50;
        _valueOutside = 200;
        _valueBoundary = 10;

        _naiveInner = NaiveInterval.Closed(20, 80);
        _modernInner = Range.Closed<int>(20, 80);
    }

    /// <summary>
    /// Hot path: value clearly inside range
    /// Most common case in validation scenarios
    /// </summary>
    [Benchmark(Baseline = true)]
    public bool Naive_Contains_Inside()
    {
        return _naiveRange.Contains(_valueInside);
    }

    [Benchmark]
    public bool IntervalsNet_Contains_Inside()
    {
        return _modernRange.Contains(_valueInside);
    }

    /// <summary>
    /// Value outside range - should fail fast
    /// Branch predictor should optimize for this in validation code
    /// </summary>
    [Benchmark]
    public bool Naive_Contains_Outside()
    {
        return _naiveRange.Contains(_valueOutside);
    }

    [Benchmark]
    public bool IntervalsNet_Contains_Outside()
    {
        return _modernRange.Contains(_valueOutside);
    }

    /// <summary>
    /// Boundary check - tests inclusive/exclusive logic
    /// Edge case but important for correctness
    /// </summary>
    [Benchmark]
    public bool Naive_Contains_Boundary()
    {
        return _naiveRange.Contains(_valueBoundary);
    }

    [Benchmark]
    public bool IntervalsNet_Contains_Boundary()
    {
        return _modernRange.Contains(_valueBoundary);
    }

    /// <summary>
    /// Range containment: checks if one range fully contains another
    /// More complex logic, multiple comparisons
    /// </summary>
    [Benchmark]
    public bool Naive_Contains_Range()
    {
        // Naive doesn't have Contains(range), manual implementation
        return (_naiveInner.Start ?? int.MinValue) >= (_naiveRange.Start ?? int.MinValue) &&
               (_naiveInner.End ?? int.MaxValue) <= (_naiveRange.End ?? int.MaxValue);
    }

    [Benchmark]
    public bool IntervalsNet_Contains_Range()
    {
        return _modernRange.Contains(_modernInner);
    }

    /// <summary>
    /// NodaTime comparison for DateTime-like scenarios
    /// Note: NodaTime.Interval.Contains() is for Instant, not Interval
    /// So we test the comparable scenario
    /// </summary>
    [Benchmark]
    public bool NodaTime_Contains_Instant()
    {
        var instant = Instant.FromUnixTimeSeconds(_valueInside);
        return _nodaInterval.Contains(instant);
    }
}