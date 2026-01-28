using BenchmarkDotNet.Attributes;
using Intervals.NET.Benchmarks.Competitors;
using NodaTime;
using Range = Intervals.NET.Factories.Range;

namespace Intervals.NET.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks construction patterns for interval/range types.
/// 
/// RATIONALE:
/// - Construction is the entry point for all range operations
/// - Stack vs heap allocation has profound implications
/// - Tests common patterns: finite ranges, infinite bounds, mixed inclusivity
/// 
/// COMPETITORS:
/// - NaiveInterval: Typical class-based implementation (int-based)
/// - NodaTime.Interval: Industry-standard, class-based (Instant/DateTime-based)
/// - Intervals.NET: Zero-allocation struct-based design (both int and DateTime)
/// </summary>
[MemoryDiagnoser]
[ThreadingDiagnoser]
public class ConstructionBenchmarks
{
    private readonly Instant _instant1 = Instant.FromUnixTimeSeconds(1000000);
    private readonly Instant _instant2 = Instant.FromUnixTimeSeconds(2000000);
    private readonly DateTime _dateTime1 = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
    private readonly DateTime _dateTime2 = new DateTime(2024, 12, 31, 18, 0, 0, DateTimeKind.Utc);

    #region Integer Ranges

    /// <summary>
    /// Baseline: Closed finite range [10, 20] with integers
    /// Measures pure construction overhead for numeric types
    /// </summary>
    [Benchmark(Baseline = true)]
    public void Naive_Int_FiniteClosed()
    {
        _ = NaiveInterval.Closed(10, 20);
    }

    [Benchmark]
    public void IntervalsNet_Int_FiniteClosed()
    {
        _ = Range.Closed<int>(10, 20);
    }

    #endregion

    #region DateTime Ranges - Fair NodaTime Comparison

    /// <summary>
    /// DateTime/Instant range construction - fair comparison
    /// Both NodaTime.Interval and Range&lt;DateTime&gt; operate on date/time values
    /// Pre-constructed Instant/DateTime values to measure only interval construction cost
    /// </summary>
    [Benchmark]
    public void NodaTime_DateTime_FiniteClosed()
    {
        _ = new Interval(_instant1, _instant2);
    }

    [Benchmark]
    public void IntervalsNet_DateTime_FiniteClosed()
    {
        _ = Range.Closed<DateTime>(_dateTime1, _dateTime2);
    }

    #endregion

    #region Open and Half-Open Ranges (Int)

    /// <summary>
    /// Open range: (10, 20)
    /// Tests different bracket handling
    /// </summary>
    [Benchmark]
    public void Naive_Int_FiniteOpen()
    {
        _ = NaiveInterval.Open(10, 20);
    }

    [Benchmark]
    public void IntervalsNet_Int_FiniteOpen()
    {
        _ = Range.Open<int>(10, 20);
    }

    /// <summary>
    /// Half-open: [10, 20)
    /// Common pattern for array indexing, iteration
    /// </summary>
    [Benchmark]
    public void Naive_Int_HalfOpen()
    {
        _ = NaiveInterval.ClosedOpen(10, 20);
    }

    [Benchmark]
    public void IntervalsNet_Int_HalfOpen()
    {
        _ = Range.ClosedOpen<int>(10, 20);
    }

    #endregion

    #region Unbounded Ranges (Int)

    /// <summary>
    /// Unbounded range: (-∞, 100]
    /// Tests infinity representation overhead
    /// Naive uses nullable int (boxing potential)
    /// Intervals.NET uses dedicated RangeValue&lt;T&gt; struct
    /// </summary>
    [Benchmark]
    public void Naive_UnboundedStart()
    {
        _ = new NaiveInterval(null, 100, false, true);
    }

    [Benchmark]
    public void IntervalsNet_UnboundedStart()
    {
        _ = Range.Closed(RangeValue<int>.NegativeInfinity, 100);
    }

    /// <summary>
    /// Unbounded end: [0, +∞)
    /// Common for "at least N" validation
    /// </summary>
    [Benchmark]
    public void Naive_UnboundedEnd()
    {
        _ = new NaiveInterval(0, null, true, false);
    }

    [Benchmark]
    public void IntervalsNet_UnboundedEnd()
    {
        _ = Range.ClosedOpen(0, RangeValue<int>.PositiveInfinity);
    }

    /// <summary>
    /// Fully unbounded: (-∞, +∞)
    /// Edge case but important for completeness
    /// </summary>
    [Benchmark]
    public void Naive_FullyUnbounded()
    {
        _ = new NaiveInterval(null, null, false, false);
    }

    [Benchmark]
    public void IntervalsNet_FullyUnbounded()
    {
        _ = Range.Open(RangeValue<int>.NegativeInfinity, RangeValue<int>.PositiveInfinity);
    }

    #endregion
}