```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.6456/22H2/2022Update)
Intel Core i7-1065G7 CPU 1.30GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.403
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Method                             | Mean       | Error     | StdDev    | Median     | Ratio | RatioSD | Completed Work Items | Lock Contentions | Gen0   | Allocated | Alloc Ratio |
|----------------------------------- |-----------:|----------:|----------:|-----------:|------:|--------:|---------------------:|-----------------:|-------:|----------:|------------:|
| Naive_Int_FiniteClosed             |  6.9027 ns | 0.1479 ns | 0.1235 ns |  6.9050 ns | 1.000 |    0.00 |                    - |                - | 0.0096 |      40 B |        1.00 |
| IntervalsNet_Int_FiniteClosed      |  8.5737 ns | 0.1993 ns | 0.2448 ns |  8.6321 ns | 1.241 |    0.05 |                    - |                - |      - |         - |        0.00 |
| NodaTime_DateTime_FiniteClosed     |  0.3755 ns | 0.0442 ns | 0.0509 ns |  0.3777 ns | 0.055 |    0.00 |                    - |                - |      - |         - |        0.00 |
| IntervalsNet_DateTime_FiniteClosed |  2.2877 ns | 0.0763 ns | 0.1315 ns |  2.3048 ns | 0.327 |    0.02 |                    - |                - |      - |         - |        0.00 |
| Naive_Int_FiniteOpen               |  6.5531 ns | 0.1780 ns | 0.2495 ns |  6.6058 ns | 0.952 |    0.05 |                    - |                - | 0.0096 |      40 B |        1.00 |
| IntervalsNet_Int_FiniteOpen        |  8.6691 ns | 0.2070 ns | 0.2464 ns |  8.7148 ns | 1.252 |    0.03 |                    - |                - |      - |         - |        0.00 |
| Naive_Int_HalfOpen                 |  6.7073 ns | 0.1933 ns | 0.2646 ns |  6.7183 ns | 0.968 |    0.05 |                    - |                - | 0.0096 |      40 B |        1.00 |
| IntervalsNet_Int_HalfOpen          |  8.4516 ns | 0.2021 ns | 0.3026 ns |  8.4758 ns | 1.206 |    0.05 |                    - |                - |      - |         - |        0.00 |
| Naive_UnboundedStart               |  9.9141 ns | 0.2623 ns | 0.4083 ns |  9.9568 ns | 1.436 |    0.06 |                    - |                - | 0.0096 |      40 B |        1.00 |
| IntervalsNet_UnboundedStart        |  0.3051 ns | 0.0576 ns | 0.1672 ns |  0.3413 ns | 0.005 |    0.01 |                    - |                - |      - |         - |        0.00 |
| Naive_UnboundedEnd                 | 10.4031 ns | 0.2509 ns | 0.2347 ns | 10.3813 ns | 1.502 |    0.05 |                    - |                - | 0.0096 |      40 B |        1.00 |
| IntervalsNet_UnboundedEnd          |  0.6057 ns | 0.1066 ns | 0.3144 ns |  0.4878 ns | 0.126 |    0.01 |                    - |                - |      - |         - |        0.00 |
| Naive_FullyUnbounded               |  5.6539 ns | 0.1757 ns | 0.3213 ns |  5.6071 ns | 0.816 |    0.06 |                    - |                - | 0.0096 |      40 B |        1.00 |
| IntervalsNet_FullyUnbounded        |  0.0982 ns | 0.0331 ns | 0.0544 ns |  0.0698 ns | 0.019 |    0.01 |                    - |                - |      - |         - |        0.00 |
