# 📊 Intervals.NET

> **Type-safe mathematical intervals and ranges for .NET**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Intervals.NET.svg)](https://www.nuget.org/packages/Intervals.NET/)

A production-ready .NET library for working with mathematical intervals and ranges. Designed for correctness, performance, and zero allocations.

**Intervals.NET** provides robust, type-safe interval operations over any `IComparable<T>`. Whether you're validating business rules, scheduling time windows, or filtering numeric data, this library delivers correct range semantics with comprehensive edge case handling—without heap allocations.

**Key characteristics:**
- ✅ **Correctness first**: Explicit infinity, validated boundaries, fail-fast construction
- ⚡ **Zero-allocation design**: Struct-based API, no boxing, stack-allocated ranges
- 🎯 **Generic and expressive**: Works with int, double, DateTime, TimeSpan, strings, custom types
- 🛡️ **Real-world ready**: 100% test coverage, battle-tested edge cases, production semantics

## 📦 Installation

```bash
dotnet add package Intervals.NET
```

## 🚀 Quick Start

```csharp
using Intervals.NET.Factories;

// Create ranges with mathematical notation
var closed = Range.Closed(10, 20);        // [10, 20]
var open = Range.Open(0, 100);            // (0, 100)
var halfOpen = Range.ClosedOpen(1, 10);   // [1, 10)

// Check containment
bool inside = closed.Contains(15);        // true
bool outside = closed.Contains(25);       // false

// Set operations
var a = Range.Closed(10, 30);
var b = Range.Closed(20, 40);
var intersection = a.Intersect(b);        // [20, 30]
var union = a.Union(b);                   // [10, 40]

// Unbounded ranges (infinity support)
var adults = Range.Closed(18, RangeValue<int>.PositiveInfinity);  // [18, ∞)
var past = Range.Open(RangeValue<DateTime>.NegativeInfinity, DateTime.Now);

// Parse from strings
var parsed = Range.FromString<int>("[10, 20]");

// Generic over any IComparable<T>
var dates = Range.Closed(DateTime.Today, DateTime.Today.AddDays(7));
var times = Range.Closed(TimeSpan.FromHours(9), TimeSpan.FromHours(17));
```

## 💼 Real-World Use Cases

### Scheduling & Calendar Systems
```csharp
// Business hours
var businessHours = Range.Closed(TimeSpan.FromHours(9), TimeSpan.FromHours(17));
bool isWorkingTime = businessHours.Contains(DateTime.Now.TimeOfDay);

// Meeting room availability - detect conflicts
var meeting1 = Range.Closed(new DateTime(2024, 1, 15, 10, 0, 0), 
                             new DateTime(2024, 1, 15, 11, 0, 0));
var meeting2 = Range.Closed(new DateTime(2024, 1, 15, 10, 30, 0), 
                             new DateTime(2024, 1, 15, 12, 0, 0));

if (meeting1.Overlaps(meeting2))
{
    var conflict = meeting1.Intersect(meeting2);  // [10:30, 11:00]
    Console.WriteLine($"Conflict detected: {conflict}");
}
```

### Booking Systems & Resource Allocation
```csharp
// Hotel room availability
var booking1 = Range.ClosedOpen(new DateTime(2024, 1, 1), new DateTime(2024, 1, 5));
var booking2 = Range.ClosedOpen(new DateTime(2024, 1, 3), new DateTime(2024, 1, 8));

// Check if bookings overlap (double-booking detection)
if (booking1.Overlaps(booking2))
{
    throw new InvalidOperationException("Room already booked during this period");
}

// Find available windows after removing booked periods
var fullMonth = Range.Closed(new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));
var available = fullMonth.Except(booking1).Concat(fullMonth.Except(booking2));
```

### Validation & Configuration
```csharp
// Input validation
var validPort = Range.Closed(1, 65535);
var validPercentage = Range.Closed(0.0, 100.0);
var validAge = Range.Closed(0, 150);

public void ValidateConfig(int port, double discount, int age)
{
    if (!validPort.Contains(port))
        throw new ArgumentOutOfRangeException(nameof(port), $"Must be in {validPort}");
    if (!validPercentage.Contains(discount))
        throw new ArgumentOutOfRangeException(nameof(discount));
    if (!validAge.Contains(age))
        throw new ArgumentOutOfRangeException(nameof(age));
}
```

### Pricing Tiers & Discounts
```csharp
// Progressive pricing based on quantity
var tier1 = Range.ClosedOpen(1, 100);       // 1-99 units
var tier2 = Range.ClosedOpen(100, 500);     // 100-499 units
var tier3 = Range.Closed(500, RangeValue<int>.PositiveInfinity);  // 500+

decimal GetUnitPrice(int quantity)
{
    if (tier1.Contains(quantity)) return 10.00m;
    if (tier2.Contains(quantity)) return 8.50m;
    if (tier3.Contains(quantity)) return 7.00m;
    throw new ArgumentOutOfRangeException(nameof(quantity));
}

// Seasonal pricing periods
var peakSeason = Range.Closed(new DateTime(2024, 6, 1), new DateTime(2024, 8, 31));
var holidaySeason = Range.Closed(new DateTime(2024, 12, 15), new DateTime(2024, 12, 31));

decimal GetSeasonalMultiplier(DateTime date)
{
    if (peakSeason.Contains(date)) return 1.5m;
    if (holidaySeason.Contains(date)) return 2.0m;
    return 1.0m;
}
```

### Access Control & Time Windows
```csharp
// Feature flag rollout windows
var betaAccessWindow = Range.Closed(
    new DateTime(2024, 1, 1),
    new DateTime(2024, 3, 31)
);

bool HasBetaAccess(DateTime currentTime) => betaAccessWindow.Contains(currentTime);

// Rate limiting time windows
var rateLimitWindow = Range.ClosedOpen(
    DateTime.UtcNow,
    DateTime.UtcNow.AddMinutes(1)
);

// Check if request falls within current rate limit window
bool IsWithinCurrentWindow(DateTime requestTime) => rateLimitWindow.Contains(requestTime);
```

### Data Filtering & Analytics
```csharp
// Temperature monitoring
var normalTemp = Range.Closed(-10.0, 30.0);
var warningTemp = Range.Open(30.0, 50.0);
var dangerTemp = Range.Closed(50.0, RangeValue<double>.PositiveInfinity);

var readings = GetSensorReadings();
var normal = readings.Where(r => normalTemp.Contains(r.Temperature));
var warnings = readings.Where(r => warningTemp.Contains(r.Temperature));
var critical = readings.Where(r => dangerTemp.Contains(r.Temperature));

// Age demographics
var children = Range.ClosedOpen(0, 13);
var teenagers = Range.ClosedOpen(13, 18);
var adults = Range.Closed(18, RangeValue<int>.PositiveInfinity);

var users = GetUsers();
var adultUsers = users.Where(u => adults.Contains(u.Age));
```

### Sliding Window Validation
```csharp
// Process sensor data with moving time window
var windowSize = TimeSpan.FromMinutes(5);

foreach (var dataPoint in sensorStream)
{
    var window = Range.ClosedOpen(
        dataPoint.Timestamp.Subtract(windowSize),
        dataPoint.Timestamp
    );
    
    var recentData = allData.Where(d => window.Contains(d.Timestamp));
    var average = recentData.Average(d => d.Value);
    
    if (!normalRange.Contains(average))
    {
        TriggerAlert(dataPoint.Timestamp, average);
    }
}
```

## 🔑 Core Concepts

### Range Notation

Intervals.NET uses standard mathematical interval notation:

| Notation | Name        | Meaning                      | Example Code                  |
|----------|-------------|------------------------------|-------------------------------|
| `[a, b]` | Closed      | Includes both `a` and `b`    | `Range.Closed(1, 10)`         |
| `(a, b)` | Open        | Excludes both `a` and `b`    | `Range.Open(0, 100)`          |
| `[a, b)` | Half-open   | Includes `a`, excludes `b`   | `Range.ClosedOpen(1, 10)`     |
| `(a, b]` | Half-closed | Excludes `a`, includes `b`   | `Range.OpenClosed(1, 10)`     |

### Infinity Support

Represent unbounded ranges with explicit infinity:

```csharp
// Positive infinity: [18, ∞)
var adults = Range.Closed(18, RangeValue<int>.PositiveInfinity);

// Negative infinity: (-∞, 2024)
var past = Range.Open(RangeValue<DateTime>.NegativeInfinity, new DateTime(2024, 1, 1));

// Both directions: (-∞, ∞)
var everything = Range.Open(
    RangeValue<int>.NegativeInfinity,
    RangeValue<int>.PositiveInfinity
);

// Parse from strings: [-∞, 100] or [, 100]
var parsed = Range.FromString<int>("[-∞, 100]");
var shorthand = Range.FromString<int>("[, 100]");
```

**Why explicit infinity?** Avoids null-checking and makes unbounded semantics clear in code.

## 📚 API Overview

### Creating Ranges

```csharp
// Factory methods
var closed = Range.Closed(1, 10);           // [1, 10]
var open = Range.Open(0, 100);              // (0, 100)
var halfOpen = Range.ClosedOpen(1, 10);     // [1, 10)
var halfClosed = Range.OpenClosed(1, 10);   // (1, 10]

// With different types
var intRange = Range.Closed(1, 100);
var doubleRange = Range.Open(0.0, 1.0);
var dateRange = Range.Closed(DateTime.Today, DateTime.Today.AddDays(7));
var timeRange = Range.Closed(TimeSpan.FromHours(9), TimeSpan.FromHours(17));

// Unbounded ranges
var positiveInts = Range.Closed(0, RangeValue<int>.PositiveInfinity);
var allPast = Range.Open(RangeValue<DateTime>.NegativeInfinity, DateTime.Now);
```

### Containment Checks

```csharp
var range = Range.Closed(10, 30);

// Value containment
bool contains = range.Contains(20);         // true
bool outside = range.Contains(40);          // false
bool atBoundary = range.Contains(10);       // true (inclusive)

// Range containment
var inner = Range.Closed(15, 25);
bool fullyInside = range.Contains(inner);   // true

var overlap = Range.Closed(25, 35);
bool notContained = range.Contains(overlap); // false (extends beyond)
```

### Set Operations

```csharp
var a = Range.Closed(10, 30);
var b = Range.Closed(20, 40);

// Intersection (returns Range<T>?)
var intersection = a.Intersect(b);          // [20, 30]
var intersection2 = a & b;                  // Operator syntax

// Union (returns Range<T>? if ranges overlap or are adjacent)
var union = a.Union(b);                     // [10, 40]
var union2 = a | b;                         // Operator syntax

// Overlap check
bool overlaps = a.Overlaps(b);              // true

// Subtraction (returns IEnumerable<Range<T>>)
var remaining = a.Except(b).ToList();       // [[10, 20), (30, 30]] → effectively [10, 20)
```

### Range Relationships

```csharp
var range1 = Range.Closed(10, 20);
var range2 = Range.Closed(20, 30);
var range3 = Range.Closed(25, 35);

// Adjacency
bool adjacent = range1.IsAdjacent(range2);  // true (share boundary at 20)

// Ordering
bool before = range1.IsBefore(range3);      // true
bool after = range3.IsAfter(range1);        // true

// Properties
bool bounded = range1.IsBounded();          // true
bool infinite = Range.Open(
    RangeValue<int>.NegativeInfinity,
    RangeValue<int>.PositiveInfinity
).IsInfinite();                             // true
```

### Parsing from Strings

```csharp
using Intervals.NET.Parsers;

// Parse standard notation
var range1 = Range.FromString<int>("[10, 20]");
var range2 = Range.FromString<double>("(0.0, 1.0)");
var range3 = Range.FromString<DateTime>("[2024-01-01, 2024-12-31]");

// Parse with infinity
var unbounded = Range.FromString<int>("[-∞, ∞)");
var leftUnbounded = Range.FromString<int>("[, 100]");
var rightUnbounded = Range.FromString<int>("[0, ]");

// Safe parsing
if (RangeParser.TryParse<int>("[10, 20)", out var range))
{
    Console.WriteLine($"Parsed: {range}");
}

// Custom culture for decimal separators
var culture = new System.Globalization.CultureInfo("de-DE");
var germanRange = Range.FromString<double>("[1,5; 9,5]", culture);
```

### Zero-Allocation Parsing

**Interpolated string handler** eliminates intermediate allocations:

```csharp
int start = 10, end = 20;

// Traditional (allocates ~40 bytes: boxing, concat, string builder)
string str = $"[{start}, {end}]";
var range1 = Range.FromString<int>(str);

// Optimized (only ~24 bytes for final string)
var range2 = Range.FromString<int>($"[{start}, {end}]");  // ⚡ 3.6× faster

// Works with expressions and different types
var computed = Range.FromString<int>($"[{start * 2}, {end + 10})");
var dateRange = Range.FromString<DateTime>($"[{DateTime.Today}, {DateTime.Today.AddDays(7)})");

// True zero-allocation: use span-based overload
var spanRange = Range.FromString<int>("[10, 20]".AsSpan());  // 0 bytes
```

**Performance:**
- **Interpolated:** 3.6× faster than traditional, 89% less allocation
- **Span-based:** Zero allocations, 2.2× faster than traditional

**Trade-off:** Interpolated strings still allocate one final `string` (~24B) due to CLR design—unavoidable for string-based APIs.

### Working with Custom Types

```csharp
// Any IComparable<T> works
public record Temperature(double Celsius) : IComparable<Temperature>
{
    public int CompareTo(Temperature? other) =>
        Celsius.CompareTo(other?.Celsius ?? double.NegativeInfinity);
}

var comfortable = Range.Closed(new Temperature(18), new Temperature(24));
var current = new Temperature(21);

if (comfortable.Contains(current))
{
    Console.WriteLine("Temperature is comfortable");
}

// String ranges (lexicographic)
var alphabet = Range.Closed("A", "Z");
bool isLetter = alphabet.Contains("M");  // true
```

<details>
<summary><strong>Advanced Usage Examples</strong></summary>

### Building Complex Conditions

```csharp
// Age-based categorization
var children = Range.ClosedOpen(0, 13);
var teenagers = Range.ClosedOpen(13, 18);
var adults = Range.Closed(18, RangeValue<int>.PositiveInfinity);

string GetAgeCategory(int age)
{
    if (children.Contains(age)) return "Child";
    if (teenagers.Contains(age)) return "Teenager";
    if (adults.Contains(age)) return "Adult";
    throw new ArgumentOutOfRangeException(nameof(age));
}
```

### Progressive Discount System

```csharp
var tier1 = Range.ClosedOpen(0m, 100m);
var tier2 = Range.ClosedOpen(100m, 500m);
var tier3 = Range.Closed(500m, RangeValue<decimal>.PositiveInfinity);

decimal GetDiscount(decimal orderTotal)
{
    if (tier1.Contains(orderTotal)) return 0m;
    if (tier2.Contains(orderTotal)) return 0.10m;
    if (tier3.Contains(orderTotal)) return 0.15m;
    throw new ArgumentException("Invalid order total");
}
```

### Range-Based Configuration

```csharp
public class ServiceConfiguration
{
    public Range<int> AllowedPorts { get; init; } = Range.Closed(8000, 9000);
    public Range<TimeSpan> MaintenanceWindow { get; init; } = Range.Closed(
        TimeSpan.FromHours(2),
        TimeSpan.FromHours(4)
    );
    
    public bool IsMaintenanceTime(DateTime now) =>
        MaintenanceWindow.Contains(now.TimeOfDay);
        
    public bool IsValidPort(int port) =>
        AllowedPorts.Contains(port);
}
```

### Safe Range Operations

```csharp
public Range<T>? SafeIntersect<T>(Range<T> r1, Range<T> r2) 
    where T : IComparable<T>
{
    return r1.Overlaps(r2) ? r1.Intersect(r2) : null;
}

public Range<T>? SafeUnion<T>(Range<T> r1, Range<T> r2)
    where T : IComparable<T>
{
    if (r1.Overlaps(r2) || r1.IsAdjacent(r2))
        return r1.Union(r2);
    return null;
}
```

### Validation Helpers

```csharp
public static class ValidationRanges
{
    public static readonly Range<int> ValidPort = Range.Closed(1, 65535);
    public static readonly Range<int> ValidPercentage = Range.Closed(0, 100);
    public static readonly Range<double> ValidLatitude = Range.Closed(-90.0, 90.0);
    public static readonly Range<double> ValidLongitude = Range.Closed(-180.0, 180.0);
    public static readonly Range<int> ValidHttpStatus = Range.Closed(100, 599);
}

public void ValidateCoordinates(double lat, double lon)
{
    if (!ValidationRanges.ValidLatitude.Contains(lat))
        throw new ArgumentOutOfRangeException(nameof(lat));
    if (!ValidationRanges.ValidLongitude.Contains(lon))
        throw new ArgumentOutOfRangeException(nameof(lon));
}
```

</details>

## ⚡ Performance

**Intervals.NET is designed for zero allocations and high throughput:**

- **Struct-based design**: Ranges live on the stack, no heap allocations
- **Zero boxing**: Generic constraints eliminate boxing overhead
- **Span-based parsing**: `ReadOnlySpan<char>` for allocation-free parsing
- **Interpolated string handler**: Custom handler eliminates intermediate allocations
- **Inline-friendly**: Small methods optimized for JIT inlining

**Performance characteristics:**
- All operations are O(1) constant time
- Parsing: **3.6× faster** with interpolated strings vs traditional
- Containment checks: **1.7× faster** than naive implementations
- Set operations: **Zero allocations** (100% reduction vs class-based)
- Real-world scenarios: **1.7× faster** for validation hot paths

**Allocation behavior:**
- Construction: **0 bytes** (struct-based)
- Set operations: **0 bytes** (nullable struct returns)
- String parsing (span): **0 bytes**
- Interpolated parsing: **~24 bytes** (unavoidable final string allocation due to CLR design)

**Trade-off:** Some set operations are slower than ultra-simple implementations due to comprehensive edge case validation, generic type support, and production-ready correctness guarantees.

<details>
<summary><strong>Detailed Benchmark Results</strong></summary>

### About These Benchmarks

These benchmarks compare Intervals.NET against a "naive" baseline implementation. The baseline is **simpler but less capable**—hardcoded to `int`, uses nullable types, and has minimal edge case handling.

**Where naive appears faster:** This reflects the cost of generic type support, comprehensive validation, and production-ready edge case handling.

**Where Intervals.NET is faster:** This shows the benefits of modern .NET patterns (spans, aggressive inlining, struct design).

**The allocation story:** Intervals.NET consistently shows zero or near-zero allocations due to struct-based design, while naive uses class-based design (heap allocation).

### Environment
- **Hardware:** Intel Core i7-1065G7
- **Runtime:** .NET 8.0.11
- **Benchmark Tool:** BenchmarkDotNet

---

### Parsing Performance

| Method                          | Mean         | Allocated | vs Baseline                              |
|---------------------------------|--------------|-----------|------------------------------------------|
| **Naive (Baseline)**            | 96.95 ns     | 216 B     | 1.00×                                    |
| **IntervalsNet (String)**       | 44.19 ns     | 0 B       | **2.19× faster, 0% allocation**          |
| **IntervalsNet (Span)**         | 44.78 ns     | 0 B       | **2.17× faster, 0% allocation**          |
| **IntervalsNet (Interpolated)** | **26.90 ns** | 24 B      | **🚀 3.60× faster, 89% less allocation** |
| Traditional Interpolated        | 105.54 ns    | 40 B      | 0.92×                                    |

**Key Insights:**
- ⚡ Interpolated string handler is **3.6× faster** than naive parsing
- 🎯 **Zero-allocation** for span-based parsing
- 📉 **89% allocation reduction** with interpolated strings vs naive
- 💎 Fully inlined - no code size overhead

### Construction Performance

| Method                     | Mean        | Allocated  | vs Baseline                      |
|----------------------------|-------------|------------|----------------------------------|
| **Naive Int (Baseline)**   | 6.90 ns     | 40 B       | 1.00×                            |
| **IntervalsNet Int**       | 8.57 ns     | 0 B        | 0.80×, **100% less allocation**  |
| **IntervalsNet Unbounded** | **0.31 ns** | 0 B        | **🚀 22× faster, 0% allocation** |
| **IntervalsNet DateTime**  | 2.29 ns     | 0 B        | **3× faster, 0% allocation**     |
| NodaTime DateTime          | 0.38 ns     | 0 B        | 18× faster                       |

**Key Insights:**
- 🔥 Unbounded ranges: **22× faster** than naive (nearly free)
- 💪 Struct-based design: **zero heap allocations**
- ⚡ DateTime ranges: **3× faster** than naive

Note: Intervals.NET uses fail-fast constructors that validate range correctness,
which may introduce slight overhead compared to naive or NodaTime implementations that skip validation.

### Containment Checks (Hot Path)

| Method                        | Mean        | vs Baseline         |
|-------------------------------|-------------|---------------------|
| **Naive Contains (Baseline)** | 2.87 ns     | 1.00×               |
| **IntervalsNet Contains**     | **1.67 ns** | **🚀 1.72× faster** |
| **IntervalsNet Boundary**     | 1.75 ns     | **1.64× faster**    |
| NodaTime Contains             | 10.14 ns    | 0.28×               |

**Key Insights:**
- ⚡ **72% faster** for inside checks (hot path)
- 🎯 **64% faster** for boundary checks
- 💎 Zero allocations for all operations

### Set Operations Performance

| Method                         | Mean     | Allocated   | vs Baseline                     |
|--------------------------------|----------|-------------|---------------------------------|
| **Naive Intersect (Baseline)** | 13.77 ns | 40 B        | 1.00×                           |
| **IntervalsNet Intersect**     | 48.19 ns | 0 B         | 0.29×, **100% less allocation** |
| **IntervalsNet Union**         | 46.54 ns | 0 B         | **0% allocation**               |
| **IntervalsNet Overlaps**      | 17.07 ns | 0 B         | **0% allocation**               |

> ⚠️ **IMPORTANT BENCHMARK CAVEAT**
>
> The "naive" baseline is **not functionally equivalent** to Intervals.NET:
> - Uses nullable int (boxing potential on some operations)
> - Simplified edge case handling
> - No generic type support (int-only)
> - No RangeValue abstraction for infinity
> - Less comprehensive boundary validation
>
> **The speed difference reflects:** implementation complexity for correct, generic, edge-case-complete behavior.
>
> **The allocation difference reflects:** fundamental design (struct vs class, RangeValue<T> vs nullable).

**Key Insights:**
- 🎯 **Zero heap allocations** for all set operations
- 💪 Nullable struct return (Range<T>?) - no boxing
- ⚠️ Slower due to **comprehensive edge case handling** and **generic constraints**
- ✅ Handles infinity, all boundary combinations, and generic types correctly

### Real-World Scenarios

| Scenario                           | Naive               | IntervalsNet   | Improvement                        |
|------------------------------------|---------------------|----------------|------------------------------------|
| **Sliding Window (1000 values)**   | 3,039 ns            | 1,781 ns       | **🚀 1.71× faster, 0% allocation** |
| **Overlap Detection (100 ranges)** | 13,592 ns           | 54,676 ns      | 0.25× (see note below)             |
| **Compute Intersections**          | 31,141 ns, 19,400 B | 80,351 ns, 0 B | **🎯 100% less allocation**        |
| **LINQ Filter**                    | 559 ns              | 428 ns         | **1.31× faster**                   |

> ⚠️ **Why Overlap Detection Shows Slower:**
>
> This scenario demonstrates the trade-off between **simple fast code** vs **correct comprehensive code**:
> - **Naive:** Simple overlap check, minimal validation (13,592 ns)
> - **Intervals.NET:** Full edge case handling, generic constraints, comprehensive validation (54,676 ns)
>
> **What you get for the extra 41µs over 100 ranges:**
> - ✅ Handles infinity correctly
> - ✅ All boundary combinations validated
> - ✅ Works with any `IComparable<T>`, not just int
> - ✅ Production-ready correctness
>
> **Per operation:** 410 ns difference (~0.0004 milliseconds) - negligible in most scenarios.

**Key Insights:**
- ⚡ **71% faster** for validation hot paths (sliding window)
- 💎 **Zero allocations** in intersection computations (vs 19 KB)
- 🔥 **31% faster** in LINQ scenarios
- ⚠️ **Some scenarios slower** due to comprehensive correctness (acceptable trade-off for production use)

---

### Performance Summary

```
🚀 Parsing:      3.6× faster with interpolated strings
💎 Construction: 0 bytes allocated (struct-based)
⚡ Containment:   1.7× faster for hot path validation
🎯 Set Ops:       0 bytes allocated (100% reduction)
🔥 Real-World:    1.7× faster for sliding windows
```

**Design Trade-offs:**
- **Slower set operations** → Comprehensive edge case handling, generic constraints, infinity support
- **Struct-based design** → Zero heap allocations, better cache locality
- **Fail-fast validation** → Catches errors early, slight construction overhead vs unsafe implementations
- **Generic over IComparable<T>** → Works with any type, adds minimal constraint overhead

---

### Understanding "Naive" Baseline

The naive implementation represents a **typical developer implementation** without:
- Generic type support (hardcoded to `int`)
- Comprehensive infinity handling (uses nullable)
- Full edge case validation
- Modern .NET performance patterns (spans, handlers)

**What Intervals.NET adds:**
- ✅ Generic over any `IComparable<T>` (not just int)
- ✅ Explicit infinity representation (`RangeValue<T>`)
- ✅ Comprehensive boundary validation (all combinations)
- ✅ Zero boxing (even with nullable structs)
- ✅ Span-based parsing (zero allocation)
- ✅ `InterpolatedStringHandler` (revolutionary)
- ✅ Production-ready edge case handling

**Recommendation:** Don't choose based solely on raw benchmark numbers. Intervals.NET's correctness, zero-allocation design, and feature completeness outweigh nanosecond differences in set operations for production code.

**Run benchmarks yourself:**
```bash
cd benchmarks/Intervals.NET.Benchmarks
dotnet run -c Release
```

**View detailed results:** [benchmarks/Results](benchmarks/Results)

</details>

## 🧪 Testing & Quality

**100% test coverage** across all public APIs. Unit tests serve as executable documentation and cover:
- All range construction patterns
- Edge cases (infinity, empty, adjacent, overlapping)
- Boundary conditions (inclusive/exclusive combinations)
- Set operations (intersection, union, except)
- Parsing (strings, spans, interpolated strings, cultures)
- Custom comparable types

**Test projects:**
- `RangeStructTests.cs` - Core Range<T> functionality
- `RangeValueTests.cs` - RangeValue<T> and infinity handling
- `RangeExtensionsTests.cs` - Extension method behavior
- `RangeFactoryTests.cs` - Factory method patterns
- `RangeStringParserTests.cs` - String parsing edge cases
- `RangeInterpolatedStringParserTests.cs` - Interpolated string handler

Run tests:
```bash
dotnet test
```

## API Reference

### Factory Methods

```csharp
// Create ranges with different boundary inclusivity
Range.Closed<T>(start, end)      // [start, end]
Range.Open<T>(start, end)        // (start, end)
Range.ClosedOpen<T>(start, end)  // [start, end)
Range.OpenClosed<T>(start, end)  // (start, end]

// Parse from string representations
Range.FromString<T>(string input, IFormatProvider? provider = null)
Range.FromString<T>(ReadOnlySpan<char> input, IFormatProvider? provider = null)
Range.FromString<T>($"[{start}, {end}]")  // Interpolated (optimized)
```

### Range Properties

```csharp
range.Start                // RangeValue<T> - Start boundary
range.End                  // RangeValue<T> - End boundary
range.IsStartInclusive     // bool - Start boundary inclusivity
range.IsEndInclusive       // bool - End boundary inclusivity
```

### Extension Methods

```csharp
// Containment checks
range.Contains(value)           // bool - Value in range?
range.Contains(otherRange)      // bool - Range fully contained?

// Set operations
range.Intersect(other)          // Range<T>? - Overlapping region
range.Union(other)              // Range<T>? - Combined range (if adjacent/overlapping)
range.Except(other)             // IEnumerable<Range<T>> - Subtraction (0-2 ranges)
range.Overlaps(other)           // bool - Ranges share any values?

// Relationships
range.IsAdjacent(other)         // bool - Share boundary but don't overlap?
range.IsBefore(other)           // bool - Entirely before other?
range.IsAfter(other)            // bool - Entirely after other?

// Properties
range.IsBounded()               // bool - Both boundaries finite?
range.IsUnbounded()             // bool - Any boundary infinite?
range.IsInfinite()              // bool - Both boundaries infinite?
range.IsEmpty()                 // bool - No values in range? (always false)
```

### Operators

```csharp
var intersection = range1 & range2;  // Same as range1.Intersect(range2)
var union = range1 | range2;         // Same as range1.Union(range2)
```

### RangeValue<T> API

```csharp
// Static infinity values
RangeValue<T>.PositiveInfinity
RangeValue<T>.NegativeInfinity

// Instance properties
value.IsFinite               // bool
value.IsPositiveInfinity     // bool
value.IsNegativeInfinity     // bool
value.Value                  // T (throws if infinite)
value.TryGetValue(out T val) // bool - Safe extraction
```

### RangeParser API

```csharp
// Safe parsing
RangeParser.TryParse<T>(string input, out Range<T> result)
RangeParser.TryParse<T>(ReadOnlySpan<char> input, out Range<T> result)
RangeParser.TryParse<T>(string input, IFormatProvider provider, out Range<T> result)
```

## 💎 Best Practices

<details>
<summary><strong>✅ Do's and ❌ Don'ts</strong></summary>

### ✅ Do's

```csharp
// DO: Use appropriate inclusivity for your domain
var age = Range.ClosedOpen(0, 18);  // 0 ≤ age < 18 (excludes 18)

// DO: Use infinity for unbounded ranges
var positive = Range.Closed(0, RangeValue<int>.PositiveInfinity);

// DO: Check HasValue for nullable results
var intersection = range1.Intersect(range2);
if (intersection.HasValue)
{
    ProcessRange(intersection.Value);
}

// DO: Use TryParse for untrusted input
if (RangeParser.TryParse<int>(userInput, out var range))
{
    // Use range safely
}

// DO: Use factory methods for clarity
var range = Range.Closed(1, 10);  // Intent is clear

// DO: Use span-based parsing when allocations matter
var range = Range.FromString<int>("[1, 10]".AsSpan());
```

### ❌ Don'ts

```csharp
// DON'T: Create invalid ranges (throws ArgumentException)
// var invalid = Range.Closed(20, 10);  // start > end

// DON'T: Assume union/intersect always succeed
var union = range1.Union(range2);
// Always check union.HasValue!

// DON'T: Ignore culture for parsing decimals
// var bad = Range.FromString<double>("[1,5, 9,5]");  // Depends on current culture!
// var bad = Range.FromString<double>("[1.5, 9.5]", CultureInfo.GetCultureInfo("de-DE"));  // Depends on provided culture!
var good = Range.FromString<double>("[1,5, 9,5]", CultureInfo.GetCultureInfo("de-DE"));

// DON'T: Box ranges unnecessarily
// object boxed = range;  // Avoid boxing structs
```

</details>

## 🆚 Why Use Intervals.NET?

### vs. Manual Implementation

| Aspect         | Intervals.NET             | Manual Implementation   |
|----------------|---------------------------|-------------------------|
| Type Safety    | ✅ Generic constraints     | ⚠️ Must implement       |
| Edge Cases     | ✅ All handled (100% test) | ❌ Often forgotten      |
| Infinity       | ✅ Built-in, explicit      | ❌ Nullable or custom   |
| Parsing        | ✅ Span + interpolated     | ❌ Must implement       |
| Set Operations | ✅ Rich API (6+ methods)   | ❌ Must implement       |
| Allocations    | ✅ Zero (struct-based)     | ⚠️ Usually class-based  |
| Testing        | ✅ 100% coverage           | ⚠️ Your responsibility  |

### vs. Other Libraries

**Intervals.NET excels at:**
- Zero-allocation design (struct-based)
- Modern C# features (spans, interpolated string handlers)
- Explicit infinity semantics
- Generic type support with fail-fast validation
- Production-ready correctness over raw speed

## 🤝 Contributing

Contributions are welcome! Please:
1. Open an issue to discuss major changes
2. Follow existing code style and conventions
3. Add tests for new functionality
4. Update documentation as needed

### Development

**Requirements:**
- .NET 8.0 SDK or later
- Any compatible IDE (Visual Studio, Rider, VS Code)

**Build:**
```bash
dotnet build
```

**Run tests:**
```bash
dotnet test
```

**Run benchmarks:**
```bash
cd benchmarks/Intervals.NET.Benchmarks
dotnet run -c Release
```

## 📄 License

MIT License - see [LICENSE](LICENSE) file for details.

## 📖 Resources

- [Mathematical Interval Notation](https://en.wikipedia.org/wiki/Interval_(mathematics))
- [.NET Generic Constraints](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/constraints-on-type-parameters)
- [InterpolatedStringHandler Attribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.interpolatedstringhandlerattribute)

---

**Built with modern C# for the .NET community**