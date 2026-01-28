using Intervals.NET.Parsers;

namespace Intervals.NET.Tests;

public class RangeStringParserTests
{
    #region Parse Method Tests

    [Fact]
    public void Parse_WithClosedRange_ParsesCorrectly()
    {
        // Arrange
        var input = "[10, 20]";

        // Act
        var range = RangeStringParser.Parse<int>(input);

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void Parse_WithOpenRange_ParsesCorrectly()
    {
        // Arrange
        var input = "(10, 20)";

        // Act
        var range = RangeStringParser.Parse<int>(input);

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
        Assert.False(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void Parse_WithOpenClosedRange_ParsesCorrectly()
    {
        // Arrange
        var input = "(10, 20]";

        // Act
        var range = RangeStringParser.Parse<int>(input);

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
        Assert.False(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void Parse_WithClosedOpenRange_ParsesCorrectly()
    {
        // Arrange
        var input = "[10, 20)";

        // Act
        var range = RangeStringParser.Parse<int>(input);

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    #endregion

    #region Infinity Parsing - Empty Values

    [Fact]
    public void Parse_WithEmptyStart_ParsesAsNegativeInfinity()
    {
        // Arrange
        var input = "[, 20]";

        // Act
        var range = RangeStringParser.Parse<int>(input);

        // Assert
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.Equal(20, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void Parse_WithEmptyEnd_ParsesAsPositiveInfinity()
    {
        // Arrange
        var input = "[10, ]";

        // Act
        var range = RangeStringParser.Parse<int>(input);

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.True(range.End.IsPositiveInfinity);
        Assert.True(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void Parse_WithBothEmpty_ParsesAsBothInfinities()
    {
        // Arrange
        var input = "(, )";

        // Act
        var range = RangeStringParser.Parse<int>(input);

        // Assert
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.True(range.End.IsPositiveInfinity);
        Assert.False(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    #endregion

    #region Infinity Parsing - Symbols

    [Fact]
    public void Parse_WithNegativeInfinitySymbol_ParsesCorrectly()
    {
        // Arrange
        var input = "[-∞, 20]";

        // Act
        var range = RangeStringParser.Parse<int>(input);

        // Assert
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.Equal(20, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void Parse_WithPositiveInfinitySymbol_ParsesCorrectly()
    {
        // Arrange
        var input = "[10, ∞]";

        // Act
        var range = RangeStringParser.Parse<int>(input);

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.True(range.End.IsPositiveInfinity);
        Assert.True(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void Parse_WithBothInfinitySymbols_ParsesCorrectly()
    {
        // Arrange
        var input = "(-∞, ∞)";

        // Act
        var range = RangeStringParser.Parse<int>(input);

        // Assert
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.True(range.End.IsPositiveInfinity);
        Assert.False(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void Parse_WithMixedEmptyAndSymbol_NegativeInfinityEmpty_ParsesCorrectly()
    {
        // Arrange
        var input = "[, ∞)";

        // Act
        var range = RangeStringParser.Parse<int>(input);

        // Assert
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.True(range.End.IsPositiveInfinity);
        Assert.True(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void Parse_WithMixedSymbolAndEmpty_PositiveInfinityEmpty_ParsesCorrectly()
    {
        // Arrange
        var input = "(-∞, ]";

        // Act
        var range = RangeStringParser.Parse<int>(input);

        // Assert
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.True(range.End.IsPositiveInfinity);
        Assert.False(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void Parse_WithPositiveInfinitySymbolAsStart_ParsesCorrectly()
    {
        // Arrange - edge case: positive infinity as start (logically unusual but syntactically valid)
        var input = "[∞, ∞]";

        // Act
        var range = RangeStringParser.Parse<int>(input);

        // Assert
        Assert.True(range.Start.IsPositiveInfinity);
        Assert.True(range.End.IsPositiveInfinity);
    }

    [Fact]
    public void Parse_WithNegativeInfinitySymbolAsEnd_ParsesCorrectly()
    {
        // Arrange - edge case: negative infinity as end (logically unusual but syntactically valid)
        var input = "[-∞, -∞]";

        // Act
        var range = RangeStringParser.Parse<int>(input);

        // Assert
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.True(range.End.IsNegativeInfinity);
    }

    #endregion

    #region Different Types

    [Fact]
    public void Parse_WithDoubleValues_ParsesCorrectly()
    {
        // Arrange
        var input = "[1.5, 9.5)";

        // Act
        var range = RangeStringParser.Parse<double>(input);

        // Assert
        Assert.Equal(1.5, range.Start.Value);
        Assert.Equal(9.5, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void Parse_WithNegativeNumbers_ParsesCorrectly()
    {
        // Arrange
        var input = "[-100, -10]";

        // Act
        var range = RangeStringParser.Parse<int>(input);

        // Assert
        Assert.Equal(-100, range.Start.Value);
        Assert.Equal(-10, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void Parse_WithWhitespace_ParsesCorrectly()
    {
        // Arrange
        var input = "[  10  ,  20  ]";

        // Act
        var range = RangeStringParser.Parse<int>(input);

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
    }

    #endregion

    #region Format Provider Support

    [Fact]
    public void Parse_WithCustomFormatProvider_ParsesCorrectly()
    {
        // Arrange
        var input = "[1,5, 9,5]"; // European decimal separator (comma)
        var culture = new System.Globalization.CultureInfo("de-DE");

        // Act
        var range = RangeStringParser.Parse<double>(input, culture);

        // Assert
        Assert.Equal(1.5, range.Start.Value);
        Assert.Equal(9.5, range.End.Value);
    }

    [Fact]
    public void Parse_WithInvariantCulture_ParsesCorrectly()
    {
        // Arrange
        var input = "[1.5, 9.5]";
        var culture = System.Globalization.CultureInfo.InvariantCulture;

        // Act
        var range = RangeStringParser.Parse<double>(input, culture);

        // Assert
        Assert.Equal(1.5, range.Start.Value);
        Assert.Equal(9.5, range.End.Value);
    }

    #endregion

    #region Error Handling

    [Fact]
    public void Parse_WithInvalidFormat_ThrowsFormatException()
    {
        // Arrange
        var input = "invalid";

        // Act
        var exception = Record.Exception(() => RangeStringParser.Parse<int>(input));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<FormatException>(exception);
    }

    [Fact]
    public void Parse_WithMissingComma_ThrowsFormatException()
    {
        // Arrange
        var input = "[10 20]";

        // Act
        var exception = Record.Exception(() => RangeStringParser.Parse<int>(input));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<FormatException>(exception);
        Assert.Contains("comma", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Parse_WithInvalidStartBracket_ThrowsFormatException()
    {
        // Arrange
        var input = "{10, 20]";

        // Act
        var exception = Record.Exception(() => RangeStringParser.Parse<int>(input));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<FormatException>(exception);
    }

    [Fact]
    public void Parse_WithInvalidEndBracket_ThrowsFormatException()
    {
        // Arrange
        var input = "[10, 20}";

        // Act
        var exception = Record.Exception(() => RangeStringParser.Parse<int>(input));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<FormatException>(exception);
    }

    [Fact]
    public void Parse_WithTooShortInput_ThrowsFormatException()
    {
        // Arrange
        var input = "[]";

        // Act
        var exception = Record.Exception(() => RangeStringParser.Parse<int>(input));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<FormatException>(exception);
    }

    [Fact]
    public void Parse_WithInvalidStartValue_ThrowsFormatException()
    {
        // Arrange
        var input = "[abc, 20]";

        // Act
        var exception = Record.Exception(() => RangeStringParser.Parse<int>(input));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<FormatException>(exception);
        Assert.Contains("start", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Parse_WithInvalidEndValue_ThrowsFormatException()
    {
        // Arrange
        var input = "[10, xyz]";

        // Act
        var exception = Record.Exception(() => RangeStringParser.Parse<int>(input));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<FormatException>(exception);
        Assert.Contains("end", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region TryParse Method Tests

    [Fact]
    public void TryParse_WithValidInput_ReturnsTrue()
    {
        // Arrange
        var input = "[10, 20]";

        // Act
        var result = RangeStringParser.TryParse<int>(input, out var range);

        // Assert
        Assert.True(result);
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
    }

    [Fact]
    public void TryParse_WithInvalidInput_ReturnsFalse()
    {
        // Arrange
        var input = "invalid";

        // Act
        var result = RangeStringParser.TryParse<int>(input, out var range);

        // Assert
        Assert.False(result);
        Assert.Equal(default, range);
    }

    [Fact]
    public void TryParse_WithMissingComma_ReturnsFalse()
    {
        // Arrange
        var input = "[10 20]";

        // Act
        var result = RangeStringParser.TryParse<int>(input, out var range);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TryParse_WithInvalidValue_ReturnsFalse()
    {
        // Arrange
        var input = "[abc, 20]";

        // Act
        var result = RangeStringParser.TryParse<int>(input, out var range);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TryParse_WithCustomFormatProvider_ReturnsTrue()
    {
        // Arrange
        var input = "[1,5, 9,5]";
        var culture = new System.Globalization.CultureInfo("de-DE");

        // Act
        var result = RangeStringParser.TryParse<double>(input, out var range, culture);

        // Assert
        Assert.True(result);
        Assert.Equal(1.5, range.Start.Value);
        Assert.Equal(9.5, range.End.Value);
    }

    #endregion

    #region Round-Trip Tests

    [Fact]
    public void Parse_RoundTrip_WithClosedRange_PreservesRange()
    {
        // Arrange
        var original = new Range<int>(10, 20, true, true);
        var stringRepresentation = original.ToString();

        // Act
        var parsed = RangeStringParser.Parse<int>(stringRepresentation);

        // Assert
        Assert.Equal(original, parsed);
    }

    [Fact]
    public void Parse_RoundTrip_WithOpenRange_PreservesRange()
    {
        // Arrange
        var original = new Range<int>(10, 20, false, false);
        var stringRepresentation = original.ToString();

        // Act
        var parsed = RangeStringParser.Parse<int>(stringRepresentation);

        // Assert
        Assert.Equal(original, parsed);
    }

    [Fact]
    public void Parse_RoundTrip_WithInfinitySymbols_PreservesRange()
    {
        // Arrange
        var original = new Range<int>(RangeValue<int>.NegativeInfinity, RangeValue<int>.PositiveInfinity, false, false);
        var stringRepresentation = original.ToString();

        // Verify ToString produces infinity symbols
        Assert.Equal("(-∞, ∞)", stringRepresentation);

        // Act
        var parsed = RangeStringParser.Parse<int>(stringRepresentation);

        // Assert
        Assert.True(parsed.Start.IsNegativeInfinity);
        Assert.True(parsed.End.IsPositiveInfinity);
        Assert.Equal(original.IsStartInclusive, parsed.IsStartInclusive);
        Assert.Equal(original.IsEndInclusive, parsed.IsEndInclusive);
    }

    [Fact]
    public void Parse_RoundTrip_WithDoubleValues_PreservesRange()
    {
        // Arrange
        var original = new Range<double>(1.5, 9.5, true, false);
        var stringRepresentation = original.ToString();

        // Act
        var parsed = RangeStringParser.Parse<double>(stringRepresentation);

        // Assert
        Assert.Equal(original.Start.Value, parsed.Start.Value);
        Assert.Equal(original.End.Value, parsed.End.Value);
        Assert.Equal(original.IsStartInclusive, parsed.IsStartInclusive);
        Assert.Equal(original.IsEndInclusive, parsed.IsEndInclusive);
    }

    #endregion
}
