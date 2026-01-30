using Intervals.NET.Domain.Default.TimeSpan;

namespace Intervals.NET.Domain.Default.Tests.TimeSpan;

public class TimeSpanTickFixedStepDomainTests
{
    private readonly TimeSpanTickFixedStepDomain _domain = new();

    [Fact]
    public void Floor_ReturnsValueItself_BecauseTickIsFinestGranularity()
    {
        // Arrange - any TimeSpan value
        var value = global::System.TimeSpan.FromTicks(123456789);

        // Act
        var result = _domain.Floor(value);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void Ceiling_ReturnsValueItself_BecauseTickIsFinestGranularity()
    {
        // Arrange
        var value = global::System.TimeSpan.FromTicks(123456789);

        // Act
        var result = _domain.Ceiling(value);

        // Assert
        Assert.Equal(value, result);
    }

    [Theory]
    [InlineData(1000L, 2000L, 1000L)]
    [InlineData(0L, 10000000L, 10000000L)] // 1 second in ticks
    [InlineData(2000L, 1000L, -1000L)]
    public void Distance_CalculatesTicksCorrectly(long startTicks, long endTicks, long expected)
    {
        // Arrange
        var start = global::System.TimeSpan.FromTicks(startTicks);
        var end = global::System.TimeSpan.FromTicks(endTicks);

        // Act
        var result = _domain.Distance(start, end);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Add_AddsTicksCorrectly()
    {
        // Arrange
        var value = global::System.TimeSpan.FromTicks(1000);
        long offset = 500;

        // Act
        var result = _domain.Add(value, offset);

        // Assert
        Assert.Equal(global::System.TimeSpan.FromTicks(1500), result);
    }

    [Fact]
    public void Subtract_SubtractsTicksCorrectly()
    {
        // Arrange
        var value = global::System.TimeSpan.FromTicks(1000);
        long offset = 500;

        // Act
        var result = _domain.Subtract(value, offset);

        // Assert
        Assert.Equal(global::System.TimeSpan.FromTicks(500), result);
    }
}
