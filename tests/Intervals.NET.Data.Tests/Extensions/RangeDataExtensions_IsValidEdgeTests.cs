using Intervals.NET.Data.Extensions;
using Intervals.NET.Domain.Default.Numeric;
using Range = Intervals.NET.Factories.Range;

namespace Intervals.NET.Data.Tests.Extensions;

public class RangeDataExtensions_IsValidEdgeTests
{
    private readonly IntegerFixedStepDomain _domain = new();

    [Fact]
    public void IsValid_WithEmptyLogicalRange_ReturnsTrueAndNoMessage()
    {
        // Arrange
        // Create a range that results in zero logical steps: open-closed range with same start and end
        var range = Range.OpenClosed<int>(10, 10);
        var rd = new RangeData<int, int, IntegerFixedStepDomain>(range, Enumerable.Empty<int>(), _domain);

        // Act
        var isValid = RangeDataExtensions.IsValid(rd, out var message);

        // Assert
        Assert.True(isValid);
        Assert.Null(message);
    }
}