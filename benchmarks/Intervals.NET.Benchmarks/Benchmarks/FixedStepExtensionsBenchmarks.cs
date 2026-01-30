using BenchmarkDotNet.Attributes;
using Intervals.NET.Domain.Default.Numeric;
using Intervals.NET.Domain.Default.DateTime;
using Intervals.NET.Domain.Extensions;
using Intervals.NET.Domain.Extensions.Fixed;
using Range = Intervals.NET.Factories.Range;

namespace Intervals.NET.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks fixed-step domain extension methods: Span, Shift, Expand, ExpandByRatio.
/// 
/// RATIONALE:
/// - Extension methods provide high-level range manipulation operations
/// - All operations should be O(1) for fixed-step domains
/// - Performance should NOT scale with range size (constant time)
/// - Zero allocations expected for struct-based operations
/// 
/// KEY INSIGHT:
/// - Small range [1, 10]: span = 10
/// - Medium range [1, 1000]: span = 1000
/// - Large range [1, 1000000]: span = 1000000
/// - All should have IDENTICAL performance (O(1) guarantee)
/// 
/// FOCUS:
/// - Span calculation (count steps in range)
/// - Shift operation (move range boundaries)
/// - Expand operation (widen range by fixed amounts)
/// - ExpandByRatio operation (proportional expansion)
/// </summary>
[MemoryDiagnoser]
[ThreadingDiagnoser]
public class FixedStepExtensionsBenchmarks
{
    private readonly IntegerFixedStepDomain _intDomain = new();
    private readonly DateTimeDayFixedStepDomain _dateTimeDomain = new();

    // Integer ranges - different sizes to prove O(1)
    private Range<int> _intRangeSmall;      // [1, 10]
    private Range<int> _intRangeMedium;     // [1, 1000]
    private Range<int> _intRangeLarge;      // [1, 1000000]

    // DateTime ranges - different sizes
    private Range<DateTime> _dateRangeSmall;   // 10 days
    private Range<DateTime> _dateRangeMedium;  // 365 days
    private Range<DateTime> _dateRangeLarge;   // 10 years

    [GlobalSetup]
    public void Setup()
    {
        // Integer ranges
        _intRangeSmall = Range.Closed<int>(1, 10);
        _intRangeMedium = Range.Closed<int>(1, 1000);
        _intRangeLarge = Range.Closed<int>(1, 1000000);

        // DateTime ranges
        var start = new DateTime(2024, 1, 1);
        _dateRangeSmall = Range.Closed<DateTime>(start, start.AddDays(10));
        _dateRangeMedium = Range.Closed<DateTime>(start, start.AddDays(365));
        _dateRangeLarge = Range.Closed<DateTime>(start, start.AddYears(10));
    }

    #region Span Operations - O(1) Regardless of Range Size

    /// <summary>
    /// Baseline: Span calculation for small integer range [1, 10].
    /// Expected: O(1), zero allocations, ~10 steps.
    /// </summary>
    [Benchmark(Baseline = true)]
    public long IntegerDomain_Span_Small()
    {
        return _intRangeSmall.Span(_intDomain).Value;
    }

    /// <summary>
    /// Span calculation for medium integer range [1, 1000].
    /// Expected: O(1), zero allocations, SAME TIME as small range (proves O(1)).
    /// </summary>
    [Benchmark]
    public long IntegerDomain_Span_Medium()
    {
        return _intRangeMedium.Span(_intDomain).Value;
    }

    /// <summary>
    /// Span calculation for large integer range [1, 1000000].
    /// Expected: O(1), zero allocations, SAME TIME as small/medium (proves O(1)).
    /// </summary>
    [Benchmark]
    public long IntegerDomain_Span_Large()
    {
        return _intRangeLarge.Span(_intDomain).Value;
    }

    /// <summary>
    /// Span calculation for small DateTime range (10 days).
    /// Expected: O(1), zero allocations.
    /// </summary>
    [Benchmark]
    public long DateTimeDomain_Span_Small()
    {
        return _dateRangeSmall.Span(_dateTimeDomain).Value;
    }

    /// <summary>
    /// Span calculation for medium DateTime range (365 days).
    /// Expected: O(1), zero allocations, SAME TIME as small range.
    /// </summary>
    [Benchmark]
    public long DateTimeDomain_Span_Medium()
    {
        return _dateRangeMedium.Span(_dateTimeDomain).Value;
    }

    /// <summary>
    /// Span calculation for large DateTime range (10 years ≈ 3650 days).
    /// Expected: O(1), zero allocations, SAME TIME as small/medium range.
    /// </summary>
    [Benchmark]
    public long DateTimeDomain_Span_Large()
    {
        return _dateRangeLarge.Span(_dateTimeDomain).Value;
    }

    #endregion

    #region Shift Operations - O(1) Expected

    /// <summary>
    /// Shift small integer range by 5 steps.
    /// Expected: O(1), creates new Range struct (stack allocated).
    /// </summary>
    [Benchmark]
    public Range<int> IntegerDomain_Shift_Small()
    {
        return _intRangeSmall.Shift(_intDomain, 5);
    }

