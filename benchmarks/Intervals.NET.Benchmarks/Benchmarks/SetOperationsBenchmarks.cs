using BenchmarkDotNet.Attributes;
using Intervals.NET.Benchmarks.Competitors;
using Intervals.NET.Extensions;
using Range = Intervals.NET.Factories.Range;

namespace Intervals.NET.Benchmarks;

/// <summary>
/// Benchmarks set operations: Intersect, Union, Except
/// 
/// RATIONALE:
/// - Set operations are fundamental to interval arithmetic
/// - Return types matter: nullable structs vs nullable classes
/// - Allocation behavior differs dramatically
/// - Common in scheduling, time management, range merging
/// 
/// ALLOCATION FOCUS:
/// - Intersect: returns Range&lt;T&gt;? (nullable struct - no heap allocation for the struct itself)
/// - Union: returns Range&lt;T&gt;? (nullable struct - no heap allocation for the struct itself)
/// - Except: returns IEnumerable&lt;Range&lt;T&gt;&gt; (0-2 elements, may allocate array)
/// 
/// COMPETITORS:
/// - NodaTime.Interval doesn't have built-in set ops, so we compare with Naive
/// </summary>
[MemoryDiagnoser]
[ThreadingDiagnoser]
public class SetOperationsBenchmarks
{
    private NaiveInterval _naiveRange1 = null!;
    private NaiveInterval _naiveRange2Overlapping = null!;
    private NaiveInterval _naiveRange2NonOverlapping = null!;

    private Range<int> _modernRange1;
    private Range<int> _modernRange2Overlapping;
    private Range<int> _modernRange2NonOverlapping;

    [GlobalSetup]
    public void Setup()
    {
        // Range1: [10, 50]
        _naiveRange1 = NaiveInterval.Closed(10, 50);
        _modernRange1 = Range.Closed<int>(10, 50);

        // Overlapping: [30, 70]
        _naiveRange2Overlapping = NaiveInterval.Closed(30, 70);
        _modernRange2Overlapping = Range.Closed<int>(30, 70);

        // Non-overlapping: [100, 150]
        _naiveRange2NonOverlapping = NaiveInterval.Closed(100, 150);
        _modernRange2NonOverlapping = Range.Closed<int>(100, 150);
    }

    #region Intersect - Overlapping Ranges

    /// <summary>
    /// Intersect with overlapping ranges: [10, 50] ∩ [30, 70] = [30, 50]
    /// Expected: Successful intersection
    /// Allocation: Naive allocates new object, Modern returns nullable struct
    /// </summary>
    [Benchmark(Baseline = true)]
    public void Naive_Intersect_Overlapping()
    {
        _ = _naiveRange1.Intersect(_naiveRange2Overlapping);
    }

    [Benchmark]
    public void IntervalsNet_Intersect_Overlapping()
    {
        _ = _modernRange1.Intersect(_modernRange2Overlapping);
    }

    /// <summary>
    /// Using operator & for intersection
    /// Tests operator overload performance
    /// </summary>
    [Benchmark]
    public void IntervalsNet_Intersect_Operator_Overlapping()
    {
        _ = _modernRange1 & _modernRange2Overlapping;
    }

    #endregion

    #region Intersect - Non-overlapping Ranges

    /// <summary>
    /// Intersect with non-overlapping ranges: [10, 50] ∩ [100, 150] = ∅
    /// Expected: null result
    /// Tests early-exit path
    /// </summary>
    [Benchmark]
    public void Naive_Intersect_NonOverlapping()
    {
        _ = _naiveRange1.Intersect(_naiveRange2NonOverlapping);
    }

    [Benchmark]
    public void IntervalsNet_Intersect_NonOverlapping()
    {
        _ = _modernRange1.Intersect(_modernRange2NonOverlapping);
    }

    #endregion

    #region Union - Overlapping Ranges

    /// <summary>
    /// Union with overlapping ranges: [10, 50] ∪ [30, 70] = [10, 70]
    /// Expected: Successful union
    /// </summary>
    [Benchmark]
    public void IntervalsNet_Union_Overlapping()
    {
        _ = _modernRange1.Union(_modernRange2Overlapping);
    }

    /// <summary>
    /// Using operator | for union
    /// </summary>
    [Benchmark]
    public void IntervalsNet_Union_Operator_Overlapping()
    {
        _ = _modernRange1 | _modernRange2Overlapping;
    }

    #endregion

    #region Union - Non-overlapping Ranges

    /// <summary>
    /// Union with non-overlapping ranges: [10, 50] ∪ [100, 150] = ∅
    /// Expected: null (can't form contiguous range)
    /// </summary>
    [Benchmark]
    public void IntervalsNet_Union_NonOverlapping()
    {
        _ = _modernRange1.Union(_modernRange2NonOverlapping);
    }

    #endregion

    #region Except - Overlapping Ranges

    /// <summary>
    /// Except with overlapping ranges: [10, 50] \ [30, 70] = [10, 30)
    /// Expected: 1 range (left portion)
    /// Allocation: Returns IEnumerable, may allocate array/list
    /// </summary>
    [Benchmark]
    public int IntervalsNet_Except_Overlapping_Count()
    {
        // Count forces enumeration
        return _modernRange1.Except(_modernRange2Overlapping).Count();
    }

    /// <summary>
    /// Except with fully contained range: [10, 50] \ [20, 30] = [10, 20) ∪ (30, 50]
    /// Expected: 2 ranges (split result)
    /// Tests maximum allocation case for Except
    /// </summary>
    [Benchmark]
    public int IntervalsNet_Except_Middle_Count()
    {
        var middle = Range.Closed<int>(20, 30);
        return _modernRange1.Except(middle).Count();
    }

    #endregion

    #region Overlaps Check

    /// <summary>
    /// Simple overlap check without creating intersection
    /// Cheaper than full intersection when only boolean result needed
    /// </summary>
    [Benchmark]
    public bool Naive_Overlaps()
    {
        return _naiveRange1.Overlaps(_naiveRange2Overlapping);
    }

    [Benchmark]
    public bool IntervalsNet_Overlaps()
    {
        return _modernRange1.Overlaps(_modernRange2Overlapping);
    }

    #endregion
}