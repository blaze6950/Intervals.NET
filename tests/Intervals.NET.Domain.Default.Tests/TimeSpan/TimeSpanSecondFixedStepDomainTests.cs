using Intervals.NET.Domain.Default.TimeSpan;

namespace Intervals.NET.Domain.Default.Tests.TimeSpan;

public class TimeSpanSecondFixedStepDomainTests
{
    private readonly TimeSpanSecondFixedStepDomain _domain = new();

    [Fact]
    public void Floor_RoundsDownToNearestSecond()
    {
        // Arrange - 10 seconds and 500 milliseconds
        var value = global::System.TimeSpan.FromSeconds(10.5);

        // Act
        var result = _domain.Floor(value);

        // Assert
        Assert.Equal(global::System.TimeSpan.FromSeconds(10), result);
    }

    [Fact]
    public void Floor_ReturnsValueItself_WhenAlreadyOnSecondBoundary()
    {
        // Arrange
        var value = global::System.TimeSpan.FromSeconds(10);

        // Act
        var result = _domain.Floor(value);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void Ceiling_RoundsUpToNearestSecond()
    {
        // Arrange - 10 seconds and 1 millisecond
        var value = global::System.TimeSpan.FromMilliseconds(10001);

        // Act
        var result = _domain.Ceiling(value);

        // Assert
        Assert.Equal(global::System.TimeSpan.FromSeconds(11), result);
    }

    [Fact]
    public void Ceiling_ReturnsValueItself_WhenAlreadyOnSecondBoundary()
    {
        // Arrange
        var value = global::System.TimeSpan.FromSeconds(10);

        // Act
        var result = _domain.Ceiling(value);

        // Assert
        Assert.Equal(value, result);
    }

    [Theory]
    [InlineData(10, 20, 10L)]
    [InlineData(0, 60, 60L)]
    [InlineData(60, 30, -30L)]
    public void Distance_CalculatesSecondsCorrectly(int startSeconds, int endSeconds, long expectedDistance)
    {
        // Arrange
        var start = global::System.TimeSpan.FromSeconds(startSeconds);
        var end = global::System.TimeSpan.FromSeconds(endSeconds);

        // Act
        var result = _domain.Distance(start, end);

        // Assert
        Assert.Equal(expectedDistance, result);
    }

    [Fact]
    public void Add_AddsSecondsCorrectly()
    {
        // Arrange
        var value = global::System.TimeSpan.FromSeconds(10);
        long offset = 5;

        // Act
        var result = _domain.Add(value, offset);

        // Assert
        Assert.Equal(global::System.TimeSpan.FromSeconds(15), result);
    }

    [Fact]
    public void Add_HandlesNegativeOffset()
    {
        // Arrange
        var value = global::System.TimeSpan.FromSeconds(10);
        long offset = -5;

        // Act
        var result = _domain.Add(value, offset);

        // Assert
        Assert.Equal(global::System.TimeSpan.FromSeconds(5), result);
    }

    [Fact]
    public void Subtract_SubtractsSecondsCorrectly()
    {
        // Arrange
        var value = global::System.TimeSpan.FromSeconds(20);
        long offset = 5;

        // Act
        var result = _domain.Subtract(value, offset);

        // Assert
        Assert.Equal(global::System.TimeSpan.FromSeconds(15), result);
    }

    [Fact]
    public void Subtract_HandlesNegativeOffset()
    {
        // Arrange
        var value = global::System.TimeSpan.FromSeconds(10);
        long offset = -5;

        // Act
        var result = _domain.Subtract(value, offset);

        // Assert
        Assert.Equal(global::System.TimeSpan.FromSeconds(15), result);
    }
}
