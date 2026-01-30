using Intervals.NET.Domain.Default.DateTime;

namespace Intervals.NET.Domain.Default.Tests.DateTime;

public class TimeOnlyMillisecondFixedStepDomainTests
{
    private readonly TimeOnlyMillisecondFixedStepDomain _domain = new();

    [Fact]
    public void Floor_RoundsDownToNearestMillisecond()
    {
        // Arrange - 123 milliseconds and some microseconds
        var value = new TimeOnly(10, 0, 0, 123, 500);

        // Act
        var result = _domain.Floor(value);

        // Assert
        Assert.Equal(new TimeOnly(10, 0, 0, 123), result);
    }

    [Fact]
    public void Ceiling_RoundsUpToNearestMillisecond()
    {
        // Arrange
        var value = new TimeOnly(10, 0, 0, 123, 1);

        // Act
        var result = _domain.Ceiling(value);

        // Assert
        Assert.Equal(new TimeOnly(10, 0, 0, 124), result);
    }

    [Fact]
    public void Distance_CalculatesMillisecondsCorrectly()
    {
        // Arrange
        var start = new TimeOnly(10, 0, 0);
        var end = new TimeOnly(10, 0, 5); // 5 seconds = 5000 milliseconds

        // Act
        var result = _domain.Distance(start, end);

        // Assert
        Assert.Equal(5000L, result);
    }

    [Fact]
    public void Add_AddsMillisecondsCorrectly()
    {
        // Arrange
        var value = new TimeOnly(10, 0, 0);

        // Act
        var result = _domain.Add(value, 5000);

        // Assert
        Assert.Equal(new TimeOnly(10, 0, 5), result);
    }

    [Fact]
    public void Subtract_SubtractsMillisecondsCorrectly()
    {
        // Arrange
        var value = new TimeOnly(10, 0, 5);

        // Act
        var result = _domain.Subtract(value, 5000);

        // Assert
        Assert.Equal(new TimeOnly(10, 0, 0), result);
    }
}
