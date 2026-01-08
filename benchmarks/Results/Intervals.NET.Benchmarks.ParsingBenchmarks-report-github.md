```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.6456/22H2/2022Update)
Intel Core i7-1065G7 CPU 1.30GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.403
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Method                                 | Mean      | Error    | StdDev   | Ratio | RatioSD | Completed Work Items | Lock Contentions | Gen0   | Code Size | Allocated | Alloc Ratio |
|--------------------------------------- |----------:|---------:|---------:|------:|--------:|---------------------:|-----------------:|-------:|----------:|----------:|------------:|
| Naive_Parse_String                     |  96.95 ns | 1.959 ns | 2.256 ns |  1.00 |    0.00 |                    - |                - | 0.0516 |   2,074 B |     216 B |        1.00 |
| IntervalsNet_Parse_String              |  44.19 ns | 0.895 ns | 1.367 ns |  0.46 |    0.02 |                    - |                - |      - |     210 B |         - |        0.00 |
| IntervalsNet_Parse_Span                |  44.78 ns | 0.921 ns | 1.131 ns |  0.46 |    0.02 |                    - |                - |      - |     211 B |         - |        0.00 |
| Traditional_InterpolatedString_TwoStep | 105.54 ns | 2.115 ns | 2.750 ns |  1.09 |    0.04 |                    - |                - | 0.0095 |   2,043 B |      40 B |        0.19 |
| IntervalsNet_Parse_InterpolatedString  |  26.90 ns | 0.441 ns | 0.391 ns |  0.28 |    0.01 |                    - |                - | 0.0057 |        NA |      24 B |        0.11 |
| IntervalsNet_Parse_Closed              |  44.03 ns | 0.888 ns | 0.987 ns |  0.45 |    0.01 |                    - |                - |      - |     199 B |         - |        0.00 |
| IntervalsNet_Parse_Open                |  45.87 ns | 0.857 ns | 0.716 ns |  0.48 |    0.01 |                    - |                - |      - |     199 B |         - |        0.00 |
| IntervalsNet_Parse_HalfOpen            |  45.04 ns | 0.436 ns | 0.340 ns |  0.47 |    0.01 |                    - |                - |      - |     199 B |         - |        0.00 |
| IntervalsNet_Parse_UnboundedStart      |  39.16 ns | 0.565 ns | 0.528 ns |  0.41 |    0.01 |                    - |                - |      - |     199 B |         - |        0.00 |
| IntervalsNet_Parse_UnboundedEnd        |  34.82 ns | 0.425 ns | 0.397 ns |  0.36 |    0.01 |                    - |                - |      - |     199 B |         - |        0.00 |
| IntervalsNet_Parse_InfinitySymbol      |  37.71 ns | 0.188 ns | 0.176 ns |  0.39 |    0.01 |                    - |                - |      - |     199 B |         - |        0.00 |
| ConfigScenario_Traditional             |  98.33 ns | 0.875 ns | 0.818 ns |  1.02 |    0.02 |                    - |                - | 0.0095 |   2,052 B |      40 B |        0.19 |
| ConfigScenario_ZeroAllocation          |  31.96 ns | 0.436 ns | 0.364 ns |  0.33 |    0.01 |                    - |                - | 0.0057 |        NA |      24 B |        0.11 |
