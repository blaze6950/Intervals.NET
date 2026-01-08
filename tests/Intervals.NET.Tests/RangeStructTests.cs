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

    #region Operator & (Intersection) Tests

    [Fact]
    public void OperatorAnd_WithOverlappingRanges_ReturnsIntersection()
    {
        // Arrange
        var range1 = new Range<int>(new RangeValue<int>(10), new RangeValue<int>(30), true, false);
        var range2 = new Range<int>(new RangeValue<int>(20), new RangeValue<int>(40), true, false);

        // Act
        var result = range1 & range2;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(20, result.Value.Start.Value);
        Assert.Equal(30, result.Value.End.Value);
    }

    [Fact]
    public void OperatorAnd_WithNonOverlappingRanges_ReturnsNull()
    {
        // Arrange
        var range1 = new Range<int>(new RangeValue<int>(10), new RangeValue<int>(20), true, false);
        var range2 = new Range<int>(new RangeValue<int>(30), new RangeValue<int>(40), true, false);

        // Act
        var result = range1 & range2;

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void OperatorAnd_WithIdenticalRanges_ReturnsSameRange()
    {
        // Arrange
        var range1 = new Range<int>(new RangeValue<int>(10), new RangeValue<int>(20), true, false);
        var range2 = new Range<int>(new RangeValue<int>(10), new RangeValue<int>(20), true, false);

        // Act
        var result = range1 & range2;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Value.Start.Value);
        Assert.Equal(20, result.Value.End.Value);
        Assert.True(result.Value.IsStartInclusive);
        Assert.False(result.Value.IsEndInclusive);
    }

    [Fact]
    public void OperatorAnd_WithOneRangeContainingAnother_ReturnsSmallerRange()
    {
        // Arrange
        var range1 = new Range<int>(new RangeValue<int>(10), new RangeValue<int>(50), true, true);
        var range2 = new Range<int>(new RangeValue<int>(20), new RangeValue<int>(30), true, true);

        // Act
        var result = range1 & range2;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(20, result.Value.Start.Value);
        Assert.Equal(30, result.Value.End.Value);
    }

    #endregion

    #region Operator | (Union) Tests

    [Fact]
    public void OperatorOr_WithOverlappingRanges_ReturnsUnion()
    {
        // Arrange
        var range1 = new Range<int>(new RangeValue<int>(10), new RangeValue<int>(30), true, false);
        var range2 = new Range<int>(new RangeValue<int>(20), new RangeValue<int>(40), true, false);

        // Act
        var result = range1 | range2;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Value.Start.Value);
        Assert.Equal(40, result.Value.End.Value);
    }

    [Fact]
    public void OperatorOr_WithNonOverlappingNonAdjacentRanges_ReturnsNull()
    {
        // Arrange
        var range1 = new Range<int>(new RangeValue<int>(10), new RangeValue<int>(20), true, false);
        var range2 = new Range<int>(new RangeValue<int>(30), new RangeValue<int>(40), true, false);

        // Act
        var result = range1 | range2;

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void OperatorOr_WithAdjacentRanges_ReturnsUnion()
    {
        // Arrange
        var range1 = new Range<int>(new RangeValue<int>(10), new RangeValue<int>(20), true, false);
        var range2 = new Range<int>(new RangeValue<int>(20), new RangeValue<int>(30), true, false);

        // Act
        var result = range1 | range2;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Value.Start.Value);
        Assert.Equal(30, result.Value.End.Value);
    }

    [Fact]
    public void OperatorOr_WithIdenticalRanges_ReturnsSameRange()
    {
        // Arrange
        var range1 = new Range<int>(new RangeValue<int>(10), new RangeValue<int>(20), true, false);
        var range2 = new Range<int>(new RangeValue<int>(10), new RangeValue<int>(20), true, false);

        // Act
        var result = range1 | range2;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Value.Start.Value);
        Assert.Equal(20, result.Value.End.Value);
    }

    #endregion

    #region Record Struct Tests

    [Fact]
    public void RecordStruct_EqualityWithSameValues_ReturnsTrue()
    {
        // Arrange
        var range1 = new Range<int>(new RangeValue<int>(10), new RangeValue<int>(20), true, false);
        var range2 = new Range<int>(new RangeValue<int>(10), new RangeValue<int>(20), true, false);

        // Act
        var result = range1.Equals(range2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void RecordStruct_EqualityWithDifferentStart_ReturnsFalse()
    {
        // Arrange
        var range1 = new Range<int>(new RangeValue<int>(10), new RangeValue<int>(20), true, false);
        var range2 = new Range<int>(new RangeValue<int>(15), new RangeValue<int>(20), true, false);

        // Act
        var result = range1.Equals(range2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void RecordStruct_EqualityWithDifferentEnd_ReturnsFalse()
    {
        // Arrange
        var range1 = new Range<int>(new RangeValue<int>(10), new RangeValue<int>(20), true, false);
        var range2 = new Range<int>(new RangeValue<int>(10), new RangeValue<int>(25), true, false);

        // Act
        var result = range1.Equals(range2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void RecordStruct_EqualityWithDifferentIsStartInclusive_ReturnsFalse()
    {
        // Arrange
        var range1 = new Range<int>(new RangeValue<int>(10), new RangeValue<int>(20), true, false);
        var range2 = new Range<int>(new RangeValue<int>(10), new RangeValue<int>(20), false, false);

        // Act
        var result = range1.Equals(range2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void RecordStruct_EqualityWithDifferentIsEndInclusive_ReturnsFalse()
    {
        // Arrange
        var range1 = new Range<int>(new RangeValue<int>(10), new RangeValue<int>(20), true, false);
        var range2 = new Range<int>(new RangeValue<int>(10), new RangeValue<int>(20), true, true);

        // Act
        var result = range1.Equals(range2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void RecordStruct_OperatorEquals_WorksCorrectly()
    {
        // Arrange
        var range1 = new Range<int>(new RangeValue<int>(10), new RangeValue<int>(20), true, false);
        var range2 = new Range<int>(new RangeValue<int>(10), new RangeValue<int>(20), true, false);

        // Act
        var result = range1 == range2;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void RecordStruct_OperatorNotEquals_WorksCorrectly()
    {
        // Arrange
        var range1 = new Range<int>(new RangeValue<int>(10), new RangeValue<int>(20), true, false);
        var range2 = new Range<int>(new RangeValue<int>(15), new RangeValue<int>(20), true, false);

        // Act
        var result = range1 != range2;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void RecordStruct_GetHashCode_EqualRangesHaveSameHashCode()
    {
        // Arrange
        var range1 = new Range<int>(new RangeValue<int>(10), new RangeValue<int>(20), true, false);
        var range2 = new Range<int>(new RangeValue<int>(10), new RangeValue<int>(20), true, false);

        // Act
        var hash1 = range1.GetHashCode();
        var hash2 = range2.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void RecordStruct_GetHashCode_DifferentRangesHaveDifferentHashCodes()
    {
        // Arrange
        var range1 = new Range<int>(new RangeValue<int>(10), new RangeValue<int>(20), true, false);
        var range2 = new Range<int>(new RangeValue<int>(15), new RangeValue<int>(25), true, false);

        // Act
        var hash1 = range1.GetHashCode();
        var hash2 = range2.GetHashCode();

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    #endregion

    #region Edge Cases and Different Types

    [Fact]
    public void Range_WithDoubleType_WorksCorrectly()
    {
        // Arrange & Act
        var range = new Range<double>(new RangeValue<double>(1.5), new RangeValue<double>(9.5), true, false);

        // Assert
        Assert.Equal(1.5, range.Start.Value);
        Assert.Equal(9.5, range.End.Value);
    }

    [Fact]
    public void Range_WithStringType_WorksCorrectly()
    {
        // Arrange & Act
        var range = new Range<string>(new RangeValue<string>("a"), new RangeValue<string>("z"), true, true);

        // Assert
        Assert.Equal("a", range.Start.Value);
        Assert.Equal("z", range.End.Value);
    }

    [Fact]
    public void Range_WithDateTimeType_WorksCorrectly()
    {
        // Arrange
        var start = new DateTime(2020, 1, 1);
        var end = new DateTime(2020, 12, 31);

        // Act
        var range = new Range<DateTime>(new RangeValue<DateTime>(start), new RangeValue<DateTime>(end));

        // Assert
        Assert.Equal(start, range.Start.Value);
        Assert.Equal(end, range.End.Value);
    }

    [Fact]
    public void Range_WithNegativeNumbers_WorksCorrectly()
    {
        // Arrange & Act
        var range = new Range<int>(new RangeValue<int>(-100), new RangeValue<int>(-10), true, true);

        // Assert
        Assert.Equal(-100, range.Start.Value);
        Assert.Equal(-10, range.End.Value);
    }

    [Fact]
    public void Range_WithZeroValues_WorksCorrectly()
    {
        // Arrange & Act
        var range = new Range<int>(new RangeValue<int>(0), new RangeValue<int>(0), true, true);

        // Assert
        Assert.Equal(0, range.Start.Value);
        Assert.Equal(0, range.End.Value);
    }

    [Fact]
    public void Range_WithLargeNumbers_WorksCorrectly()
    {
        // Arrange & Act
        var range = new Range<int>(new RangeValue<int>(int.MinValue), new RangeValue<int>(int.MaxValue), true, true);

        // Assert
        Assert.Equal(int.MinValue, range.Start.Value);
        Assert.Equal(int.MaxValue, range.End.Value);
    }

    [Fact]
    public void Range_ImplicitConversionInConstructor_WorksCorrectly()
    {
        // Arrange
        RangeValue<int> start = 10; // Implicit conversion
        RangeValue<int> end = 20; // Implicit conversion

        // Act
        var range = new Range<int>(start, end);

        // Assert
        Assert.Equal(10, range.Start.Value);
        Assert.Equal(20, range.End.Value);
    }

    #endregion
}