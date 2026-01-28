# Span Method Validation - Why `start > end` Check is NOT Redundant

## Question
Should we simplify the Span methods by removing the `start > end` validation since ranges can't be created with such values?

## Answer: NO - The validation is NOT redundant

### Why the Check is Necessary

The `start > end` check in Span methods is checking **domain-aligned boundaries**, not the original range boundaries. After Floor/Ceiling operations, the aligned boundaries can cross even when the original range was valid.

### Example Scenario

Consider an **open integer range** that's smaller than one step:

```csharp
var range = Range.Open(10.2, 10.8);  // Valid range: 10.2 < 10.8
var domain = new IntegerFixedStepDomain();
var span = range.Span(domain);
```

**What happens:**
1. **Original range**: `(10.2, 10.8)` - Valid! Start < End
2. **Domain alignment**:
   - `firstStep = Floor(10.2) + 1 = 10 + 1 = 11` (exclusive start, so skip to next step)
   - `lastStep = Floor(10.8) - 1 = 10 - 1 = 9` (exclusive end, so back up one step)
3. **After alignment**: `firstStep (11) > lastStep (9)` ❌
4. **Result**: Return `0` (no complete integer steps in the range)

### Code Location

Both Fixed and Variable Span methods have this check:

```csharp
// After domain alignment, boundaries can cross (e.g., open range smaller than one step)
// Example: (Jan 1 00:00, Jan 1 00:01) with day domain -> firstStep=Jan 2, lastStep=Dec 31
if (firstStep.CompareTo(lastStep) > 0)
{
    return 0;  // or 0.0 for variable domains
}
```

### More Examples

**DateTime with Day Domain:**
```csharp
// Range smaller than a day
var range = Range.Open(
    new DateTime(2024, 1, 1, 10, 0, 0),
    new DateTime(2024, 1, 1, 15, 0, 0)
);
var domain = new DateTimeDayFixedStepDomain();

// firstStep: Jan 2 (floor Jan 1 10:00 → Jan 1, then +1 day)
// lastStep: Dec 31 prev year (floor Jan 1 15:00 → Jan 1, then -1 day)
// firstStep > lastStep → return 0
```

**Integer Range:**
```csharp
var range = Range.Create(5, 5, false, false);  // (5, 5) - empty range
// This already throws in constructor: "When start equals end, at least one bound must be inclusive"

var range = Range.Open(5, 6);  // (5, 6) - valid but contains no integers
var domain = new IntegerFixedStepDomain();

// firstStep: 6 (floor 5 → 5, then +1)
// lastStep: 5 (floor 6 → 6, then -1)
// firstStep > lastStep → return 0
```

## Conclusion

**The validation is essential** because:
1. ✅ Range constructor validates: `original start <= original end`
2. ✅ Span method validates: `aligned firstStep <= aligned lastStep`

These are **two different validations** for **two different concepts**. Removing the Span validation would cause incorrect results for ranges smaller than one domain step.

## Test Coverage

The following tests verify this behavior:
- `Span_SingleStepRange_BothBoundariesBetweenSteps_ReturnsZero`
- `Span_InvertedRange_StartGreaterThanEnd_ReturnsZero`
- `Span_DateTimeDaySingleDayMisaligned_ReturnsZero`
