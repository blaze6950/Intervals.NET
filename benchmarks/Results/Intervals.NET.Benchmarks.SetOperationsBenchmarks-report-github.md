```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.6456/22H2/2022Update)
Intel Core i7-1065G7 CPU 1.30GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.403
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Method                                      | Mean      | Error     | StdDev    | Ratio | RatioSD | Completed Work Items | Lock Contentions | Gen0   | Allocated | Alloc Ratio |
|-------------------------------------------- |----------:|----------:|----------:|------:|--------:|---------------------:|-----------------:|-------:|----------:|------------:|
| Naive_Intersect_Overlapping                 | 13.771 ns | 0.3370 ns | 0.4139 ns |  1.00 |    0.00 |                    - |                - | 0.0095 |      40 B |        1.00 |
| IntervalsNet_Intersect_Overlapping          | 48.192 ns | 0.8224 ns | 0.7291 ns |  3.54 |    0.10 |                    - |                - |      - |         - |        0.00 |
| IntervalsNet_Intersect_Operator_Overlapping | 47.624 ns | 0.9831 ns | 1.2074 ns |  3.46 |    0.14 |                    - |                - |      - |         - |        0.00 |
| Naive_Intersect_NonOverlapping              |  5.367 ns | 0.1661 ns | 0.2729 ns |  0.38 |    0.02 |                    - |                - |      - |         - |        0.00 |
| IntervalsNet_Intersect_NonOverlapping       | 10.874 ns | 0.2490 ns | 0.4677 ns |  0.79 |    0.05 |                    - |                - |      - |         - |        0.00 |
| IntervalsNet_Union_Overlapping              | 46.539 ns | 0.9618 ns | 1.9864 ns |  3.39 |    0.22 |                    - |                - |      - |         - |        0.00 |
| IntervalsNet_Union_Operator_Overlapping     | 46.606 ns | 0.9359 ns | 1.2811 ns |  3.39 |    0.14 |                    - |                - |      - |         - |        0.00 |
| IntervalsNet_Union_NonOverlapping           | 31.468 ns | 0.6645 ns | 1.1812 ns |  2.28 |    0.12 |                    - |                - |      - |         - |        0.00 |
| IntervalsNet_Except_Overlapping_Count       | 71.135 ns | 1.4362 ns | 1.6539 ns |  5.17 |    0.14 |                    - |                - | 0.0305 |     128 B |        3.20 |
| IntervalsNet_Except_Middle_Count            | 91.444 ns | 1.8327 ns | 2.1106 ns |  6.65 |    0.24 |                    - |                - | 0.0305 |     128 B |        3.20 |
| Naive_Overlaps                              |  3.013 ns | 0.0941 ns | 0.1190 ns |  0.22 |    0.01 |                    - |                - |      - |         - |        0.00 |
| IntervalsNet_Overlaps                       | 17.069 ns | 0.3714 ns | 0.6205 ns |  1.23 |    0.08 |                    - |                - |      - |         - |        0.00 |
