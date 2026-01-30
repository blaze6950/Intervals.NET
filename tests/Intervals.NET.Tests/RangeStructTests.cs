namespace Intervals.NET.Tests;

public class RangeStructTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidFiniteValues_CreatesRange()
    {
        // Arrange
        var start = new RangeValue<int>(10);
        var end = new RangeValue<int>(20);

        // Act
        var range = new Range<int>(start, end, true, false);

        // Assert
        var s = (int)range.Start;
        Assert.Equal(start, range.Start);
        Assert.Equal(end, range.End);
        Assert.True(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void Constructor_WithStartGreaterThanEnd_ThrowsArgumentException()
    {
        // Arrange
        var start = new RangeValue<int>(20);
        var end = new RangeValue<int>(10);

        // Act
        var exception = Record.Exception(() => new Range<int>(start, end));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        var argException = (ArgumentException)exception;
        Assert.Contains("Start value cannot be greater than end value", argException.Message);
        Assert.Equal("start", argException.ParamName);
    }

    [Fact]
    public void Constructor_WithEqualValuesAndBothExclusive_ThrowsArgumentException()
    {
        // Arrange
        var start = new RangeValue<int>(10);
        var end = new RangeValue<int>(10);

        // Act
        var exception = Record.Exception(() => new Range<int>(start, end, false, false));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        var argException = (ArgumentException)exception;
        Assert.Contains("at least one bound must be inclusive", argException.Message);
        Assert.Equal("start", argException.ParamName);
    }

    [Fact]
    public void Constructor_WithEqualValuesAndStartInclusive_CreatesRange()
    {
        // Arrange
        var start = new RangeValue<int>(10);
        var end = new RangeValue<int>(10);

        // Act
        var range = new Range<int>(start, end, true, false);

        // Assert
        Assert.Equal(start, range.Start);
        Assert.Equal(end, range.End);
    }

    [Fact]
    public void Constructor_WithEqualValuesAndEndInclusive_CreatesRange()
    {
        // Arrange
        var start = new RangeValue<int>(10);
        var end = new RangeValue<int>(10);

        // Act
        var range = new Range<int>(start, end, false, true);

        // Assert
        Assert.Equal(start, range.Start);
        Assert.Equal(end, range.End);
    }

    [Fact]
    public void Constructor_WithEqualValuesAndBothInclusive_CreatesRange()
    {
        // Arrange
        var start = new RangeValue<int>(10);
        var end = new RangeValue<int>(10);

        // Act
        var range = new Range<int>(start, end, true, true);

        // Assert
        Assert.Equal(start, range.Start);
        Assert.Equal(end, range.End);
    }

    [Fact]
    public void Constructor_WithNegativeInfinityStart_CreatesRange()
    {
        // Arrange
        var start = RangeValue<int>.NegativeInfinity;
        var end = new RangeValue<int>(10);

        // Act
        var range = new Range<int>(start, end);

        // Assert
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.Equal(end, range.End);
    }

    [Fact]
    public void Constructor_WithPositiveInfinityEnd_CreatesRange()
    {
        // Arrange
        var start = new RangeValue<int>(10);
        var end = RangeValue<int>.PositiveInfinity;

        // Act
        var range = new Range<int>(start, end);

        // Assert
        Assert.Equal(start, range.Start);
        Assert.True(range.End.IsPositiveInfinity);
    }

    [Fact]
    public void Constructor_WithBothInfinities_CreatesRange()
    {
        // Arrange
        var start = RangeValue<int>.NegativeInfinity;
        var end = RangeValue<int>.PositiveInfinity;

        // Act
        var range = new Range<int>(start, end);

        // Assert
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.True(range.End.IsPositiveInfinity);
    }

    [Fact]
    public void Constructor_WithDefaultParameters_UsesStartInclusiveEndExclusive()
    {
        // Arrange
        var start = new RangeValue<int>(10);
        var end = new RangeValue<int>(20);

        // Act
        var range = new Range<int>(start, end);

        // Assert
        Assert.True(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void Constructor_WithInfinityStartAndFiniteEnd_DoesNotValidateOrder()
    {
        // Arrange
        var start = RangeValue<int>.NegativeInfinity;
        var end = new RangeValue<int>(10);

        // Act
        var range = new Range<int>(start, end);

        // Assert - Should not throw
        Assert.True(range.Start.IsNegativeInfinity);
        Assert.True(range.End.IsFinite);
    }

    [Fact]
    public void Constructor_WithFiniteStartAndInfinityEnd_DoesNotValidateOrder()
    {
        // Arrange
        var start = new RangeValue<int>(10);
        var end = RangeValue<int>.PositiveInfinity;

        // Act
        var range = new Range<int>(start, end);

        // Assert - Should not throw
        Assert.True(range.Start.IsFinite);
        Assert.True(range.End.IsPositiveInfinity);
    }

    [Fact]
    public void Constructor_WithPositiveInfinityStartAndNegativeInfinityEnd_DoesNotValidate()
    {
        // Arrange - This is logically wrong but not validated when infinities are involved
        var start = RangeValue<int>.PositiveInfinity;
        var end = RangeValue<int>.NegativeInfinity;

        // Act
        var range = new Range<int>(start, end);

        // Assert - Constructor doesn't validate infinity order
        Assert.True(range.Start.IsPositiveInfinity);
        Assert.True(range.End.IsNegativeInfinity);
    }

    #endregion

    #region Internal Constructor with skipValidation Tests

    [Fact]
    public void Constructor_WithSkipValidation_DoesNotValidateOrder()
    {
        // Arrange
        var start = new RangeValue<int>(20);
        var end = new RangeValue<int>(10);

        // Act - Using internal constructor that skips validation
        var range = new Range<int>(start, end, true, false, skipValidation: true);

        // Assert - Should not throw even though start > end
        Assert.Equal(20, range.Start.Value);
        Assert.Equal(10, range.End.Value);
    }

    [Fact]
    public void Constructor_WithSkipValidation_AllowsInvalidBothExclusive()
    {
        // Arrange
        var start = new RangeValue<int>(10);
        var end = new RangeValue<int>(10);

        // Act - Using internal constructor with skipValidation=true
        var range = new Range<int>(start, end, false, false, skipValidation: true);

        // Assert - Should not throw even with equal values and both exclusive
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(10, range.End.Value);
        Assert.False(range.IsStartInclusive);
        Assert.False(range.IsEndInclusive);
    }

    [Fact]
    public void Constructor_WithSkipValidation_PreservesAllProperties()
    {
        // Arrange
        var start = new RangeValue<int>(5);
        var end = new RangeValue<int>(15);

        // Act
        var range = new Range<int>(start, end, true, true, skipValidation: true);

        // Assert
        Assert.Equal(5, range.Start.Value);
        Assert.Equal(15, range.End.Value);
        Assert.True(range.IsStartInclusive);
        Assert.True(range.IsEndInclusive);
    }

    [Fact]
    public void Constructor_WithSkipValidation_WorksWithInfinities()
    {
        // Arrange
        var start = RangeValue<int>.PositiveInfinity;
        var end = RangeValue<int>.NegativeInfinity;

        // Act - This is logically invalid but should not throw with skipValidation
        var range = new Range<int>(start, end, true, false, skipValidation: true);

        // Assert
        Assert.True(range.Start.IsPositiveInfinity);
        Assert.True(range.End.IsNegativeInfinity);
    }

    #endregion

    #region Property Tests

    [Fact]
    public void Start_ReturnsCorrectValue()
    {
        // Arrange
        var start = new RangeValue<int>(10);
        var end = new RangeValue<int>(20);
        var range = new Range<int>(start, end);

        // Act
        var result = range.Start;

        // Assert
        Assert.Equal(start, result);
        Assert.Equal(10, result.Value);
    }

    [Fact]
    public void End_ReturnsCorrectValue()
    {
        // Arrange
        var start = new RangeValue<int>(10);
        var end = new RangeValue<int>(20);
        var range = new Range<int>(start, end);

        // Act
        var result = range.End;

        // Assert
        Assert.Equal(end, result);
        Assert.Equal(20, result.Value);
    }

    [Fact]
    public void IsStartInclusive_ReturnsCorrectValue()
    {
        // Arrange
        var start = new RangeValue<int>(10);
        var end = new RangeValue<int>(20);
        var range = new Range<int>(start, end, true, false);

        // Act
        var result = range.IsStartInclusive;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsEndInclusive_ReturnsCorrectValue()
    {
        // Arrange
        var start = new RangeValue<int>(10);
        var end = new RangeValue<int>(20);
        var range = new Range<int>(start, end, false, true);

        // Act
        var result = range.IsEndInclusive;

        // Assert
        Assert.True(result);
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_WithStartInclusiveEndExclusive_ReturnsCorrectFormat()
    {
        // Arrange
        var start = new RangeValue<int>(10);
        var end = new RangeValue<int>(20);
        var range = new Range<int>(start, end, true, false);

        // Act
        var result = range.ToString();

        // Assert
        Assert.Equal("[10, 20)", result);
    }

    [Fact]
    public void ToString_WithStartExclusiveEndInclusive_ReturnsCorrectFormat()
    {
        // Arrange
        var start = new RangeValue<int>(10);
        var end = new RangeValue<int>(20);
        var range = new Range<int>(start, end, false, true);

        // Act
        var result = range.ToString();

        // Assert
        Assert.Equal("(10, 20]", result);
    }

    [Fact]
    public void ToString_WithBothInclusive_ReturnsCorrectFormat()
    {
        // Arrange
        var start = new RangeValue<int>(10);
        var end = new RangeValue<int>(20);
        var range = new Range<int>(start, end, true, true);

        // Act
        var result = range.ToString();

        // Assert
        Assert.Equal("[10, 20]", result);
    }

    [Fact]
    public void ToString_WithBothExclusive_ReturnsCorrectFormat()
    {
        // Arrange
        var start = new RangeValue<int>(10);
        var end = new RangeValue<int>(30);
        var range = new Range<int>(start, end, false, false);

        // Act
        var result = range.ToString();

        // Assert
        Assert.Equal("(10, 30)", result);
    }

    [Fact]
    public void ToString_WithNegativeInfinityStart_ReturnsCorrectFormat()
    {
        // Arrange
        var start = RangeValue<int>.NegativeInfinity;
        var end = new RangeValue<int>(10);
        var range = new Range<int>(start, end, true, false);

        // Act
        var result = range.ToString();

        // Assert
        Assert.Equal("[-∞, 10)", result);
    }

    [Fact]
    public void ToString_WithPositiveInfinityEnd_ReturnsCorrectFormat()
    {
        // Arrange
        var start = new RangeValue<int>(10);
        var end = RangeValue<int>.PositiveInfinity;
        var range = new Range<int>(start, end, true, false);

        // Act
        var result = range.ToString();

        // Assert
        Assert.Equal("[10, ∞)", result);
    }

    [Fact]
    public void ToString_WithBothInfinities_ReturnsCorrectFormat()
    {
        // Arrange
        var start = RangeValue<int>.NegativeInfinity;
        var end = RangeValue<int>.PositiveInfinity;
        var range = new Range<int>(start, end, false, false);

        // Act
        var result = range.ToString();

        // Assert
        Assert.Equal("(-∞, ∞)", result);
    }

    [Fact]
    public void ToString_WithStringType_ReturnsCorrectFormat()
    {
        // Arrange
        var start = new RangeValue<string>("alpha");
        var end = new RangeValue<string>("omega");
        var range = new Range<string>(start, end, true, true);

        // Act
        var result = range.ToString();

        // Assert
        Assert.Equal("[alpha, omega]", result);
    }

    [Fact]
    public void ToString_WithDateTimeType_ReturnsCorrectFormat()
    {
        // Arrange
        var startDate = new DateTime(2020, 1, 1);
        var endDate = new DateTime(2020, 12, 31);
        var start = new RangeValue<DateTime>(startDate);
        var end = new RangeValue<DateTime>(endDate);
        var range = new Range<DateTime>(start, end, true, false);

        // Act
        var result = range.ToString();

        // Assert
        var expected = $"[{startDate}, {endDate})";
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToString_WithNegativeNumbers_ReturnsCorrectFormat()
    {
        // Arrange
        var start = new RangeValue<int>(-100);
        var end = new RangeValue<int>(-10);
        var range = new Range<int>(start, end, true, true);

        // Act
        var result = range.ToString();

        // Assert
        Assert.Equal("[-100, -10]", result);
    }

    #endregion

    #region Nullable Reference Type ToString Tests

    [Fact]
    public void Range_NullableString_WithNullValues_ToStringFormatsCorrectly()
    {
        // Arrange
        string? nullStart = null;
        string? nullEnd = null;
        var range = new Range<string?>(nullStart, nullEnd, true, true);

        // Act
        var result = range.ToString();

        // Assert - Should not throw, handles null correctly
        Assert.NotNull(result);
        Assert.Contains("[", result);
        Assert.Contains(",", result);
        Assert.Contains("]", result);
    }

    [Fact]
    public void Range_String_WithEmptyStringBoundaries_ToStringFormatsCorrectly()
    {
        // Arrange
        string emptyStart = "";
        string emptyEnd = "";
        var range = new Range<string>(emptyStart, emptyEnd, true, true);

        // Act
        var result = range.ToString();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("[, ]", result); // Empty strings show as nothing between brackets
    }

    #endregion
}