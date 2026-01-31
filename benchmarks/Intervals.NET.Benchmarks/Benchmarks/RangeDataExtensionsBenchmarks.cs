using BenchmarkDotNet.Attributes;
using Intervals.NET.Data;
using Intervals.NET.Data.Extensions;
using Intervals.NET.Domain.Default.Numeric;
using System.Linq;
using Range = Intervals.NET.Factories.Range;

namespace Intervals.NET.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class RangeDataExtensionsBenchmarks
{
    private IntegerFixedStepDomain _domain = new();
    private RangeData<int, int, IntegerFixedStepDomain> _left = null!;
    private RangeData<int, int, IntegerFixedStepDomain> _right = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Left: [0, 999]
        var leftRange = Range.Closed<int>(0, 999);
        var leftData = Enumerable.Range(0, 1000).ToArray();
        _left = new RangeData<int, int, IntegerFixedStepDomain>(leftRange, leftData, _domain);

        // Right: [500, 1499]
        var rightRange = Range.Closed<int>(500, 1499);
        var rightData = Enumerable.Range(500, 1000).ToArray();
        _right = new RangeData<int, int, IntegerFixedStepDomain>(rightRange, rightData, _domain);
    }

    [Benchmark(Baseline = true)]
    public RangeData<int, int, IntegerFixedStepDomain>? Intersect()
    {
        return _left.Intersect(_right);
    }

    [Benchmark]
    public RangeData<int, int, IntegerFixedStepDomain>? Union()
    {
        return _left.Union(_right);
    }

    [Benchmark]
    public RangeData<int, int, IntegerFixedStepDomain>? TrimStart()
    {
        // Trim start to 250 => resulting range [250, 999]
        return _left.TrimStart(250);
    }

    [Benchmark]
    public int Union_Enumerate()
    {
        var u = _left.Union(_right);
        if (u is null) return -1;
        var sum = 0;
        foreach (var v in u.Data)
            sum += v;
        return sum;
    }
}
