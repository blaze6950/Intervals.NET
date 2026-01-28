using Intervals.NET.Domain.Default.Numeric;

namespace Intervals.NET.Domain.Default.Tests.Numeric;

public class FloatFixedStepDomainTests
{
    private readonly FloatFixedStepDomain _domain = new();

    [Theory]
    [InlineData(10.7f, 10.0f)]
    [InlineData(10.0f, 10.0f)]
    [InlineData(-10.3f, -11.0f)]
    [InlineData(0.5f, 0.0f)]
    public void Floor_RoundsDownToNearestInteger(float value, float expected)
    {
        var result = _domain.Floor(value);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(10.3f, 11.0f)]
    [InlineData(10.0f, 10.0f)]
    [InlineData(-10.7f, -10.0f)]
    [InlineData(0.1f, 1.0f)]
    public void Ceiling_RoundsUpToNearestInteger(float value, float expected)
    {
        var result = _domain.Ceiling(value);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(10.5f, 20.5f, 10L)]
    [InlineData(0.0f, 100.0f, 100L)]
    [InlineData(20.9f, 10.1f, -10L)]
    [InlineData(-5.5f, 5.5f, 11L)]
    public void Distance_CalculatesDiscreteSteps(float start, float end, long expected)
    {
        var result = _domain.Distance(start, end);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(10.0f, 5L, 15.0f)]
    [InlineData(10.0f, -5L, 5.0f)]
    [InlineData(0.0f, 100L, 100.0f)]
    public void Add_AddsStepsCorrectly(float value, long offset, float expected)
    {
        var result = _domain.Add(value, offset);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(20.0f, 5L, 15.0f)]
    [InlineData(10.0f, -5L, 15.0f)]
    public void Subtract_SubtractsStepsCorrectly(float value, long offset, float expected)
    {
        var result = _domain.Subtract(value, offset);
        Assert.Equal(expected, result);
    }
}
