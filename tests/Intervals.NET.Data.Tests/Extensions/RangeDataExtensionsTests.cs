using Intervals.NET.Data.Extensions;
using Intervals.NET.Domain.Default.Numeric;
using Range = Intervals.NET.Factories.Range;

namespace Intervals.NET.Data.Tests.Extensions;

/// <summary>
/// Tests for RangeData extension methods.
/// </summary>
public class RangeDataExtensionsTests
{
    private readonly IntegerFixedStepDomain _domain = new();
    private readonly IntegerFixedStepDomain _differentDomain = new();

    #region Intersect Tests

    [Fact]
    public void Intersect_WithOverlappingRanges_ReturnsIntersection()
    {
        // Arrange
        var data1 = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }; // [10, 20]
        var data2 = new[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 }; // [20, 30]

        var rd1 = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data1, _domain);
        var rd2 = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(20, 30), data2, _domain);

        // Act
        var result = rd1.Intersect(rd2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(20, result.Range.Start.Value);
        Assert.Equal(20, result.Range.End.Value);
        Assert.Single(result.Data); // Only element at index 10 (value 20)
        Assert.Equal(11, result.Data.First());
    }

    [Fact]
    public void Intersect_WithNonOverlappingRanges_ReturnsNull()
    {
        // Arrange
        var data1 = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }; // [10, 20]
        var data2 = new[] { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 }; // [30, 40]

        var rd1 = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data1, _domain);
        var rd2 = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(30, 40), data2, _domain);

        // Act
        var result = rd1.Intersect(rd2);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Intersect_WithDifferentStatelessStructDomains_NoException()
    {
        // Arrange
        var data1 = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
        var data2 = new[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 };

        var rd1 = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data1, _domain);
        var rd2 = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(20, 30), data2, _differentDomain);

        // Act
        var ex = Record.Exception(() => rd1.Intersect(rd2));

        // Assert
        Assert.Null(ex);
    }

    [Fact]
    public void Intersect_WithOneRangeContainedInAnother_ReturnsSmaller()
    {
        // Arrange
        var data1 = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 }; // [10, 30]
        var data2 = new[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 }; // [15, 25]

        var rd1 = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 30), data1, _domain);
        var rd2 = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(15, 25), data2, _domain);

        // Act
        var result = rd1.Intersect(rd2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(15, result.Range.Start.Value);
        Assert.Equal(25, result.Range.End.Value);
        Assert.Equal(11, result.Data.Count());
    }

    #endregion

    #region Union Tests

    [Fact]
    public void Union_WithAdjacentRanges_ReturnsUnion()
    {
        // Arrange
        var data1 = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }; // [10, 20]
        var data2 = new[] { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 }; // (20, 30]

        var rd1 = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data1, _domain);
        var rd2 = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.OpenClosed<int>(20, 30), data2, _domain);

        // Act
        var result = rd1.Union(rd2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Range.Start.Value);
        Assert.Equal(30, result.Range.End.Value);
        Assert.Equal(22, result.Data.Count()); // 11 + 11 elements
    }

    [Fact]
    public void Union_WithOverlappingRanges_ReturnsUnionButWithRightValuesForTheIntersection()
    {
        // Arrange
        var data1 = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }; // [10, 20]
        var data2 = new[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 }; // [20, 30]

        var rd1 = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data1, _domain);
        var rd2 = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(20, 30), data2, _domain);

        // Act
        var result = rd1.Union(rd2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Range.Start.Value);
        Assert.Equal(30, result.Range.End.Value);
        Assert.Equal(21, result.Data.Count()); // One intersected point is right-bias merged
    }

    [Fact]
    public void Union_WithDisjointRanges_ReturnsNull()
    {
        // Arrange
        var data1 = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }; // [10, 20]
        var data2 = new[] { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 }; // [30, 40]

        var rd1 = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data1, _domain);
        var rd2 = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(30, 40), data2, _domain);

        // Act
        var result = rd1.Union(rd2);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Union_WithDifferentStructDomainsWithoutState_TreatedAsSingletonNoException()
    {
        // Arrange
        var data1 = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
        var data2 = new[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 };

        var rd1 = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data1, _domain);
        var rd2 = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(20, 30), data2, _differentDomain);

        // Act
        var ex = Record.Exception(() => rd1.Union(rd2));

        // Assert
        Assert.Null(ex);
    }

    [Fact]
    public void Union_IntersectionAtDiscretePoint_RightValueTakesPreferenceWithinIntersection()
    {
        // Arrange
        var data1 = new[] { 1, 2, 3 }; // [10, 12]
        var data2 = new[] { 4, 5, 6 }; // [12, 14]

        var rd1 = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 12), data1, _domain);
        var rd2 = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(12, 14), data2, _domain);

        // Act
        var result = rd1.Union(rd2);

        // Assert
        Assert.NotNull(result);
        var resultData = result.Data.ToList();
        Assert.Equal(new[] { 1, 2, 4, 5, 6 }, resultData);
    }

    #endregion

    #region TrimStart Tests

    [Fact]
    public void TrimStart_WithValidNewStart_ReturnsTrimmedRange()
    {
        // Arrange
        var data = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }; // [10, 20]
        var rd = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data, _domain);

        // Act
        var result = rd.TrimStart(15);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(15, result.Range.Start.Value);
        Assert.Equal(20, result.Range.End.Value);
        Assert.Equal(6, result.Data.Count()); // [15, 20]
    }

    [Fact]
    public void TrimStart_WithStartBeyondEnd_ReturnsNull()
    {
        // Arrange
        var data = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }; // [10, 20]
        var rd = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data, _domain);

        // Act
        var result = rd.TrimStart(30);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void TrimStart_WithStartAtEnd_ReturnsOneElement()
    {
        // Arrange
        var data = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }; // [10, 20]
        var rd = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data, _domain);

        // Act
        var result = rd.TrimStart(20);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(20, result.Range.Start.Value);
        Assert.Equal(20, result.Range.End.Value);
        Assert.Single(result.Data);
    }

    #endregion

    #region TrimEnd Tests

    [Fact]
    public void TrimEnd_WithValidNewEnd_ReturnsTrimmedRange()
    {
        // Arrange
        var data = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }; // [10, 20]
        var rd = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data, _domain);

        // Act
        var result = rd.TrimEnd(15);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Range.Start.Value);
        Assert.Equal(15, result.Range.End.Value);
        Assert.Equal(6, result.Data.Count()); // [10, 15]
    }

    [Fact]
    public void TrimEnd_WithEndBeforeStart_ReturnsNull()
    {
        // Arrange
        var data = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }; // [10, 20]
        var rd = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data, _domain);

        // Act
        var result = rd.TrimEnd(5);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void TrimEnd_WithEndAtStart_ReturnsOneElement()
    {
        // Arrange
        var data = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }; // [10, 20]
        var rd = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data, _domain);

        // Act
        var result = rd.TrimEnd(10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Range.Start.Value);
        Assert.Equal(10, result.Range.End.Value);
        Assert.Single(result.Data);
    }

    #endregion

    #region Contains Tests

    [Fact]
    public void Contains_Value_WithValueInRange_ReturnsTrue()
    {
        // Arrange
        var data = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }; // [10, 20]
        var rd = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data, _domain);

        // Act & Assert
        Assert.True(rd.Contains(10));
        Assert.True(rd.Contains(15));
        Assert.True(rd.Contains(20));
    }

    [Fact]
    public void Contains_Value_WithValueOutsideRange_ReturnsFalse()
    {
        // Arrange
        var data = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }; // [10, 20]
        var rd = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data, _domain);

        // Act & Assert
        Assert.False(rd.Contains(5));
        Assert.False(rd.Contains(25));
    }

    [Fact]
    public void Contains_Range_WithContainedRange_ReturnsTrue()
    {
        // Arrange
        var data = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }; // [10, 20]
        var rd = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data, _domain);

        // Act & Assert
        Assert.True(rd.Contains(Range.Closed<int>(12, 18)));
        Assert.True(rd.Contains(Range.Closed<int>(10, 20)));
    }

    [Fact]
    public void Contains_Range_WithNonContainedRange_ReturnsFalse()
    {
        // Arrange
        var data = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }; // [10, 20]
        var rd = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data, _domain);

        // Act & Assert
        Assert.False(rd.Contains(Range.Closed<int>(15, 25)));
        Assert.False(rd.Contains(Range.Closed<int>(5, 15)));
    }

    #endregion

    #region IsTouching Tests

    [Fact]
    public void IsTouching_WithAdjacentRanges_ReturnsTrue()
    {
        // Arrange
        var data1 = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }; // [10, 20]
        var data2 = new[] { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 }; // (20, 30]

        var rd1 = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data1, _domain);
        var rd2 = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.OpenClosed<int>(20, 30), data2, _domain);

        // Act & Assert
        Assert.True(rd1.IsTouching(rd2));
    }

    [Fact]
    public void IsTouching_WithOverlappingRanges_ReturnsTrue()
    {
        // Arrange
        var data1 = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }; // [10, 20]
        var data2 = new[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 }; // [20, 30]

        var rd1 = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data1, _domain);
        var rd2 = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(20, 30), data2, _domain);

        // Act & Assert
        Assert.True(rd1.IsTouching(rd2));
    }

    [Fact]
    public void IsTouching_WithDisjointRanges_ReturnsFalse()
    {
        // Arrange
        var data1 = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }; // [10, 20]
        var data2 = new[] { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 }; // [30, 40]

        var rd1 = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data1, _domain);
        var rd2 = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(30, 40), data2, _domain);

        // Act & Assert
        Assert.False(rd1.IsTouching(rd2));
    }

    #endregion
}