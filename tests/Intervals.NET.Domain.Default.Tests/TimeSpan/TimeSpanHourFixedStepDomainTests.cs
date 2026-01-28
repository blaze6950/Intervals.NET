using Intervals.NET.Domain.Default.TimeSpan;

namespace Intervals.NET.Domain.Default.Tests.TimeSpan;

public class TimeSpanHourFixedStepDomainTests
{
    private readonly TimeSpanHourFixedStepDomain _domain = new();

    [Fact]
    public void Floor_RoundsDownToNearestHour()
    {
        // Arrange - 2 hours 30 minutes
        var value = global::System.TimeSpan.FromMinutes(150);

        // Act
        var result = _domain.Floor(value);

        // Assert
        Assert.Equal(global::System.TimeSpan.FromHours(2), result);
    }

    [Fact]
    public void Ceiling_RoundsUpToNearestHour()
    {
        // Arrange - 2 hours and 1 minute
        var value = global::System.TimeSpan.FromMinutes(121);

        // Act
        var result = _domain.Ceiling(value);

        // Assert
        Assert.Equal(global::System.TimeSpan.FromHours(3), result);
    }

    [Theory]
    [InlineData(2, 5, 3L)]
    [InlineData(0, 24, 24L)]
    [InlineData(10, 5, -5L)]
    public void Distance_CalculatesHoursCorrectly(int startHours, int endHours, long expected)
    {
        var start = global::System.TimeSpan.FromHours(startHours);
        var end = global::System.TimeSpan.FromHours(endHours);

        var result = _domain.Distance(start, end);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Add_AddsHoursCorrectly()
    {
        var value = global::System.TimeSpan.FromHours(5);
        var result = _domain.Add(value, 3);

        Assert.Equal(global::System.TimeSpan.FromHours(8), result);
    }

    [Fact]
    public void Subtract_SubtractsHoursCorrectly()
    {
        var value = global::System.TimeSpan.FromHours(8);
        var result = _domain.Subtract(value, 3);

        Assert.Equal(global::System.TimeSpan.FromHours(5), result);
    }
}
