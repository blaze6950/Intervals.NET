using Range = Intervals.NET.Factories.Range;

namespace Intervals.NET.Tests;

public class RangeFactoryTests
{
    #region Closed Method Tests

    [Fact]
    public void Closed_WithFiniteValues_CreatesClosedRange()
    {
        // Arrange
        RangeValue<int> start = 10;
        RangeValue<int> end = 20;

        // Act
        var range = Range.Closed(start, end);

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void Closed_WithImplicitConversion_WorksCorrectly()
    {
        // Arrange & Act
        var range = Range.Closed<int>(5, 15);

        // Assert
        Assert.Equal(5, range.Start.Value);
        Assert.Equal(15, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void Closed_WithNegativeInfinityStart_CreatesUnboundedStartRange()
    {
        // Arrange & Act
        var range = Range.Closed(RangeValue<int>.NegativeInfinity, 100);

        // Assert
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.Equal(100, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void Closed_WithPositiveInfinityEnd_CreatesUnboundedEndRange()
    {
        // Arrange & Act
        var range = Range.Closed(0, RangeValue<int>.PositiveInfinity);

        // Assert
        Assert.Equal(0, range.Start.Value);
        Assert.True(range.End.IsPositiveInfinity);
        Assert.True(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void Closed_WithBothInfinities_CreatesFullyUnboundedRange()
    {
        // Arrange & Act
        var range = Range.Closed(RangeValue<int>.NegativeInfinity, RangeValue<int>.PositiveInfinity);

        // Assert
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.True(range.End.IsPositiveInfinity);
        Assert.True(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void Closed_WithEqualValues_CreatesSinglePointRange()
    {
        // Arrange & Act
        var range = Range.Closed<int>(42, 42);

        // Assert
        Assert.Equal(42, range.Start.Value);
        Assert.Equal(42, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void Closed_ToString_ReturnsCorrectFormat()
    {
        // Arrange & Act
        var range = Range.Closed<int>(10, 20);
        var result = range.ToString();

        // Assert
        Assert.Equal("[10, 20]", result);
    }

    [Fact]
    public void Closed_WithStringType_WorksCorrectly()
    {
        // Arrange & Act
        var range = Range.Closed<string>("alpha", "omega");

        // Assert
        Assert.Equal("alpha", range.Start.Value);
        Assert.Equal("omega", range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void Closed_WithDoubleType_WorksCorrectly()
    {
        // Arrange & Act
        var range = Range.Closed<double>(1.5, 9.5);

        // Assert
        Assert.Equal(1.5, range.Start.Value);
        Assert.Equal(9.5, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    #endregion

    #region Open Method Tests

    [Fact]
    public void Open_WithFiniteValues_CreatesOpenRange()
    {
        // Arrange
        RangeValue<int> start = 10;
        RangeValue<int> end = 20;

        // Act
        var range = Range.Open(start, end);

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
        Assert.False(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void Open_WithImplicitConversion_WorksCorrectly()
    {
        // Arrange & Act
        var range = Range.Open<int>(5, 15);

        // Assert
        Assert.Equal(5, range.Start.Value);
        Assert.Equal(15, range.End.Value);
        Assert.False(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void Open_WithNegativeInfinityStart_CreatesUnboundedStartRange()
    {
        // Arrange & Act
        var range = Range.Open<int>(RangeValue<int>.NegativeInfinity, 100);

        // Assert
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.Equal(100, range.End.Value);
        Assert.False(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void Open_WithPositiveInfinityEnd_CreatesUnboundedEndRange()
    {
        // Arrange & Act
        var range = Range.Open<int>(0, RangeValue<int>.PositiveInfinity);

        // Assert
        Assert.Equal(0, range.Start.Value);
        Assert.True(range.End.IsPositiveInfinity);
        Assert.False(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void Open_WithBothInfinities_CreatesFullyUnboundedRange()
    {
        // Arrange & Act
        var range = Range.Open(RangeValue<int>.NegativeInfinity, RangeValue<int>.PositiveInfinity);

        // Assert
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.True(range.End.IsPositiveInfinity);
        Assert.False(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void Open_ToString_ReturnsCorrectFormat()
    {
        // Arrange & Act
        var range = Range.Open<int>(10, 20);
        var result = range.ToString();

        // Assert
        Assert.Equal("(10, 20)", result);
    }

    [Fact]
    public void Open_WithStringType_WorksCorrectly()
    {
        // Arrange & Act
        var range = Range.Open<string>("alpha", "omega");

        // Assert
        Assert.Equal("alpha", range.Start.Value);
        Assert.Equal("omega", range.End.Value);
        Assert.False(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    #endregion

    #region OpenClosed Method Tests

    [Fact]
    public void OpenClosed_WithFiniteValues_CreatesOpenClosedRange()
    {
        // Arrange
        RangeValue<int> start = 10;
        RangeValue<int> end = 20;

        // Act
        var range = Range.OpenClosed(start, end);

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
        Assert.False(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void OpenClosed_WithImplicitConversion_WorksCorrectly()
    {
        // Arrange & Act
        var range = Range.OpenClosed<int>(5, 15);

        // Assert
        Assert.Equal(5, range.Start.Value);
        Assert.Equal(15, range.End.Value);
        Assert.False(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void OpenClosed_WithNegativeInfinityStart_CreatesUnboundedStartRange()
    {
        // Arrange & Act
        var range = Range.OpenClosed<int>(RangeValue<int>.NegativeInfinity, 100);

        // Assert
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.Equal(100, range.End.Value);
        Assert.False(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void OpenClosed_WithPositiveInfinityEnd_CreatesUnboundedEndRange()
    {
        // Arrange & Act
        var range = Range.OpenClosed<int>(0, RangeValue<int>.PositiveInfinity);

        // Assert
        Assert.Equal(0, range.Start.Value);
        Assert.True(range.End.IsPositiveInfinity);
        Assert.False(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void OpenClosed_ToString_ReturnsCorrectFormat()
    {
        // Arrange & Act
        var range = Range.OpenClosed<int>(10, 20);
        var result = range.ToString();

        // Assert
        Assert.Equal("(10, 20]", result);
    }

    [Fact]
    public void OpenClosed_WithStringType_WorksCorrectly()
    {
        // Arrange & Act
        var range = Range.OpenClosed<string>("alpha", "omega");

        // Assert
        Assert.Equal("alpha", range.Start.Value);
        Assert.Equal("omega", range.End.Value);
        Assert.False(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    #endregion

    #region ClosedOpen Method Tests

    [Fact]
    public void ClosedOpen_WithFiniteValues_CreatesClosedOpenRange()
    {
        // Arrange
        RangeValue<int> start = 10;
        RangeValue<int> end = 20;

        // Act
        var range = Range.ClosedOpen(start, end);

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void ClosedOpen_WithImplicitConversion_WorksCorrectly()
    {
        // Arrange & Act
        var range = Range.ClosedOpen<int>(5, 15);

        // Assert
        Assert.Equal(5, range.Start.Value);
        Assert.Equal(15, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void ClosedOpen_WithNegativeInfinityStart_CreatesUnboundedStartRange()
    {
        // Arrange & Act
        var range = Range.ClosedOpen<int>(RangeValue<int>.NegativeInfinity, 100);

        // Assert
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.Equal(100, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void ClosedOpen_WithPositiveInfinityEnd_CreatesUnboundedEndRange()
    {
        // Arrange & Act
        var range = Range.ClosedOpen<int>(0, RangeValue<int>.PositiveInfinity);

        // Assert
        Assert.Equal(0, range.Start.Value);
        Assert.True(range.End.IsPositiveInfinity);
        Assert.True(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void ClosedOpen_ToString_ReturnsCorrectFormat()
    {
        // Arrange & Act
        var range = Range.ClosedOpen<int>(10, 20);
        var result = range.ToString();

        // Assert
        Assert.Equal("[10, 20)", result);
    }

    [Fact]
    public void ClosedOpen_WithStringType_WorksCorrectly()
    {
        // Arrange & Act
        var range = Range.ClosedOpen<string>("alpha", "omega");

        // Assert
        Assert.Equal("alpha", range.Start.Value);
        Assert.Equal("omega", range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void ClosedOpen_WithDateTimeType_WorksCorrectly()
    {
        // Arrange
        var startDate = new DateTime(2020, 1, 1);
        var endDate = new DateTime(2020, 12, 31);

        // Act
        var range = Range.ClosedOpen<DateTime>(startDate, endDate);

        // Assert
        Assert.Equal(startDate, range.Start.Value);
        Assert.Equal(endDate, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    #endregion

    #region FromString Method Tests - Basic Usage
    // Note: Comprehensive parsing tests are in RangeParserTests.cs
    // These tests verify the factory method delegates to RangeParser correctly

    [Fact]
    public void FromString_WithValidInput_ParsesCorrectly()
    {
        // Arrange
        var input = "[10, 20)";

        // Act
        var range = Range.FromString<int>(input);

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void FromString_WithInfinity_ParsesCorrectly()
    {
        // Arrange
        var input = "(-∞, ∞)";

        // Act
        var range = Range.FromString<int>(input);

        // Assert
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.True(range.End.IsPositiveInfinity);
    }

    [Fact]
    public void FromString_WithInvalidInput_ThrowsFormatException()
    {
        // Arrange
        var input = "invalid";

        // Act
        var exception = Record.Exception(() => Range.FromString<int>(input));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<FormatException>(exception);
    }

    [Fact]
    public void FromString_RoundTrip_PreservesRange()
    {
        // Arrange
        var original = Range.ClosedOpen<int>(10, 20);
        var stringRepresentation = original.ToString();

        // Act
        var parsed = Range.FromString<int>(stringRepresentation);

        // Assert
        Assert.Equal(original.Start.Value, parsed.Start.Value);
        Assert.Equal(original.End.Value, parsed.End.Value);
        Assert.Equal(original.IsStartInclusive, parsed.IsStartInclusive);
        Assert.Equal(original.IsEndInclusive, parsed.IsEndInclusive);
    }

    [Fact]
    public void FromString_RoundTrip_ClosedRange_PreservesRange()
    {
        // Arrange
        var original = Range.Closed<int>(5, 15);
        var stringRepresentation = original.ToString();

        // Act
        var parsed = Range.FromString<int>(stringRepresentation);

        // Assert
        Assert.Equal(original, parsed);
    }

    [Fact]
    public void FromString_RoundTrip_OpenRange_PreservesRange()
    {
        // Arrange
        var original = Range.Open<int>(5, 15);
        var stringRepresentation = original.ToString();

        // Act
        var parsed = Range.FromString<int>(stringRepresentation);

        // Assert
        Assert.Equal(original, parsed);
    }

    [Fact]
    public void FromString_RoundTrip_OpenClosedRange_PreservesRange()
    {
        // Arrange
        var original = Range.OpenClosed<int>(5, 15);
        var stringRepresentation = original.ToString();

        // Act
        var parsed = Range.FromString<int>(stringRepresentation);

        // Assert
        Assert.Equal(original, parsed);
    }

    [Fact]
    public void FromString_RoundTrip_WithDoubles_PreservesRange()
    {
        // Arrange
        var original = Range.ClosedOpen<double>(1.5, 9.5);
        var stringRepresentation = original.ToString();

        // Act
        var parsed = Range.FromString<double>(stringRepresentation);

        // Assert
        Assert.Equal(original.Start.Value, parsed.Start.Value);
        Assert.Equal(original.End.Value, parsed.End.Value);
        Assert.Equal(original.IsStartInclusive, parsed.IsStartInclusive);
        Assert.Equal(original.IsEndInclusive, parsed.IsEndInclusive);
    }

    [Fact]
    public void FromString_RoundTrip_WithNegativeInfinity_PreservesRange()
    {
        // Arrange
        var original = Range.Closed<int>(RangeValue<int>.NegativeInfinity, 100);
        var stringRepresentation = original.ToString();

        // Act
        var parsed = Range.FromString<int>(stringRepresentation);

        // Assert
        Assert.True(parsed.Start.IsNegativeInfinity);
        Assert.Equal(original.End.Value, parsed.End.Value);
        Assert.Equal(original.IsStartInclusive, parsed.IsStartInclusive);
        Assert.Equal(original.IsEndInclusive, parsed.IsEndInclusive);
    }

    [Fact]
    public void FromString_RoundTrip_WithPositiveInfinity_PreservesRange()
    {
        // Arrange
        var original = Range.Closed<int>(0, RangeValue<int>.PositiveInfinity);
        var stringRepresentation = original.ToString();

        // Act
        var parsed = Range.FromString<int>(stringRepresentation);

        // Assert
        Assert.Equal(original.Start.Value, parsed.Start.Value);
        Assert.True(parsed.End.IsPositiveInfinity);
        Assert.Equal(original.IsStartInclusive, parsed.IsStartInclusive);
        Assert.Equal(original.IsEndInclusive, parsed.IsEndInclusive);
    }

    [Fact]
    public void FromString_RoundTrip_WithBothInfinities_PreservesRange()
    {
        // Arrange
        var original = Range.Open(RangeValue<int>.NegativeInfinity, RangeValue<int>.PositiveInfinity);
        var stringRepresentation = original.ToString();

        // Act
        var parsed = Range.FromString<int>(stringRepresentation);

        // Assert
        Assert.True(parsed.Start.IsNegativeInfinity);
        Assert.True(parsed.End.IsPositiveInfinity);
        Assert.Equal(original.IsStartInclusive, parsed.IsStartInclusive);
        Assert.Equal(original.IsEndInclusive, parsed.IsEndInclusive);
    }

    [Fact]
    public void FromString_RoundTrip_WithNegativeNumbers_PreservesRange()
    {
        // Arrange
        var original = Range.Closed<int>(-100, -10);
        var stringRepresentation = original.ToString();

        // Act
        var parsed = Range.FromString<int>(stringRepresentation);

        // Assert
        Assert.Equal(original, parsed);
    }

    [Fact]
    public void FromString_InfinitySymbol_RoundTrip_WithNegativeInfinity_PreservesRange()
    {
        // Arrange - ToString outputs "-∞"
        var original = Range.Closed<int>(RangeValue<int>.NegativeInfinity, 100);
        var stringRepresentation = original.ToString();

        // Verify ToString produces infinity symbol
        Assert.Contains("-∞", stringRepresentation);

        // Act
        var parsed = Range.FromString<int>(stringRepresentation);

        // Assert
        Assert.True(parsed.Start.IsNegativeInfinity);
        Assert.Equal(100, parsed.End.Value);
        Assert.Equal(original.IsStartInclusive, parsed.IsStartInclusive);
        Assert.Equal(original.IsEndInclusive, parsed.IsEndInclusive);
    }

    [Fact]
    public void FromString_InfinitySymbol_RoundTrip_WithPositiveInfinity_PreservesRange()
    {
        // Arrange - ToString outputs "∞"
        var original = Range.Open<int>(0, RangeValue<int>.PositiveInfinity);
        var stringRepresentation = original.ToString();

        // Verify ToString produces infinity symbol
        Assert.Contains("∞", stringRepresentation);

        // Act
        var parsed = Range.FromString<int>(stringRepresentation);

        // Assert
        Assert.Equal(0, parsed.Start.Value);
        Assert.True(parsed.End.IsPositiveInfinity);
        Assert.Equal(original.IsStartInclusive, parsed.IsStartInclusive);
        Assert.Equal(original.IsEndInclusive, parsed.IsEndInclusive);
    }

    [Fact]
    public void FromString_InfinitySymbol_RoundTrip_WithBothInfinities_PreservesRange()
    {
        // Arrange - ToString outputs "(-∞, ∞)"
        var original = Range.Open(RangeValue<int>.NegativeInfinity, RangeValue<int>.PositiveInfinity);
        var stringRepresentation = original.ToString();

        // Verify ToString produces both infinity symbols
        Assert.Equal("(-∞, ∞)", stringRepresentation);

        // Act
        var parsed = Range.FromString<int>(stringRepresentation);

        // Assert
        Assert.True(parsed.Start.IsNegativeInfinity);
        Assert.True(parsed.End.IsPositiveInfinity);
        Assert.Equal(original.IsStartInclusive, parsed.IsStartInclusive);
        Assert.Equal(original.IsEndInclusive, parsed.IsEndInclusive);
    }

    #endregion

    #region Comparison Between Factory Methods Tests

    [Fact]
    public void FactoryMethods_CreateDifferentRangeTypes_WithSameValues()
    {
        // Arrange
        var start = 10;
        var end = 20;

        // Act
        var closed = Range.Closed<int>(start, end);
        var open = Range.Open<int>(start, end);
        var openClosed = Range.OpenClosed<int>(start, end);
        var closedOpen = Range.ClosedOpen<int>(start, end);

        // Assert - All have same values but different inclusivity
        Assert.Equal(10, closed.Start.Value);
        Assert.Equal(10, open.Start.Value);
        Assert.Equal(10, openClosed.Start.Value);
        Assert.Equal(10, closedOpen.Start.Value);

        Assert.Equal(20, closed.End.Value);
        Assert.Equal(20, open.End.Value);
        Assert.Equal(20, openClosed.End.Value);
        Assert.Equal(20, closedOpen.End.Value);

        // Different inclusivity
        Assert.True(closed.IsStartInclusive);
        Assert.False(open.IsStartInclusive);
        Assert.False(openClosed.IsStartInclusive);
        Assert.True(closedOpen.IsStartInclusive);

        Assert.True(closed.IsEndInclusive);
        Assert.False(open.IsEndInclusive);
        Assert.True(openClosed.IsEndInclusive);
        Assert.False(closedOpen.IsEndInclusive);
    }

    [Fact]
    public void FactoryMethods_WithInfinity_WorkConsistently()
    {
        // Arrange & Act
        var closedInf = Range.Closed(RangeValue<int>.NegativeInfinity, RangeValue<int>.PositiveInfinity);
        var openInf = Range.Open(RangeValue<int>.NegativeInfinity, RangeValue<int>.PositiveInfinity);

        // Assert
        Assert.True(closedInf.Start.IsNegativeInfinity);
        Assert.True(closedInf.End.IsPositiveInfinity);
        Assert.True(openInf.Start.IsNegativeInfinity);
        Assert.True(openInf.End.IsPositiveInfinity);

        // Inclusivity is different
        Assert.True(closedInf.IsStartInclusive);
        Assert.False(openInf.IsStartInclusive);
    }

    #endregion

    #region Edge Cases Tests

    [Fact]
    public void FactoryMethods_WithZeroValues_WorkCorrectly()
    {
        // Arrange & Act
        var range = Range.Closed<int>(0, 0);

        // Assert
        Assert.Equal(0, range.Start.Value);
        Assert.Equal(0, range.End.Value);
    }

    [Fact]
    public void FactoryMethods_WithMinMaxValues_WorkCorrectly()
    {
        // Arrange & Act
        var range = Range.Closed<int>(int.MinValue, int.MaxValue);

        // Assert
        Assert.Equal(int.MinValue, range.Start.Value);
        Assert.Equal(int.MaxValue, range.End.Value);
    }

    [Fact]
    public void FactoryMethods_ChainedUsage_WorksCorrectly()
    {
        // Arrange
        var range1 = Range.Closed<int>(0, 10);
        var range2 = Range.Closed<int>(5, 15);

        // Act
        var intersection = range1 & range2;

        // Assert
        Assert.NotNull(intersection);
        Assert.Equal(5, intersection.Value.Start.Value);
        Assert.Equal(10, intersection.Value.End.Value);
    }

    #endregion
}