using Intervals.NET.Domain.Default.TimeSpan;

namespace Intervals.NET.Domain.Default.Tests.TimeSpan;

public class TimeSpanMinuteFixedStepDomainTests
{
    private readonly TimeSpanMinuteFixedStepDomain _domain = new();

    [Fact]
    public void Floor_RoundsDownToNearestMinute()
    {
        // Arrange - 10 minutes 30 seconds
        var value = global::System.TimeSpan.FromSeconds(630);

        // Act
        var result = _domain.Floor(value);

        // Assert
        Assert.Equal(global::System.TimeSpan.FromMinutes(10), result);
    }

    [Fact]
    public void Ceiling_RoundsUpToNearestMinute()
    {
        // Arrange - 10 minutes and 1 second
        var value = global::System.TimeSpan.FromSeconds(601);

        // Act
        var result = _domain.Ceiling(value);

        // Assert
        Assert.Equal(global::System.TimeSpan.FromMinutes(11), result);
    }

    [Theory]
    [InlineData(10, 20, 10L)]
    [InlineData(0, 60, 60L)]
    [InlineData(60, 30, -30L)]
    public void Distance_CalculatesMinutesCorrectly(int startMinutes, int endMinutes, long expected)
    {
        var start = global::System.TimeSpan.FromMinutes(startMinutes);
        var end = global::System.TimeSpan.FromMinutes(endMinutes);

        var result = _domain.Distance(start, end);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Add_AddsMinutesCorrectly()
    {
        var value = global::System.TimeSpan.FromMinutes(30);
        var result = _domain.Add(value, 15);

        Assert.Equal(global::System.TimeSpan.FromMinutes(45), result);
    }

    [Fact]
    public void Subtract_SubtractsMinutesCorrectly()
    {
        var value = global::System.TimeSpan.FromMinutes(45);
        var result = _domain.Subtract(value, 15);

        Assert.Equal(global::System.TimeSpan.FromMinutes(30), result);
    }
}
