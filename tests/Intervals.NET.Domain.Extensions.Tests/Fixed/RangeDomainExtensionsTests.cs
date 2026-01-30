using Intervals.NET.Domain.Default.DateTime;
using Intervals.NET.Domain.Default.Numeric;
using Intervals.NET.Domain.Extensions.Fixed;
using RangeFactory = Intervals.NET.Factories.Range;

namespace Intervals.NET.Domain.Extensions.Tests.Fixed;

/// <summary>
/// Tests for Fixed-step domain extension methods (O(1) operations).
/// Tests the extension methods in Intervals.NET.Domain.Extensions.Fixed.RangeDomainExtensions.
/// </summary>
public class RangeDomainExtensionsTests
{
    #region Span Tests - IntegerFixedStepDomain

    [Fact]
    public void Span_IntegerClosedRange_ReturnsCorrectDistance()
    {
        // Arrange
        var range = RangeFactory.Closed<int>(10, 20);
        var domain = new IntegerFixedStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        // [10, 20] includes values 10, 11, 12, ..., 20 = 11 total values
        Assert.Equal(11, span.Value);
    }

    [Fact]
    public void Span_IntegerOpenRange_ReturnsCorrectDistance()
    {
        // Arrange
        var range = RangeFactory.Open<int>(10, 20);
        var domain = new IntegerFixedStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        // (10, 20) includes values 11, 12, ..., 19 = 9 total values
        Assert.Equal(9, span.Value);
    }

    [Fact]
    public void Span_IntegerClosedOpenRange_ReturnsCorrectDistance()
    {
        // Arrange
        var range = RangeFactory.ClosedOpen<int>(10, 20);
        var domain = new IntegerFixedStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        // [10, 20) includes 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 = 10 values
        Assert.Equal(10, span.Value);
    }

    [Fact]
    public void Span_IntegerUnboundedEnd_ReturnsPositiveInfinity()
    {
        // Arrange
        var range = RangeFactory.Closed(10, RangeValue<int>.PositiveInfinity);
        var domain = new IntegerFixedStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.False(span.IsFinite);
        Assert.True(span.IsPositiveInfinity);
    }

    #endregion

    #region Span Tests - DateTimeDayFixedStepDomain

    [Fact]
    public void Span_DateTimeDayAlignedBoundaries_ReturnsCorrectDistance()
    {
        // Arrange
        var start = new DateTime(2024, 1, 1, 0, 0, 0);
        var end = new DateTime(2024, 1, 5, 0, 0, 0);
        var range = RangeFactory.Closed<DateTime>(start, end);
        var domain = new DateTimeDayFixedStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        // [2024-01-01, 2024-01-05] includes days: Jan 1, 2, 3, 4, 5 = 5 days
        Assert.Equal(5, span.Value);
    }

    [Fact]
    public void Span_DateTimeDaySingleDayMisaligned_ReturnsZero()
    {
        // Arrange
        var start = new DateTime(2024, 1, 1, 10, 0, 0);
        var end = new DateTime(2024, 1, 1, 15, 0, 0);
        var range = RangeFactory.Closed<DateTime>(start, end);
        var domain = new DateTimeDayFixedStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        Assert.Equal(0, span.Value);
    }

    #endregion

    #region Span Tests - DateTimeMonthFixedStepDomain

    [Fact]
    public void Span_DateTimeMonthAlignedBoundaries_ReturnsCorrectDistance()
    {
        // Arrange
        var start = new DateTime(2024, 1, 1);
        var end = new DateTime(2024, 3, 1);
        var range = RangeFactory.Closed<DateTime>(start, end);
        var domain = new DateTimeMonthFixedStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        // [2024-01-01, 2024-03-01] includes months: Jan, Feb, Mar = 3 months
        Assert.Equal(3, span.Value);
    }

    #endregion

    #region ExpandByRatio Tests

    [Fact]
    public void ExpandByRatio_IntegerRange_ExpandsCorrectly()
    {
        // Arrange
        var range = RangeFactory.Closed<int>(10, 20); // span = 11
        var domain = new IntegerFixedStepDomain();

        // Act
        var expanded = range.ExpandByRatio(domain, leftRatio: 0.5, rightRatio: 0.5);

        // Assert
        Assert.Equal(5, expanded.Start.Value);  // 10 - (11 * 0.5) = 10 - 5 = 5 (rounded)
        Assert.Equal(25, expanded.End.Value);   // 20 + (11 * 0.5) = 20 + 5 = 25 (rounded)
    }

    [Fact]
    public void ExpandByRatio_InfiniteRange_ThrowsArgumentException()
    {
        // Arrange
        var range = RangeFactory.Closed(10, RangeValue<int>.PositiveInfinity);
        var domain = new IntegerFixedStepDomain();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            range.ExpandByRatio(domain, leftRatio: 0.5, rightRatio: 0.5));

