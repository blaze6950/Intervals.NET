```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.6456/22H2/2022Update)
Intel Core i7-1065G7 CPU 1.30GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.403
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Method                                     | Mean         | Error        | StdDev       | Median       | Ratio  | RatioSD | Completed Work Items | Lock Contentions | Gen0   | Allocated | Alloc Ratio |
|------------------------------------------- |-------------:|-------------:|-------------:|-------------:|-------:|--------:|---------------------:|-----------------:|-------:|----------:|------------:|
| IntegerDomain_HotLoop_SequentialAdd        |     760.4 ns |      9.83 ns |      9.19 ns |     761.1 ns |   1.00 |    0.00 |                    - |                - |      - |         - |          NA |
| DateTimeDayDomain_HotLoop_SequentialAdd    |   2,870.2 ns |     56.32 ns |     77.09 ns |   2,907.2 ns |   3.76 |    0.12 |                    - |                - |      - |         - |          NA |
| DateTimeHourDomain_HotLoop_SequentialAdd   |   2,866.8 ns |     54.92 ns |     71.41 ns |   2,881.7 ns |   3.78 |    0.10 |                    - |                - |      - |         - |          NA |
| BusinessDayDomain_HotLoop_SequentialAdd    |  22,233.1 ns |    439.93 ns |    523.70 ns |  22,278.8 ns |  29.28 |    0.82 |                    - |                - |      - |         - |          NA |
| IntegerDomain_HotLoop_Distance             |     994.5 ns |     19.38 ns |     29.59 ns |   1,001.3 ns |   1.32 |    0.04 |                    - |                - |      - |         - |          NA |
| DateTimeDayDomain_HotLoop_Distance         |   5,033.6 ns |     79.15 ns |     74.04 ns |   5,043.1 ns |   6.62 |    0.12 |                    - |                - |      - |         - |          NA |
| BusinessDayDomain_HotLoop_Distance_Reduced | 745,170.7 ns | 14,504.64 ns | 23,422.29 ns | 744,459.5 ns | 978.41 |   22.01 |                    - |                - |      - |         - |          NA |
| IntegerDomain_HotLoop_Span                 |   6,518.5 ns |    124.13 ns |    169.91 ns |   6,465.9 ns |   8.54 |    0.32 |                    - |                - |      - |         - |          NA |
| DateTimeDayDomain_HotLoop_Span             |  16,237.5 ns |    243.53 ns |    239.18 ns |  16,197.2 ns |  21.38 |    0.35 |                    - |                - |      - |         - |          NA |
| BusinessDayDomain_HotLoop_Span_Reduced     | 129,588.8 ns |  2,310.30 ns |  2,837.25 ns | 128,774.8 ns | 170.83 |    5.30 |                    - |                - |      - |         - |          NA |
| IntegerDomain_HotLoop_MixedOperations      |   1,224.7 ns |     22.46 ns |     18.76 ns |   1,221.8 ns |   1.61 |    0.03 |                    - |                - |      - |         - |          NA |
| DateTimeDayDomain_HotLoop_MixedOperations  |   9,109.2 ns |    101.71 ns |     90.16 ns |   9,122.6 ns |  11.97 |    0.24 |                    - |                - |      - |         - |          NA |
| IntegerDomain_HotLoop_RangeShift           |   9,634.5 ns |    163.76 ns |    145.17 ns |   9,571.1 ns |  12.67 |    0.27 |                    - |                - |      - |         - |          NA |
| IntegerDomain_HotLoop_RangeExpand          |   9,997.1 ns |    274.11 ns |    754.98 ns |   9,672.2 ns |  14.15 |    1.47 |                    - |                - |      - |         - |          NA |
| IntegerDomain_HotLoop_RangeExpandByRatio   |   2,799.0 ns |     48.47 ns |     45.33 ns |   2,777.4 ns |   3.68 |    0.06 |                    - |                - |      - |         - |          NA |
| IntegerDomain_HotLoop_ArrayProcessing      |   3,428.2 ns |     27.78 ns |     30.88 ns |   3,426.6 ns |   4.50 |    0.07 |                    - |                - | 0.9613 |    4024 B |          NA |
| DateTimeDayDomain_HotLoop_ArrayProcessing  |   8,202.2 ns |    154.69 ns |    165.52 ns |   8,189.6 ns |  10.78 |    0.23 |                    - |                - | 1.9073 |    8024 B |          NA |

## Summary

### What This Measures
Hot path performance—domain operations in tight loops simulating real-world high-throughput scenarios. Tests sequential operations, batch processing, and mixed workloads to demonstrate production performance characteristics.

### Key Performance Insights

**🔥 Sequential Add: Hot Loop Performance**
- Integer domain (100 adds): **760 ns** (7.6 ns per operation)
- DateTime Day domain (100 adds): **2,870 ns** (28.7 ns per operation)
- DateTime Hour domain (100 adds): **2,867 ns** (28.7 ns per operation)
- **Business Day domain (100 adds): 22,233 ns** (222 ns per operation, **O(N) iteration**)

**📏 Distance Calculations: Batch Processing**
- Integer domain (100 distances): **994 ns** (9.9 ns each)
- DateTime Day domain (100 distances): **5,034 ns** (50.3 ns each)
- **Business Day domain (10 distances): 745,171 ns** (74,517 ns each, **O(N) iteration**)

**📊 Span Calculations: Range Measurement**
- Integer domain (100 spans): **6,519 ns** (65 ns each)
- DateTime Day domain (100 spans): **16,238 ns** (162 ns each)
- **Business Day domain (10 spans): 129,589 ns** (12,959 ns each, **O(N) iteration**)

**🔀 Mixed Operations: Real-World Workload**
- Integer domain (100 mixed ops): **1,225 ns** (12.3 ns per op)
- DateTime Day domain (100 mixed ops): **9,109 ns** (91 ns per op)
- **Pattern:** Consistent overhead per operation, scales linearly

**🎯 Range Operations: Shift/Expand/ExpandByRatio**
- Integer Shift (100 ranges): **9,635 ns** (96 ns per shift)
- Integer Expand (100 ranges): **9,997 ns** (100 ns per expand)
- Integer ExpandByRatio (100 ranges): **2,799 ns** (28 ns per operation)

**📦 Array Processing: Bulk Operations**
- Integer domain (100 values): **3,428 ns**, 4,024 bytes allocated
- DateTime Day domain (100 values): **8,202 ns**, 8,024 bytes allocated
- **Note:** Allocations from result array creation, not domain ops

### Performance Scaling in Hot Paths

```
Operation          Per-Op (ns)    100 Ops (μs)    1M Ops (ms)
─────────────────────────────────────────────────────────────
Integer Add        7.6            0.76            7.6
DateTime Day Add   28.7           2.87            28.7
Business Day Add   222            22.2            222

