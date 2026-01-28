using Intervals.NET.Domain.Default.Numeric;

namespace Intervals.NET.Domain.Default.Tests.Numeric;

public class ULongFixedStepDomainTests
{
    private readonly ULongFixedStepDomain _domain = new();

    [Theory]
    [InlineData(0ul, 0ul)]
    [InlineData(9223372036854775808ul, 9223372036854775808ul)]
    [InlineData(18446744073709551615ul, 18446744073709551615ul)]
    public void Floor_ReturnsValueItself(ulong value, ulong expected)
    {
        var result = _domain.Floor(value);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1000ul, 2000ul, 1000L)]
    [InlineData(0ul, 100000ul, 100000L)]
    [InlineData(2000ul, 1000ul, -1000L)]
    public void Distance_CalculatesCorrectly(ulong start, ulong end, long expected)
    {
        var result = _domain.Distance(start, end);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Distance_ClampsToLongMaxValue_WhenDistanceExceedsLongMax()
    {
        // Arrange
        ulong start = 0;
        ulong end = ulong.MaxValue; // Distance would be > long.MaxValue

        // Act
        var result = _domain.Distance(start, end);

        // Assert
        Assert.Equal(long.MaxValue, result);
    }

    [Theory]
    [InlineData(1000ul, 500L, 1500ul)]
    [InlineData(1000ul, -500L, 500ul)]
    public void Add_AddsOffsetCorrectly(ulong value, long offset, ulong expected)
    {
        var result = _domain.Add(value, offset);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Add_ThrowsOverflowException_WhenExceedsMaxValue()
    {
        Assert.Throws<OverflowException>(() => _domain.Add(ulong.MaxValue, 1));
    }

    [Fact]
    public void Add_ThrowsOverflowException_WhenBelowMinValue()
    {
        Assert.Throws<OverflowException>(() => _domain.Add(0, -1));
    }

    [Fact]
    public void Subtract_HandlesPositiveOffset()
    {
        var result = _domain.Subtract(1000ul, 500L);
        Assert.Equal(500ul, result);
    }

    [Fact]
    public void Subtract_HandlesNegativeOffset()
    {
        var result = _domain.Subtract(1000ul, -500L);
        Assert.Equal(1500ul, result);
    }
}
