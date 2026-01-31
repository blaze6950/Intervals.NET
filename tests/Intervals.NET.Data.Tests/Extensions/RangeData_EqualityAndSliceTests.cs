using Intervals.NET.Data.Extensions;
using Intervals.NET.Domain.Default.Numeric;
using Range = Intervals.NET.Factories.Range;

namespace Intervals.NET.Data.Tests.Extensions;

public class RangeData_EqualityAndSliceTests
{
    private readonly IntegerFixedStepDomain _domain = new();

    [Fact]
    public void ToRangeData_CreatesRangeDataWithSameRangeAndData()
    {
        // Arrange
        var data = Enumerable.Range(1, 11).ToArray();
        var range = Range.Closed<int>(10, 20);

        // Act
        var rd = data.ToRangeData(range, _domain);

        // Assert
        Assert.Equal(range, rd.Range);
        Assert.Equal(_domain, rd.Domain);
        Assert.True(rd.Data.SequenceEqual(data));
    }

    [Fact]
    public void Equals_And_GetHashCode_ConsiderOnlyRangeAndDomain()
    {
        // Arrange
        var data1 = Enumerable.Range(1, 11);
        var data2 = Enumerable.Range(101, 11); // different content
        var range = Range.Closed<int>(10, 20);

        var rd1 = new RangeData<int, int, IntegerFixedStepDomain>(range, data1, _domain);
        var rd2 = new RangeData<int, int, IntegerFixedStepDomain>(range, data2, _domain);

        // Act & Assert
        Assert.True(rd1.Equals(rd2));
        Assert.Equal(rd1.GetHashCode(), rd2.GetHashCode());
    }

    [Fact]
    public void Equals_WithDifferentRange_ReturnsFalse()
    {
        // Arrange
        var data = Enumerable.Range(1, 11).ToArray();
        var leftRange = Range.Closed<int>(10, 20);
        var rightRange = Range.Closed<int>(11, 21);

        var rd1 = new RangeData<int, int, IntegerFixedStepDomain>(leftRange, data, _domain);
        var rd2 = new RangeData<int, int, IntegerFixedStepDomain>(rightRange, data, _domain);

        // Act & Assert
        Assert.False(rd1.Equals(rd2));
    }

    [Fact]
    public void Slice_ReturnsExpectedSubset()
    {
        // Arrange
        var data = Enumerable.Range(1, 11).ToArray(); // indices 0..10 correspond to 10..20
        var rd = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data, _domain);

        var subRange = Range.Closed<int>(12, 14); // should yield elements at indices 2..4 => values 3,4,5

        // Act
        var sliced = rd.Slice(subRange);

        // Assert
        Assert.Equal(subRange, sliced.Range);
        Assert.True(sliced.Data.SequenceEqual([3, 4, 5]));
    }

    [Fact]
    public void Slice_WithExclusiveStartSameAsEnd_ReturnsEmpty()
    {
        // Arrange
        var data = Enumerable.Range(1, 11).ToArray();
        var rd = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data, _domain);

        // Create sub-range where start == end but start is exclusive and end is inclusive
        // This should result in an empty slice per RangeData.TryGet logic
        var subRange = Range.OpenClosed<int>(10, 10);

        // Act
        var sliced = rd.Slice(subRange);

        // Assert
        Assert.Equal(subRange, sliced.Range);
        Assert.False(sliced.Data.Any());
    }
}
