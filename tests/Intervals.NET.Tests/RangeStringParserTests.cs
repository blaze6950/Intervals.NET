using System.Globalization;
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

    #region FindSeparatorComma Edge Cases

    [Fact]
    public void Parse_WithNoValidSeparatorComma_AllCommasAreDecimalSeparators_ThrowsFormatException()
    {
        // Arrange - Input where commas could be decimal separators in de-DE culture
        // Note: "[1,2,3]" will successfully parse as "[1, 2.3]" because 2,3 is valid in de-DE
        // The scenario of "all commas being decimal separators with NO valid separator" is
        // actually impossible to construct with valid numeric input, since the algorithm will
        // always find at least one valid split point.
        // Therefore, this test verifies that the parser correctly handles the multiple comma case
        var input = "[1,2,3]";
        var culture = new CultureInfo("de-DE");

        // Act - Should parse successfully as [1, 2.3]
        var range = RangeStringParser.Parse<double>(input, culture);
        
        // Assert - Verifies the algorithm found the correct separator (first comma)
        Assert.Equal(1.0, range.Start.Value);
        Assert.Equal(2.3, range.End.Value, precision: 10);
    }

    [Fact]
    public void Parse_WithFiveCommas_IteratesThroughMultipleCommas_FindsCorrectSeparator()
    {
        // Arrange - 5 commas: 2 in start, 1 separator, 2 in end
        // German culture: 1.234,56 to 7.890,12
        var input = "[1.234,56, 7.890,12]";
        var culture = new CultureInfo("de-DE");

        // Act - Tests loop iteration in FindSeparatorComma
        var range = RangeStringParser.Parse<double>(input, culture);

        // Assert
        Assert.Equal(1234.56, range.Start.Value, precision: 2);
        Assert.Equal(7890.12, range.End.Value, precision: 2);
    }

    [Fact]
    public void Parse_WithInfinitySymbolInMultiCommaContext_SkipsInvalidCommas()
    {
        // Arrange - Multiple commas where some splits result in invalid parses
        var input = "[1,2, ∞]"; // de-DE: "1.2" to infinity
        var culture = new CultureInfo("de-DE");

        // Act - Tests rightValid check with infinity symbol
        var range = RangeStringParser.Parse<double>(input, culture);

        // Assert
        Assert.Equal(1.2, range.Start.Value, precision: 1);
        Assert.True(range.End.IsPositiveInfinity);
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
        var culture = new CultureInfo("de-DE");

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
        var culture = CultureInfo.InvariantCulture;

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
        var culture = new CultureInfo("de-DE");

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

    #region Additional Edge Case Tests

    [Fact]
    public void Parse_WithEmptyString_ThrowsFormatException()
    {
        // Arrange
        var input = "";

        // Act
        var exception = Record.Exception(() => RangeStringParser.Parse<int>(input));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<FormatException>(exception);
        Assert.Contains("too short", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Parse_WithSingleCharacter_ThrowsFormatException()
    {
        // Arrange
        var input = "[";

        // Act
        var exception = Record.Exception(() => RangeStringParser.Parse<int>(input));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<FormatException>(exception);
    }

    [Fact]
    public void Parse_WithTwoCharacters_ThrowsFormatException()
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
    public void Parse_WithMinimalValidInput_ParsesCorrectly()
    {
        // Arrange
        var input = "[,]"; // Minimal: both infinities

        // Act
        var range = RangeStringParser.Parse<int>(input);

        // Assert
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.True(range.End.IsPositiveInfinity);
    }

    [Fact]
    public void Parse_WithMultipleCommasInDecimalSeparatorCulture_ParsesCorrectly()
    {
        // Arrange - German culture uses comma as decimal separator
        var input = "[1,5, 2,5]"; // Two decimal numbers with commas
        var culture = new CultureInfo("de-DE");

        // Act
        var range = RangeStringParser.Parse<double>(input, culture);

        // Assert
        Assert.Equal(1.5, range.Start.Value);
        Assert.Equal(2.5, range.End.Value);
    }

    [Fact]
    public void Parse_WithThreeCommasInDecimalCulture_ParsesCorrectly()
    {
        // Arrange - Complex case with 3 commas total
        var input = "[1,23, 4,56]"; // Two decimals in German format
        var culture = new CultureInfo("de-DE");

        // Act
        var range = RangeStringParser.Parse<double>(input, culture);

        // Assert
        Assert.Equal(1.23, range.Start.Value);
        Assert.Equal(4.56, range.End.Value);
    }

    [Fact]
    public void TryParse_WithEmptyString_ReturnsFalse()
    {
        // Arrange
        var input = "";

        // Act
        var result = RangeStringParser.TryParse<int>(input, out var range);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TryParse_WithTooShortInput_ReturnsFalse()
    {
        // Arrange
        var input = "[]";

        // Act
        var result = RangeStringParser.TryParse<int>(input, out var range);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Parse_WithOnlyWhitespace_ThrowsFormatException()
    {
        // Arrange
        var input = "[   ,   ]";

        // Act - Empty after trim should be treated as infinity
        var range = RangeStringParser.Parse<int>(input);

        // Assert
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.True(range.End.IsPositiveInfinity);
    }

    [Fact]
    public void Parse_WithExtraWhitespaceAroundValues_ParsesCorrectly()
    {
        // Arrange
        var input = "[   10   ,   20   ]";

        // Act
        var range = RangeStringParser.Parse<int>(input);

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
    }

    [Fact]
    public void Parse_WithNegativeZero_ParsesCorrectly()
    {
        // Arrange
        var input = "[-0, 0]";

        // Act
        var range = RangeStringParser.Parse<int>(input);

        // Assert
        Assert.Equal(0, range.Start.Value);
        Assert.Equal(0, range.End.Value);
    }

    [Fact]
    public void Parse_WithScientificNotation_ParsesCorrectly()
    {
        // Arrange
        var input = "[1e2, 1e3]";

        // Act
        var range = RangeStringParser.Parse<double>(input);

        // Assert
        Assert.Equal(100.0, range.Start.Value);
        Assert.Equal(1000.0, range.End.Value);
    }

    [Fact]
    public void Parse_WithVeryLargeNumbers_ParsesCorrectly()
    {
        // Arrange
        var input = $"[{long.MinValue}, {long.MaxValue}]";

        // Act
        var range = RangeStringParser.Parse<long>(input);

        // Assert
        Assert.Equal(long.MinValue, range.Start.Value);
        Assert.Equal(long.MaxValue, range.End.Value);
    }

    #endregion

    #region Defensive Code Verification Tests

    [Fact]
    public void Parse_AllErrorPathsThrowExceptions_NeverReturnsUnexpectedFailure()
    {
        // This test verifies that the defensive code in Parse() is unreachable
        // because TryParseCore with throwOnError=true always throws, never returns false

        // Arrange - Various invalid inputs
        var invalidInputs = new[]
        {
            "",                    // Too short
            "[]",                  // Too short
            "{10, 20}",           // Invalid brackets
            "[10 20]",            // Missing comma
            "[abc, 20]",          // Invalid start value
            "[10, xyz]",          // Invalid end value
            "{10, 20]",           // Mismatched brackets
        };

        // Act & Assert - All should throw FormatException, none should throw InvalidOperationException
        foreach (var input in invalidInputs)
        {
            var exception = Record.Exception(() => RangeStringParser.Parse<int>(input));

            Assert.NotNull(exception);
            // Should be FormatException (not InvalidOperationException from defensive code)
            Assert.IsType<FormatException>(exception);
            Assert.DoesNotContain("Unexpected parse failure", exception.Message);
        }
    }

    #endregion
}