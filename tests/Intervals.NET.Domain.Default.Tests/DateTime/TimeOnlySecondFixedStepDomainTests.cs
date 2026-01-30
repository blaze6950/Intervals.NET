using Intervals.NET.Domain.Default.DateTime;

namespace Intervals.NET.Domain.Default.Tests.DateTime;

public class TimeOnlySecondFixedStepDomainTests
{
    private readonly TimeOnlySecondFixedStepDomain _domain = new();

    [Fact]
    public void Floor_RoundsDownToNearestSecond()
    {
        // Arrange - 10:30:45.500
        var value = new TimeOnly(10, 30, 45, 500);

        // Act
        var result = _domain.Floor(value);

        // Assert
        Assert.Equal(new TimeOnly(10, 30, 45), result);
    }

    [Fact]
    public void Floor_ReturnsValueItself_WhenAlreadyOnSecondBoundary()
    {
        // Arrange
        var value = new TimeOnly(10, 30, 45);

        // Act
        var result = _domain.Floor(value);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void Ceiling_RoundsUpToNearestSecond()
    {
        // Arrange - 10:30:45 and 1 millisecond
        var value = new TimeOnly(10, 30, 45, 1);

        // Act
        var result = _domain.Ceiling(value);

        // Assert
        Assert.Equal(new TimeOnly(10, 30, 46), result);
    }

    [Fact]
    public void Distance_CalculatesSecondsCorrectly()
    {
        // Arrange
        var start = new TimeOnly(10, 0, 0);
        var end = new TimeOnly(10, 1, 30);

        // Act
        var result = _domain.Distance(start, end);

        // Assert - 1 minute 30 seconds = 90 seconds
        Assert.Equal(90L, result);
    }

    [Fact]
    public void Add_AddsSecondsCorrectly()
    {
        // Arrange
        var value = new TimeOnly(10, 0, 0);

        // Act
        var result = _domain.Add(value, 90);

        // Assert
        Assert.Equal(new TimeOnly(10, 1, 30), result);
    }

    [Fact]
    public void Subtract_SubtractsSecondsCorrectly()
    {
        // Arrange
        var value = new TimeOnly(10, 1, 30);

        // Act
        var result = _domain.Subtract(value, 90);

        // Assert
        Assert.Equal(new TimeOnly(10, 0, 0), result);
    }

    [Fact]
    public void Distance_HandlesHourBoundaries()
    {
        // Arrange
        var start = new TimeOnly(9, 59, 50);
        var end = new TimeOnly(10, 0, 10);

        // Act
        var result = _domain.Distance(start, end);

        // Assert - 20 seconds
        Assert.Equal(20L, result);
    }
}
