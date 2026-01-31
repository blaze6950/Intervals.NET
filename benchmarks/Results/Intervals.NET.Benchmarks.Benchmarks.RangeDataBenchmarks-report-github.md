```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.6456/22H2/2022Update)
Intel Core i7-1065G7 CPU 1.30GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.403
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Method           | N      | Mean          | Error        | StdDev       | Ratio | RatioSD | Completed Work Items | Lock Contentions | Gen0     | Gen1     | Gen2     | Allocated | Alloc Ratio |
|----------------- |------- |--------------:|-------------:|-------------:|------:|--------:|---------------------:|-----------------:|---------:|---------:|---------:|----------:|------------:|
| **Construction**     | **10**     |      **38.46 ns** |     **0.835 ns** |     **0.962 ns** |  **1.00** |    **0.00** |                    **-** |                **-** |   **0.0363** |        **-** |        **-** |     **152 B** |        **1.00** |
| TryGet_Hit       | 10     |      33.36 ns |     0.603 ns |     0.694 ns |  0.87 |    0.02 |                    - |                - |   0.0114 |        - |        - |      48 B |        0.32 |
| TryGet_Miss      | 10     |      27.84 ns |     0.590 ns |     1.079 ns |  0.73 |    0.03 |                    - |                - |   0.0114 |        - |        - |      48 B |        0.32 |
| Indexer_Hit      | 10     |      32.06 ns |     0.678 ns |     1.014 ns |  0.83 |    0.03 |                    - |                - |   0.0114 |        - |        - |      48 B |        0.32 |
| Slice_Small      | 10     |      52.21 ns |     0.857 ns |     0.986 ns |  1.36 |    0.04 |                    - |                - |   0.0344 |        - |        - |     144 B |        0.95 |
| Slice_Medium     | 10     |      60.00 ns |     1.244 ns |     2.044 ns |  1.54 |    0.08 |                    - |                - |   0.0343 |        - |        - |     144 B |        0.95 |
| Iterate_First100 | 10     |      93.23 ns |     0.912 ns |     0.761 ns |  2.41 |    0.08 |                    - |                - |   0.0114 |        - |        - |      48 B |        0.32 |
|                  |        |               |              |              |       |         |                      |                  |          |          |          |           |             |
| **Construction**     | **1000**   |     **321.89 ns** |     **5.495 ns** |     **5.643 ns** |  **1.00** |    **0.00** |                    **-** |                **-** |   **0.9828** |        **-** |        **-** |    **4112 B** |        **1.00** |
| TryGet_Hit       | 1000   |      36.75 ns |     0.585 ns |     0.457 ns |  0.11 |    0.00 |                    - |                - |   0.0114 |        - |        - |      48 B |        0.01 |
| TryGet_Miss      | 1000   |      33.48 ns |     0.722 ns |     1.013 ns |  0.11 |    0.00 |                    - |                - |   0.0114 |        - |        - |      48 B |        0.01 |
| Indexer_Hit      | 1000   |      45.48 ns |     0.923 ns |     0.987 ns |  0.14 |    0.00 |                    - |                - |   0.0114 |        - |        - |      48 B |        0.01 |
| Slice_Small      | 1000   |      59.89 ns |     1.147 ns |     1.073 ns |  0.19 |    0.01 |                    - |                - |   0.0343 |        - |        - |     144 B |        0.04 |
| Slice_Medium     | 1000   |      62.33 ns |     0.958 ns |     0.800 ns |  0.19 |    0.00 |                    - |                - |   0.0343 |        - |        - |     144 B |        0.04 |
| Iterate_First100 | 1000   |     733.13 ns |    12.717 ns |    24.804 ns |  2.32 |    0.10 |                    - |                - |   0.0114 |        - |        - |      48 B |        0.01 |
|                  |        |               |              |              |       |         |                      |                  |          |          |          |           |             |
| **Construction**     | **100000** | **238,733.68 ns** | **3,409.826 ns** | **3,189.553 ns** | **1.000** |    **0.00** |                    **-** |                **-** | **124.5117** | **124.5117** | **124.5117** |  **400154 B** |       **1.000** |
| TryGet_Hit       | 100000 |      40.35 ns |     0.360 ns |     0.281 ns | 0.000 |    0.00 |                    - |                - |   0.0114 |        - |        - |      48 B |       0.000 |
| TryGet_Miss      | 100000 |      39.82 ns |     0.802 ns |     0.750 ns | 0.000 |    0.00 |                    - |                - |   0.0114 |        - |        - |      48 B |       0.000 |
| Indexer_Hit      | 100000 |      50.10 ns |     0.717 ns |     0.670 ns | 0.000 |    0.00 |                    - |                - |   0.0114 |        - |        - |      48 B |       0.000 |
| Slice_Small      | 100000 |      58.86 ns |     0.787 ns |     0.657 ns | 0.000 |    0.00 |                    - |                - |   0.0343 |        - |        - |     144 B |       0.000 |
| Slice_Medium     | 100000 |      59.12 ns |     1.252 ns |     2.126 ns | 0.000 |    0.00 |                    - |                - |   0.0343 |        - |        - |     144 B |       0.000 |
| Iterate_First100 | 100000 |     720.83 ns |    10.438 ns |    10.251 ns | 0.003 |    0.00 |                    - |                - |   0.0114 |        - |        - |      48 B |       0.000 |

## Summary

This run captures RangeData microbenchmarks executed with BenchmarkDotNet v0.13.12 on .NET SDK 8.0.403 (Host .NET 8.0.11) on an Intel Core i7-1065G7. Measurements were taken for three dataset sizes (N = 10, 1,000, 100,000) and report timing, allocations, and GC activity for construction, lookup, slicing, and iteration.

Key findings (exact numbers):

- Construction cost and allocations grow strongly with N: N=10 — 38.46 ns / 152 B (Gen0 0.0363); N=1,000 — 321.89 ns / 4,112 B (Gen0 0.9828); N=100,000 — 238,733.68 ns / 400,154 B (Gen0 124.5117).
- Lookups are low-latency and allocation-stable: TryGet_Hit = 33.36 ns (N=10), 36.75 ns (N=1,000), 40.35 ns (N=100,000); TryGet_Miss = 27.84 ns → 33.48 ns → 39.82 ns; Indexer_Hit = 32.06 ns → 45.48 ns → 50.10 ns. These lookup cases report ~48 B allocated.
- Slice_Small is steady (~52–60 ns) across sizes: 52.21 ns (N=10), 59.89 ns (N=1,000), 58.86 ns (N=100,000) and allocates ~144 B.
- Iterating the first 100 elements shows a mid/large-N peak: 93.23 ns (N=10), 733.13 ns (N=1,000), 720.83 ns (N=100,000); allocated ≈48 B.

Guidance

- Prefer reusing constructed `RangeData` instances in latency-sensitive paths: lookups (TryGet/Indexer) are very cheap (~30–50 ns) and stable.
- Avoid constructing many large `RangeData` instances repeatedly; construction time and memory pressure increase dramatically with N. Prebuild, reuse, or batch construction to reduce GC churn (Gen0 climbs from 0.0363 to 124.5117 across sizes).
- When iterating small prefixes frequently (e.g., first 100 items), benchmark your workload — iteration shows a notable increase at mid/large N and may benefit from optimized access patterns.

Short artifacts

- Release-note blurb: RangeData microbenchmarks show low-cost lookups (TryGet/Indexer ≈ 30–50 ns; ~48 B) while Construction time and allocations grow with dataset size (38.46 ns → 238,733.68 ns; 152 B → 400,154 B). See the full table above for per-case numbers.

- Tweet-style highlight: Fast lookups: TryGet ~33–40 ns (48 B); Construction scales from 38.46 ns (N=10) to 238,733.68 ns (N=100,000) — full benchmarks in the report.

- README quick bullets:
  - Low-latency lookups: TryGet/Indexer ~27.8–50.1 ns, ~48 B allocations.
  - Construction cost increases with N; large N causes significant allocations and Gen0 activity.
  - Slice_Small ~52–60 ns (144 B); iterating first 100 elements can be costly at mid/large N (≈720–733 ns).

## Assessment (grades)

This section assigns simple grades for Performance and Memory usage for each benchmarked operation. Grades use a pragmatic scale: A = excellent, B = good, C = acceptable / worth attention, D = poor. Grades are relative to the microbenchmark context and realistic expectations for hot-path code.

Notes on methodology and root causes:
- The benchmark code materializes arrays in `Construction()` using `Enumerable.Range(...).ToArray()` — these allocations dominate the Construction measurements.
- Many library operations use LINQ operators (`Skip`, `Take`, `Concat`, `Concat` chains) and return lazy `IEnumerable<T>` instances; the measured small allocations (48 B, 144 B, 192 B, etc.) are consistent with a tiny iterator or wrapper object created by LINQ plus the `RangeData` record allocation when applicable.
- Where possible, comments reference implementation points in `src/Intervals.NET.Data/RangeData.cs` and `src/Intervals.NET.Data/Extensions/RangeDataExtensions.cs` to explain unavoidable allocations.

Grades (using the exact numbers reported)

- Construction (N=10: 38.46 ns / 152 B; N=1,000: 321.89 ns / 4,112 B; N=100,000: 238,733.68 ns / 400,154 B):
  - Performance: D (large N shows high cost — dominated by materializing the backing array in the benchmark); Memory: D (hundreds of KB at N=100k). Reason: the benchmark intentionally materializes with `ToArray()`; that allocation and copying are the main contributors (see `Construction()` in `RangeDataBenchmarks.cs`). If the workload reuses a prebuilt array, construction cost is much lower.

- TryGet_Hit (N=10: 33.36 ns / 48 B; N=1,000: 36.75 ns / 48 B; N=100,000: 40.35 ns / 48 B):
  - Performance: A (very low-latency lookups ~33–40 ns). Memory: B (48 B allocation). Reason: `TryGet` uses `Data.Skip(intIndex)` and then enumerates — `Skip` returns a small iterator object. When `Data` is an array (the benchmark's common case), an alternative zero-allocation indexed path would be possible (see note below).

- TryGet_Miss (N=10: 27.84 ns / 48 B; N=1,000: 33.48 ns / 48 B; N=100,000: 39.82 ns / 48 B):
  - Performance: A; Memory: B. Reason: similar to TryGet_Hit — fast but a small iterator allocation from LINQ.

- Indexer_Hit (N=10: 32.06 ns / 48 B; N=1,000: 45.48 ns / 48 B; N=100,000: 50.10 ns / 48 B):
  - Performance: A; Memory: B. Reason: indexer delegates to `TryGet` and inherits the same allocation profile.

- Slice_Small (N=10: 52.21 ns / 144 B; N=1,000: 59.89 ns / 144 B; N=100,000: 58.86 ns / 144 B):
  - Performance: A-; Memory: B. Reason: `Slice`/indexer for sub-ranges builds a lazy `IEnumerable` via `Skip().Take()` and returns a new `RangeData` instance; that produces slightly larger per-call allocations (the 144 B reflects an iterator plus the `RangeData` allocation).

- Slice_Medium (N=10: 60.00 ns / 144 B; N=1,000: 62.33 ns / 144 B; N=100,000: 59.12 ns / 144 B):
  - Performance: A-; Memory: B. Same reasoning as Slice_Small.

- Iterate_First100 (N=10: 93.23 ns / 48 B; N=1,000: 733.13 ns / 48 B; N=100,000: 720.83 ns / 48 B):
  - Performance: B (fast for small N, but iterating 100 items in the mid/large cases costs ~720–733 ns). Memory: B (48 B). Reason: the benchmark's `Take(100)` iterates up to 100 elements — for N=10 fewer elements are iterated (hence cheaper). The allocation is the small enumerator/iterator wrapper.

Assessment summary and optimization guidance

- What is unavoidable vs. fixable:
  - The large allocations shown for `Construction` are not inherent to `RangeData` itself but are dominated by the benchmark's `ToArray()` materialization; in practice, if callers provide already-materialized arrays, the per-construction overhead is only the `RangeData` object reference (small). Therefore Construction's large numbers are avoidable by changing caller behavior (reuse/prebuild backing arrays) rather than library internals.
  - The small per-call allocations (48 B, 144 B, 192 B, 424 B, 456 B) come from creating LINQ iterator/adapter objects (`Skip`, `Take`, `Concat`, `Union` helpers) and the `RangeData` record allocation when a new instance is returned. These are typically small and expected with the current lazy-LINQ design. They are near-minimal for the current implementation that exposes `IEnumerable<T>` semantics.
  - If zero-allocation lookups are required when the backing sequence is an array or list, the library could add overloads or specialized fast-paths that accept `IReadOnlyList<T>`/`T[]`/`Memory<T>` or detect array-backed sequences and use direct indexing. Doing so would remove the iterator allocation for `TryGet`/`Indexer` and further reduce latency.