using Intervals.NET.Data.Extensions;
using Intervals.NET.Data.Tests.Helpers;
using Range = Intervals.NET.Factories.Range;

namespace Intervals.NET.Data.Tests.Extensions;

public class RangeDataExtensions_TrimOverflowTests
{
    [Fact]
    public void TrimStart_WithHugeDistanceDomain_ReturnsNull()
    {
        // Arrange
        var hugeDomain = new HugeDistanceDomainStub(((long)int.MaxValue) + 1000);
        var data = Enumerable.Empty<int>();
        var originalRange = Range.Closed<int>(0, 100);
        var rd = new RangeData<int, int, HugeDistanceDomainStub>(originalRange, data, hugeDomain);

        // Act
        var result = RangeDataExtensions.TrimStart(rd, 1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void TrimEnd_WithHugeDistanceDomain_ReturnsNull()
    {
        // Arrange
        var hugeDomain = new HugeDistanceDomainStub(((long)int.MaxValue) + 1000);
        var data = Enumerable.Empty<int>();
        var originalRange = Range.Closed<int>(0, 100);
        var rd = new RangeData<int, int, HugeDistanceDomainStub>(originalRange, data, hugeDomain);

        // Act
        var result = RangeDataExtensions.TrimEnd(rd, 99);

        // Assert
        Assert.Null(result);
    }
}