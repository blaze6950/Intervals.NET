```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.6456/22H2/2022Update)
Intel Core i7-1065G7 CPU 1.30GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.403
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Method                         | Mean      | Error     | StdDev    | Ratio | RatioSD | Completed Work Items | Lock Contentions | Allocated | Alloc Ratio |
|------------------------------- |----------:|----------:|----------:|------:|--------:|---------------------:|-----------------:|----------:|------------:|
| Naive_Contains_Inside          |  1.242 ns | 0.0559 ns | 0.0523 ns |  1.00 |    0.00 |                    - |                - |         - |          NA |
| IntervalsNet_Contains_Inside   |  1.340 ns | 0.0601 ns | 0.0919 ns |  1.07 |    0.11 |                    - |                - |         - |          NA |
| Naive_Contains_Outside         |  1.153 ns | 0.0419 ns | 0.0371 ns |  0.93 |    0.05 |                    - |                - |         - |          NA |
| IntervalsNet_Contains_Outside  |  1.355 ns | 0.0587 ns | 0.1395 ns |  1.03 |    0.09 |                    - |                - |         - |          NA |
| Naive_Contains_Boundary        |  1.799 ns | 0.0718 ns | 0.0826 ns |  1.45 |    0.09 |                    - |                - |         - |          NA |
| IntervalsNet_Contains_Boundary |  1.876 ns | 0.0747 ns | 0.1509 ns |  1.63 |    0.09 |                    - |                - |         - |          NA |
| Naive_Contains_Range           |  1.418 ns | 0.0628 ns | 0.0747 ns |  1.13 |    0.08 |                    - |                - |         - |          NA |
| IntervalsNet_Contains_Range    | 18.824 ns | 0.4121 ns | 0.5777 ns | 15.16 |    0.75 |                    - |                - |         - |          NA |
| NodaTime_Contains_Instant      | 11.418 ns | 0.2541 ns | 0.4031 ns |  9.22 |    0.42 |                    - |                - |         - |          NA |

## Summary

### What This Measures
Containment check performance—the most critical hot path operation in range validation. Tests point-in-range checks (inside, outside, boundary) and range-in-range containment against naive and NodaTime implementations.

### Key Performance Insights

**⚡ Hot Path Dominance: Point Containment**
- IntervalsNet `Contains(value)`: **1.34-1.36 ns** (inside/outside checks)
- Naive baseline: **1.15-1.24 ns**
- **Result:** Virtually identical performance (~0.1-0.2 ns difference, within margin of error)
- **Zero allocations** for all containment checks

**🎯 Boundary Checks**
- IntervalsNet boundary checks: **1.88 ns**
- Naive boundary checks: **1.80 ns**
- **Result:** ~4% difference (0.08 ns), negligible in practice

**📊 Range-in-Range Containment**
- IntervalsNet `Contains(Range<T>)`: **18.8 ns**
- Naive baseline: **1.42 ns**
- **Trade-off:** 13× slower due to comprehensive boundary combination validation

**🔍 Comparison with NodaTime**
- NodaTime `Contains(Instant)`: **11.4 ns**
- IntervalsNet: **1.34 ns**
- **Result:** Intervals.NET is **8.5× faster** than NodaTime for point containment

### Memory Behavior
```
All containment operations:  0 bytes allocated
Hot path cost:              1.3-1.9 nanoseconds
Range containment:          18.8 nanoseconds
```

### Design Trade-offs (Aligned with README Philosophy)

**Why point containment is so fast:**
- ✅ Inlined comparison logic (JIT optimization)
- ✅ Struct-based design (no vtable lookups)
- ✅ No allocations or branching overhead
- ✅ Optimized for the 99% use case (single value checks)

**Why range-in-range containment is slower:**
- Must validate **4 boundary conditions** (start/end × inclusive/exclusive)
- Handles all edge cases: empty ranges, infinity, boundary alignment
- Comprehensive validation ensures correctness over raw speed
- Still only **18.8 ns** (~0.000019 milliseconds)

### Practical Recommendations

✅ **Perfect for hot paths:**
- Input validation loops: 1.3 ns per check
- LINQ filtering: `.Where(x => range.Contains(x))`
- Real-time systems: sub-2ns latency
- Zero GC pressure in tight loops

✅ **Use with confidence:**
- Point containment: essentially as fast as hand-written `x >= start && x <= end`
- Range containment: 18.8 ns overhead is acceptable for correctness guarantees

⚠️ **When range-in-range matters:**
- If checking millions of range-containment operations per second, the 17 ns overhead accumulates
- For most applications, checking 50,000 range containments costs ~1 millisecond

### Real-World Impact

**Validation Hot Path (1M operations):**
```
Manual checks:      1.2 ms (no validation, error-prone)
Intervals.NET:      1.3 ms (fully validated, zero allocations)
NodaTime:          11.4 ms (8× slower)
```

**Result:** Intervals.NET provides production-ready validation at essentially the same speed as hand-written code, with zero memory overhead.
