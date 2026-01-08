```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.6456/22H2/2022Update)
Intel Core i7-1065G7 CPU 1.30GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.403
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Method                         | Mean      | Error     | StdDev    | Ratio | RatioSD | Completed Work Items | Lock Contentions | Allocated | Alloc Ratio |
|------------------------------- |----------:|----------:|----------:|------:|--------:|---------------------:|-----------------:|----------:|------------:|
| Naive_Contains_Inside          |  2.867 ns | 0.0927 ns | 0.1695 ns |  1.00 |    0.00 |                    - |                - |         - |          NA |
| IntervalsNet_Contains_Inside   |  1.669 ns | 0.0678 ns | 0.0928 ns |  0.58 |    0.05 |                    - |                - |         - |          NA |
| Naive_Contains_Outside         |  1.960 ns | 0.0752 ns | 0.1170 ns |  0.68 |    0.05 |                    - |                - |         - |          NA |
| IntervalsNet_Contains_Outside  |  1.614 ns | 0.0570 ns | 0.0781 ns |  0.56 |    0.05 |                    - |                - |         - |          NA |
| Naive_Contains_Boundary        |  1.930 ns | 0.0650 ns | 0.0576 ns |  0.68 |    0.05 |                    - |                - |         - |          NA |
| IntervalsNet_Contains_Boundary |  1.745 ns | 0.0684 ns | 0.0732 ns |  0.61 |    0.04 |                    - |                - |         - |          NA |
| Naive_Contains_Range           |  1.222 ns | 0.0592 ns | 0.1312 ns |  0.43 |    0.05 |                    - |                - |         - |          NA |
| IntervalsNet_Contains_Range    | 18.458 ns | 0.3305 ns | 0.3092 ns |  6.47 |    0.41 |                    - |                - |         - |          NA |
| NodaTime_Contains_Instant      | 10.141 ns | 0.2198 ns | 0.1949 ns |  3.55 |    0.24 |                    - |                - |         - |          NA |
