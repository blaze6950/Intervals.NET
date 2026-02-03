<div align="center">

# 📏

# Intervals.NET

**Type-safe mathematical intervals and ranges for .NET**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Intervals.NET.svg)](https://www.nuget.org/packages/Intervals.NET/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Intervals.NET.svg)](https://www.nuget.org/packages/Intervals.NET/)
[![Build - Intervals.NET](https://img.shields.io/github/actions/workflow/status/blaze6950/Intervals.NET/intervals-net.yml?branch=main&label=Intervals.NET)](https://github.com/blaze6950/Intervals.NET/actions/workflows/intervals-net.yml)
[![Build - Domain.Abstractions](https://img.shields.io/github/actions/workflow/status/blaze6950/Intervals.NET/domain-abstractions.yml?branch=main&label=Domain.Abstractions)](https://github.com/blaze6950/Intervals.NET/actions/workflows/domain-abstractions.yml)
[![Build - Domain.Default](https://img.shields.io/github/actions/workflow/status/blaze6950/Intervals.NET/domain-default.yml?branch=main&label=Domain.Default)](https://github.com/blaze6950/Intervals.NET/actions/workflows/domain-default.yml)
[![Build - Domain.Extensions](https://img.shields.io/github/actions/workflow/status/blaze6950/Intervals.NET/domain-extensions.yml?branch=main&label=Domain.Extensions)](https://github.com/blaze6950/Intervals.NET/actions/workflows/domain-extensions.yml)

</div>

---

A production-ready .NET library for working with mathematical intervals and ranges. Designed for correctness, performance, and zero allocations.

**Intervals.NET** provides robust, type-safe interval operations over any `IComparable<T>`. Whether you're validating business rules, scheduling time windows, or filtering numeric data, this library delivers correct range semantics with comprehensive edge case handling—without heap allocations.

<div align="center">

**Key characteristics:**

✅ **Correctness first**: Explicit infinity, validated boundaries, fail-fast construction

⚡ **Zero-allocation design**: Struct-based API, no boxing, stack-allocated ranges

🎯 **Generic and expressive**: Works with int, double, DateTime, TimeSpan, strings, custom types

🛡️ **Real-world ready**: 100% test coverage, battle-tested edge cases, production semantics

</div>

## 📑 Table of Contents

- [Installation](#-installation)
- [Understanding Intervals](#-understanding-intervals) 👈 *Start here if new to intervals*
  - [What Are Intervals?](#what-are-intervals)
  - [Visual Guide](#visual-guide)
  - [Mathematical Foundation](#mathematical-foundation) *(collapsible)*
  - [When to Use Intervals](#when-to-use-intervals) *(collapsible)*
- [Quick Start](#-quick-start)
  - [Getting Started Guide](#getting-started-guide) *(collapsible)*
- [Real-World Use Cases](#-real-world-use-cases) 👈 *Click to expand examples*
- [Core Concepts](#-core-concepts)
  - [Range Notation](#range-notation)
  - [Infinity Support](#infinity-support)
- [API Overview](#-api-overview)
  - [Creating Ranges](#creating-ranges)
  - [Containment Checks](#containment-checks)
  - [Set Operations](#set-operations)
  - [Range Relationships](#range-relationships)
  - [Parsing from Strings](#parsing-from-strings)
  - [Zero-Allocation Parsing](#zero-allocation-parsing)
  - [Working with Custom Types](#working-with-custom-types)
  - [Domain Extensions](#domain-extensions) 👈 *NEW: Step-based operations*
  - [Advanced Usage Examples](#advanced-usage-examples) 👈 *Click to expand*
- [RangeData Library](#rangedata-library) 👈 *Click to expand*
- [Performance](#-performance)
  - [Detailed Benchmark Results](#detailed-benchmark-results) 👈 *Click to expand*
- [Testing & Quality](#-testing--quality)
- [API Reference](#api-reference)
- [Best Practices](#-best-practices) 👈 *Click to expand*
- [Why Use Intervals.NET?](#-why-use-intervalsnet)
- [Contributing](#-contributing)
- [License](#-license)
- [Resources](#-resources)

> 💡 **Tip**: Look for sections marked with 👈 or **▶ Click to expand** — they contain detailed examples and advanced content!

## 📦 Installation

<div align="center">

```bash
dotnet add package Intervals.NET
```

</div>

---

## 📐 Understanding Intervals

### What Are Intervals?

An **interval** (or range) is a mathematical concept representing all values between two endpoints. In programming, intervals provide a precise way to express continuous or discrete value ranges with explicit boundary behavior—whether endpoints are included or excluded.

**Why intervals matter:** They transform implicit boundary logic scattered across conditionals into explicit, reusable, testable data structures. Instead of `if (x >= 10 && x <= 20)`, you write `range.Contains(x)`.

**Common applications:** Date ranges, numeric validation, time windows, pricing tiers, access control, scheduling conflicts, data filtering, and any domain requiring boundary semantics.

---

### Visual Guide

Understanding boundary inclusivity is crucial. Here's how the four interval types work:

```
Number Line: ... 8 --- 9 --- 10 --- 11 --- 12 --- 13 --- 14 --- 15 --- 16 ...

Closed Interval [10, 15]
    Includes both endpoints (10 and 15)
    ●━━━━━━━━━━━━━━━━━━━━━━━━━━●
    10                         15
    Values: {10, 11, 12, 13, 14, 15}
    Code: Range.Closed(10, 15)

Open Interval (10, 15)
    Excludes both endpoints
    ○━━━━━━━━━━━━━━━━━━━━━━━━━━○
    10                         15
    Values: {11, 12, 13, 14}
    Code: Range.Open(10, 15)

Half-Open Interval [10, 15)
    Includes start (10), excludes end (15)
    ●━━━━━━━━━━━━━━━━━━━━━━━━━━○
    10                         15
    Values: {10, 11, 12, 13, 14}
    Code: Range.ClosedOpen(10, 15)
    Common for: Array indices, iteration bounds

Half-Closed Interval (10, 15]
    Excludes start (10), includes end (15)
    ○━━━━━━━━━━━━━━━━━━━━━━━━━━●
    10                         15
    Values: {11, 12, 13, 14, 15}
    Code: Range.OpenClosed(10, 15)

Legend: ● = included endpoint  ○ = excluded endpoint  ━ = values in range
```

**Unbounded intervals** use infinity (∞) to represent ranges with no upper or lower limit:

```
Positive Unbounded [18, ∞)
    All values from 18 onwards
    ●━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━→
    18                                  ∞
    Code: Range.Closed(18, RangeValue<int>.PositiveInfinity)
    Example: Adult age ranges

Negative Unbounded (-∞, 0)
    All values before 0
    ←━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━○
    -∞                                  0
    Code: Range.Open(RangeValue<int>.NegativeInfinity, 0)
    Example: Historical dates

Fully Unbounded (-∞, ∞)
    All possible values
    ←━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━→
    -∞                                  ∞
    Code: Range.Open(RangeValue<T>.NegativeInfinity, RangeValue<T>.PositiveInfinity)
```

---

<details>
<summary><strong>▶ Click to expand: Mathematical Foundation</strong></summary>

> 🎓 **Deep Dive:** Mathematical theory behind intervals

### Set Theory Perspective

In mathematics, an interval is a **convex subset** of an ordered set. For real numbers:

- **Closed interval:** `[a, b] = {x ∈ ℝ : a ≤ x ≤ b}`
- **Open interval:** `(a, b) = {x ∈ ℝ : a < x < b}`
- **Half-open:** `[a, b) = {x ∈ ℝ : a ≤ x < b}`
- **Half-closed:** `(a, b] = {x ∈ ℝ : a < x ≤ b}`

Where `∈` means "is an element of" and `ℝ` represents all real numbers.

### Key Properties

**Convexity:** If two values are in an interval, all values between them are also in the interval.
- If `x ∈ I` and `y ∈ I`, then for all `z` where `x < z < y`, we have `z ∈ I`
- This property distinguishes intervals from arbitrary sets

**Ordering:** Intervals require an ordering relation (≤) on elements.
- In Intervals.NET, this is enforced via `IComparable<T>` constraint
- Enables intervals over integers, decimals, dates, times, and custom types

**Boundary Semantics:** The crucial distinction between interval types:
- **Closed boundaries** satisfy `≤` (less than or equal)
- **Open boundaries** satisfy `<` (strictly less than)
- Mixed boundaries combine both semantics

### Set Operations

Intervals support standard set operations:

**Intersection (∩):** `A ∩ B` contains values in both A and B
```
[10, 30] ∩ [20, 40] = [20, 30]
```

**Union (∪):** `A ∪ B` combines A and B (only if contiguous/overlapping)
```
[10, 30] ∪ [20, 40] = [10, 40]
[10, 20] ∪ [30, 40] = undefined (disjoint)
```

**Difference (∖):** `A ∖ B` contains values in A but not in B
```
[10, 30] ∖ [20, 40] = [10, 20)
```

**Containment (⊆):** `A ⊆ B` means A is fully contained within B
```
[15, 25] ⊆ [10, 30] = true
```

### Domain Theory

Intervals operate over **continuous** or **discrete** domains:

**Continuous domains** (ℝ, floating-point):
- Infinite values between any two points
- Open/closed boundaries have subtle differences
- Example: Temperature ranges, probabilities

**Discrete domains** (ℤ, integers):
- Finite values between points
- `(10, 15)` and `[11, 14]` are equivalent in integers
- Example: Array indices, counts, discrete time units

**Hybrid domains** (DateTime, calendar):
- Continuous representation (ticks) with discrete semantics (days)
- Domain extensions handle granularity (see [Domain Extensions](#domain-extensions))

### Why Explicit Boundaries Matter

Consider age validation:

```csharp
// Ambiguous: Is 18 adult or minor?
if (age >= 18) { /* adult */ }

// Explicit: Minor range excludes 18
var minorRange = Range.ClosedOpen(0, 18);  // [0, 18) - 18 is NOT included
minorRange.Contains(17); // true
minorRange.Contains(18); // false - unambiguous!
```

**Correctness through precision:** Explicit boundary semantics eliminate entire classes of off-by-one errors.

</details>

---

<details>
<summary><strong>▶ Click to expand: When to Use Intervals</strong></summary>

> 🎯 **Decision Guide:** Choosing the right tool for the job

### ✅ Use Intervals When You Need

**Boundary Validation**
- ✅ Port numbers must be 1-65535
- ✅ Percentage values must be 0.0-100.0
- ✅ Age must be 0-150
- ✅ HTTP status codes must be 100-599

```csharp
var validPort = Range.Closed(1, 65535);
if (!validPort.Contains(port))
    throw new ArgumentOutOfRangeException(nameof(port));
```

**Time Window Operations**
- ✅ Business hours: 9 AM - 5 PM
- ✅ Meeting conflict detection
- ✅ Booking/reservation overlaps
- ✅ Rate limiting time windows
- ✅ Maintenance windows

```csharp
var meeting1 = Range.Closed(startTime1, endTime1);
var meeting2 = Range.Closed(startTime2, endTime2);
if (meeting1.Overlaps(meeting2))
    throw new InvalidOperationException("Meetings conflict!");
```

**Tiered Systems**
- ✅ Pricing tiers based on quantity
- ✅ Discount brackets
- ✅ Age demographics
- ✅ Performance bands
- ✅ Risk categories

```csharp
var tier1 = Range.ClosedOpen(0, 100);     // 0-99 units
var tier2 = Range.ClosedOpen(100, 500);   // 100-499 units
var tier3 = Range.Closed(500, RangeValue<int>.PositiveInfinity);  // 500+
```

**Range Queries**
- ✅ Filter data by date range
- ✅ Find values within bounds
- ✅ Temperature/sensor thresholds
- ✅ Geographic bounding boxes (with lat/lon)

```csharp
var criticalTemp = Range.Closed(50.0, RangeValue<double>.PositiveInfinity);
var alerts = readings.Where(r => criticalTemp.Contains(r.Temperature));
```

**Complex Scheduling**
- ✅ Shift patterns
- ✅ Seasonal pricing
- ✅ Access control windows
- ✅ Feature flag rollouts
- ✅ Sliding time windows

### ❌ Don't Use Intervals When

**Simple Equality Checks**
- ❌ Checking if value equals specific constant
- ❌ Boolean flags
- ❌ Enum matching
- **Use:** Direct equality (`==`) or switch expressions

**Discrete Set Membership**
- ❌ Value must be one of {1, 5, 9, 15} (non-contiguous)
- ❌ Allowed values: {"admin", "user", "guest"}
- ❌ Valid status codes: {200, 201, 204} only
- **Use:** `HashSet<T>`, arrays, or enum flags

**Complex Non-Convex Regions**
- ❌ Multiple disjoint ranges: [1-10] OR [50-60] OR [100-110]
- ❌ Exclusion ranges: All values EXCEPT [20-30]
- ❌ Irregular polygons, non-continuous shapes
- **Use:** Collections of intervals, custom predicates, or spatial libraries

**Performance-Critical Simple Comparisons**
- ❌ Ultra-hot path with single boundary check: `x >= min`
- ❌ JIT-sensitive tight loops with minimal logic
- **Use:** Direct comparison (though benchmark first—intervals may inline!)

### Decision Flowchart

```
Do you need to check if a value falls within boundaries?
├─ YES → Are the boundaries continuous/contiguous?
│        ├─ YES → Are boundary semantics important (inclusive/exclusive)?
│        │        ├─ YES → ✅ USE INTERVALS.NET
│        │        └─ NO  → ⚠️ Consider intervals for clarity anyway
│        └─ NO  → Are there multiple disjoint ranges?
│                 ├─ YES → Use List<Range<T>> or custom logic
│                 └─ NO  → Use HashSet<T> or enum
└─ NO → Use direct equality or boolean logic
```

### Real-World Pattern Recognition

**You probably need intervals if your code has:**
- Multiple `if (x >= a && x <= b)` checks
- Scattered boundary validation logic
- Date/time overlap detection
- Tiered pricing/categorization
- Scheduling conflict detection
- Range-based filtering in LINQ
- Off-by-one errors in boundary conditions

**Example transformation:**

```csharp
// ❌ Before: Scattered, error-prone
if (age >= 0 && age < 13) return "Child";
if (age >= 13 && age < 18) return "Teen";  // Bug: overlaps at 13!
if (age >= 18) return "Adult";

// ✅ After: Explicit, testable, reusable
var childRange = Range.ClosedOpen(0, 13);   // [0, 13)
var teenRange = Range.ClosedOpen(13, 18);   // [13, 18)
var adultRange = Range.Closed(18, RangeValue<int>.PositiveInfinity);

if (childRange.Contains(age)) return "Child";
if (teenRange.Contains(age)) return "Teen";
if (adultRange.Contains(age)) return "Adult";
```

</details>

---

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

---

<details>
<summary><strong>▶ Click to expand: Getting Started Guide</strong></summary>

> 🎓 **Complete walkthrough** from problem to solution

### Scenario: E-Commerce Discount System

**Problem:** You need to apply different discount rates based on order totals:
- Orders under $100: No discount
- Orders $100-$499.99: 10% discount
- Orders $500+: 15% discount

**Traditional approach (error-prone):**

```csharp
// ❌ Problems: Magic numbers, duplicate boundaries, easy to introduce gaps/overlaps
decimal GetDiscount(decimal orderTotal)
{
    if (orderTotal < 100) return 0m;
    if (orderTotal >= 100 && orderTotal < 500) return 0.10m;
    if (orderTotal >= 500) return 0.15m;
    return 0m;  // Unreachable but needed for compiler
}
```

**Issues with traditional approach:**
- Boundary value `100` appears twice (DRY violation)
- Easy to create gaps: What if someone writes `orderTotal > 100` instead of `>=`?
- Easy to create overlaps at boundaries
- Not reusable—logic is embedded in function
- Hard to test—can't validate ranges independently
- No explicit handling of edge cases (negative values, infinity)

---

**Intervals.NET approach (explicit, testable, reusable):**

```csharp
using Intervals.NET.Factories;

// ✅ Step 1: Define your ranges explicitly (declare once, reuse everywhere)
public static class DiscountTiers
{
    // No discount tier: $0 to just under $100
    public static readonly Range<decimal> NoDiscount = 
        Range.ClosedOpen(0m, 100m);  // [0, 100)
    
    // Standard discount tier: $100 to just under $500
    public static readonly Range<decimal> StandardDiscount = 
        Range.ClosedOpen(100m, 500m);  // [100, 500)
    
    // Premium discount tier: $500 and above
    public static readonly Range<decimal> PremiumDiscount = 
        Range.Closed(500m, RangeValue<decimal>.PositiveInfinity);  // [500, ∞)
}

// ✅ Step 2: Use ranges for clear, readable logic
decimal GetDiscount(decimal orderTotal)
{
    if (DiscountTiers.NoDiscount.Contains(orderTotal)) return 0m;
    if (DiscountTiers.StandardDiscount.Contains(orderTotal)) return 0.10m;
    if (DiscountTiers.PremiumDiscount.Contains(orderTotal)) return 0.15m;
    
    // Invalid input (negative, NaN, etc.)
    throw new ArgumentOutOfRangeException(nameof(orderTotal), 
        $"Order total must be non-negative: {orderTotal}");
}

// ✅ Step 3: Easy to extend with additional features
decimal CalculateFinalPrice(decimal orderTotal)
{
    var discount = GetDiscount(orderTotal);
    var discountAmount = orderTotal * discount;
    var finalPrice = orderTotal - discountAmount;
    
    Console.WriteLine($"Order Total: {orderTotal:C}");
    Console.WriteLine($"Discount: {discount:P0}");
    Console.WriteLine($"You Save: {discountAmount:C}");
    Console.WriteLine($"Final Price: {finalPrice:C}");
    
    return finalPrice;
}
```

**Try it out:**

```csharp
CalculateFinalPrice(50m);    // No discount
// Order Total: $50.00
// Discount: 0%
// Final Price: $50.00

CalculateFinalPrice(150m);   // 10% discount
// Order Total: $150.00
// Discount: 10%
// You Save: $15.00
// Final Price: $135.00

CalculateFinalPrice(600m);   // 15% discount
// Order Total: $600.00
// Discount: 15%
// You Save: $90.00
// Final Price: $510.00
```

---

**Benefits achieved:**

✅ **No boundary duplication** - Each boundary defined once  
✅ **No gaps or overlaps** - Ranges are explicitly defined  
✅ **Reusable** - `DiscountTiers` can be used across application  
✅ **Testable** - Can unit test ranges independently  
✅ **Self-documenting** - Range names explain business rules  
✅ **Type-safe** - Works with decimal, int, DateTime, etc.  
✅ **Explicit infinity** - Clear unbounded upper limit  

---

**Testing your ranges:**

```csharp
[Test]
public void DiscountTiers_ShouldNotOverlap()
{
    // Verify no overlaps between tiers
    Assert.IsFalse(DiscountTiers.NoDiscount.Overlaps(DiscountTiers.StandardDiscount));
    Assert.IsFalse(DiscountTiers.StandardDiscount.Overlaps(DiscountTiers.PremiumDiscount));
}

[Test]
public void DiscountTiers_ShouldBeAdjacent()
{
    // Verify tiers are properly adjacent (no gaps)
    Assert.IsTrue(DiscountTiers.NoDiscount.IsAdjacent(DiscountTiers.StandardDiscount));
    Assert.IsTrue(DiscountTiers.StandardDiscount.IsAdjacent(DiscountTiers.PremiumDiscount));
}

[Test]
public void DiscountTiers_BoundaryValues()
{
    // Verify boundary behavior
    Assert.IsTrue(DiscountTiers.NoDiscount.Contains(99.99m));
    Assert.IsFalse(DiscountTiers.NoDiscount.Contains(100m));
    Assert.IsTrue(DiscountTiers.StandardDiscount.Contains(100m));
    Assert.IsFalse(DiscountTiers.StandardDiscount.Contains(500m));
    Assert.IsTrue(DiscountTiers.PremiumDiscount.Contains(500m));
}
```

**Key Insight:** Intervals transform boundary logic from imperative conditionals into declarative, testable data structures—making your code more maintainable and less error-prone.

</details>

---

## 💼 Real-World Use Cases

<details>
<summary><strong>▶ Click to expand: 8 Real-World Scenarios</strong></summary>

> 📖 **Inside this section:**
> - Scheduling & Calendar Systems
> - Booking Systems & Resource Allocation
> - Validation & Configuration
> - Pricing Tiers & Discounts
> - Access Control & Time Windows
> - Data Filtering & Analytics
> - Sliding Window Validation

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

</details>

## 🔑 Core Concepts

<details open>
<summary><strong>▶ Range Notation</strong></summary>

Intervals.NET uses standard mathematical interval notation:

| Notation | Name        | Meaning                      | Example Code                  |
|----------|-------------|------------------------------|-------------------------------|
| `[a, b]` | Closed      | Includes both `a` and `b`    | `Range.Closed(1, 10)`         |
| `(a, b)` | Open        | Excludes both `a` and `b`    | `Range.Open(0, 100)`          |
| `[a, b)` | Half-open   | Includes `a`, excludes `b`   | `Range.ClosedOpen(1, 10)`     |
| `(a, b]` | Half-closed | Excludes `a`, includes `b`   | `Range.OpenClosed(1, 10)`     |

</details>

<details open>
<summary><strong>▶ Infinity Support</strong></summary>

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

</details>

---

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

---

### Domain Extensions

**Domain extensions** bridge the gap between continuous ranges and discrete step-based operations. A **domain** (`IRangeDomain<T>`) defines how to work with discrete points within a continuous value space, enabling operations like counting discrete values in a range, shifting boundaries by steps, and expanding ranges proportionally.

#### 📦 Installation

```bash
dotnet add package Intervals.NET.Domain.Abstractions
dotnet add package Intervals.NET.Domain.Default
dotnet add package Intervals.NET.Domain.Extensions
```

#### 🎯 Core Concepts: What is a Domain?

A **domain** is an abstraction that transforms continuous value spaces into discrete step-based systems. It provides:

**Discrete Point Operations:**
- **`Add(value, steps)`** - Navigate forward/backward by discrete steps
- **`Subtract(value, steps)`** - Convenience method for backward navigation
- **`Distance(start, end)`** - Calculate the number of discrete steps between values

**Boundary Alignment:**
- **`Floor(value)`** - Round down to the nearest discrete step boundary
- **`Ceiling(value)`** - Round up to the nearest discrete step boundary

**Why Domains Matter:**

Think of a domain as a "ruler" that defines measurement units and tick marks:
- **Integer domain**: Every integer is a discrete point (ruler marked 1, 2, 3, ...)
- **Day domain**: Each day boundary is a discrete point (midnight transitions)
- **Month domain**: Each month start is a discrete point (variable-length "ticks")
- **Business day domain**: Only weekdays are discrete points (weekends skipped)

**Two Domain Types:**

| Type | Interface | Distance Complexity | Step Size | Examples |
|------|-----------|---------------------|-----------|----------|
| **Fixed-Step** | `IFixedStepDomain<T>` | O(1) - Constant time | Uniform | Integers, days, hours, minutes |
| **Variable-Step** | `IVariableStepDomain<T>` | O(N) - May iterate | Non-uniform | Months (28-31 days), business days |

**Extension Methods Connect Domains to Ranges:**

Domains alone work with individual values. Extension methods combine domains with ranges to enable:
- **`Span(domain)`** - Count discrete points within a range (returns `long` for fixed, `double` for variable)
- **`Shift(domain, offset)`** - Move range boundaries by N steps
- **`Expand(domain, left, right)`** - Expand/contract range by fixed step counts
- **`ExpandByRatio(domain, leftRatio, rightRatio)`** - Proportional expansion based on span

#### 🔢 Quick Example - Integer Domain

```csharp
using Intervals.NET.Domain.Default.Numeric;
using Intervals.NET.Domain.Extensions.Fixed;

var range = Range.Closed(10, 20);  // [10, 20] - continuous range
var domain = new IntegerFixedStepDomain();  // Defines discrete integer steps

// Span: Count discrete integer values within the range
var span = range.Span(domain);  // 11 discrete values: {10, 11, 12, ..., 19, 20}

// The domain defines the "measurement units" for the range:
// - Floor/Ceiling align values to integer boundaries (already aligned for integers)
// - Distance calculates steps between boundaries
// - Extension method Span() uses domain operations to count discrete points

// Expand range by 50% on each side (50% of 11 values = 5 steps on each side)
var expanded = range.ExpandByRatio(domain, 0.5, 0.5);  // [5, 25]
// [10, 20] → span of 11 → 11 * 0.5 = 5.5 → truncated to 5 steps
// Left: 10 - 5 = 5; Right: 20 + 5 = 25

// Shift range forward by 5 discrete integer steps
var shifted = range.Shift(domain, 5);  // [15, 25]
```

#### 📅 DateTime Example - Day Granularity

```csharp
using Intervals.NET.Domain.Default.DateTime;
using Intervals.NET.Domain.Extensions.Fixed;

var week = Range.Closed(
    new DateTime(2026, 1, 20, 14, 30, 0),  // Tuesday 2:30 PM
    new DateTime(2026, 1, 26, 9, 15, 0)    // Monday 9:15 AM
);
var dayDomain = new DateTimeDayFixedStepDomain();

// Domain discretizes continuous DateTime into day boundaries
// Floor/Ceiling align to midnight: Jan 20 00:00, Jan 21 00:00, ..., Jan 26 00:00

// Count complete day boundaries within the range
var days = week.Span(dayDomain);  // 7 discrete day boundaries
// Includes: Jan 20, 21, 22, 23, 24, 25, 26 (7 days)

// Expand by 1 day boundary on each side
var expanded = week.Expand(dayDomain, left: 1, right: 1);
// Adds 1 day step to start: Jan 19 14:30 PM
// Adds 1 day step to end: Jan 27 9:15 AM
// Preserves original times within the day!

// Key insight: Domain defines "what is a discrete step"
// - Day domain: midnight boundaries are steps
// - Hour domain: top-of-hour boundaries are steps  
// - Month domain: first-of-month boundaries are steps
```

#### 💼 Business Days Example - Variable-Step Domain

```csharp
using Intervals.NET.Domain.Default.Calendar;
using Intervals.NET.Domain.Extensions.Variable;

var workWeek = Range.Closed(
    new DateTime(2026, 1, 20),  // Tuesday
    new DateTime(2026, 1, 26)   // Monday (next week)
);
var businessDayDomain = new StandardDateTimeBusinessDaysVariableStepDomain();

// Variable-step domain: weekends are skipped, only weekdays count
// Domain logic: Floor/Ceiling align to nearest business day boundary
// Distance calculation: May iterate through range checking each day

// Count only business days (Mon-Fri, skips Sat/Sun)
var businessDays = workWeek.Span(businessDayDomain);  // 5.0 discrete business days
// Includes: Jan 20 (Tue), 21 (Wed), 22 (Thu), 23 (Fri), 26 (Mon)
// Excludes: Jan 24 (Sat), 25 (Sun) - not in domain's discrete point set

// Add 3 business day steps - domain automatically skips weekends
var deadline = businessDayDomain.Add(new DateTime(2026, 1, 23), 3);  
// Jan 23 (Fri) + 3 business days = Jan 28 (Wed)
// Calculation: Fri 23 → Mon 26 → Tue 27 → Wed 28

// Why variable-step? 
// - The "distance" between Friday and Monday is 1 business day, not 3 calendar days
// - Step size varies based on position (weekday-to-weekday vs crossing weekend)
// - Distance() may need to iterate to count actual business days
```

#### 📊 Available Domains (36 Total)

<details>
<summary><strong>▶ Numeric Domains</strong> (11 domains - all O(1))</summary>

```csharp
using Intervals.NET.Domain.Default.Numeric;

new IntegerFixedStepDomain();        // int, step = 1
new LongFixedStepDomain();           // long, step = 1
new ShortFixedStepDomain();          // short, step = 1
new ByteFixedStepDomain();           // byte, step = 1
new SByteFixedStepDomain();          // sbyte, step = 1
new UIntFixedStepDomain();           // uint, step = 1
new ULongFixedStepDomain();          // ulong, step = 1
new UShortFixedStepDomain();         // ushort, step = 1
new FloatFixedStepDomain();          // float, step = 1.0f
new DoubleFixedStepDomain();         // double, step = 1.0
new DecimalFixedStepDomain();        // decimal, step = 1.0m
```

</details>

<details>
<summary><strong>▶ DateTime Domains</strong> (9 domains - all O(1))</summary>

```csharp
using Intervals.NET.Domain.Default.DateTime;

new DateTimeDayFixedStepDomain();           // Step = 1 day
new DateTimeHourFixedStepDomain();          // Step = 1 hour
new DateTimeMinuteFixedStepDomain();        // Step = 1 minute
new DateTimeSecondFixedStepDomain();        // Step = 1 second
new DateTimeMillisecondFixedStepDomain();   // Step = 1 millisecond
new DateTimeMicrosecondFixedStepDomain();   // Step = 1 microsecond
new DateTimeTicksFixedStepDomain();         // Step = 1 tick (100ns)
new DateTimeMonthFixedStepDomain();         // Step = 1 month
new DateTimeYearFixedStepDomain();          // Step = 1 year
```

</details>

<details>
<summary><strong>▶ DateOnly / TimeOnly Domains</strong> (.NET 6+, 7 domains - all O(1))</summary>

```csharp
using Intervals.NET.Domain.Default.DateTime;

// DateOnly
new DateOnlyDayFixedStepDomain();           // Step = 1 day

// TimeOnly (various granularities)
new TimeOnlyTickFixedStepDomain();          // Step = 1 tick (100ns)
new TimeOnlyMicrosecondFixedStepDomain();   // Step = 1 microsecond
new TimeOnlyMillisecondFixedStepDomain();   // Step = 1 millisecond
new TimeOnlySecondFixedStepDomain();        // Step = 1 second
new TimeOnlyMinuteFixedStepDomain();        // Step = 1 minute
new TimeOnlyHourFixedStepDomain();          // Step = 1 hour
```

</details>

<details>
<summary><strong>▶ TimeSpan Domains</strong> (7 domains - all O(1))</summary>

```csharp
using Intervals.NET.Domain.Default.TimeSpan;

new TimeSpanTickFixedStepDomain();          // Step = 1 tick (100ns)
new TimeSpanMicrosecondFixedStepDomain();   // Step = 1 microsecond
new TimeSpanMillisecondFixedStepDomain();   // Step = 1 millisecond
new TimeSpanSecondFixedStepDomain();        // Step = 1 second
new TimeSpanMinuteFixedStepDomain();        // Step = 1 minute
new TimeSpanHourFixedStepDomain();          // Step = 1 hour
new TimeSpanDayFixedStepDomain();           // Step = 1 day (24 hours)
```

</details>

<details>
<summary><strong>▶ Calendar / Business Day Domains</strong> (2 domains - O(N) ⚠️)</summary>

```csharp
using Intervals.NET.Domain.Default.Calendar;

// Standard Mon-Fri business week (no holidays)
new StandardDateTimeBusinessDaysVariableStepDomain();  // DateTime version
new StandardDateOnlyBusinessDaysVariableStepDomain();  // DateOnly version

// ⚠️ Variable-step: Operations iterate through days
// 💡 For custom calendars (holidays, different work weeks), implement IVariableStepDomain<T>
```

</details>

#### 🔧 Extension Methods: Connecting Domains to Ranges

**Extension methods bridge domains and ranges** - domains provide discrete point operations, extensions apply them to range boundaries.

<details>
<summary><strong>▶ Fixed-Step Extensions</strong> (O(1) - Guaranteed Constant Time)</summary>

```csharp
using Intervals.NET.Domain.Extensions.Fixed;

// All methods in this namespace are O(1) and work with IFixedStepDomain<T>

// Span: Count discrete domain steps within the range
var span = range.Span(domain);  // Returns RangeValue<long>
// How it works:
// 1. Floor/Ceiling align range boundaries to domain steps (respecting inclusivity)
// 2. domain.Distance(start, end) calculates steps between aligned boundaries (O(1))
// 3. Returns count of discrete points

// ExpandByRatio: Proportional expansion based on span
var expanded = range.ExpandByRatio(domain, leftRatio: 0.5, rightRatio: 0.5);
// How it works:
// 1. Calculate span (count of discrete points)
// 2. leftSteps = (long)(span * leftRatio), rightSteps = (long)(span * rightRatio)
// 3. domain.Add(start, -leftSteps) and domain.Add(end, rightSteps)
// 4. Returns new range with expanded boundaries

// Example with integers
var r = Range.Closed(10, 20);  // span = 11 discrete values
var e = r.ExpandByRatio(new IntegerFixedStepDomain(), 0.5, 0.5);  
// 11 * 0.5 = 5.5 → truncated to 5 steps
// [10 - 5, 20 + 5] = [5, 25]
```

**Why O(1)?** Fixed-step domains have uniform step sizes, so `Distance()` uses arithmetic: `(end - start) / stepSize`.

</details>

<details>
<summary><strong>▶ Variable-Step Extensions</strong> (O(N) - May Require Iteration ⚠️)</summary>

```csharp
using Intervals.NET.Domain.Extensions.Variable;

// ⚠️ Methods may be O(N) depending on domain implementation
// Work with IVariableStepDomain<T>

// Span: Count domain steps (may iterate through range)
var span = range.Span(domain);  // Returns RangeValue<double>
// How it works:
// 1. Floor/Ceiling align boundaries to domain steps
// 2. domain.Distance(start, end) may iterate each step to count (O(N))
// 3. Returns count (potentially fractional for partial steps)

// ExpandByRatio: Proportional expansion (calculates span first)
var expanded = range.ExpandByRatio(domain, leftRatio: 0.5, rightRatio: 0.5);
// How it works:
// 1. Calculate span (may be O(N))
// 2. leftSteps = (long)(span * leftRatio), rightSteps = (long)(span * rightRatio)
// 3. domain.Add() may iterate each step (O(N) per call)

// Example with business days
var week = Range.Closed(new DateTime(2026, 1, 20), new DateTime(2026, 1, 26));
var businessDayDomain = new StandardDateTimeBusinessDaysVariableStepDomain();
var businessDays = week.Span(businessDayDomain);  // 5.0 (iterates checking weekends)
```

**Why O(N)?** Variable-step domains have non-uniform steps (weekends, month lengths, holidays), requiring iteration to count.

</details>

<details>
<summary><strong>▶ Common Extensions</strong> (Work with Any Domain)</summary>

```csharp
using Intervals.NET.Domain.Extensions;

// These work with IRangeDomain<T> - both fixed and variable-step domains
// Don't calculate span, so performance depends only on domain's Add() method

// Shift: Move range by fixed step count (preserves span)
var shifted = range.Shift(domain, offset: 5);  // Move 5 steps forward
// How it works:
// newStart = domain.Add(start, offset)
// newEnd = domain.Add(end, offset)
// Returns new range with same inclusivity

// Expand: Expand/contract by fixed step amounts
var expanded = range.Expand(domain, left: 2, right: 3);  // Expand 2 left, 3 right
// How it works:
// newStart = domain.Add(start, -left)   // Negative = move backward
// newEnd = domain.Add(end, right)       // Positive = move forward
// Returns new range with adjusted boundaries

// Both preserve:
// - Inclusivity flags (IsStartInclusive, IsEndInclusive)
// - Infinity (infinity + offset = infinity)
```

**Performance:** Typically O(1) for most domains - just calls `Add()` twice. Variable-step domains may have O(N) `Add()` if they need to iterate.

</details>

#### 🎓 Real-World Scenarios

<details>
<summary><strong>▶ Scenario 1: Shift Maintenance Window</strong></summary>

```csharp
using Intervals.NET.Domain.Default.DateTime;
using Intervals.NET.Domain.Extensions;

// Original maintenance window: 2 AM - 4 AM
var window = Range.Closed(
    new DateTime(2025, 1, 28, 2, 0, 0),
    new DateTime(2025, 1, 28, 4, 0, 0)
);

var hourDomain = new DateTimeHourFixedStepDomain();

// Shift to next day (24 hours forward)
var nextDay = window.Shift(hourDomain, 24);

// Expand by 1 hour on each side: 1 AM - 5 AM
var extended = window.Expand(hourDomain, left: 1, right: 1);
```

</details>

<details>
<summary><strong>▶ Scenario 2: Project Sprint Planning</strong></summary>

```csharp
using Intervals.NET.Domain.Default.Calendar;
using Intervals.NET.Domain.Extensions.Variable;

var sprint = Range.Closed(
    new DateTime(2025, 1, 20),  // Sprint start (Monday)
    new DateTime(2025, 2, 2)    // Sprint end (Sunday)
);

var businessDayDomain = new StandardDateTimeBusinessDaysVariableStepDomain();

// Count working days in sprint
var workingDays = sprint.Span(businessDayDomain);  // 10.0 business days

// Add buffer: extend by 2 business days at end
var withBuffer = sprint.Expand(businessDayDomain, left: 0, right: 2);
```

</details>

<details>
<summary><strong>▶ Scenario 3: Sliding Window Analysis</strong></summary>

```csharp
using Intervals.NET.Domain.Default.Numeric;
using Intervals.NET.Domain.Extensions;

var domain = new IntegerFixedStepDomain();

// Start with window [0, 100]
var window = Range.Closed(0, 100);

// Slide window forward by 50 steps
var next = window.Shift(domain, 50);  // [50, 150]

// Expand window by 20% on each side
var wider = window.ExpandByRatio(domain, 0.2, 0.2);  // [-20, 120]
```

</details>

#### 🛠️ Creating Custom Domains

You can define your own fixed or variable-step domains by implementing the appropriate interface:

<details>
<summary><strong>▶ Custom Fixed-Step Domain Example</strong></summary>

```csharp
using Intervals.NET.Domain.Abstractions;

// Example: Temperature domain with 0.5°C steps
public class HalfDegreeCelsiusDomain : IFixedStepDomain<double>
{
    private const double StepSize = 0.5;
    
    public double Add(double value, long steps) => value + (steps * StepSize);
    
    public double Subtract(double value, long steps) => value - (steps * StepSize);
    
    public double Floor(double value) => Math.Floor(value / StepSize) * StepSize;
    
    public double Ceiling(double value) => Math.Ceiling(value / StepSize) * StepSize;
    
    // O(1) distance calculation - fixed step size
    public long Distance(double start, double end)
    {
        var alignedStart = Floor(start);
        var alignedEnd = Floor(end);
        return (long)Math.Round((alignedEnd - alignedStart) / StepSize);
    }
}

// Usage
var tempRange = Range.Closed(20.3, 22.7);
var domain = new HalfDegreeCelsiusDomain();
var steps = tempRange.Span(domain);  // Counts 0.5°C increments: 20.5, 21.0, 21.5, 22.0, 22.5
```

</details>

<details>
<summary><strong>▶ Custom Variable-Step Domain Example</strong></summary>

```csharp
using Intervals.NET.Domain.Abstractions;

// Example: Custom business calendar with holidays
public class CustomBusinessDayDomain : IVariableStepDomain<DateTime>
{
    private readonly HashSet<DateTime> _holidays;
    
    public CustomBusinessDayDomain(IEnumerable<DateTime> holidays)
    {
        _holidays = holidays.Select(d => d.Date).ToHashSet();
    }
    
    private bool IsBusinessDay(DateTime date)
    {
        var dayOfWeek = date.DayOfWeek;
        return dayOfWeek != DayOfWeek.Saturday 
            && dayOfWeek != DayOfWeek.Sunday
            && !_holidays.Contains(date.Date);
    }
    
    public DateTime Add(DateTime value, long steps)
    {
        // Iterate through days, counting only business days
        var current = value.Date;
        var direction = steps > 0 ? 1 : -1;
        var remaining = Math.Abs(steps);
        
        while (remaining > 0)
        {
            current = current.AddDays(direction);
            if (IsBusinessDay(current)) remaining--;
        }
        
        return current.Add(value.TimeOfDay);  // Preserve time component
    }
    
    public DateTime Subtract(DateTime value, long steps) => Add(value, -steps);
    
    public DateTime Floor(DateTime value) => value.Date;
    
    public DateTime Ceiling(DateTime value) => 
        value.TimeOfDay == TimeSpan.Zero ? value.Date : value.Date.AddDays(1);
    
    // O(N) distance - must check each day
    public double Distance(DateTime start, DateTime end)
    {
        var current = Floor(start);
        var endDate = Floor(end);
        double count = 0;
        
        while (current <= endDate)
        {
            if (IsBusinessDay(current)) count++;
            current = current.AddDays(1);
        }
        
        return count;
    }
}

// Usage
var holidays = new[] { new DateTime(2026, 1, 26) };  // Monday holiday
var customDomain = new CustomBusinessDayDomain(holidays);

var range = Range.Closed(
    new DateTime(2026, 1, 23),  // Friday
    new DateTime(2026, 1, 27)   // Tuesday
);

var businessDays = range.Span(customDomain);  // 2.0 (Fri 23, Tue 27 - skips weekend and holiday)
```

</details>

---

#### ⚠️ Important Notes

**Performance Awareness:**
- Fixed-step namespaces: Guaranteed O(1)
- Variable-step namespaces: May be O(N) - check domain docs
- Use appropriate domain for your data type

**Overflow Protection:**
- Month/Year/DateOnly domains validate offset ranges
- Throws `ArgumentOutOfRangeException` if offset exceeds int.MaxValue
- Prevents silent data corruption

**Truncation in ExpandByRatio:**
- Offset = `(long)(span * ratio)` - fractional parts truncated
- For variable-step domains with double spans, precision loss may occur
- Use `Expand()` directly if exact offsets needed

#### 🔗 Learn More

- [Domain Abstractions](src/Domain/Intervals.NET.Domain.Abstractions/) - Interfaces for custom domains
- [Default Implementations](src/Domain/Intervals.NET.Domain.Default/) - 36 ready-to-use domains
- [Extension Methods](src/Domain/Intervals.NET.Domain.Extensions/) - Span, Expand, Shift operations

---

<details>
<summary><strong>▶ Click to expand: Advanced Usage Examples</strong></summary>

> 📚 **Inside this section:**
> - Building Complex Conditions
> - Progressive Discount System
> - Range-Based Configuration
> - Safe Range Operations
> - Validation Helpers

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

## RangeData Library

### RangeData Overview

`RangeData<TRange, TData, TDomain>` is a lightweight, in-process, **lazy, domain-aware** data structure that combines ranges with associated data sequences. It allows **composable operations** like intersection, union, trimming, and projections while maintaining strict invariants.

| Feature / Library                                               | RangeData | Intervals.NET | System.Range | Rx        | Pandas    | C++20 Ranges | Kafka Streams / EventStore   |
|-----------------------------------------------------------------|-----------|---------------|--------------|-----------|-----------|--------------|------------------------------|
| **Lazy evaluation**                                             | ✅ Yes     | ✅ Partial     | ✅ Yes        | ✅ Yes     | ❌ No      | ✅ Yes        | ✅ Yes                        |
| **Domain-aware discrete ranges**                                | ✅ Yes     | ✅ Yes         | ❌ No         | ❌ No      | ❌ No      | ✅ Partial    | ✅ Partial                    |
| **Associated data (`IEnumerable`)**                             | ✅ Yes     | ❌ No          | ❌ No         | ✅ Yes     | ✅ Yes     | ❌ No         | ✅ Yes                        |
| **Strict invariant (range length = data length)**               | ✅ Yes     | ❌ No          | ❌ No         | ❌ No      | ❌ No      | ❌ No         | ❌ No                         |
| **Right-biased union / intersection**                           | ✅ Yes     | ❌ No          | ❌ No         | ❌ No      | ❌ No      | ❌ No         | ✅ Yes                        |
| **Lazy composition (skip/take/concat without materialization)** | ✅ Yes     | ❌ No          | ❌ No         | ✅ Yes     | ❌ No      | ✅ Yes        | ✅ Partial                    |
| **In-process, single-machine**                                  | ✅ Yes     | ✅ Yes         | ✅ Yes        | ✅ Yes     | ✅ Yes     | ✅ Yes        | ❌ No (distributed)           |
| **Distributed / persisted event streams**                       | ❌ No      | ❌ No          | ❌ No         | ❌ No      | ❌ No      | ❌ No         | ✅ Yes                        |
| **Composable slices / trimming / projections**                  | ✅ Yes     | ❌ No          | ❌ No         | ✅ Partial | ✅ Partial | ✅ Partial    | ✅ Partial                    |
| **Generic over any data / domain**                              | ✅ Yes     | ✅ Partial     | ❌ No         | ✅ Partial | ❌ No      | ✅ Partial    | ✅ Partial                    |
| **Use case: in-memory sliding window / cache / projections**    | ✅ Yes     | ❌ No          | ❌ No         | ✅ Partial | ✅ Partial | ✅ Partial    | ✅ Yes                        |

<details>
<summary>🛠️ Implementation Details & Notes</summary>

- **Lazy evaluation:** `RangeData` builds **iterator graphs** using `IEnumerable`. Data is only materialized when iterated. Operations like `Skip`, `Take`, `Concat` do **not allocate new arrays or lists**.
- **Domain-awareness:** Supports any discrete domain via `IRangeDomain<T>`. This allows flexible steps, custom metrics, and ensures consistent range arithmetic.
- **Expected invariant/contract:** The **range length should equal the data sequence length**. `RangeData` and `RangeDataExtensions` do **not** enforce this at runtime for performance reasons; callers are responsible for providing consistent inputs or can validate them (for example with `IsValid`) when safety is more important than allocation/CPU overhead.
- **Right-biased operations:** `Union` and `Intersect` always take **data from the right operand** in overlapping regions, ideal for cache updates or incremental data ingestion.
- **Composable slices:** Supports trimming (`TrimStart`, `TrimEnd`) and projections while keeping laziness intact. You can work with a `RangeData` without ever iterating the data.
- **Trade-offs:** Zero allocation is **not fully achievable** because `IEnumerable` is a reference type. Some intermediate enumerables may exist, but memory usage remains minimal.
- **Comparison to event streaming:** Conceptually similar to event sourcing projections or Kafka streams (right-biased, discrete offsets), but fully **in-process**, lightweight, and generic.
- **Ideal use cases:** Sliding window caches, time-series processing, projections of incremental datasets, or any scenario requiring **efficient, composable range-data operations**.

</details>


### Overview

`RangeData<TRange, TData, TDomain>` is an abstraction that couples:

- a **logical range** (`Range<TRange>`),
- a **data sequence** (`IEnumerable<TData>`),
- a **discrete domain** (`IRangeDomain<TRange>`) that defines steps and distances.

> **Key Invariant:** The length of the range (measured in domain steps) **must exactly match** the number of data elements. This ensures strict consistency between the range and its data.

This abstraction allows working with **large or dynamic sequences** without immediately materializing them, making all operations lazy and memory-efficient.

---

### Core Design Principles

- **Immutability:** All operations return new `RangeData` instances; originals remain unchanged.
- **Lazy evaluation:** LINQ operators and iterators are used; data is processed only on enumeration.
- **Domain-agnostic:** Supports any `IRangeDomain<T>` implementation.
- **Right-biased operations:** On intersection or union, data from the *right* (fresh/new) range takes priority.
- **Minimal allocations:** No unnecessary arrays or lists; only `IEnumerable` iterators are created.

<details>
<summary>Extension Methods Details</summary>

#### Intersection (`Intersect`)
- Returns the intersection of two `RangeData` objects.
- Data is **sourced from the right range** (fresh data).
- Returns `null` if there is no overlap.
- Lazy, O(n) for skip/take on the data sequence.

#### Union (`Union`)
- Combines two ranges if they are **overlapping or adjacent**.
- In overlapping regions, **right range data takes priority**.
- Returns `null` if ranges are completely disjoint.
- Handles three cases:
  1. Left fully contained in right → only right data used.
  2. Partial overlap → left non-overlapping portion + right data.
  3. Left wraps around right → left non-overlapping left + right + left non-overlapping right.

#### TrimStart / TrimEnd
- Trim the range from the start or end.
- Returns new `RangeData` with sliced data.
- Returns `null` if the trim removes the entire range.

#### Containment & Adjacency Checks
- `Contains(value)` / `Contains(range)` check range membership.
- `IsTouching`, `IsBeforeAndAdjacentTo`, `IsAfterAndAdjacentTo` verify **overlap or adjacency**.
- Useful for merging sequences or building ordered chains.

</details>

<details>
<summary>Trade-offs & Limitations</summary>

- `IEnumerable` does not automatically validate the invariant — users are responsible for ensuring data length matches range length.
- Lazy operations only incur complexity O(n) **when iterating**.
- Not fully zero-allocation: iterators themselves are allocated, but overhead is minimal.
- Lazy iterators enable **Sliding Window Cache** scenarios: data can expire without being enumerated.

</details>

<details>
<summary>Use Cases & Examples</summary>

- **Time-series processing:** merging and slicing measurements over time ranges.
- **Event-sourcing projections:** managing streams of events with metadata.
- **Sliding Window Cache:** lazy access to partially loaded sequences.
- **Incremental datasets:** combining fresh updates with historical data.

```csharp
var domain = new IntegerFixedStepDomain();
var oldData = new RangeData(Range.Closed(10, 20), oldValues, domain);
var newData = new RangeData(Range.Closed(18, 30), newValues, domain);

// Right-biased union
var union = oldData.Union(newData); // Range [10, 30], overlapping [18,20] comes from newData
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
<summary><strong>▶ Click to expand: Detailed Benchmark Results</strong></summary>

> 📊 **Inside this section:**
> - About These Benchmarks
> - Parsing Performance
> - Construction Performance
> - Containment Checks (Hot Path)
> - Set Operations Performance
> - Real-World Scenarios
> - Performance Summary
> - Understanding "Naive" Baseline

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

<div align="center">

```
🚀 Parsing:      3.6× faster with interpolated strings
💎 Construction: 0 bytes allocated (struct-based)
⚡ Containment:   1.7× faster for hot path validation
🎯 Set Ops:       0 bytes allocated (100% reduction)
🔥 Real-World:    1.7× faster for sliding windows
```

</div>

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
<summary><strong>▶ Click to expand: Do's and Don'ts</strong></summary>

> ✅ **Inside this section:**
> - Recommended patterns and best practices
> - Common pitfalls to avoid
> - Safe usage examples

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

<div align="center">

| Aspect         | Intervals.NET             | Manual Implementation   |
|----------------|---------------------------|-------------------------|
| Type Safety    | ✅ Generic constraints     | ⚠️ Must implement       |
| Edge Cases     | ✅ All handled (100% test) | ❌ Often forgotten      |
| Infinity       | ✅ Built-in, explicit      | ❌ Nullable or custom   |
| Parsing        | ✅ Span + interpolated     | ❌ Must implement       |
| Set Operations | ✅ Rich API (6+ methods)   | ❌ Must implement       |
| Allocations    | ✅ Zero (struct-based)     | ⚠️ Usually class-based  |
| Testing        | ✅ 100% coverage           | ⚠️ Your responsibility  |

</div>

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

<div align="center">

**Built with modern C# for the .NET community**

</div>