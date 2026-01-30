using Intervals.NET.Domain.Default.Numeric;

namespace Intervals.NET.Domain.Default.Tests.Numeric;

public class SByteFixedStepDomainTests
{
    private readonly SByteFixedStepDomain _domain = new();

    [Theory]
    [InlineData((sbyte)0, (sbyte)0)]
    [InlineData((sbyte)-128, (sbyte)-128)]
    [InlineData((sbyte)127, (sbyte)127)]
    public void Floor_ReturnsValueItself(sbyte value, sbyte expected)
    {
        var result = _domain.Floor(value);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData((sbyte)0, (sbyte)0)]
    [InlineData((sbyte)-128, (sbyte)-128)]
    [InlineData((sbyte)127, (sbyte)127)]
    public void Ceiling_ReturnsValueItself(sbyte value, sbyte expected)
    {
        var result = _domain.Ceiling(value);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData((sbyte)-50, (sbyte)50, 100L)]
    [InlineData((sbyte)10, (sbyte)20, 10L)]
    [InlineData((sbyte)0, (sbyte)-10, -10L)]
    [InlineData((sbyte)-128, (sbyte)127, 255L)]
    public void Distance_CalculatesCorrectly(sbyte start, sbyte end, long expected)
    {
        var result = _domain.Distance(start, end);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData((sbyte)10, 5L, (sbyte)15)]
    [InlineData((sbyte)10, -5L, (sbyte)5)]
    [InlineData((sbyte)-10, 20L, (sbyte)10)]
    public void Add_AddsOffsetCorrectly(sbyte value, long offset, sbyte expected)
    {
        var result = _domain.Add(value, offset);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Add_ThrowsOverflowException_WhenExceedsMaxValue()
    {
        Assert.Throws<OverflowException>(() => _domain.Add(127, 1));
    }

    [Fact]
    public void Add_ThrowsOverflowException_WhenBelowMinValue()
    {
        Assert.Throws<OverflowException>(() => _domain.Add(-128, -1));
    }

    [Theory]
    [InlineData((sbyte)20, 5L, (sbyte)15)]
    [InlineData((sbyte)10, -5L, (sbyte)15)]
    public void Subtract_SubtractsOffsetCorrectly(sbyte value, long offset, sbyte expected)
    {
        var result = _domain.Subtract(value, offset);
        Assert.Equal(expected, result);
    }
}
