using Intervals.NET.Domain.Default.Numeric;

namespace Intervals.NET.Domain.Default.Tests.Numeric;

public class UShortFixedStepDomainTests
{
    private readonly UShortFixedStepDomain _domain = new();

    [Theory]
    [InlineData((ushort)0, (ushort)0)]
    [InlineData((ushort)32768, (ushort)32768)]
    [InlineData((ushort)65535, (ushort)65535)]
    public void Floor_ReturnsValueItself(ushort value, ushort expected)
    {
        var result = _domain.Floor(value);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData((ushort)0, (ushort)0)]
    [InlineData((ushort)32768, (ushort)32768)]
    [InlineData((ushort)65535, (ushort)65535)]
    public void Ceiling_ReturnsValueItself(ushort value, ushort expected)
    {
        var result = _domain.Ceiling(value);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData((ushort)100, (ushort)200, 100L)]
    [InlineData((ushort)0, (ushort)65535, 65535L)]
    [InlineData((ushort)1000, (ushort)500, -500L)]
    public void Distance_CalculatesCorrectly(ushort start, ushort end, long expected)
    {
        var result = _domain.Distance(start, end);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData((ushort)100, 50L, (ushort)150)]
    [InlineData((ushort)1000, -500L, (ushort)500)]
    [InlineData((ushort)0, 100L, (ushort)100)]
    public void Add_AddsOffsetCorrectly(ushort value, long offset, ushort expected)
    {
        var result = _domain.Add(value, offset);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Add_ThrowsOverflowException_WhenExceedsMaxValue()
    {
        Assert.Throws<OverflowException>(() => _domain.Add(65535, 1));
    }

    [Fact]
    public void Add_ThrowsOverflowException_WhenBelowMinValue()
    {
        Assert.Throws<OverflowException>(() => _domain.Add(0, -1));
    }

    [Fact]
    public void Subtract_SubtractsCorrectly()
    {
        // Arrange & Act
        var result = _domain.Subtract((ushort)100, 25);

        // Assert
        Assert.Equal((ushort)75, result);
    }
}
