using Intervals.NET.Domain.Default.Numeric;

namespace Intervals.NET.Domain.Default.Tests.Numeric;

public class UIntFixedStepDomainTests
{
    private readonly UIntFixedStepDomain _domain = new();

    [Theory]
    [InlineData(0u, 0u)]
    [InlineData(2147483648u, 2147483648u)]
    [InlineData(4294967295u, 4294967295u)]
    public void Floor_ReturnsValueItself(uint value, uint expected)
    {
        var result = _domain.Floor(value);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1000u, 2000u, 1000L)]
    [InlineData(0u, 4294967295u, 4294967295L)]
    [InlineData(2000u, 1000u, -1000L)]
    public void Distance_CalculatesCorrectly(uint start, uint end, long expected)
    {
        var result = _domain.Distance(start, end);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1000u, 500L, 1500u)]
    [InlineData(1000u, -500L, 500u)]
    public void Add_AddsOffsetCorrectly(uint value, long offset, uint expected)
    {
        var result = _domain.Add(value, offset);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Add_ThrowsOverflowException_WhenExceedsMaxValue()
    {
        Assert.Throws<OverflowException>(() => _domain.Add(uint.MaxValue, 1));
    }

    [Fact]
    public void Add_ThrowsOverflowException_WhenBelowMinValue()
    {
        Assert.Throws<OverflowException>(() => _domain.Add(0, -1));
    }

    [Fact]
    public void Ceiling_ReturnsUnchanged()
    {
        // Arrange & Act - Unsigned integers don't need rounding
        var result = _domain.Ceiling(42u);

        // Assert
        Assert.Equal(42u, result);
    }

    [Fact]
    public void Subtract_SubtractsCorrectly()
    {
        // Arrange & Act
        var result = _domain.Subtract(100u, 25);

        // Assert
        Assert.Equal(75u, result);
    }
}