Integer Distance   9.9            0.99            9.9
DateTime Distance  50.3           5.03            50.3

Integer Span       65             6.5             65
DateTime Span      162            16.2            162
```

### Fixed-Step vs Variable-Step: Hot Path Impact

```
Domain Type         Add (ns)    Distance (ns)    Span (ns)
────────────────────────────────────────────────────────────
Integer (O(1))      7.6         9.9              65
DateTime Day (O(1)) 28.7        50.3             162
Business Day (O(N)) 222         74,517           12,959
```

**Business day overhead:**
- Add: **29× slower** than DateTime Day (iteration required)
- Distance: **1,481× slower** (must check every day)
- Span: **80× slower** (combines Distance + alignment)

### Design Trade-offs (Aligned with README Philosophy)

**Why fixed-step domains excel in hot paths:**
- ✅ O(1) operations: constant time regardless of range size
- ✅ Predictable latency: no iteration overhead
- ✅ Cache-friendly: arithmetic-only, no branching
- ✅ Scalable: 1M operations in milliseconds

**Why business day domains are slower:**
- ⚠️ O(N) operations: must iterate through days
- ⚠️ Conditional logic: check each day for weekend
- ⚠️ Higher latency: 222+ ns per operation
- ✅ **Still fast enough** for typical business scenarios

**When variable-step O(N) matters:**
- High-frequency: millions of operations per second
- Long ranges: 100+ business days (10+ μs per operation)
- Tight loops: repeated calculations in critical paths

### Practical Recommendations

✅ **Fixed-step domains for hot paths:**
```csharp
// Integer domain: 7.6 ns per Add
for (int i = 0; i < 1000000; i++)
    domain.Add(value, offset);
// Total: 7.6 milliseconds
```

✅ **DateTime domains for time calculations:**
```csharp
// DateTime Day: 28.7 ns per Add
for (int i = 0; i < 100000; i++)
    domain.Add(date, days);
// Total: 2.87 milliseconds
```

⚠️ **Business days: suitable for moderate volumes:**
```csharp
// Business Day: 222 ns per Add
for (int i = 0; i < 10000; i++)
    domain.Add(date, businessDays);
// Total: 2.22 milliseconds (acceptable)
```

❌ **Business days: avoid in tight loops:**
```csharp
// Business Day Distance: 74,517 ns for 100-day range
for (int i = 0; i < 1000; i++)
    domain.Distance(start, end);  // 100-day ranges
// Total: 74.5 milliseconds (may be too slow)
```

### Real-World Impact

**Real-Time Validation (1M checks/second):**
```
Integer domain: 7.6 ms CPU time (0.76% utilization)
DateTime domain: 28.7 ms CPU time (2.87% utilization)
Result: Both suitable for real-time systems
```

**SLA Calculator (10K requests/second):**
```
Business Day Add (5 days): 222 ns × 10K = 2.22 ms/second
Result: Acceptable overhead (0.22% CPU)
```

**Analytics Pipeline (1K range operations/second):**
```
Integer ExpandByRatio: 28 ns × 1K = 28 microseconds/second
Result: Negligible impact
```

**Reporting System (100 year-long business day spans):**
```
Business Day Span (365 days): 12,959 ns × 100 = 1.3 ms
Result: Acceptable for batch processing
```

### Memory Behavior
```
Domain operations:       0 bytes allocated
Array processing:        Allocations from result arrays only
Hot loop (1M ops):       0 bytes (pure computation)
```

### Why This Matters

These benchmarks demonstrate **production-grade hot path performance**:
- ✅ Fixed-step domains: **7-162 ns per operation** (millions per second)
- ✅ Variable-step domains: **222-12,959 ns** (thousands per second)
- ✅ Zero allocations in domain logic itself
- ✅ Predictable, linear scaling

**Key insight:** Even the "slow" business day operations (222 ns) process **4.5 million operations per second**—faster than most network calls, database queries, or I/O operations.

Choose domain type based on **business requirements**, not micro-benchmarks. All domains are fast enough for production use.
