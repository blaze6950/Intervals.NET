using Intervals.NET.Domain.Default.DateTime;

namespace Intervals.NET.Domain.Default.Tests.DateTime;

public class TimeOnlyMinuteFixedStepDomainTests
{
    private readonly TimeOnlyMinuteFixedStepDomain _domain = new();

    [Fact]
    public void Floor_RoundsDownToNearestMinute()
    {
        // Arrange
        var value = new TimeOnly(10, 30, 45);

        // Act
        var result = _domain.Floor(value);

        // Assert
        Assert.Equal(new TimeOnly(10, 30, 0), result);
    }

    [Fact]
    public void Ceiling_RoundsUpToNearestMinute()
    {
        // Arrange
        var value = new TimeOnly(10, 30, 1);

        // Act
        var result = _domain.Ceiling(value);

        // Assert
        Assert.Equal(new TimeOnly(10, 31, 0), result);
    }

    [Fact]
    public void Distance_CalculatesMinutesCorrectly()
    {
        // Arrange
        var start = new TimeOnly(10, 0, 0);
        var end = new TimeOnly(12, 30, 0);

        // Act
        var result = _domain.Distance(start, end);

        // Assert - 2 hours 30 minutes = 150 minutes
        Assert.Equal(150L, result);
    }

    [Fact]
    public void Add_AddsMinutesCorrectly()
    {
        // Arrange
        var value = new TimeOnly(10, 0, 0);

        // Act
        var result = _domain.Add(value, 90); // Add 90 minutes

        // Assert
        Assert.Equal(new TimeOnly(11, 30, 0), result);
    }

    [Fact]
    public void Subtract_SubtractsMinutesCorrectly()
    {
        // Arrange
        var value = new TimeOnly(11, 30, 0);

        // Act
        var result = _domain.Subtract(value, 90);

        // Assert
        Assert.Equal(new TimeOnly(10, 0, 0), result);
    }
}
