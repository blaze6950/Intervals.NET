using Intervals.NET.Data;
using Intervals.NET.Domain.Default.Numeric;
using Range = Intervals.NET.Factories.Range;
using Xunit;

namespace Intervals.NET.Data.Tests.TempTests;

public class TestEmptySubrangeInOpenClosedParent
{
    private readonly IntegerFixedStepDomain _domain = new();

    [Fact]
    public void TryGet_SubRange_EmptyButContained_WithExclusiveStartParent_ShouldReturnTrue()
    {
        // Arrange - Parent range: (10,20] means it contains 11, 12, ..., 20
        var parentRange = Range.OpenClosed<int>(10, 20);
        var data = new[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
        var rangeData = new RangeData<int, int, IntegerFixedStepDomain>(parentRange, data, _domain);

        // Act - Sub-range (10,10] is logically empty but contained in parent
        var subRange = Range.OpenClosed<int>(10, 10);
        
        var success = rangeData.TryGet(subRange, out var result);

        // Assert - Should return true with empty data, not false
        // This currently FAILS because endIndex becomes -1, and the negative-index guard
        // returns false before the empty-range handling is reached
        Assert.True(success, "TryGet should return true for an empty but contained subrange");
        Assert.NotNull(result);
        Assert.Empty(result.Data);
    }
}
