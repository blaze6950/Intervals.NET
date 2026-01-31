using BenchmarkDotNet.Attributes;
using Intervals.NET.Data;
using Intervals.NET.Data.Extensions;
using Intervals.NET.Domain.Default.Numeric;
using System.Linq;
using Range = Intervals.NET.Factories.Range;

namespace Intervals.NET.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class RangeDataBenchmarks
{
    [Params(10, 1000, 100000)]
    public int N;

    private readonly IntegerFixedStepDomain _domain = new();
    private Range<int> _fullRange;
    private RangeData<int, int, IntegerFixedStepDomain> _rangeData;
    private int[] _backingArray = null!;
    private int _midPoint;

    [GlobalSetup]
    public void Setup()
    {
        _fullRange = Range.Closed<int>(0, N - 1);
        _backingArray = Enumerable.Range(0, N).ToArray();
        _rangeData = new RangeData<int, int, IntegerFixedStepDomain>(_fullRange, _backingArray, _domain);
        _midPoint = N / 2;
    }

    [Benchmark(Baseline = true)]
    public RangeData<int, int, IntegerFixedStepDomain> Construction()
    {
        // Materialize a fresh array to measure construction cost
        var data = Enumerable.Range(0, N).ToArray();
        return new RangeData<int, int, IntegerFixedStepDomain>(Range.Closed<int>(0, N - 1), data, _domain);
    }

    [Benchmark]
    public bool TryGet_Hit()
    {
        return _rangeData.TryGet(_midPoint, out _);
    }

    [Benchmark]
    public bool TryGet_Miss()
    {
        // Point outside the range
        return _rangeData.TryGet(N + 10, out _);
    }

    [Benchmark]
    public int Indexer_Hit()
    {
        return _rangeData[_midPoint];
    }

    [Benchmark]
    public RangeData<int, int, IntegerFixedStepDomain> Slice_Small()
    {
        var sub = Range.Closed<int>(_midPoint, Math.Min(_midPoint + 4, N - 1));
        return _rangeData.Slice(sub);
    }

    [Benchmark]
    public RangeData<int, int, IntegerFixedStepDomain> Slice_Medium()
    {
        var len = Math.Max(1, N / 10);
        var sub = Range.Closed<int>(_midPoint, Math.Min(_midPoint + len - 1, N - 1));
        return _rangeData.Slice(sub);
    }

    [Benchmark]
    public int Iterate_First100()
    {
        var sum = 0;
        foreach (var v in _rangeData.Data.Take(100))
        {
            sum += v;
        }

        return sum;
    }
}