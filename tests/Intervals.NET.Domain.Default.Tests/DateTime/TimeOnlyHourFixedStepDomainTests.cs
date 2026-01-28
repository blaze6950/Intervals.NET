using Intervals.NET.Domain.Default.DateTime;

namespace Intervals.NET.Domain.Default.Tests.DateTime;

public class TimeOnlyHourFixedStepDomainTests
{
    private readonly TimeOnlyHourFixedStepDomain _domain = new();

    [Fact]
    public void Floor_RoundsDownToNearestHour()
    {
        // Arrange
        var value = new TimeOnly(10, 30, 45);

        // Act
        var result = _domain.Floor(value);

        // Assert
        Assert.Equal(new TimeOnly(10, 0, 0), result);
    }

    [Fact]
    public void Ceiling_RoundsUpToNearestHour()
    {
        // Arrange
        var value = new TimeOnly(10, 0, 1);

        // Act
        var result = _domain.Ceiling(value);

        // Assert
        Assert.Equal(new TimeOnly(11, 0, 0), result);
    }

    [Fact]
    public void Distance_CalculatesHoursCorrectly()
    {
        // Arrange
        var start = new TimeOnly(8, 0, 0);
        var end = new TimeOnly(17, 0, 0);

        // Act
        var result = _domain.Distance(start, end);

        // Assert - 9 hours
        Assert.Equal(9L, result);
    }

    [Fact]
    public void Add_AddsHoursCorrectly()
    {
        // Arrange
        var value = new TimeOnly(10, 0, 0);

        // Act
        var result = _domain.Add(value, 5);

        // Assert
        Assert.Equal(new TimeOnly(15, 0, 0), result);
    }

    [Fact]
    public void Subtract_SubtractsHoursCorrectly()
    {
        // Arrange
        var value = new TimeOnly(15, 0, 0);

        // Act
        var result = _domain.Subtract(value, 5);

        // Assert
        Assert.Equal(new TimeOnly(10, 0, 0), result);
    }

    [Fact]
    public void Distance_HandlesAcrossMidnight()
    {
        // Arrange
        var start = new TimeOnly(22, 0, 0); // 10 PM
        var end = new TimeOnly(2, 0, 0);   // 2 AM (next day conceptually)

        // Act
        var result = _domain.Distance(start, end);

        // Assert - negative because end < start in same day context
        Assert.Equal(-20L, result);
    }
}
