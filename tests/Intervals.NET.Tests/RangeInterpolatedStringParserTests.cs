using Intervals.NET.Parsers;

namespace Intervals.NET.Tests;

public class RangeInterpolatedStringParserTests
{
    #region Parse Tests

    [Fact]
    public void Parse_WithCharBrackets_ClosedRange_ParsesCorrectly()
    {
        // Arrange
        char openBracket = '[';
        int start = 10;
        int end = 20;
        char closeBracket = ']';

        // Act
        var range = RangeInterpolatedStringParser.Parse<int>($"{openBracket}{start}, {end}{closeBracket}");

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void Parse_WithLiteralBrackets_ClosedRange_ParsesCorrectly()
    {
        // Arrange
        int start = 10;
        int end = 20;

        // Act
        var range = RangeInterpolatedStringParser.Parse<int>($"[{start}, {end}]");

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void Parse_WithLiteralBrackets_OpenRange_ParsesCorrectly()
    {
        // Arrange
        int start = 10;
        int end = 20;

        // Act
        var range = RangeInterpolatedStringParser.Parse<int>($"({start}, {end})");

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
        Assert.False(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void Parse_WithLiteralBrackets_HalfOpenRange_ParsesCorrectly()
    {
        // Arrange
        int start = 10;
        int end = 20;

        // Act
        var range = RangeInterpolatedStringParser.Parse<int>($"[{start}, {end})");

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void Parse_WithOpenRange_ParsesCorrectly()
    {
        // Arrange
        char openBracket = '(';
        int start = 10;
        int end = 20;
        char closeBracket = ')';

        // Act
        var range = RangeInterpolatedStringParser.Parse<int>($"{openBracket}{start}, {end}{closeBracket}");

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
        Assert.False(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void Parse_WithHalfOpenRange_ParsesCorrectly()
    {
        // Arrange
        char openBracket = '[';
        int start = 10;
        int end = 20;
        char closeBracket = ')';

        // Act
        var range = RangeInterpolatedStringParser.Parse<int>($"{openBracket}{start}, {end}{closeBracket}");

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void Parse_WithHalfClosedRange_ParsesCorrectly()
    {
        // Arrange
        char openBracket = '(';
        int start = 10;
        int end = 20;
        char closeBracket = ']';

        // Act
        var range = RangeInterpolatedStringParser.Parse<int>($"{openBracket}{start}, {end}{closeBracket}");

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
        Assert.False(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void Parse_WithDoubleValues_ParsesCorrectly()
    {
        // Arrange
        char openBracket = '[';
        double start = 1.5;
        double end = 9.5;
        char closeBracket = ')';

        // Act
        var range = RangeInterpolatedStringParser.Parse<double>($"{openBracket}{start}, {end}{closeBracket}");

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
        char openBracket = '[';
        int start = -100;
        int end = -10;
        char closeBracket = ']';

        // Act
        var range = RangeInterpolatedStringParser.Parse<int>($"{openBracket}{start}, {end}{closeBracket}");

        // Assert
        Assert.Equal(-100, range.Start.Value);
        Assert.Equal(-10, range.End.Value);
    }

    #endregion

    #region Infinity Tests

    [Fact]
    public void Parse_WithNegativeInfinityStart_ParsesCorrectly()
    {
        // Arrange
        char openBracket = '[';
        var start = RangeValue<int>.NegativeInfinity;
        int end = 20;
        char closeBracket = ']';

        // Act
        var range = RangeInterpolatedStringParser.Parse<int>($"{openBracket}{start}, {end}{closeBracket}");

        // Assert
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.Equal(20, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void Parse_WithPositiveInfinityEnd_ParsesCorrectly()
    {
        // Arrange
        char openBracket = '[';
        int start = 10;
        var end = RangeValue<int>.PositiveInfinity;
        char closeBracket = ']';

        // Act
        var range = RangeInterpolatedStringParser.Parse<int>($"{openBracket}{start}, {end}{closeBracket}");

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.True(range.End.IsPositiveInfinity);
    }

    [Fact]
    public void Parse_WithBothInfinities_ParsesCorrectly()
    {
        // Arrange
        char openBracket = '(';
        var start = RangeValue<int>.NegativeInfinity;
        var end = RangeValue<int>.PositiveInfinity;
        char closeBracket = ')';

        // Act
        var range = RangeInterpolatedStringParser.Parse<int>($"{openBracket}{start}, {end}{closeBracket}");

        // Assert
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.True(range.End.IsPositiveInfinity);
    }

    #endregion

    #region TryParse Tests

    [Fact]
    public void TryParse_WithValidInput_ReturnsTrue()
    {
        // Arrange
        char openBracket = '[';
        int start = 10;
        int end = 20;
        char closeBracket = ']';

        // Act
        var result = RangeInterpolatedStringParser.TryParse<int>($"{openBracket}{start}, {end}{closeBracket}", out var range);

        // Assert
        Assert.True(result);
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
    }

    [Fact]
    public void TryParse_WithInvalidOpenBracket_ReturnsFalse()
    {
        // Arrange
        char openBracket = '{';
        int start = 10;
        int end = 20;
        char closeBracket = ']';

        // Act
        var result = RangeInterpolatedStringParser.TryParse<int>($"{openBracket}{start}, {end}{closeBracket}", out var range);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TryParse_WithInvalidCloseBracket_ReturnsFalse()
    {
        // Arrange
        char openBracket = '[';
        int start = 10;
        int end = 20;
        char closeBracket = '}';

        // Act
        var result = RangeInterpolatedStringParser.TryParse<int>($"{openBracket}{start}, {end}{closeBracket}", out var range);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public void Parse_WithInvalidOpenBracket_ThrowsFormatException()
    {
        // Arrange
        char openBracket = '{';
        int start = 10;
        int end = 20;
        char closeBracket = ']';

        // Act
        var exception = Record.Exception(() =>
            RangeInterpolatedStringParser.Parse<int>($"{openBracket}{start}, {end}{closeBracket}"));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<FormatException>(exception);
        // Generic error message since we removed _errorMessage field for zero-allocation
        Assert.Contains("Failed to parse range", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Parse_WithInvalidCloseBracket_ThrowsFormatException()
    {
        // Arrange
        char openBracket = '[';
        int start = 10;
        int end = 20;
        char closeBracket = '}';

        // Act
        var exception = Record.Exception(() =>
            RangeInterpolatedStringParser.Parse<int>($"{openBracket}{start}, {end}{closeBracket}"));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<FormatException>(exception);
        // Generic error message since we removed _errorMessage field for zero-allocation
        Assert.Contains("Failed to parse range", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region Variable Reference Tests

    [Fact]
    public void Parse_WithVariables_WorksCorrectly()
    {
        // Arrange
        var brackets = new[] { ('[', ']'), ('(', ')'), ('[', ')'), ('(', ']') };
        var expectedInclusivity = new[]
        {
            (true, true),
            (false, false),
            (true, false),
            (false, true)
        };

        for (int i = 0; i < brackets.Length; i++)
        {
            var (open, close) = brackets[i];
            var (startInc, endInc) = expectedInclusivity[i];

            int start = 10;
            int end = 20;

            // Act
            var range = RangeInterpolatedStringParser.Parse<int>($"{open}{start}, {end}{close}");

            // Assert
            Assert.Equal(startInc, range.IsStartInclusive);
            Assert.Equal(endInc, range.IsEndInclusive);
        }
    }

    [Fact]
    public void Parse_WithComputedValues_WorksCorrectly()
    {
        // Arrange
        char openBracket = '[';
        int baseValue = 10;
        int start = baseValue * 2;  // 20
        int end = baseValue * 5;    // 50
        char closeBracket = ')';

        // Act
        var range = RangeInterpolatedStringParser.Parse<int>($"{openBracket}{start}, {end}{closeBracket}");

        // Assert
        Assert.Equal(20, range.Start.Value);
        Assert.Equal(50, range.End.Value);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Parse_RoundTrip_WithToString_PreservesFormat()
    {
        // Arrange
        char openBracket = '[';
        int start = 10;
        int end = 20;
        char closeBracket = ')';

        // Act
        var range = RangeInterpolatedStringParser.Parse<int>($"{openBracket}{start}, {end}{closeBracket}");
        var str = range.ToString();

        // Assert
        Assert.Equal("[10, 20)", str);
    }

    [Fact]
    public void Parse_WithDifferentTypes_WorksCorrectly()
    {
        // Arrange & Act
        var intRange = RangeInterpolatedStringParser.Parse<int>($"{'['}{10}, {20}{']'}");
        var doubleRange = RangeInterpolatedStringParser.Parse<double>($"{'('}{1.5}, {9.5}{')'}");
        var dateRange = RangeInterpolatedStringParser.Parse<DateTime>(
            $"{'['}{new DateTime(2024, 1, 1)}, {new DateTime(2024, 12, 31)}{')'}");

        // Assert
        Assert.Equal(10, intRange.Start.Value);
        Assert.Equal(1.5, doubleRange.Start.Value);
        Assert.Equal(new DateTime(2024, 1, 1), dateRange.Start.Value);
    }

    #endregion

    #region AppendFormatted String Tests

    [Fact]
    public void Parse_WithStringValue_ParsesCorrectly()
    {
        // Arrange
        char openBracket = '[';
        string start = "10";
        string end = "20";
        char closeBracket = ']';

        // Act - String values should be parsed as T
        var range = RangeInterpolatedStringParser.Parse<int>($"{openBracket}{start}, {end}{closeBracket}");

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
    }

    [Fact]
    public void Parse_WithEmptyStringAsStart_ParsesAsNegativeInfinity()
    {
        // Arrange
        char openBracket = '[';
        string start = "";
        int end = 20;
        char closeBracket = ']';

        // Act
        var range = RangeInterpolatedStringParser.Parse<int>($"{openBracket}{start}, {end}{closeBracket}");

        // Assert
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.Equal(20, range.End.Value);
    }

    [Fact]
    public void Parse_WithEmptyStringAsEnd_ParsesAsPositiveInfinity()
    {
        // Arrange
        char openBracket = '[';
        int start = 10;
        string end = "";
        char closeBracket = ']';

        // Act
        var range = RangeInterpolatedStringParser.Parse<int>($"{openBracket}{start}, {end}{closeBracket}");

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.True(range.End.IsPositiveInfinity);
    }

    [Fact]
    public void Parse_WithWhitespaceStringAsStart_ParsesAsNegativeInfinity()
    {
        // Arrange
        char openBracket = '[';
        string start = "   ";
        int end = 20;
        char closeBracket = ']';

        // Act
        var range = RangeInterpolatedStringParser.Parse<int>($"{openBracket}{start}, {end}{closeBracket}");

        // Assert
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.Equal(20, range.End.Value);
    }

    [Fact]
    public void Parse_WithNullStringAsStart_ParsesAsNegativeInfinity()
    {
        // Arrange
        char openBracket = '[';
        string? start = null;
        int end = 20;
        char closeBracket = ']';

        // Act
        var range = RangeInterpolatedStringParser.Parse<int>($"{openBracket}{start}, {end}{closeBracket}");

        // Assert
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.Equal(20, range.End.Value);
    }

    [Fact]
    public void Parse_WithInvalidStringValue_ThrowsFormatException()
    {
        // Arrange
        char openBracket = '[';
        string start = "not-a-number";
        int end = 20;
        char closeBracket = ']';

        // Act
        var exception = Record.Exception(() =>
            RangeInterpolatedStringParser.Parse<int>($"{openBracket}{start}, {end}{closeBracket}"));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<FormatException>(exception);
    }

    [Fact]
    public void TryParse_WithInvalidStringValue_ReturnsFalse()
    {
        // Arrange
        char openBracket = '[';
        string start = "invalid";
        int end = 20;
        char closeBracket = ']';

        // Act
        var result = RangeInterpolatedStringParser.TryParse<int>(
            $"{openBracket}{start}, {end}{closeBracket}", out var range);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region State Machine Error Tests

    [Fact]
    public void Parse_WithWrongFormattedCount_ThrowsFormatException()
    {
        // Arrange & Act - Handler with wrong formattedCount (3 instead of 2 or 4)
        var handler = new RangeInterpolatedStringHandler<int>(5, 3, null);
        
        // The constructor should detect this and set error state
        // Try to get range
        try
        {
            var range = handler.GetRange();
            Assert.Fail("Should have thrown FormatException");
        }
        catch (FormatException ex)
        {
            // Assert
            Assert.Contains("Failed to parse range", ex.Message);
        }
    }

    [Fact]
    public void Parse_GetRangeBeforeComplete_ThrowsFormatException()
    {
        // Arrange
        var handler = new RangeInterpolatedStringHandler<int>(5, 4, null);
        handler.AppendFormatted('[');
        handler.AppendFormatted(10);

        // Act & Assert - Try to get range before parsing is complete
        try
        {
            var range = handler.GetRange();
            Assert.Fail("Should have thrown FormatException");
        }
        catch (FormatException ex)
        {
            Assert.Contains("Incomplete", ex.Message);
        }
    }

    [Fact]
    public void TryGetRange_BeforeComplete_ReturnsFalse()
    {
        // Arrange
        var handler = new RangeInterpolatedStringHandler<int>(5, 4, null);
        handler.AppendFormatted('[');
        handler.AppendFormatted(10);

        // Act
        var result = handler.TryGetRange(out var range);

        // Assert
        Assert.False(result);
        Assert.Equal(default, range);
    }

    [Fact]
    public void Parse_WithInvalidStateTransition_HandlesGracefully()
    {
        // Arrange
        var handler = new RangeInterpolatedStringHandler<int>(5, 4, null);
        handler.AppendFormatted('[');
        
        // Act - Try to append bracket when value is expected
        var result = handler.AppendFormatted(']');

        // Assert - Should set error state
        Assert.False(result);
        
        try
        {
            var range = handler.GetRange();
            Assert.Fail("Should have thrown FormatException");
        }
        catch (FormatException)
        {
            // Expected
        }
    }

    #endregion

    #region Literal Processing Edge Cases

    [Fact]
    public void Parse_WithCharBracketsNoLiterals_ParsesCorrectly()
    {
        // Arrange - Brackets come as char interpolations, not string literals
        char openBracket = '[';
        char closeBracket = ']';
        int start = 15;
        int end = 25;

        // Act - This path should have empty literals, brackets as chars
        var range = RangeInterpolatedStringParser.Parse<int>($"{openBracket}{start}, {end}{closeBracket}");

        // Assert
        Assert.Equal(15, range.Start.Value);
        Assert.Equal(25, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void Parse_WithAllCharsNoBracketLiterals_ParsesCorrectly()
    {
        // Arrange - All brackets and comma as interpolations, testing empty literal paths
        char openBracket = '(';
        int start = 5;
        int end = 15;
        char closeBracket = ')';

        // Act - This exercises the empty literal code paths
        var range = RangeInterpolatedStringParser.Parse<int>($"{openBracket}{start}, {end}{closeBracket}");

        // Assert
        Assert.Equal(5, range.Start.Value);
        Assert.Equal(15, range.End.Value);
        Assert.False(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void Parse_WithInvalidCommaFormat_ThrowsFormatException()
    {
        // Arrange - Create handler manually with invalid comma literal
        var handler = new RangeInterpolatedStringHandler<int>(10, 4, null);
        handler.AppendFormatted('[');
        handler.AppendFormatted(10);
        
        // Act - Append literal that doesn't have comma after trim
        var result = handler.AppendLiteral("  invalid  "); // No comma
        handler.AppendFormatted(20);
        handler.AppendFormatted(']');

        // Assert - Should have set error state
        Assert.False(result);
        
        try
        {
            _ = handler.GetRange();
            Assert.Fail("Should have thrown FormatException");
        }
        catch (FormatException)
        {
            // Expected
        }
    }

    [Fact]
    public void TryParse_WithInvalidCommaFormat_ReturnsFalse()
    {
        // Arrange - Create handler with invalid comma literal
        var handler = new RangeInterpolatedStringHandler<int>(10, 4, null);
        handler.AppendFormatted('[');
        handler.AppendFormatted(10);
        handler.AppendLiteral("  no-comma  ");
        handler.AppendFormatted(20);
        handler.AppendFormatted(']');

        // Act
        var result = handler.TryGetRange(out var range);

        // Assert
        Assert.False(result);
        Assert.Equal(default, range);
    }

    [Fact]
    public void Parse_WithExtraWhitespaceLiterals_ParsesCorrectly()
    {
        // Arrange - Testing whitespace handling in literals
        int start = 10;
        int end = 20;
        
        // Act - Literals with extra whitespace
        var range = RangeInterpolatedStringParser.Parse<int>($"  [  {start}  ,  {end}  ]  ");

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    #endregion
}