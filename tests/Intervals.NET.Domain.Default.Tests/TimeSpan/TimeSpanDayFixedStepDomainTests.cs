using Intervals.NET.Domain.Default.TimeSpan;

namespace Intervals.NET.Domain.Default.Tests.TimeSpan;

public class TimeSpanDayFixedStepDomainTests
{
    private readonly TimeSpanDayFixedStepDomain _domain = new();

    [Fact]
    public void Floor_RoundsDownToNearestDay()
    {
        // Arrange - 2 days and 12 hours
        var value = global::System.TimeSpan.FromHours(60);

        // Act
        var result = _domain.Floor(value);

        // Assert
        Assert.Equal(global::System.TimeSpan.FromDays(2), result);
    }

    [Fact]
    public void Ceiling_RoundsUpToNearestDay()
    {
        // Arrange - 2 days and 1 hour
        var value = global::System.TimeSpan.FromHours(49);

        // Act
        var result = _domain.Ceiling(value);

        // Assert
        Assert.Equal(global::System.TimeSpan.FromDays(3), result);
    }

    [Theory]
    [InlineData(1, 5, 4L)]
    [InlineData(0, 7, 7L)]
    [InlineData(10, 5, -5L)]
    public void Distance_CalculatesDaysCorrectly(int startDays, int endDays, long expected)
    {
        var start = global::System.TimeSpan.FromDays(startDays);
        var end = global::System.TimeSpan.FromDays(endDays);

        var result = _domain.Distance(start, end);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Add_AddsDaysCorrectly()
    {
        var value = global::System.TimeSpan.FromDays(5);
        var result = _domain.Add(value, 3);

        Assert.Equal(global::System.TimeSpan.FromDays(8), result);
    }

    [Fact]
    public void Subtract_SubtractsDaysCorrectly()
    {
        var value = global::System.TimeSpan.FromDays(8);
        var result = _domain.Subtract(value, 3);

        Assert.Equal(global::System.TimeSpan.FromDays(5), result);
    }
}
