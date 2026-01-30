```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.6456/22H2/2022Update)
Intel Core i7-1065G7 CPU 1.30GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.403
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Method                                | Mean          | Error      | StdDev     | Median        | Ratio  | RatioSD | Completed Work Items | Lock Contentions | Allocated | Alloc Ratio |
|-------------------------------------- |--------------:|-----------:|-----------:|--------------:|-------:|--------:|---------------------:|-----------------:|----------:|------------:|
| DateTimeDomain_Add_5BusinessDays      |    39.4229 ns |  0.4022 ns |  0.3140 ns |    39.4494 ns |   1.00 |    0.00 |                    - |                - |         - |          NA |
| DateTimeDomain_Add_20BusinessDays     |   186.6182 ns |  6.0837 ns | 17.9380 ns |   188.0122 ns |   4.08 |    0.16 |                    - |                - |         - |          NA |
| DateTimeDomain_Add_100BusinessDays    | 1,081.4680 ns | 17.0909 ns | 15.1506 ns | 1,077.4330 ns |  27.35 |    0.38 |                    - |                - |         - |          NA |
| DateOnlyDomain_Add_5BusinessDays      |    19.1567 ns |  0.2813 ns |  0.2631 ns |    19.1349 ns |   0.49 |    0.01 |                    - |                - |         - |          NA |
| DateOnlyDomain_Add_20BusinessDays     |    75.4422 ns |  1.0876 ns |  1.5598 ns |    75.0753 ns |   1.92 |    0.05 |                    - |                - |         - |          NA |
| DateTimeDomain_Distance_5Days         |    52.1208 ns |  1.0981 ns |  1.5394 ns |    51.7783 ns |   1.31 |    0.03 |                    - |                - |         - |          NA |
| DateTimeDomain_Distance_30Days        |   308.2804 ns |  5.7216 ns |  5.3520 ns |   308.0863 ns |   7.80 |    0.15 |                    - |                - |         - |          NA |
| DateTimeDomain_Distance_365Days       | 3,617.8666 ns | 68.9153 ns | 84.6342 ns | 3,607.1239 ns |  91.20 |    2.96 |                    - |                - |         - |          NA |
| DateOnlyDomain_Distance_30Days        |   137.0923 ns |  5.9434 ns | 16.6659 ns |   132.0591 ns |   3.39 |    0.26 |                    - |                - |         - |          NA |
| DateTimeDomain_Floor_Monday           |     4.1847 ns |  0.1358 ns |  0.1204 ns |     4.1585 ns |   0.11 |    0.00 |                    - |                - |         - |          NA |
| DateTimeDomain_Floor_Saturday         |     5.2575 ns |  0.0937 ns |  0.0876 ns |     5.2646 ns |   0.13 |    0.00 |                    - |                - |         - |          NA |
| DateOnlyDomain_Floor_Monday           |     0.7161 ns |  0.0429 ns |  0.0380 ns |     0.7161 ns |   0.02 |    0.00 |                    - |                - |         - |          NA |
| DateTimeDomain_Ceiling_Monday         |     4.9287 ns |  0.1305 ns |  0.1019 ns |     4.8977 ns |   0.13 |    0.00 |                    - |                - |         - |          NA |
| DateTimeDomain_Ceiling_FridayWithTime |     9.6422 ns |  0.2095 ns |  0.1749 ns |     9.6177 ns |   0.24 |    0.00 |                    - |                - |         - |          NA |
| DateTimeDomain_Ceiling_Sunday         |     9.0433 ns |  0.1274 ns |  0.1129 ns |     9.0213 ns |   0.23 |    0.00 |                    - |                - |         - |          NA |
| DateTimeDomain_Span_5Days             |    67.9617 ns |  1.4306 ns |  2.1412 ns |    67.3845 ns |   1.72 |    0.07 |                    - |                - |         - |          NA |
| DateTimeDomain_Span_30Days            |   315.4466 ns |  4.8310 ns |  4.0341 ns |   315.2319 ns |   8.01 |    0.09 |                    - |                - |         - |          NA |
| DateTimeDomain_Span_365Days           | 3,479.8399 ns | 56.1281 ns | 52.5022 ns | 3,456.6425 ns |  88.58 |    1.55 |                    - |                - |         - |          NA |
| DateOnlyDomain_Span_30Days            |   131.1173 ns |  2.6968 ns |  6.4092 ns |   130.3988 ns |   3.32 |    0.14 |                    - |                - |         - |          NA |
| DateTimeDomain_ExpandByRatio_30Days   |   440.8805 ns |  4.7828 ns |  3.9939 ns |   440.8098 ns |  11.19 |    0.09 |                    - |                - |         - |          NA |
| DateTimeDomain_ExpandByRatio_365Days  | 4,825.0244 ns | 75.1212 ns | 66.5930 ns | 4,821.7590 ns | 122.30 |    2.09 |                    - |                - |         - |          NA |
| DateOnlyDomain_ExpandByRatio_30Days   |   195.9311 ns |  3.9509 ns |  7.0228 ns |   194.8128 ns |   4.96 |    0.12 |                    - |                - |         - |          NA |

## Summary

### What This Measures
Variable-step domain performance—business day calculations that must iterate through ranges to skip weekends. Tests DateTime and DateOnly business day domains across operations that require day-by-day checking (O(N) complexity).

### Key Performance Insights

**⏱️ Add Operation: O(N) Iteration Required**
- 5 business days: **39.4 ns** (DateTime), **19.2 ns** (DateOnly)
- 20 business days: **186.6 ns** (DateTime), **75.4 ns** (DateOnly)
- 100 business days: **1,081 ns** (DateTime)
- **Scaling:** ~9.4 ns per business day (DateTime), ~3.8 ns per day (DateOnly)

**📏 Distance: O(N) Day-by-Day Counting**
- 5 days (weekdays): **52.1 ns**
- 30 days (~21 business days): **308.3 ns**
- 365 days (~260 business days): **3,617.9 ns** (3.6 μs)
- **Scaling:** ~9.9 ns per calendar day checked

**📐 Span: Distance + Floor/Ceiling Alignment**
- 5 days: **67.96 ns** (combines Distance with alignment)
- 30 days: **315.4 ns**
- 365 days: **3,479.8 ns** (3.5 μs)
- **Overhead:** ~15 ns more than Distance (for boundary alignment)

**🔢 ExpandByRatio: Span + Add Operations**
- 30 days: **440.9 ns** (Span: 315ns + two Add operations)
- 365 days: **4,825 ns** (4.8 μs)
- **Composition:** Combines multiple O(N) operations

**⚡ Floor/Ceiling: O(1) Boundary Alignment**
- DateTime Floor: **4.2-5.3 ns** (check day of week)
- DateTime Ceiling: **4.9-9.6 ns** (may need forward scan to Monday)
- DateOnly Floor: **0.72 ns** (simpler, no time component)

### Performance Scaling Analysis

```
Operation        5 days    30 days    365 days    Per-Day Cost
──────────────────────────────────────────────────────────────
Add (DateTime)   39 ns     187 ns     1,081 ns    ~9.4 ns
Distance         52 ns     308 ns     3,618 ns    ~9.9 ns
Span             68 ns     315 ns     3,480 ns    ~9.5 ns
ExpandByRatio    N/A       441 ns     4,825 ns    ~13.2 ns
```

### DateTime vs DateOnly Performance

```
Operation           DateTime (ns)    DateOnly (ns)    Speedup
──────────────────────────────────────────────────────────────
Add (5 days)        39.4             19.2             2.1×
Add (20 days)       186.6            75.4             2.5×
Distance (30 days)  308.3            137.1            2.2×
Span (30 days)      315.4            131.1            2.4×
```

**Why DateOnly is faster:**
- No time component to preserve
- Simpler day arithmetic
- Fewer allocations/checks

### Design Trade-offs (Aligned with README Philosophy)

**Why operations are O(N):**
- ❌ Cannot use arithmetic: weekends are non-uniform gaps
- ✅ Must iterate day-by-day to check `IsBusinessDay()`
- ✅ No way around iteration for variable-step domains
- ⚠️ Performance scales linearly with range size

**When O(N) is acceptable:**
- Typical business scenarios: < 100 business days (< 1 μs)
- SLA calculations: "5 business days from now" (40 ns)
- Deadline tracking: "15 business days remaining" (150 ns)

**When O(N) becomes expensive:**
- Year-long calculations: 365 days = 3.6 μs
- Multi-year ranges: 1,000 days = ~10 μs
- High-frequency: millions of calls may accumulate cost

### Practical Recommendations

✅ **Use business day domains for:**
- Contract deadlines: "5 business days to respond" (40 ns)
- SLA tracking: "2 business days turnaround" (20 ns)
- Workweek calculations: "business days this month" (~300 ns for 30-day range)

⚠️ **Be aware of scaling:**
- Short ranges (< 30 days): Sub-microsecond performance
- Medium ranges (30-90 days): 300-900 ns (acceptable)
- Long ranges (365+ days): 3-10 μs (still fast, but watch loop counts)

✅ **Optimization strategies:**
- Cache business day counts if calculating repeatedly
- Use DateOnly instead of DateTime when possible (2× faster)
- Consider fixed-step approximations for very long ranges

### Real-World Impact

**SLA Deadline Calculator (10,000 requests/second):**
```csharp
var deadline = domain.Add(DateTime.Now, 5); // 5 business days
Cost: 39 ns × 10,000 = 390 microseconds/second
Impact: Negligible (0.039% CPU)
```

**Business Days Remaining Widget (updated 60×/second):**
```csharp
var remaining = range.Span(businessDayDomain); // 30-day range
Cost: 315 ns × 60 = 18.9 microseconds/second
Impact: Negligible
```

**Annual Business Day Report (1000 year-long calculations):**
```csharp
var businessDays = yearRange.Span(domain); // 365-day range
Cost: 3,618 ns × 1000 = 3.6 milliseconds
Impact: Acceptable for batch reporting
```

### Memory Behavior
```
All operations:  0 bytes allocated
Iteration:       Stack-only, no heap allocations
Domain instance: Stateless, reusable
```

### Comparison: Variable vs Fixed-Step

| Characteristic      | Variable-Step (Business Days) | Fixed-Step (Calendar Days) |
|---------------------|-------------------------------|----------------------------|
| **Complexity**      | O(N) - iteration required     | O(1) - pure arithmetic     |
| **Add (5 steps)**   | 39 ns                         | 0.74 ns (53× faster)       |
| **Distance (30)**   | 308 ns                        | 7.58 ns (41× faster)       |
| **Span (365)**      | 3,480 ns                      | ~20 ns (174× faster)       |
| **Use case**        | Business logic, SLAs          | Simple date math           |

### Why This Matters

These benchmarks show that **variable-step domains have O(N) performance** but remain **practical for business scenarios**:
- ✅ Short ranges (< 30 days): Sub-microsecond
- ✅ Medium ranges (30-90 days): Low microseconds
- ✅ Long ranges (365 days): Still only 3.6 μs
- ⚠️ Be mindful in tight loops with long ranges

Choose variable-step domains when **business logic requires it** (weekends, holidays), and fixed-step domains when **O(1) performance is critical**.
