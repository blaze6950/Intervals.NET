using Intervals.NET.Domain.Default.TimeSpan;

namespace Intervals.NET.Domain.Default.Tests.TimeSpan;

public class TimeSpanMicrosecondFixedStepDomainTests
{
    private readonly TimeSpanMicrosecondFixedStepDomain _domain = new();

    [Fact]
    public void Floor_RoundsDownToNearestMicrosecond()
    {
        // Arrange - 10 microseconds and 5 ticks (500ns)
        var value = global::System.TimeSpan.FromTicks(105);

        // Act
        var result = _domain.Floor(value);

        // Assert - should be 10 microseconds (100 ticks)
        Assert.Equal(global::System.TimeSpan.FromTicks(100), result);
    }

    [Fact]
    public void Ceiling_RoundsUpToNearestMicrosecond()
    {
        // Arrange - 10 microseconds and 1 tick
        var value = global::System.TimeSpan.FromTicks(101);

        // Act
        var result = _domain.Ceiling(value);

        // Assert - should be 11 microseconds (110 ticks)
        Assert.Equal(global::System.TimeSpan.FromTicks(110), result);
    }

    [Fact]
    public void Ceiling_ReturnsValueItself_WhenOnBoundary()
    {
        // Arrange - exactly 10 microseconds
        var value = global::System.TimeSpan.FromTicks(100);

        // Act
        var result = _domain.Ceiling(value);

        // Assert
        Assert.Equal(value, result);
    }

    [Theory]
    [InlineData(100L, 200L, 10L)] // 10 microseconds to 20 microseconds = 10μs distance
    [InlineData(0L, 10000L, 1000L)] // 0 to 1ms = 1000 microseconds
    public void Distance_CalculatesMicrosecondsCorrectly(long startTicks, long endTicks, long expected)
    {
        var start = global::System.TimeSpan.FromTicks(startTicks);
        var end = global::System.TimeSpan.FromTicks(endTicks);

        var result = _domain.Distance(start, end);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Add_AddsMicrosecondsCorrectly()
    {
        var value = global::System.TimeSpan.FromTicks(100); // 10μs
        var result = _domain.Add(value, 5); // Add 5μs

        Assert.Equal(global::System.TimeSpan.FromTicks(150), result); // 15μs
    }

    [Fact]
    public void Subtract_SubtractsMicrosecondsCorrectly()
    {
        var value = global::System.TimeSpan.FromTicks(150); // 15μs
        var result = _domain.Subtract(value, 5); // Subtract 5μs

        Assert.Equal(global::System.TimeSpan.FromTicks(100), result); // 10μs
    }
}
