using RangeFactory = Intervals.NET.Factories.Range;

namespace Intervals.NET.Tests;

/// <summary>
/// Tests for Range.FromString overload that accepts interpolated strings.
/// This overload provides zero-allocation parsing by using a custom interpolated string handler.
/// </summary>
public class RangeFactoryFromStringInterpolatedTests
{
    #region FromString - Literal Brackets (Interpolated)

    [Fact]
    public void FromString_WithClosedRange_LiteralBrackets_ParsesCorrectly()
    {
        // Arrange & Act
        var range = RangeFactory.FromString<int>($"[{10}, {20}]");

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void FromString_WithOpenRange_LiteralBrackets_ParsesCorrectly()
    {
        // Arrange & Act
        var range = RangeFactory.FromString<int>($"({10}, {20})");

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
        Assert.False(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void FromString_WithHalfOpenRange_LiteralBrackets_ParsesCorrectly()
    {
        // Arrange & Act
        var range = RangeFactory.FromString<int>($"[{10}, {20})");

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void FromString_WithHalfClosedRange_LiteralBrackets_ParsesCorrectly()
    {
        // Arrange & Act
        var range = RangeFactory.FromString<int>($"({10}, {20}]");

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
        Assert.False(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    #endregion

    #region FromString - Char Brackets

    [Fact]
    public void FromString_WithClosedRange_CharBrackets_ParsesCorrectly()
    {
        // Arrange
        char open = '[';
        char close = ']';
        int start = 10;
        int end = 20;

        // Act
        var range = RangeFactory.FromString<int>($"{open}{start}, {end}{close}");

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void FromString_WithOpenRange_CharBrackets_ParsesCorrectly()
    {
        // Arrange
        char open = '(';
        char close = ')';
        int start = 10;
        int end = 20;

        // Act
        var range = RangeFactory.FromString<int>($"{open}{start}, {end}{close}");

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
        Assert.False(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    #endregion

    #region FromString - Variables

    [Fact]
    public void FromString_WithVariables_ParsesCorrectly()
    {
        // Arrange
        int start = 15;
        int end = 35;

        // Act
        var range = RangeFactory.FromString<int>($"[{start}, {end}]");

        // Assert
        Assert.Equal(15, range.Start.Value);
        Assert.Equal(35, range.End.Value);
    }

    [Fact]
    public void FromString_WithComputedValues_ParsesCorrectly()
    {
        // Arrange
        int baseValue = 10;

        // Act
        var range = RangeFactory.FromString<int>($"[{baseValue * 2}, {baseValue * 5})");

        // Assert
        Assert.Equal(20, range.Start.Value);
        Assert.Equal(50, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    #endregion

    #region FromString - Different Types

    [Fact]
    public void FromString_WithDoubleValues_ParsesCorrectly()
    {
        // Arrange & Act
        var range = RangeFactory.FromString<double>($"[{1.5}, {9.5})");

        // Assert
        Assert.Equal(1.5, range.Start.Value);
        Assert.Equal(9.5, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void FromString_WithDateTimeValues_ParsesCorrectly()
    {
        // Arrange
        var start = new DateTime(2024, 1, 1);
        var end = new DateTime(2024, 12, 31);

        // Act
        var range = RangeFactory.FromString<DateTime>($"[{start}, {end})");

        // Assert
        Assert.Equal(start, range.Start.Value);
        Assert.Equal(end, range.End.Value);
    }

    [Fact]
    public void FromString_WithNegativeNumbers_ParsesCorrectly()
    {
        // Arrange & Act
        var range = RangeFactory.FromString<int>($"[{-100}, {-10}]");

        // Assert
        Assert.Equal(-100, range.Start.Value);
        Assert.Equal(-10, range.End.Value);
    }

    #endregion

    #region FromString - Infinity

    [Fact]
    public void FromString_WithNegativeInfinity_ParsesCorrectly()
    {
        // Arrange
        var start = RangeValue<int>.NegativeInfinity;
        int end = 100;

        // Act
        var range = RangeFactory.FromString<int>($"[{start}, {end}]");

        // Assert
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.Equal(100, range.End.Value);
    }

    [Fact]
    public void FromString_WithPositiveInfinity_ParsesCorrectly()
    {
        // Arrange
        int start = 0;
        var end = RangeValue<int>.PositiveInfinity;

        // Act
        var range = RangeFactory.FromString<int>($"[{start}, {end})");

        // Assert
        Assert.Equal(0, range.Start.Value);
        Assert.True(range.End.IsPositiveInfinity);
    }

    [Fact]
    public void FromString_WithBothInfinities_ParsesCorrectly()
    {
        // Arrange
        var start = RangeValue<int>.NegativeInfinity;
        var end = RangeValue<int>.PositiveInfinity;

        // Act
        var range = RangeFactory.FromString<int>($"({start}, {end})");

        // Assert
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.True(range.End.IsPositiveInfinity);
        Assert.False(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    #endregion

    #region FromString - Error Cases

    [Fact]
    public void FromString_WithInvalidBracket_ThrowsFormatException()
    {
        // Arrange
        char invalidBracket = '{';

        // Act
        var exception = Record.Exception(() =>
            RangeFactory.FromString<int>($"{invalidBracket}{10}, {20}]"));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<FormatException>(exception);
    }

    #endregion

    #region FromString - Round Trip

    [Fact]
    public void FromString_RoundTrip_WithToString_PreservesFormat()
    {
        // Arrange
        int start = 10;
        int end = 20;

        // Act
        var range = RangeFactory.FromString<int>($"[{start}, {end})");
        var str = range.ToString();

        // Assert
        Assert.Equal("[10, 20)", str);
    }

    #endregion

    #region FromString vs FromString - Comparison

    [Fact]
    public void FromString_InterpolatedProducesSameResultAs_LiteralString()
    {
        // Arrange
        int start = 10;
        int end = 20;

        // Act
        var rangeFromInterpolated = RangeFactory.FromString<int>($"[{start}, {end})");
        var rangeFromLiteral = RangeFactory.FromString<int>("[10, 20)");

        // Assert
        Assert.Equal(rangeFromLiteral.Start.Value, rangeFromInterpolated.Start.Value);
        Assert.Equal(rangeFromLiteral.End.Value, rangeFromInterpolated.End.Value);
        Assert.Equal(rangeFromLiteral.IsStartInclusive, rangeFromInterpolated.IsStartInclusive);
        Assert.Equal(rangeFromLiteral.IsEndInclusive, rangeFromInterpolated.IsEndInclusive);
    }

    [Fact]
    public void FromString_Interpolated_WithDynamicValues_WorksWithoutStringConcatenation()
    {
        // Arrange
        int dynamicStart = new Random(42).Next(10, 50);
        int dynamicEnd = dynamicStart + 10;

        // Act - Interpolated string handler processes values directly (zero allocations)
        var range = RangeFactory.FromString<int>($"[{dynamicStart}, {dynamicEnd}]");

        // Assert - Literal string would require string concatenation/interpolation first
        Assert.Equal(dynamicStart, range.Start.Value);
        Assert.Equal(dynamicEnd, range.End.Value);
    }

    #endregion

    #region FromString - Integration with Other Factory Methods

    [Fact]
    public void FromString_Interpolated_Result_EqualsFactoryMethod()
    {
        // Arrange
        int start = 10;
        int end = 20;

        // Act
        var fromInterpolated = RangeFactory.FromString<int>($"[{start}, {end})");
        var fromFactory = RangeFactory.ClosedOpen(start, end);

        // Assert
        Assert.Equal(fromFactory, fromInterpolated);
    }

    #endregion
}
