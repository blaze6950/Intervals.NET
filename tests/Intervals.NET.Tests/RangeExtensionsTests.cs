using Intervals.NET.Extensions;
using RangeFactory = Intervals.NET.Factories.Range;

namespace Intervals.NET.Tests;

public class RangeExtensionsTests
{
    #region Overlaps Tests

    [Fact]
    public void Overlaps_WithOverlappingRanges_ReturnsTrue()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(10, 30);
        var range2 = RangeFactory.Closed<int>(20, 40);

        // Act
        var result = range1.Overlaps(range2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Overlaps_WithNonOverlappingRanges_ReturnsFalse()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(10, 20);
        var range2 = RangeFactory.Closed<int>(30, 40);

        // Act
        var result = range1.Overlaps(range2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Overlaps_WithAdjacentRanges_BothInclusive_ReturnsTrue()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(10, 20);
        var range2 = RangeFactory.Closed<int>(20, 30);

        // Act
        var result = range1.Overlaps(range2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Overlaps_WithAdjacentRanges_OneExclusive_ReturnsFalse()
    {
        // Arrange
        var range1 = RangeFactory.ClosedOpen<int>(10, 20);
        var range2 = RangeFactory.Closed<int>(20, 30);

        // Act
        var result = range1.Overlaps(range2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Overlaps_WithIdenticalRanges_ReturnsTrue()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(10, 20);
        var range2 = RangeFactory.Closed<int>(10, 20);

        // Act
        var result = range1.Overlaps(range2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Overlaps_WithContainedRange_ReturnsTrue()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(10, 40);
        var range2 = RangeFactory.Closed<int>(20, 30);

        // Act
        var result = range1.Overlaps(range2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Overlaps_WithInfiniteRanges_ReturnsTrue()
    {
        // Arrange
        var range1 = RangeFactory.Closed(RangeValue<int>.NegativeInfinity, 20);
        var range2 = RangeFactory.Closed(10, RangeValue<int>.PositiveInfinity);

        // Act
        var result = range1.Overlaps(range2);

        // Assert
        Assert.True(result);
    }

    #endregion

    #region Contains Tests

    [Fact]
    public void Contains_WithContainedRange_ReturnsTrue()
    {
        // Arrange
        var outer = RangeFactory.Closed<int>(10, 40);
        var inner = RangeFactory.Closed<int>(20, 30);

        // Act
        var result = outer.Contains(inner);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Contains_WithNonContainedRange_ReturnsFalse()
    {
        // Arrange
        var outer = RangeFactory.Closed<int>(10, 30);
        var inner = RangeFactory.Closed<int>(20, 40);

        // Act
        var result = outer.Contains(inner);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Contains_WithIdenticalRanges_ReturnsTrue()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(10, 20);
        var range2 = RangeFactory.Closed<int>(10, 20);

        // Act
        var result = range1.Contains(range2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Contains_WithSameBoundaries_InnerInclusive_OuterExclusive_ReturnsFalse()
    {
        // Arrange
        var outer = RangeFactory.Open<int>(10, 20);
        var inner = RangeFactory.Closed<int>(10, 20);

        // Act
        var result = outer.Contains(inner);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Contains_WithSameBoundaries_OuterInclusive_InnerExclusive_ReturnsTrue()
    {
        // Arrange
        var outer = RangeFactory.Closed<int>(10, 20);
        var inner = RangeFactory.Open<int>(10, 20);

        // Act
        var result = outer.Contains(inner);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Contains_WithInfiniteOuter_FiniteInner_ReturnsTrue()
    {
        // Arrange
        var outer = RangeFactory.Open(RangeValue<int>.NegativeInfinity, RangeValue<int>.PositiveInfinity);
        var inner = RangeFactory.Closed<int>(10, 20);

        // Act
        var result = outer.Contains(inner);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Contains_WithFiniteOuter_InfiniteInner_ReturnsFalse()
    {
        // Arrange
        var outer = RangeFactory.Closed<int>(10, 20);
        var inner = RangeFactory.Closed(RangeValue<int>.NegativeInfinity, 20);

        // Act
        var result = outer.Contains(inner);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Contains_WithBothInfinite_ReturnsTrue()
    {
        // Arrange
        var outer = RangeFactory.Open(RangeValue<int>.NegativeInfinity, RangeValue<int>.PositiveInfinity);
        var inner = RangeFactory.Open(RangeValue<int>.NegativeInfinity, RangeValue<int>.PositiveInfinity);

        // Act
        var result = outer.Contains(inner);

        // Assert
        Assert.True(result);
    }

    #endregion

    #region Intersect Tests

    [Fact]
    public void Intersect_WithOverlappingRanges_ReturnsIntersection()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(10, 30);
        var range2 = RangeFactory.Closed<int>(20, 40);

        // Act
        var result = range1.Intersect(range2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(20, result.Value.Start.Value);
        Assert.Equal(30, result.Value.End.Value);
        Assert.True(result.Value.IsStartInclusive);
        Assert.True(result.Value.IsEndInclusive);
    }

    [Fact]
    public void Intersect_WithNonOverlappingRanges_ReturnsNull()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(10, 20);
        var range2 = RangeFactory.Closed<int>(30, 40);

        // Act
        var result = range1.Intersect(range2);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Intersect_WithIdenticalRanges_ReturnsSameRange()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(10, 20);
        var range2 = RangeFactory.Closed<int>(10, 20);

        // Act
        var result = range1.Intersect(range2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(range1, result.Value);
    }

    [Fact]
    public void Intersect_WithDifferentInclusivity_UsesMoreRestrictive()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(10, 20);
        var range2 = RangeFactory.Open<int>(10, 20);

        // Act
        var result = range1.Intersect(range2);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Value.IsStartInclusive); // More restrictive (exclusive)
        Assert.False(result.Value.IsEndInclusive); // More restrictive (exclusive)
    }

    [Fact]
    public void Intersect_WithOneRangeContainingAnother_ReturnsSmallerRange()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(10, 50);
        var range2 = RangeFactory.Closed<int>(20, 30);

        // Act
        var result = range1.Intersect(range2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(20, result.Value.Start.Value);
        Assert.Equal(30, result.Value.End.Value);
    }

    [Fact]
    public void Intersect_WithInfiniteRanges_ReturnsFiniteIntersection()
    {
        // Arrange
        var range1 = RangeFactory.Closed(RangeValue<int>.NegativeInfinity, 30);
        var range2 = RangeFactory.Closed(10, RangeValue<int>.PositiveInfinity);

        // Act
        var result = range1.Intersect(range2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Value.Start.Value);
        Assert.Equal(30, result.Value.End.Value);
    }

    #endregion

    #region Union Tests

    [Fact]
    public void Union_WithOverlappingRanges_ReturnsUnion()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(10, 30);
        var range2 = RangeFactory.Closed<int>(20, 40);

        // Act
        var result = range1.Union(range2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Value.Start.Value);
        Assert.Equal(40, result.Value.End.Value);
    }

    [Fact]
    public void Union_WithNonOverlappingNonAdjacentRanges_ReturnsNull()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(10, 20);
        var range2 = RangeFactory.Closed<int>(30, 40);

        // Act
        var result = range1.Union(range2);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Union_WithAdjacentRanges_ReturnsUnion()
    {
        // Arrange
        var range1 = RangeFactory.ClosedOpen<int>(10, 20);
        var range2 = RangeFactory.Closed<int>(20, 30);

        // Act
        var result = range1.Union(range2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Value.Start.Value);
        Assert.Equal(30, result.Value.End.Value);
    }

    [Fact]
    public void Union_WithIdenticalRanges_ReturnsSameRange()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(10, 20);
        var range2 = RangeFactory.Closed<int>(10, 20);

        // Act
        var result = range1.Union(range2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(range1, result.Value);
    }

    [Fact]
    public void Union_WithDifferentInclusivity_UsesMorePermissive()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(10, 20);
        var range2 = RangeFactory.Open<int>(10, 20);

        // Act
        var result = range1.Union(range2);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Value.IsStartInclusive); // More permissive (inclusive)
        Assert.True(result.Value.IsEndInclusive); // More permissive (inclusive)
    }

    [Fact]
    public void Union_WithInfiniteRanges_ReturnsInfiniteUnion()
    {
        // Arrange
        var range1 = RangeFactory.Closed(RangeValue<int>.NegativeInfinity, 30);
        var range2 = RangeFactory.Closed(10, RangeValue<int>.PositiveInfinity);

        // Act
        var result = range1.Union(range2);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Value.Start.IsNegativeInfinity);
        Assert.True(result.Value.End.IsPositiveInfinity);
    }

    #endregion

    #region IsAdjacent Tests

    [Fact]
    public void IsAdjacent_WithAdjacentRanges_OneInclusive_ReturnsTrue()
    {
        // Arrange
        var range1 = RangeFactory.ClosedOpen<int>(10, 20);
        var range2 = RangeFactory.Closed<int>(20, 30);

        // Act
        var result = range1.IsAdjacent(range2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsAdjacent_WithAdjacentRanges_BothInclusive_ReturnsFalse()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(10, 20);
        var range2 = RangeFactory.Closed<int>(20, 30);

        // Act
        var result = range1.IsAdjacent(range2);

        // Assert
        Assert.False(result); // They overlap at 20, not adjacent
    }

    [Fact]
    public void IsAdjacent_WithAdjacentRanges_BothExclusive_ReturnsFalse()
    {
        // Arrange
        var range1 = RangeFactory.Open<int>(10, 20);
        var range2 = RangeFactory.Open<int>(20, 30);

        // Act
        var result = range1.IsAdjacent(range2);

        // Assert
        Assert.False(result); // Gap at 20, not adjacent
    }

    [Fact]
    public void IsAdjacent_WithNonAdjacentRanges_ReturnsFalse()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(10, 20);
        var range2 = RangeFactory.Closed<int>(30, 40);

        // Act
        var result = range1.IsAdjacent(range2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsAdjacent_WithReverseOrder_ReturnsTrue()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(20, 30);
        var range2 = RangeFactory.ClosedOpen<int>(10, 20);

        // Act
        var result = range1.IsAdjacent(range2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsAdjacent_WithInfiniteRanges_ReturnsFalse()
    {
        // Arrange
        var range1 = RangeFactory.Closed(RangeValue<int>.NegativeInfinity, 20);
        var range2 = RangeFactory.Closed(20, RangeValue<int>.PositiveInfinity);

        // Act
        var result = range1.IsAdjacent(range2);

        // Assert
        Assert.False(result); // They overlap, not adjacent
    }

    #endregion

    #region IsBefore Tests

    [Fact]
    public void IsBefore_WithRangeCompletelyBefore_ReturnsTrue()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(10, 20);
        var range2 = RangeFactory.Closed<int>(30, 40);

        // Act
        var result = range1.IsBefore(range2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsBefore_WithOverlappingRanges_ReturnsFalse()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(10, 30);
        var range2 = RangeFactory.Closed<int>(20, 40);

        // Act
        var result = range1.IsBefore(range2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsBefore_WithAdjacentRanges_OneExclusive_ReturnsTrue()
    {
        // Arrange
        var range1 = RangeFactory.ClosedOpen<int>(10, 20);
        var range2 = RangeFactory.Closed<int>(20, 30);

        // Act
        var result = range1.IsBefore(range2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsBefore_WithAdjacentRanges_BothInclusive_ReturnsFalse()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(10, 20);
        var range2 = RangeFactory.Closed<int>(20, 30);

        // Act
        var result = range1.IsBefore(range2);

        // Assert
        Assert.False(result); // They touch at 20
    }

    [Fact]
    public void IsBefore_WithPositiveInfinityEnd_ReturnsFalse()
    {
        // Arrange
        var range1 = RangeFactory.Closed(10, RangeValue<int>.PositiveInfinity);
        var range2 = RangeFactory.Closed<int>(20, 30);

        // Act
        var result = range1.IsBefore(range2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsBefore_WithNegativeInfinityStart_ReturnsFalse()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(10, 20);
        var range2 = RangeFactory.Closed(RangeValue<int>.NegativeInfinity, 30);

        // Act
        var result = range1.IsBefore(range2);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region IsAfter Tests

    [Fact]
    public void IsAfter_WithRangeCompletelyAfter_ReturnsTrue()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(30, 40);
        var range2 = RangeFactory.Closed<int>(10, 20);

        // Act
        var result = range1.IsAfter(range2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsAfter_WithOverlappingRanges_ReturnsFalse()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(20, 40);
        var range2 = RangeFactory.Closed<int>(10, 30);

        // Act
        var result = range1.IsAfter(range2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsAfter_DelegatesToIsBefore_WorksCorrectly()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(30, 40);
        var range2 = RangeFactory.Closed<int>(10, 20);

        // Act
        var afterResult = range1.IsAfter(range2);
        var beforeResult = range2.IsBefore(range1);

        // Assert
        Assert.Equal(beforeResult, afterResult);
    }

    #endregion

    #region IsEmpty Tests

    [Fact]
    public void IsEmpty_WithNormalRange_ReturnsFalse()
    {
        // Arrange
        var range = RangeFactory.Closed<int>(10, 20);

        // Act
        var result = range.IsEmpty();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsEmpty_WithSinglePoint_BothInclusive_ReturnsFalse()
    {
        // Arrange
        var range = RangeFactory.Closed<int>(10, 10);

        // Act
        var result = range.IsEmpty();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsEmpty_WithInfiniteRange_ReturnsFalse()
    {
        // Arrange
        var range = RangeFactory.Open(RangeValue<int>.NegativeInfinity, RangeValue<int>.PositiveInfinity);

        // Act
        var result = range.IsEmpty();

        // Assert
        Assert.False(result);
    }

    #endregion

    #region IsBounded Tests

    [Fact]
    public void IsBounded_WithFiniteBoundaries_ReturnsTrue()
    {
        // Arrange
        var range = RangeFactory.Closed<int>(10, 20);

        // Act
        var result = range.IsBounded();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsBounded_WithNegativeInfinityStart_ReturnsFalse()
    {
        // Arrange
        var range = RangeFactory.Closed(RangeValue<int>.NegativeInfinity, 20);

        // Act
        var result = range.IsBounded();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsBounded_WithPositiveInfinityEnd_ReturnsFalse()
    {
        // Arrange
        var range = RangeFactory.Closed(10, RangeValue<int>.PositiveInfinity);

        // Act
        var result = range.IsBounded();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsBounded_WithBothInfinities_ReturnsFalse()
    {
        // Arrange
        var range = RangeFactory.Open(RangeValue<int>.NegativeInfinity, RangeValue<int>.PositiveInfinity);

        // Act
        var result = range.IsBounded();

        // Assert
        Assert.False(result);
    }

    #endregion

    #region IsUnbounded Tests

    [Fact]
    public void IsUnbounded_WithFiniteBoundaries_ReturnsFalse()
    {
        // Arrange
        var range = RangeFactory.Closed<int>(10, 20);

        // Act
        var result = range.IsUnbounded();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsUnbounded_WithNegativeInfinityStart_ReturnsTrue()
    {
        // Arrange
        var range = RangeFactory.Closed(RangeValue<int>.NegativeInfinity, 20);

        // Act
        var result = range.IsUnbounded();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsUnbounded_WithPositiveInfinityEnd_ReturnsTrue()
    {
        // Arrange
        var range = RangeFactory.Closed(10, RangeValue<int>.PositiveInfinity);

        // Act
        var result = range.IsUnbounded();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsUnbounded_WithBothInfinities_ReturnsTrue()
    {
        // Arrange
        var range = RangeFactory.Open(RangeValue<int>.NegativeInfinity, RangeValue<int>.PositiveInfinity);

        // Act
        var result = range.IsUnbounded();

        // Assert
        Assert.True(result);
    }

    #endregion

    #region IsInfinite Tests

    [Fact]
    public void IsInfinite_WithBothInfinities_ReturnsTrue()
    {
        // Arrange
        var range = RangeFactory.Open(RangeValue<int>.NegativeInfinity, RangeValue<int>.PositiveInfinity);

        // Act
        var result = range.IsInfinite();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInfinite_WithFiniteBoundaries_ReturnsFalse()
    {
        // Arrange
        var range = RangeFactory.Closed<int>(10, 20);

        // Act
        var result = range.IsInfinite();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInfinite_WithOnlyNegativeInfinity_ReturnsFalse()
    {
        // Arrange
        var range = RangeFactory.Closed(RangeValue<int>.NegativeInfinity, 20);

        // Act
        var result = range.IsInfinite();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInfinite_WithOnlyPositiveInfinity_ReturnsFalse()
    {
        // Arrange
        var range = RangeFactory.Closed(10, RangeValue<int>.PositiveInfinity);

        // Act
        var result = range.IsInfinite();

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Except Tests

    [Fact]
    public void Except_WithNonOverlappingRanges_ReturnsOriginalRange()
    {
        // Arrange
        var range = RangeFactory.Closed<int>(10, 20);
        var other = RangeFactory.Closed<int>(30, 40);

        // Act
        var result = range.Except(other).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(range, result[0]);
    }

    [Fact]
    public void Except_WithCompleteOverlap_ReturnsEmpty()
    {
        // Arrange
        var range = RangeFactory.Closed<int>(20, 30);
        var other = RangeFactory.Closed<int>(10, 40);

        // Act
        var result = range.Except(other).ToList();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Except_WithPartialOverlapOnLeft_ReturnsRightPortion()
    {
        // Arrange
        var range = RangeFactory.Closed<int>(10, 30);
        var other = RangeFactory.Closed<int>(5, 20);

        // Act
        var result = range.Except(other).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(20, result[0].Start.Value);
        Assert.Equal(30, result[0].End.Value);
        Assert.False(result[0].IsStartInclusive);
        Assert.True(result[0].IsEndInclusive);
    }

    [Fact]
    public void Except_WithPartialOverlapOnRight_ReturnsLeftPortion()
    {
        // Arrange
        var range = RangeFactory.Closed<int>(10, 30);
        var other = RangeFactory.Closed<int>(20, 40);

        // Act
        var result = range.Except(other).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(10, result[0].Start.Value);
        Assert.Equal(20, result[0].End.Value);
        Assert.True(result[0].IsStartInclusive);
        Assert.False(result[0].IsEndInclusive);
    }

    [Fact]
    public void Except_WithMiddleOverlap_ReturnsTwoPortions()
    {
        // Arrange
        var range = RangeFactory.Closed<int>(10, 50);
        var other = RangeFactory.Closed<int>(20, 30);

        // Act
        var result = range.Except(other).ToList();

        // Assert
        Assert.Equal(2, result.Count);

        // Left portion
        Assert.Equal(10, result[0].Start.Value);
        Assert.Equal(20, result[0].End.Value);
        Assert.True(result[0].IsStartInclusive);
        Assert.False(result[0].IsEndInclusive);

        // Right portion
        Assert.Equal(30, result[1].Start.Value);
        Assert.Equal(50, result[1].End.Value);
        Assert.False(result[1].IsStartInclusive);
        Assert.True(result[1].IsEndInclusive);
    }

    [Fact]
    public void Except_WithInfiniteRange_AndFiniteOther_ReturnsInfinitePortions()
    {
        // Arrange
        var range = RangeFactory.Open(RangeValue<int>.NegativeInfinity, RangeValue<int>.PositiveInfinity);
        var other = RangeFactory.Closed<int>(10, 20);

        // Act
        var result = range.Except(other).ToList();

        // Assert
        Assert.Equal(2, result.Count);

        // Left portion
        Assert.True(result[0].Start.IsNegativeInfinity);
        Assert.Equal(10, result[0].End.Value);

        // Right portion
        Assert.Equal(20, result[1].Start.Value);
        Assert.True(result[1].End.IsPositiveInfinity);
    }

    [Fact]
    public void Except_WithNegativeInfinityStart_ReturnsRightPortion()
    {
        // Arrange
        var range = RangeFactory.Closed(RangeValue<int>.NegativeInfinity, 30);
        var other = RangeFactory.Closed<int>(10, 20);

        // Act
        var result = range.Except(other).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.True(result[0].Start.IsNegativeInfinity);
        Assert.Equal(20, result[1].Start.Value);
        Assert.Equal(30, result[1].End.Value);
    }

    [Fact]
    public void Except_WithPositiveInfinityEnd_ReturnsLeftPortion()
    {
        // Arrange
        var range = RangeFactory.Closed(10, RangeValue<int>.PositiveInfinity);
        var other = RangeFactory.Closed<int>(20, 30);

        // Act
        var result = range.Except(other).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(10, result[0].Start.Value);
        Assert.Equal(20, result[0].End.Value);
        Assert.True(result[1].End.IsPositiveInfinity);
    }

    [Fact]
    public void Except_WithIdenticalRanges_ReturnsEmpty()
    {
        // Arrange
        var range = RangeFactory.Closed<int>(10, 20);
        var other = RangeFactory.Closed<int>(10, 20);

        // Act
        var result = range.Except(other).ToList();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Except_WithSinglePointAtStart_PreservesSinglePoint()
    {
        // Arrange
        var range = RangeFactory.Closed<int>(10, 20);
        var other = RangeFactory.Open<int>(10, 20);

        // Act
        var result = range.Except(other).ToList();

        // Assert
        Assert.Equal(2, result.Count);

        // Single point at start
        Assert.Equal(10, result[0].Start.Value);
        Assert.Equal(10, result[0].End.Value);
        Assert.True(result[0].IsStartInclusive);
        Assert.True(result[0].IsEndInclusive);

        // Single point at end
        Assert.Equal(20, result[1].Start.Value);
        Assert.Equal(20, result[1].End.Value);
    }

    #endregion

    #region Integration Tests - Multiple Extension Methods

    [Fact]
    public void ExtensionMethods_ChainedOperations_WorkCorrectly()
    {
        // Arrange
        var range1 = RangeFactory.Closed<int>(10, 40);
        var range2 = RangeFactory.Closed<int>(20, 30);
        var range3 = RangeFactory.Closed<int>(25, 50);

        // Act
        var contains = range1.Contains(range2);
        var intersection = range1.Intersect(range3);
        var union = range1.Union(range3);

        // Assert
        Assert.True(contains);
        Assert.NotNull(intersection);
        Assert.Equal(25, intersection.Value.Start.Value);
        Assert.Equal(40, intersection.Value.End.Value);
        Assert.NotNull(union);
        Assert.Equal(10, union.Value.Start.Value);
        Assert.Equal(50, union.Value.End.Value);
    }

    [Fact]
    public void ExtensionMethods_WithDifferentTypes_WorkCorrectly()
    {
        // Arrange
        var doubleRange1 = RangeFactory.Closed<double>(1.5, 5.5);
        var doubleRange2 = RangeFactory.Closed<double>(3.0, 7.0);

        // Act
        var overlaps = doubleRange1.Overlaps(doubleRange2);
        var intersection = doubleRange1.Intersect(doubleRange2);

        // Assert
        Assert.True(overlaps);
        Assert.NotNull(intersection);
        Assert.Equal(3.0, intersection.Value.Start.Value);
        Assert.Equal(5.5, intersection.Value.End.Value);
    }

    #endregion
}