        Assert.Contains("infinite", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Span_WithNegativeInfinityStart_ReturnsPositiveInfinity()
    {
        // Arrange
        var domain = new IntegerFixedStepDomain();
        var range = RangeFactory.Closed(RangeValue<int>.NegativeInfinity, 100);

        // Act
        var result = range.Span(domain);

        // Assert
        Assert.True(result.IsPositiveInfinity);
    }

    [Fact]
    public void Span_WithPositiveInfinityEnd_ReturnsPositiveInfinity()
    {
        // Arrange
        var domain = new IntegerFixedStepDomain();
        var range = RangeFactory.Closed(0, RangeValue<int>.PositiveInfinity);

        // Act
        var result = range.Span(domain);

        // Assert
        Assert.True(result.IsPositiveInfinity);
    }

    [Fact]
    public void Span_SingleStepRange_BothBoundariesOnStep_BothInclusive_ReturnsOne()
    {
        // Arrange - both boundaries exactly on integer steps
        var range = RangeFactory.Closed<int>(10, 10);
        var domain = new IntegerFixedStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        Assert.Equal(1, span.Value);
    }

    [Fact]
    public void Span_SingleStepRange_BothBoundariesBetweenSteps_ReturnsZero()
    {
        // Arrange - DateTime values in the middle of a day (not aligned to day boundaries)
        var start = new DateTime(2024, 1, 1, 10, 0, 0);
        var end = new DateTime(2024, 1, 1, 15, 0, 0);
        var range = RangeFactory.Open<DateTime>(start, end);
        var domain = new DateTimeDayFixedStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        // Both times are within the same day, and boundaries are exclusive
        Assert.Equal(0, span.Value);
    }

    [Fact]
    public void Span_SingleStepRange_StartOnBoundary_EndExclusive_ReturnsOne()
    {
        // Arrange - start is aligned, end is not, but both floor to same step
        var start = new DateTime(2024, 1, 1, 0, 0, 0); // Midnight (on boundary)
        var end = new DateTime(2024, 1, 1, 12, 0, 0);  // Noon (not on boundary)
        var range = RangeFactory.Closed<DateTime>(start, end);
        var domain = new DateTimeDayFixedStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        Assert.Equal(1, span.Value);
    }

    [Fact]
    public void Span_EmptyRange_ExclusiveBoundariesSameValue_ReturnsZero()
    {
        // Arrange - exclusive boundaries on the same integer
        var range = RangeFactory.Open<int>(10, 11);
        var domain = new IntegerFixedStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        // (10, 11) excludes both 10 and 11, no integers in between
        Assert.Equal(0, span.Value);
    }

    [Fact]
    public void Span_InvertedRange_StartGreaterThanEnd_ReturnsZero()
    {
        // Arrange - test a range that becomes empty after floor adjustments
        var start2 = new DateTime(2024, 1, 1, 23, 0, 0);  // Jan 1, 11 PM
        var end2 = new DateTime(2024, 1, 2, 1, 0, 0);     // Jan 2, 1 AM
        var range = RangeFactory.Open<DateTime>(start2, end2);
        var domain = new DateTimeDayFixedStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        // After flooring: start -> Jan 1, end -> Jan 2
        // With both exclusive: firstStep = Jan 2, lastStep = Jan 1
        // firstStep > lastStep, so result should be 0
        Assert.Equal(0, span.Value);
    }

    [Fact]
    public void Span_StartExclusiveNotOnBoundary_SkipsToNextStep()
    {
        // Arrange - start is not on a day boundary, exclusive
        var start = new DateTime(2024, 1, 1, 12, 0, 0);  // Noon
        var end = new DateTime(2024, 1, 3, 0, 0, 0);     // Midnight
        var range = RangeFactory.Create<DateTime>(start, end, isStartInclusive: false, isEndInclusive: true);
        var domain = new DateTimeDayFixedStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        // Start exclusive from Jan 1 noon -> includes Jan 2, Jan 3
        Assert.Equal(2, span.Value);
    }

    [Fact]
    public void Span_EndExclusiveOnBoundary_ExcludesThatStep()
    {
        // Arrange
        var start = new DateTime(2024, 1, 1, 0, 0, 0);
        var end = new DateTime(2024, 1, 3, 0, 0, 0);
        var range = RangeFactory.ClosedOpen<DateTime>(start, end);
        var domain = new DateTimeDayFixedStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        // [Jan 1, Jan 3) includes Jan 1 and Jan 2, but not Jan 3
        Assert.Equal(2, span.Value);
    }

    #endregion
}
