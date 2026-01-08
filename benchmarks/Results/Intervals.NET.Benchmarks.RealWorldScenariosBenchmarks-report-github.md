```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.6456/22H2/2022Update)
Intel Core i7-1065G7 CPU 1.30GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.403
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Method                                 | Mean         | Error       | StdDev      | Ratio | RatioSD | Completed Work Items | Lock Contentions | Gen0   | Allocated | Alloc Ratio |
|--------------------------------------- |-------------:|------------:|------------:|------:|--------:|---------------------:|-----------------:|-------:|----------:|------------:|
| Naive_SlidingWindow_SingleRange        |   3,039.2 ns |    60.64 ns |   142.93 ns |  1.00 |    0.00 |                    - |                - | 0.0076 |      40 B |        1.00 |
| IntervalsNet_SlidingWindow_SingleRange |   1,780.5 ns |    35.50 ns |    65.80 ns |  0.59 |    0.03 |                    - |                - |      - |         - |        0.00 |
| Naive_SequentialValidation             | 134,085.7 ns | 2,667.04 ns | 3,824.99 ns | 43.56 |    2.39 |                    - |                - |      - |         - |        0.00 |
| IntervalsNet_SequentialValidation      | 149,490.8 ns | 2,822.50 ns | 3,956.75 ns | 48.55 |    3.14 |                    - |                - |      - |         - |        0.00 |
| Naive_OverlapDetection                 |  13,592.0 ns |   260.17 ns |   243.37 ns |  4.37 |    0.22 |                    - |                - |      - |         - |        0.00 |
| IntervalsNet_OverlapDetection          |  54,675.8 ns | 1,072.71 ns | 1,101.59 ns | 17.55 |    0.97 |                    - |                - |      - |         - |        0.00 |
| Naive_ComputeIntersections             |  31,141.0 ns |   232.56 ns |   181.57 ns | 10.06 |    0.48 |                    - |                - | 4.6387 |   19400 B |      485.00 |
| IntervalsNet_ComputeIntersections      |  80,351.0 ns | 1,559.18 ns | 1,531.32 ns | 25.89 |    1.37 |                    - |                - |      - |         - |        0.00 |
| Naive_LINQ_FilterByValue               |     559.2 ns |    11.00 ns |    15.78 ns |  0.18 |    0.01 |                    - |                - | 0.0286 |     120 B |        3.00 |
| IntervalsNet_LINQ_FilterByValue        |     427.9 ns |     5.29 ns |     4.69 ns |  0.14 |    0.01 |                    - |                - | 0.0286 |     120 B |        3.00 |
| Naive_BatchConstruction                |     621.3 ns |    11.91 ns |    11.70 ns |  0.20 |    0.01 |                    - |                - | 1.1530 |    4824 B |      120.60 |
| IntervalsNet_BatchConstruction         |   1,093.8 ns |    21.61 ns |    28.85 ns |  0.35 |    0.02 |                    - |                - | 0.4826 |    2024 B |       50.60 |
