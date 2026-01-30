using Intervals.NET.Domain.Default.Numeric;

namespace Intervals.NET.Domain.Default.Tests.Numeric;

public class IntegerFixedStepDomainTests
{
    private readonly IntegerFixedStepDomain _domain = new();

    [Fact]
    public void Add_AddsOffsetCorrectly()
    {
        // Arrange & Act
        var result = _domain.Add(10, 5);

        // Assert
        Assert.Equal(15, result);
    }

    [Fact]
    public void Ceiling_ReturnsUnchanged()
    {
        // Arrange & Act - Integers don't need rounding
        var result = _domain.Ceiling(42);

        // Assert
        Assert.Equal(42, result);
    }

    [Fact]
    public void Floor_ReturnsUnchanged()
    {
        // Arrange & Act - Integers don't need rounding
        var result = _domain.Floor(42);

        // Assert
        Assert.Equal(42, result);
    }

    [Fact]
    public void Distance_CalculatesCorrectly()
    {
        // Arrange & Act
        var result = _domain.Distance(10, 50);

        // Assert
        Assert.Equal(40, result);
    }

    [Fact]
    public void Subtract_SubtractsCorrectly()
    {
        // Arrange & Act
        var result = _domain.Subtract(100, 25);

        // Assert
        Assert.Equal(75, result);
    }
}
