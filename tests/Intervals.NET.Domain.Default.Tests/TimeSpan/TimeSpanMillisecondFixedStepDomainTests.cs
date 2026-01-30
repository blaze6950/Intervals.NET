using Intervals.NET.Domain.Default.TimeSpan;

namespace Intervals.NET.Domain.Default.Tests.TimeSpan;

public class TimeSpanMillisecondFixedStepDomainTests
{
    private readonly TimeSpanMillisecondFixedStepDomain _domain = new();

    [Fact]
    public void Floor_RoundsDownToNearestMillisecond()
    {
        // Arrange - 10ms and 5 ticks (500ns)
        var value = global::System.TimeSpan.FromTicks(100005);

        // Act
        var result = _domain.Floor(value);

        // Assert
        Assert.Equal(global::System.TimeSpan.FromMilliseconds(10), result);
    }

    [Fact]
    public void Ceiling_RoundsUpToNearestMillisecond()
    {
        // Arrange - 10ms and 1 tick
        var value = global::System.TimeSpan.FromTicks(100001);

        // Act
        var result = _domain.Ceiling(value);

        // Assert
        Assert.Equal(global::System.TimeSpan.FromMilliseconds(11), result);
    }

    [Theory]
    [InlineData(10, 20, 10L)]
    [InlineData(0, 1000, 1000L)]
    [InlineData(500, 250, -250L)]
    public void Distance_CalculatesMillisecondsCorrectly(int startMs, int endMs, long expectedDistance)
    {
        var start = global::System.TimeSpan.FromMilliseconds(startMs);
        var end = global::System.TimeSpan.FromMilliseconds(endMs);

        var result = _domain.Distance(start, end);

        Assert.Equal(expectedDistance, result);
    }

    [Fact]
    public void Add_AddsMillisecondsCorrectly()
    {
        var value = global::System.TimeSpan.FromMilliseconds(100);
        var result = _domain.Add(value, 50);

        Assert.Equal(global::System.TimeSpan.FromMilliseconds(150), result);
    }

    [Fact]
    public void Subtract_SubtractsMillisecondsCorrectly()
    {
        var value = global::System.TimeSpan.FromMilliseconds(100);
        var result = _domain.Subtract(value, 50);

        Assert.Equal(global::System.TimeSpan.FromMilliseconds(50), result);
    }
}