    /// <summary>
    /// Shift medium integer range by 5 steps.
    /// Expected: O(1), SAME TIME as small (range size doesn't matter).
    /// </summary>
    [Benchmark]
    public Range<int> IntegerDomain_Shift_Medium()
    {
        return _intRangeMedium.Shift(_intDomain, 5);
    }

    /// <summary>
    /// Shift large integer range by 5 steps.
    /// Expected: O(1), SAME TIME as small/medium.
    /// </summary>
    [Benchmark]
    public Range<int> IntegerDomain_Shift_Large()
    {
        return _intRangeLarge.Shift(_intDomain, 5);
    }

    /// <summary>
    /// Shift DateTime range by 7 days.
    /// Expected: O(1), zero additional heap allocations.
    /// </summary>
    [Benchmark]
    public Range<DateTime> DateTimeDomain_Shift()
    {
        return _dateRangeMedium.Shift(_dateTimeDomain, 7);
    }

    #endregion

    #region Expand Operations - O(1) Expected

    /// <summary>
    /// Expand small integer range by 2 on left, 3 on right.
    /// Expected: O(1), creates new Range struct.
    /// </summary>
    [Benchmark]
    public Range<int> IntegerDomain_Expand_Small()
    {
        return _intRangeSmall.Expand(_intDomain, left: 2, right: 3);
    }

    /// <summary>
    /// Expand medium integer range by 2 on left, 3 on right.
    /// Expected: O(1), SAME TIME as small.
    /// </summary>
    [Benchmark]
    public Range<int> IntegerDomain_Expand_Medium()
    {
        return _intRangeMedium.Expand(_intDomain, left: 2, right: 3);
    }

    /// <summary>
    /// Expand large integer range by 2 on left, 3 on right.
    /// Expected: O(1), SAME TIME as small/medium.
    /// </summary>
    [Benchmark]
    public Range<int> IntegerDomain_Expand_Large()
    {
        return _intRangeLarge.Expand(_intDomain, left: 2, right: 3);
    }

    /// <summary>
    /// Expand DateTime range by 5 days on each side.
    /// Expected: O(1), zero additional heap allocations.
    /// </summary>
    [Benchmark]
    public Range<DateTime> DateTimeDomain_Expand()
    {
        return _dateRangeMedium.Expand(_dateTimeDomain, left: 5, right: 5);
    }

    #endregion

    #region ExpandByRatio Operations - O(1) Expected

    /// <summary>
    /// ExpandByRatio for small integer range (50% on each side).
    /// Expected: O(1), requires Span calculation but still constant time.
    /// </summary>
    [Benchmark]
    public Range<int> IntegerDomain_ExpandByRatio_Small()
    {
        return _intRangeSmall.ExpandByRatio(_intDomain, leftRatio: 0.5, rightRatio: 0.5);
    }

    /// <summary>
    /// ExpandByRatio for medium integer range (50% on each side).
    /// Expected: O(1), SAME TIME as small (Span is O(1) for fixed-step).
    /// </summary>
    [Benchmark]
    public Range<int> IntegerDomain_ExpandByRatio_Medium()
    {
        return _intRangeMedium.ExpandByRatio(_intDomain, leftRatio: 0.5, rightRatio: 0.5);
    }

    /// <summary>
    /// ExpandByRatio for large integer range (50% on each side).
    /// Expected: O(1), SAME TIME as small/medium.
    /// </summary>
    [Benchmark]
    public Range<int> IntegerDomain_ExpandByRatio_Large()
    {
        return _intRangeLarge.ExpandByRatio(_intDomain, leftRatio: 0.5, rightRatio: 0.5);
    }

    /// <summary>
    /// ExpandByRatio for DateTime range (20% on each side).
    /// Expected: O(1), zero additional heap allocations.
    /// </summary>
    [Benchmark]
    public Range<DateTime> DateTimeDomain_ExpandByRatio()
    {
        return _dateRangeMedium.ExpandByRatio(_dateTimeDomain, leftRatio: 0.2, rightRatio: 0.2);
    }

    #endregion

    #region Asymmetric Operations

    /// <summary>
    /// Asymmetric expansion: 10% left, 30% right.
    /// Expected: O(1), tests asymmetric ratio handling.
    /// </summary>
    [Benchmark]
    public Range<int> IntegerDomain_ExpandByRatio_Asymmetric()
    {
        return _intRangeMedium.ExpandByRatio(_intDomain, leftRatio: 0.1, rightRatio: 0.3);
    }

    /// <summary>
    /// Negative expansion (contraction): -20% on each side.
    /// Expected: O(1), tests contraction behavior.
    /// </summary>
    [Benchmark]
    public Range<int> IntegerDomain_ExpandByRatio_Contraction()
    {
        return _intRangeMedium.ExpandByRatio(_intDomain, leftRatio: -0.2, rightRatio: -0.2);
    }

    #endregion
}
