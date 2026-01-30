using Intervals.NET.Domain.Default.DateTime;

namespace Intervals.NET.Domain.Default.Tests.DateTime;

public class TimeOnlyMicrosecondFixedStepDomainTests
{
    private readonly TimeOnlyMicrosecondFixedStepDomain _domain = new();

    [Fact]
    public void Floor_RoundsDownToNearestMicrosecond()
    {
        // Arrange - 105.5 microseconds = 1055 ticks (between 105 and 106 microseconds)
        var value = new TimeOnly(1055);

        // Act
        var result = _domain.Floor(value);

        // Assert - should floor to 105 microseconds = 1050 ticks
        var expected = new TimeOnly(1050);
        Assert.Equal(expected.Ticks, result.Ticks);
    }

    [Fact]
    public void Ceiling_RoundsUpToNearestMicrosecond()
    {
        // Arrange - 101.5 microseconds = 1015 ticks (between 101 and 102 microseconds)
        var value = new TimeOnly(1015);

        // Act
        var result = _domain.Ceiling(value);

        // Assert - should round up to 102 microseconds = 1020 ticks
        var expected = new TimeOnly(1020);
        Assert.Equal(expected.Ticks, result.Ticks);
    }

    [Fact]
    public void Distance_CalculatesMicrosecondsCorrectly()
    {
        // Arrange
        var start = new TimeOnly(10, 0, 0, 0);
        var end = new TimeOnly(10, 0, 0, 5); // 5 milliseconds = 5000 microseconds

        // Act
        var result = _domain.Distance(start, end);

        // Assert
        Assert.Equal(5000L, result);
    }

    [Fact]
    public void Add_AddsMicrosecondsCorrectly()
    {
        // Arrange
        var value = new TimeOnly(10, 0, 0);

        // Act
        var result = _domain.Add(value, 5000); // Add 5000 microseconds = 5ms

        // Assert
        Assert.Equal(new TimeOnly(10, 0, 0, 5), result);
    }

    [Fact]
    public void Subtract_SubtractsMicrosecondsCorrectly()
    {
        // Arrange
        var value = new TimeOnly(10, 0, 0, 5);

        // Act
        var result = _domain.Subtract(value, 5000);

        // Assert
        Assert.Equal(new TimeOnly(10, 0, 0), result);
    }
}
