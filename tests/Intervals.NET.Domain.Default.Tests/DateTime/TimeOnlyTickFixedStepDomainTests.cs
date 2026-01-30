using Intervals.NET.Domain.Default.DateTime;

namespace Intervals.NET.Domain.Default.Tests.DateTime;

public class TimeOnlyTickFixedStepDomainTests
{
    private readonly TimeOnlyTickFixedStepDomain _domain = new();

    [Fact]
    public void Floor_ReturnsValueItself_BecauseTickIsFinestGranularity()
    {
        // Arrange
        var value = new TimeOnly(10, 30, 45, 123);

        // Act
        var result = _domain.Floor(value);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void Ceiling_ReturnsValueItself_BecauseTickIsFinestGranularity()
    {
        // Arrange
        var value = new TimeOnly(10, 30, 45, 123);

        // Act
        var result = _domain.Ceiling(value);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void Distance_CalculatesTicksCorrectly()
    {
        // Arrange
        var start = new TimeOnly(10, 0, 0);
        var end = new TimeOnly(10, 0, 0, 0, 100); // 100 microseconds = 1000 ticks

        // Act
        var result = _domain.Distance(start, end);

        // Assert
        Assert.Equal(1000L, result);
    }

    [Fact]
    public void Add_AddsTicksCorrectly()
    {
        // Arrange
        var value = new TimeOnly(10, 0, 0);

        // Act - Add 1000 ticks (100 nanoseconds each = 100 microseconds total)
        var result = _domain.Add(value, 1000);

        // Assert
        Assert.Equal(new TimeOnly(10, 0, 0, 0, 100), result);
    }

    [Fact]
    public void Subtract_SubtractsTicksCorrectly()
    {
        // Arrange
        var value = new TimeOnly(10, 0, 0, 0, 100);

        // Act
        var result = _domain.Subtract(value, 1000);

        // Assert
        Assert.Equal(new TimeOnly(10, 0, 0), result);
    }
}
