# Intervals.NET

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Intervals.NET.svg)](https://www.nuget.org/packages/Intervals.NET/)

A high-performance, zero-allocation .NET library for working with mathematical intervals and ranges. Built with modern C# features including structs, spans, and records for optimal performance and type safety.

## 🎯 Purpose

**Intervals.NET** provides a robust, type-safe way to represent and manipulate intervals (ranges) of comparable values. Whether you're working with numeric ranges, date intervals, or any comparable type, this library offers:

- **Type Safety**: Generic constraints ensure you only work with comparable types
- **Zero Allocation**: Struct-based design for optimal performance
- **Infinite Ranges**: First-class support for unbounded intervals
- **Rich API**: Comprehensive set of operations (intersection, union, contains, overlaps, etc.)
- **Flexible Parsing**: Parse from strings with `ReadOnlySpan<char>` or interpolated strings for zero-allocation performance
- **Extensibility**: Extension methods for custom range operations

## 📦 Installation

```bash
dotnet add package Intervals.NET
```

## 🚀 Quick Start

```csharp
using Intervals.NET.Factories;

// Create a closed range [10, 20]
var range = Range.Closed(10, 20);

// Check if value is in range
bool contains = range.Contains(15); // true

// Create ranges with different inclusivity
var openRange = Range.Open(0, 100);        // (0, 100)
var halfOpen = Range.ClosedOpen(1, 10);    // [1, 10)
var halfClosed = Range.OpenClosed(1, 10);  // (1, 10]

// Unbounded ranges
var greaterThan = Range.ClosedOpen(0, RangeValue<int>.PositiveInfinity);  // [0, ∞)
var lessThan = Range.Open(RangeValue<int>.NegativeInfinity, 100);         // (-∞, 100)
var unbounded = Range.Open(RangeValue<int>.NegativeInfinity, 
                           RangeValue<int>.PositiveInfinity);              // (-∞, ∞)
```

## 🔑 Core Concepts

### Range Notation

Intervals.NET uses standard mathematical interval notation:

- `[a, b]` - **Closed**: includes both `a` and `b`
- `(a, b)` - **Open**: excludes both `a` and `b`
- `[a, b)` - **Half-open**: includes `a`, excludes `b`
- `(a, b]` - **Half-closed**: excludes `a`, includes `b`

### Infinity Support

Represent unbounded ranges naturally:

```csharp
// Values greater than or equal to 18
var adults = Range.Closed(18, RangeValue<int>.PositiveInfinity);

// All dates before 2024
var historical = Range.Open(
    RangeValue<DateTime>.NegativeInfinity, 
    new DateTime(2024, 1, 1)
);

// Parse infinity symbols
var range = Range.FromString<int>("[-∞, 100]");  // or "[, 100]"
```

## 💡 Usage Examples

### Basic Operations

```csharp
using Intervals.NET.Factories;

var range1 = Range.Closed(10, 30);
var range2 = Range.Closed(20, 40);

// Check overlap
if (range1.Overlaps(range2))
{
    Console.WriteLine("Ranges overlap!");
}

// Get intersection
var intersection = range1.Intersect(range2);
// Result: [20, 30]

// Get union (if ranges overlap or are adjacent)
var union = range1.Union(range2);
// Result: [10, 40]

// Check containment
bool contains = range1.Contains(Range.Closed(15, 25)); // true
bool containsValue = range1.Contains(25);               // true
```

### Set Operations

```csharp
var range = Range.Closed(10, 50);
var exclude = Range.Closed(20, 30);

// Subtract a range
var remaining = range.Except(exclude).ToList();
// Result: [10, 20) and (30, 50]

// Check if ranges are adjacent
bool adjacent = Range.ClosedOpen(10, 20).IsAdjacent(Range.Closed(20, 30));
// Result: true

// Check ordering
bool before = Range.Closed(10, 20).IsBefore(Range.Closed(30, 40));
// Result: true
```

### Range Properties

```csharp
var finiteRange = Range.Closed(1, 10);
var infiniteRange = Range.Open(
    RangeValue<int>.NegativeInfinity,
    RangeValue<int>.PositiveInfinity
);

// Check if range is bounded
bool isBounded = finiteRange.IsBounded();      // true
bool isUnbounded = infiniteRange.IsUnbounded(); // true

// Check if completely infinite
bool isInfinite = infiniteRange.IsInfinite();   // true

// Check if empty (always false - empty ranges can't be constructed)
bool isEmpty = finiteRange.IsEmpty();           // false
```

### Parsing Ranges from Strings

```csharp
using Intervals.NET.Parsers;

// Parse various formats
var range1 = Range.FromString<int>("[10, 20]");
var range2 = Range.FromString<int>("(0, 100)");
var range3 = Range.FromString<double>("[1.5, 9.5)");

// Parse with infinity
var range4 = Range.FromString<int>("[, 100]");        // (-∞, 100]
var range5 = Range.FromString<int>("[0, ]");          // [0, ∞)
var range6 = Range.FromString<int>("(-∞, ∞)");       // (-∞, ∞)

// Parse with custom culture
var culture = new System.Globalization.CultureInfo("de-DE");
var range7 = Range.FromString<double>("[1,5; 9,5]", culture);

// TryParse for safe parsing
if (RangeParser.TryParse<int>("[10, 20)", out var range))
{
    Console.WriteLine($"Parsed: {range}");
}
```

### Zero-Allocation Interpolated String Parsing

For maximum performance when building ranges from runtime values, `Range.FromString<T>()` supports **interpolated strings with zero allocations**:

```csharp
// Traditional: allocates string first
string str = $"[{start}, {end}]";  // Allocation
var range1 = Range.FromString<int>(str);

// Optimized: values parsed directly, no string allocation
int start = 10, end = 20;
var range2 = Range.FromString<int>($"[{start}, {end}]");  // Zero allocations! ⚡

// Works with expressions, different types, and infinity
var computed = Range.FromString<int>($"[{start * 2}, {end * 3})");
var dateRange = Range.FromString<DateTime>($"[{DateTime.Today}, {DateTime.Today.AddDays(7)})");
var unbounded = Range.FromString<int>($"[{RangeValue<int>.NegativeInfinity}, {100}]");
```

**When to use:** Building ranges from variables/expressions where performance matters.  
**How it works:** Custom `InterpolatedStringHandler` processes values directly without materializing the string.

### Working with Different Types

```csharp
// Numeric ranges
var intRange = Range.Closed(1, 100);
var doubleRange = Range.Open(0.0, 1.0);
var decimalRange = Range.Closed(0m, 100m);

// Date and time ranges
var dateRange = Range.ClosedOpen(
    new DateTime(2024, 1, 1),
    new DateTime(2025, 1, 1)
);

var timeRange = Range.Closed(
    TimeSpan.FromHours(9),
    TimeSpan.FromHours(17)
);

// String ranges (lexicographic ordering)
var letterRange = Range.Closed("A", "Z");

// Custom comparable types
public record Temperature(double Celsius) : IComparable<Temperature>
{
    public int CompareTo(Temperature? other) =>
        Celsius.CompareTo(other?.Celsius ?? double.NegativeInfinity);
}

var tempRange = Range.Closed(
    new Temperature(-10),
    new Temperature(30)
);
```

### Advanced Patterns

#### Building Complex Conditions

```csharp
// Age categories
var children = Range.ClosedOpen(0, 13);
var teenagers = Range.ClosedOpen(13, 18);
var adults = Range.Closed(18, RangeValue<int>.PositiveInfinity);

int age = 25;
if (adults.Contains(age))
{
    Console.WriteLine("Adult");
}
```

#### Date Range Operations

```csharp
// Business hours
var businessHours = Range.Closed(
    TimeSpan.FromHours(9),
    TimeSpan.FromHours(17)
);

// Check if time falls within business hours
bool isDuringBusinessHours = businessHours.Contains(DateTime.Now.TimeOfDay);

// Year-to-date range
var ytd = Range.Closed(
    new DateTime(DateTime.Now.Year, 1, 1),
    DateTime.Now
);
```

#### Validation Scenarios

```csharp
// Validate configuration values
var validPort = Range.Closed(1, 65535);
var validPercentage = Range.Closed(0.0, 100.0);
var validAge = Range.Closed(0, 150);

public void ValidatePort(int port)
{
    if (!validPort.Contains(port))
        throw new ArgumentOutOfRangeException(nameof(port), 
            $"Port must be in range {validPort}");
}
```

#### Price Ranges and Discounts

```csharp
var budgetRange = Range.Closed(0m, 50m);
var midRange = Range.Open(50m, 200m);
var premiumRange = Range.Closed(200m, RangeValue<decimal>.PositiveInfinity);

decimal price = 149.99m;

if (budgetRange.Contains(price))
    ApplyDiscount(0.05m);
else if (midRange.Contains(price))
    ApplyDiscount(0.10m);
else if (premiumRange.Contains(price))
    ApplyDiscount(0.15m);
```

## 🎯 Extension Methods

Intervals.NET provides rich extension methods for common operations:

```csharp
using Intervals.NET.Extensions;

var range1 = Range.Closed(10, 30);
var range2 = Range.Closed(20, 40);

// Overlap detection
bool overlaps = range1.Overlaps(range2);

// Containment
bool contains = range1.Contains(range2);
bool containsValue = range1.Contains(25);

// Set operations
var intersection = range1.Intersect(range2);  // Returns Range<T>?
var union = range1.Union(range2);             // Returns Range<T>?
var except = range1.Except(range2);           // Returns IEnumerable<Range<T>>

// Adjacency and ordering
bool adjacent = range1.IsAdjacent(range2);
bool before = range1.IsBefore(range2);
bool after = range1.IsAfter(range2);

// Range properties
bool bounded = range1.IsBounded();
bool unbounded = range1.IsUnbounded();
bool infinite = range1.IsInfinite();
bool empty = range1.IsEmpty();
```

## 🔧 Operators

Ranges support intuitive operators:

```csharp
var range1 = Range.Closed(10, 30);
var range2 = Range.Closed(20, 40);

// Intersection operator
var intersection = range1 & range2;  // [20, 30]

// Union operator
var union = range1 | range2;         // [10, 40]
```

## ⚡ Performance

Intervals.NET is designed for maximum performance:

- **Zero Allocation**: All types are structs - no heap allocations
- **Stack-based**: Ranges live on the stack
- **Inline-friendly**: Small methods optimized for inlining
- **No Boxing**: Generic constraints avoid boxing
- **Span-based Parsing**: Uses `ReadOnlySpan<char>` for efficient string parsing
- **Interpolated String Support**: Zero-allocation parsing from interpolated strings when using variables

### Performance Characteristics

```csharp
// O(1) - Constant time operations
range.Contains(value)
range.IsEmpty()
range.IsBounded()
range.ToString()

// O(1) - Intersection and union when ranges overlap
range1.Intersect(range2)
range1.Union(range2)

// O(1) - Comparison operations
range1.Overlaps(range2)
range1.IsBefore(range2)
range1.Contains(range2)

// O(n) where n ≤ 2 - Except operation
range1.Except(range2)  // Returns 0, 1, or 2 ranges
```

## 🆚 Why Use Intervals.NET?

### vs. Manual Range Implementation

| Aspect | Intervals.NET | Manual Implementation |
|--------|---------------|----------------------|
| Type Safety | ✅ Generic constraints | ⚠️ Must implement |
| Edge Cases | ✅ All handled | ❌ Often forgotten |
| Infinity | ✅ Built-in | ❌ Manual handling |
| String Parsing | ✅ Included | ❌ Must implement |
| Set Operations | ✅ Rich API | ❌ Must implement |
| Testing | ✅ Thoroughly tested | ⚠️ Your responsibility |

## 💎 Best Practices

### ✅ Do's

```csharp
// DO: Use appropriate inclusivity for your domain
var age = Range.ClosedOpen(0, 18);  // 0 ≤ age < 18

// DO: Use infinity for unbounded ranges
var positive = Range.Open(0, RangeValue<int>.PositiveInfinity);

// DO: Check for null when operations may not produce a result
var intersection = range1.Intersect(range2);
if (intersection.HasValue)
{
    ProcessRange(intersection.Value);
}

// DO: Use TryParse for user input
if (RangeParser.TryParse<int>(userInput, out var range))
{
    // Use range
}

// DO: Use factory methods for clarity
var range = Range.Closed(1, 10);  // Clear intent
```

### ❌ Don'ts

```csharp
// DON'T: Try to create invalid ranges (will throw)
// var invalid = Range.Closed(20, 10);  // ArgumentException

// DON'T: Assume union always succeeds
var union = range1.Union(range2);
// Check union.HasValue before using!

// DON'T: Ignore culture when parsing
// var range = Range.FromString<double>("[1,5, 9,5]");  
// Use appropriate CultureInfo for comma as decimal separator

// DON'T: Box ranges unnecessarily
// IComparable boxed = range;  // Avoid boxing
```

### 🎯 Common Patterns

#### Safe Range Operations

```csharp
public Range<T>? SafeIntersect<T>(Range<T> r1, Range<T> r2) 
    where T : IComparable<T>
{
    return r1.Overlaps(r2) ? r1.Intersect(r2) : null;
}
```

#### Range Validation

```csharp
public static class ValidationRanges
{
    public static readonly Range<int> ValidPort = Range.Closed(1, 65535);
    public static readonly Range<int> ValidPercentage = Range.Closed(0, 100);
    public static readonly Range<double> ValidLatitude = Range.Closed(-90.0, 90.0);
    public static readonly Range<double> ValidLongitude = Range.Closed(-180.0, 180.0);
}

public void Validate(int port)
{
    if (!ValidationRanges.ValidPort.Contains(port))
        throw new ArgumentOutOfRangeException(nameof(port));
}
```

#### Range-Based Configuration

```csharp
public class ServiceConfiguration
{
    public Range<int> AllowedPortRange { get; init; } = Range.Closed(8000, 9000);
    public Range<TimeSpan> MaintenanceWindow { get; init; } = Range.Closed(
        TimeSpan.FromHours(2),
        TimeSpan.FromHours(4)
    );
    
    public bool IsMaintenanceTime(DateTime now)
    {
        return MaintenanceWindow.Contains(now.TimeOfDay);
    }
}
```

## 📚 API Reference

### Factory Methods

```csharp
// Create ranges with different inclusivity
Range.Closed<T>(start, end)      // [start, end]
Range.Open<T>(start, end)        // (start, end)
Range.ClosedOpen<T>(start, end)  // [start, end)
Range.OpenClosed<T>(start, end)  // (start, end]

// Parse from string
Range.FromString<T>(string input, IFormatProvider? provider = null)
```

### Extension Methods

```csharp
// Overlap and containment
range.Overlaps(other)
range.Contains(value)
range.Contains(other)

// Set operations
range.Intersect(other)  // Returns Range<T>?
range.Union(other)      // Returns Range<T>?
range.Except(other)     // Returns IEnumerable<Range<T>>

// Adjacency and ordering
range.IsAdjacent(other)
range.IsBefore(other)
range.IsAfter(other)

// Properties
range.IsBounded()
range.IsUnbounded()
range.IsInfinite()
range.IsEmpty()
```

### Range Properties

```csharp
range.Start            // RangeValue<T>
range.End              // RangeValue<T>
range.IsStartInclusive // bool
range.IsEndInclusive   // bool
```

### RangeValue Properties

```csharp
value.IsFinite
value.IsPositiveInfinity
value.IsNegativeInfinity
value.Value           // T (throws for infinity)
value.TryGetValue(out T val)  // Safe extraction
```

## 🧪 Testing

The library includes comprehensive unit tests that serve as interactive documentation:

```csharp
// See RangeTests.cs for Range<T> examples
// See RangeValueTests.cs for RangeValue<T> examples
// See RangeExtensionsTests.cs for extension method examples
// See RangeParserTests.cs for parsing examples
// See RangeFactoryTests.cs for factory method examples
```

## 🤝 Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

### Development Requirements

- .NET 8.0 SDK
- Any IDE supporting C# (Visual Studio, Rider, VS Code)

### Building

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built with modern C# and .NET 8.0 features
- Designed to solve real-world interval manipulation needs in .NET applications

## 📖 Further Reading

- [Mathematical Interval Notation](https://en.wikipedia.org/wiki/Interval_(mathematics))
- [Range Operations in Mathematics](https://en.wikipedia.org/wiki/Set_operations_(mathematics))
- [.NET Generic Constraints](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/constraints-on-type-parameters)

---

**Made with ❤️ for the .NET community**