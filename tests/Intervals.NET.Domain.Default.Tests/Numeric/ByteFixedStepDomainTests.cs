using Intervals.NET.Domain.Default.Numeric;

namespace Intervals.NET.Domain.Default.Tests.Numeric;

public class ByteFixedStepDomainTests
{
    private readonly ByteFixedStepDomain _domain = new();

    [Theory]
    [InlineData((byte)0, (byte)0)]
    [InlineData((byte)127, (byte)127)]
    [InlineData((byte)255, (byte)255)]
    public void Floor_ReturnsValueItself(byte value, byte expected)
    {
        var result = _domain.Floor(value);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData((byte)0, (byte)0)]
    [InlineData((byte)127, (byte)127)]
    [InlineData((byte)255, (byte)255)]
    public void Ceiling_ReturnsValueItself(byte value, byte expected)
    {
        var result = _domain.Ceiling(value);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData((byte)10, (byte)20, 10L)]
    [InlineData((byte)0, (byte)255, 255L)]
    [InlineData((byte)100, (byte)50, -50L)]
    public void Distance_CalculatesCorrectly(byte start, byte end, long expected)
    {
        var result = _domain.Distance(start, end);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData((byte)10, 5L, (byte)15)]
    [InlineData((byte)100, -50L, (byte)50)]
    [InlineData((byte)0, 255L, (byte)255)]
    public void Add_AddsOffsetCorrectly(byte value, long offset, byte expected)
    {
        var result = _domain.Add(value, offset);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Add_ThrowsOverflowException_WhenExceedsMaxValue()
    {
        Assert.Throws<OverflowException>(() => _domain.Add(255, 1));
    }

    [Fact]
    public void Add_ThrowsOverflowException_WhenBelowMinValue()
    {
        Assert.Throws<OverflowException>(() => _domain.Add(0, -1));
    }
}
