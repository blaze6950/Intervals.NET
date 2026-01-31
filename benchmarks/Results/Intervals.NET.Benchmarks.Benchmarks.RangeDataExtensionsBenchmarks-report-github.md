```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.6456/22H2/2022Update)
Intel Core i7-1065G7 CPU 1.30GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.403
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Method          | Mean         | Error      | StdDev     | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|---------------- |-------------:|-----------:|-----------:|------:|--------:|-------:|----------:|------------:|
| Intersect       |    121.45 ns |   2.091 ns |   1.956 ns |  1.00 |    0.00 | 0.0458 |     192 B |        1.00 |
| Union           |    286.50 ns |   5.781 ns |  11.138 ns |  2.35 |    0.14 | 0.1011 |     424 B |        2.21 |
| TrimStart       |     57.91 ns |   1.465 ns |   3.884 ns |  0.48 |    0.02 | 0.0343 |     144 B |        0.75 |
| Union_Enumerate | 11,588.82 ns | 129.171 ns | 114.507 ns | 95.61 |    1.83 | 0.1068 |     456 B |        2.38 |

## Summary

This run captures RangeData extensions microbenchmarks executed with BenchmarkDotNet v0.13.12 on .NET SDK 8.0.403 (Host .NET 8.0.11) on an Intel Core i7-1065G7. The table measures common extensions (Intersect, Union, TrimStart) and an enumerate-heavy Union_Enumerate scenario, reporting timings and allocations.

Key findings (exact numbers):

- Intersect runs in 121.45 ns and allocates 192 B (Gen0 0.0458).
- Union is heavier: 286.50 ns and 424 B (Gen0 0.1011), while `Union_Enumerate` is an order of magnitude slower (11,588.82 ns) and allocates 456 B (Gen0 0.1068).
- TrimStart is cheap: 57.91 ns and 144 B (Gen0 0.0343).

Guidance

- Use `Intersect` or `TrimStart` for low-latency operations; they both complete in well under 200 ns and have modest allocations.
- `Union` performs more work and allocates more (286.50 ns / 424 B); use it when set-combining semantics are required and measure if used in tight loops.
- `Union_Enumerate` indicates that enumerating a union can be costly (≈11.6 µs); avoid repeated enumeration in hot paths or consider materializing results once if they are reused.

Short artifacts

- Release-note blurb: RangeData extension benchmarks show Intersect ~121 ns (192 B) and TrimStart ~58 ns (144 B); Union is heavier (~286 ns / 424 B) and enumerating unions can be expensive (~11,589 ns / 456 B).

- Tweet-style highlight: Intersect ~121 ns; TrimStart ~58 ns; Union ~286 ns (424 B); enumerating unions ≈11.6 µs — see benchmarks for details.

- README quick bullets:
  - Fast extensions: Intersect ~121 ns (192 B) and TrimStart ~58 ns (144 B).
  - Union costs ~286 ns and 424 B; materialize if reusing results.
  - Enumerating a union can be expensive (~11.6 µs); avoid repeated enumeration in hot code paths.

## Assessment (grades)

This section assigns pragmatic grades for Performance and Memory usage for the RangeData extension operations in the table above. Grades: A = excellent, B = good, C = acceptable/worth attention, D = poor.

Observations and root causes:
- The extension methods intentionally use lazy `IEnumerable<T>` composition (calls to `Concat`, `Skip`, `Take`, and returning existing `IEnumerable<T>` sequences) to preserve immutability and defer materialization. These LINQ combinators create small iterator/adapter objects which account for the measured allocations (192 B, 424 B, 144 B, 456 B).
- `Union` and `Union_Enumerate` perform more work and enumerate/concatenate multiple sequences; `Union_Enumerate` specifically triggers enumeration of the constructed union sequence which exposes the full cost of combining and iterating the lazy pipeline.

Grades (exact numbers from table):

- Intersect (121.45 ns / 192 B):
  - Performance: A-; Memory: B. Reason: `Intersect` validates domains and then slices the right operand (`right[intersectedRange]`) which returns a `RangeData` over `Skip/Take` — the 192 B aligns with the iterator wrappers plus the resulting `RangeData` allocation. Good throughput; small allocations are expected for lazy slicing.

- Union (286.50 ns / 424 B):
  - Performance: B; Memory: C. Reason: `Union` computes union/intersection and constructs a composed `IEnumerable<T>` via `CombineDataWithFreshPrimary`, which may concatenate several sequences; the larger allocation (424 B) reflects multiple iterator wrappers and the returned `RangeData` instance.

- TrimStart (57.91 ns / 144 B):
  - Performance: A; Memory: B. Reason: `TrimStart` delegates to `TryGet` slice logic producing a small iterator and a `RangeData` instance; a low-latency op with modest allocation.

- Union_Enumerate (11,588.82 ns / 456 B):
  - Performance: C; Memory: C. Reason: enumerating the union forces the lazy pipeline to produce and iterate potentially large sequences. The time reflects processing ~1,500 distinct elements (union of two 1,000-length ranges with overlap), and the allocation is modest (iterator wrappers + enumeration overhead). If consumers must enumerate unions frequently, materializing the union once (ToArray/ToList) may be faster overall despite the allocation.

Assessment summary and optimization guidance

- Unavoidable vs. fixable allocations:
  - The allocations for lazy combinators (192 B, 144 B) are small and expected given the design goals (lazy evaluation, immutability). They are near-minimal for returning `IEnumerable<T>` without an API change.
  - Larger allocations and the cost when enumerating unions are a consequence of combining multiple sequences; enumerating will necessarily touch each element. If repeated enumeration of the union is common, materializing the union once is a practical optimization.