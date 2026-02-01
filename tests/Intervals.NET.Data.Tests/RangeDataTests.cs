using Intervals.NET.Data.Tests.Helpers;
using Intervals.NET.Domain.Default.Numeric;
using Range = Intervals.NET.Factories.Range;

namespace Intervals.NET.Data.Tests;

/// <summary>
/// Tests for the RangeData class, focusing on correct handling of range inclusiveness.
/// </summary>
public class RangeDataTests
{
    private readonly IntegerFixedStepDomain _domain = new();

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidFiniteRange_CreatesInstance()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        // Act
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Assert
        Assert.NotNull(rangeData);
        Assert.Equal(range, rangeData.Range);
        Assert.Equal(data, rangeData.Data);
    }

    [Fact]
    public void Constructor_WithInfiniteRange_ThrowsArgumentException()
    {
        // Arrange
        var range = Range.Create(RangeValue<int>.NegativeInfinity, new RangeValue<int>(10), true, true);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain));
    }

    [Fact]
    public void Constructor_WithNullData_ThrowsArgumentNullException()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new RangeData<int, int, IntegerFixedStepDomain>(range, null!, _domain));
    }

    #endregion

    #region Point Indexer Tests

    [Fact]
    public void PointIndexer_WithValidPoint_ReturnsCorrectData()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);
        var data = new[] { "d0", "d1", "d2", "d3", "d4", "d5", "d6", "d7", "d8", "d9", "d10" };
        var rangeData = new RangeData<int, string, IntegerFixedStepDomain>(range, data, _domain);

        // Act
        var result = rangeData[5];

        // Assert
        Assert.Equal("d5", result);
    }

    [Fact]
    public void PointIndexer_WithOutOfBoundsPoint_ThrowsIndexOutOfRangeException()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);
        var data = new[] { "d0", "d1", "d2", "d3", "d4", "d5", "d6", "d7", "d8", "d9", "d10" };
        var rangeData = new RangeData<int, string, IntegerFixedStepDomain>(range, data, _domain);

        // Act & Assert
        Assert.Throws<IndexOutOfRangeException>(() => rangeData[15]);
    }

    #endregion

    #region Sub-Range Indexer Tests - Closed Ranges [start, end]

    [Fact]
    public void SubRangeIndexer_ClosedRange_ReturnsCorrectData()
    {
        // Arrange - [0, 10] with data for each point
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Act - Get [2, 5] → should return 4 elements: 2, 3, 4, 5
        var subRange = Range.Closed<int>(2, 5);
        var result = rangeData[subRange];

        // Assert
        Assert.NotNull(result);
        Assert.Equal(subRange, result.Range);
        Assert.Equal([2, 3, 4, 5], result.Data);
    }

    [Fact]
    public void SubRangeIndexer_ClosedRange_SinglePoint_ReturnsOneElement()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Act - Get [5, 5] → should return 1 element: 5
        var subRange = Range.Closed<int>(5, 5);
        var result = rangeData[subRange];

        // Assert
        Assert.NotNull(result);
        Assert.Equal(subRange, result.Range);
        Assert.Equal([5], result.Data);
    }

    #endregion

    #region Sub-Range Indexer Tests - Open Ranges (start, end)

    [Fact]
    public void SubRangeIndexer_OpenRange_ReturnsCorrectData()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Act - Get (2, 5) → should return 2 elements: 3, 4 (excludes both 2 and 5)
        var subRange = Range.Open(2, 5);
        var result = rangeData[subRange];

        // Assert
        Assert.NotNull(result);
        Assert.Equal(subRange, result.Range);
        Assert.Equal([3, 4], result.Data);
    }

    [Fact]
    public void SubRangeIndexer_OpenRange_EmptyRange_ReturnsEmpty()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Act - Get (2, 3) → should return 0 elements (no integers between 2 and 3 exclusively)
        var subRange = Range.Open(2, 3);
        var result = rangeData[subRange];

        // Assert
        Assert.NotNull(result);
        Assert.Equal(subRange, result.Range);
        Assert.Empty(result.Data);
    }

    [Fact]
    public void SubRangeIndexer_OpenRange_SinglePointRange_ThrowsArgumentException()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Act - Get (5, 5) → should throw because (5, 5) is invalid (start == end with both exclusive)
        var exception = Record.Exception(() => Range.Open(5, 5));

        // Assert - This should throw during range creation, not during indexer access
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
    }

    #endregion

    #region Sub-Range Indexer Tests - Half-Open Ranges [start, end) and (start, end]

    [Fact]
    public void SubRangeIndexer_ClosedOpenRange_ReturnsCorrectData()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Act - Get [2, 5) → should return 3 elements: 2, 3, 4 (includes 2, excludes 5)
        var subRange = Range.ClosedOpen(2, 5);
        var result = rangeData[subRange];

        // Assert
        Assert.NotNull(result);
        Assert.Equal(subRange, result.Range);
        Assert.Equal([2, 3, 4], result.Data);
    }

    [Fact]
    public void SubRangeIndexer_OpenClosedRange_ReturnsCorrectData()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Act - Get (2, 5] → should return 3 elements: 3, 4, 5 (excludes 2, includes 5)
        var subRange = Range.OpenClosed<int>(2, 5);
        var result = rangeData[subRange];

        // Assert
        Assert.NotNull(result);
        Assert.Equal(subRange, result.Range);
        Assert.Equal([3, 4, 5], result.Data);
    }

    [Fact]
    public void SubRangeIndexer_ClosedOpenRange_SinglePoint_ReturnsEmpty()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Act - Get [5, 6) → should return 1 element: 5
        var subRange = Range.ClosedOpen(5, 6);
        var result = rangeData[subRange];

        // Assert
        Assert.NotNull(result);
        Assert.Equal(subRange, result.Range);
        Assert.Equal([5], result.Data);
    }

    [Fact]
    public void SubRangeIndexer_OpenClosedRange_SinglePoint_ReturnsOneElement()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Act - Get (4, 5] → should return 1 element: 5
        var subRange = Range.OpenClosed<int>(4, 5);
        var result = rangeData[subRange];

        // Assert
        Assert.NotNull(result);
        Assert.Equal(subRange, result.Range);
        Assert.Equal([5], result.Data);
    }

    #endregion

    #region TryGet Tests - Sub-Range

    [Fact]
    public void TryGet_SubRange_WithValidClosedRange_ReturnsTrue()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Act
        var subRange = Range.Closed<int>(2, 5);
        var success = rangeData.TryGet(subRange, out var result);

        // Assert
        Assert.True(success);
        Assert.NotNull(result);
        Assert.Equal([2, 3, 4, 5], result.Data);
    }

    [Fact]
    public void TryGet_SubRange_WithValidOpenRange_ReturnsTrue()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Act
        var subRange = Range.Open(2, 5);
        var success = rangeData.TryGet(subRange, out var result);

        // Assert
        Assert.True(success);
        Assert.NotNull(result);
        Assert.Equal([3, 4], result.Data);
    }

    [Fact]
    public void TryGet_SubRange_WithInfiniteRange_ReturnsFalse()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Act
        var subRange = Range.Create(RangeValue<int>.NegativeInfinity, new RangeValue<int>(5), true, true);
        var success = rangeData.TryGet(subRange, out var result);

        // Assert
        Assert.False(success);
        Assert.Null(result);
    }

    [Fact]
    public void TryGet_SubRange_WithEmptyRange_ReturnsTrueWithEmptyData()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Act - (2, 3) contains no integers
        var subRange = Range.Open(2, 3);
        var success = rangeData.TryGet(subRange, out var result);

        // Assert
        Assert.True(success);
        Assert.NotNull(result);
        Assert.Empty(result.Data);
    }

    #endregion

    #region TryGet Tests - Point

    [Fact]
    public void TryGet_Point_WithValidPoint_ReturnsTrue()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);
        var data = new[] { "d0", "d1", "d2", "d3", "d4", "d5", "d6", "d7", "d8", "d9", "d10" };
        var rangeData = new RangeData<int, string, IntegerFixedStepDomain>(range, data, _domain);

        // Act
        var success = rangeData.TryGet(5, out var result);

        // Assert
        Assert.True(success);
        Assert.Equal("d5", result);
    }

    [Fact]
    public void TryGet_Point_WithOutOfBoundsPoint_ReturnsFalse()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);
        var data = new[] { "d0", "d1", "d2", "d3", "d4", "d5", "d6", "d7", "d8", "d9", "d10" };
        var rangeData = new RangeData<int, string, IntegerFixedStepDomain>(range, data, _domain);

        // Act
        var success = rangeData.TryGet(15, out var result);

        // Assert
        Assert.False(success);
        Assert.Null(result);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void SubRangeIndexer_FullRange_ReturnsAllData()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Act - Get the full range [0, 10]
        var result = rangeData[range];

        // Assert
        Assert.NotNull(result);
        Assert.Equal(data, result.Data);
    }

    [Fact]
    public void SubRangeIndexer_AtBoundaries_ReturnsCorrectData()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Act - Get [0, 2]
        var subRange = Range.Closed<int>(0, 2);
        var result = rangeData[subRange];

        // Assert
        Assert.NotNull(result);
        Assert.Equal([0, 1, 2], result.Data);
    }

    [Fact]
    public void SubRangeIndexer_AtEndBoundaries_ReturnsCorrectData()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Act - Get [8, 10]
        var subRange = Range.Closed<int>(8, 10);
        var result = rangeData[subRange];

        // Assert
        Assert.NotNull(result);
        Assert.Equal([8, 9, 10], result.Data);
    }

    #endregion

    #region Complex Scenarios

    [Fact]
    public void SubRangeIndexer_MultipleInclusivenessVariations_AllReturnCorrectData()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Act & Assert - Test all four combinations for range [3, 6]

        // [3, 6] → 3, 4, 5, 6 (4 elements)
        var closed = rangeData[Range.Closed<int>(3, 6)];
        Assert.Equal([3, 4, 5, 6], closed.Data);

        // (3, 6) → 4, 5 (2 elements)
        var open = rangeData[Range.Open(3, 6)];
        Assert.Equal([4, 5], open.Data);

        // [3, 6) → 3, 4, 5 (3 elements)
        var closedOpen = rangeData[Range.ClosedOpen(3, 6)];
        Assert.Equal([3, 4, 5], closedOpen.Data);

        // (3, 6] → 4, 5, 6 (3 elements)
        var openClosed = rangeData[Range.OpenClosed<int>(3, 6)];
        Assert.Equal([4, 5, 6], openClosed.Data);
    }

    [Fact]
    public void SubRangeIndexer_WithNonZeroStartRange_ReturnsCorrectData()
    {
        // Arrange - Range starts at 5, not 0
        var range = Range.Closed<int>(5, 15);
        var data = new[] { 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Act - Get [7, 10]
        var subRange = Range.Closed<int>(7, 10);
        var result = rangeData[subRange];

        // Assert
        Assert.NotNull(result);
        Assert.Equal([7, 8, 9, 10], result.Data);
    }

    [Fact]
    public void SubRangeIndexer_WithNegativeRange_ReturnsCorrectData()
    {
        // Arrange
        var range = Range.Closed<int>(-5, 5);
        var data = new[] { -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Act - Get [-2, 2]
        var subRange = Range.Closed<int>(-2, 2);
        var result = rangeData[subRange];

        // Assert
        Assert.NotNull(result);
        Assert.Equal([-2, -1, 0, 1, 2], result.Data);
    }

    #endregion

    #region Overflow and Edge Case Tests

    [Fact]
    public void PointIndexer_WithInsufficientData_ThrowsIndexOutOfRangeException()
    {
        // Arrange - Range expects 11 elements but only provide 5
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Act & Assert - Accessing beyond available data
        Assert.Throws<IndexOutOfRangeException>(() => rangeData[7]);
    }

    [Fact]
    public void TryGet_Point_WithInsufficientData_ReturnsFalse()
    {
        // Arrange - Range expects 11 elements but only provide 5
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Act
        var result = rangeData.TryGet(7, out var value);

        // Assert
        Assert.False(result);
        Assert.Equal(default, value);
    }

    [Fact]
    public void SubRangeIndexer_WithSubRangeOutsideParentRange_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Act & Assert - Sub-range extends beyond parent range
        var subRange = Range.Closed<int>(5, 15);
        Assert.Throws<ArgumentOutOfRangeException>(() => rangeData[subRange]);
    }

    [Fact]
    public void TryGet_SubRange_WithSubRangeOutsideParentRange_ReturnsFalse()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Act
        var subRange = Range.Closed<int>(5, 15);
        var result = rangeData.TryGet(subRange, out var resultData);

        // Assert
        Assert.False(result);
        Assert.Null(resultData);
    }

    [Fact]
    public void SubRangeIndexer_WithInfiniteSubRange_ThrowsArgumentException()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Act & Assert - Sub-range with infinite end
        var subRange = Range.Create(new RangeValue<int>(5), RangeValue<int>.PositiveInfinity, true, false);
        Assert.Throws<ArgumentException>(() => rangeData[subRange]);
    }

    [Fact]
    public void TryGet_SubRange_WithInfiniteSubRange_ReturnsFalse()
    {
        // Arrange
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(range, data, _domain);

        // Act
        var subRange = Range.Create(new RangeValue<int>(5), RangeValue<int>.PositiveInfinity, true, false);
        var result = rangeData.TryGet(subRange, out var resultData);

        // Assert
        Assert.False(result);
        Assert.Null(resultData);
    }

    [Fact]
    public void SubRangeIndexer_WithOverflowInTryGet_ThrowsInvalidOperationException()
    {
        // Arrange - Use domain that causes overflow in TryGet but not in Contains check
        var hugeDomain = new HugeDistanceDomainStub(((long)int.MaxValue) + 100);
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, HugeDistanceDomainStub>(range, data, hugeDomain);

        // Act & Assert - The subrange is contained, but TryGet will fail due to overflow
        // This triggers the InvalidOperationException fallback
        var subRange = Range.Closed<int>(2, 5);
        var exception = Assert.Throws<InvalidOperationException>(() => rangeData[subRange]);
        Assert.Contains("Unable to retrieve sub-range", exception.Message);
    }

    [Fact]
    public void TryGet_Point_WithNegativeIndexFromDomain_ReturnsFalse()
    {
        // Arrange - Use domain that returns negative distance
        var hugeDomain = new HugeDistanceDomainStub(-100);
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, HugeDistanceDomainStub>(range, data, hugeDomain);

        // Act
        var result = rangeData.TryGet(5, out var value);

        // Assert
        Assert.False(result);
        Assert.Equal(default, value);
    }

    [Fact]
    public void TryGet_Point_WithIndexExceedingIntMax_ReturnsFalse()
    {
        // Arrange - Use domain that returns distance > int.MaxValue
        var hugeDomain = new HugeDistanceDomainStub(long.MaxValue);
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, HugeDistanceDomainStub>(range, data, hugeDomain);

        // Act
        var result = rangeData.TryGet(5, out var value);

        // Assert
        Assert.False(result);
        Assert.Equal(default, value);
    }

    [Fact]
    public void TryGet_SubRange_WithStartIndexExceedingIntMax_ReturnsFalse()
    {
        // Arrange - Use domain that returns huge distances
        var hugeDomain = new HugeDistanceDomainStub(((long)int.MaxValue) + 100);
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, HugeDistanceDomainStub>(range, data, hugeDomain);

        // Act
        var subRange = Range.Closed<int>(2, 5);
        var result = rangeData.TryGet(subRange, out var resultData);

        // Assert
        Assert.False(result);
        Assert.Null(resultData);
    }

    [Fact]
    public void TryGet_SubRange_WithNegativeStartIndex_ReturnsFalse()
    {
        // Arrange - Use domain that returns negative distance
        var hugeDomain = new HugeDistanceDomainStub(-50);
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, HugeDistanceDomainStub>(range, data, hugeDomain);

        // Act
        var subRange = Range.Closed<int>(2, 5);
        var result = rangeData.TryGet(subRange, out var resultData);

        // Assert
        Assert.False(result);
        Assert.Null(resultData);
    }

    [Fact]
    public void TryGet_SubRange_WithEndIndexExceedingIntMax_ReturnsFalse()
    {
        // Arrange - Use domain that returns huge distance for endIndex
        var hugeDomain = new HugeDistanceDomainStub(((long)int.MaxValue) + 100);
        var range = Range.Closed<int>(0, 10);
        var data = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var rangeData = new RangeData<int, int, HugeDistanceDomainStub>(range, data, hugeDomain);

        // Act - Both start and end indices will exceed int.MaxValue
        var subRange = Range.Closed<int>(2, 5);
        var result = rangeData.TryGet(subRange, out var resultData);

        // Assert
        Assert.False(result);
        Assert.Null(resultData);
    }

    #endregion
}
