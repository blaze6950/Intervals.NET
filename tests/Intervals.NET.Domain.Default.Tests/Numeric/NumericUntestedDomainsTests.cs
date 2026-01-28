using Intervals.NET.Domain.Default.Numeric;

namespace Intervals.NET.Domain.Default.Tests.Numeric;

public class NumericUntestedDomainsTests
{
    [Fact]
    public void Decimal_Add_WorksCorrectly()
    {
        // Arrange
        var domain = new DecimalFixedStepDomain();

        // Act
        var result = domain.Add(10.5m, 5);

        // Assert
        Assert.Equal(15.5m, result);
    }

    [Fact]
    public void Decimal_Ceiling_RoundsUpCorrectly()
    {
        // Arrange
        var domain = new DecimalFixedStepDomain();

        // Act
        var result = domain.Ceiling(10.3m);

        // Assert
        Assert.Equal(11.0m, result);
    }

    [Fact]
    public void Decimal_Distance_CalculatesCorrectly()
    {
        // Arrange
        var domain = new DecimalFixedStepDomain();

        // Act
        var result = domain.Distance(10.5m, 20.5m);

        // Assert
        Assert.Equal(10, result);
    }

    [Fact]
    public void Double_Add_WorksCorrectly()
    {
        // Arrange
        var domain = new DoubleFixedStepDomain();

        // Act
        var result = domain.Add(10.5, 5);

        // Assert
        Assert.Equal(15.5, result);
    }

    [Fact]
    public void Double_Ceiling_RoundsUpCorrectly()
    {
        // Arrange
        var domain = new DoubleFixedStepDomain();

        // Act
        var result = domain.Ceiling(10.3);

        // Assert
        Assert.Equal(11.0, result);
    }

    [Fact]
    public void Double_Distance_CalculatesCorrectly()
    {
        // Arrange
        var domain = new DoubleFixedStepDomain();

        // Act
        var result = domain.Distance(10.5, 20.5);

        // Assert
        Assert.Equal(10, result);
    }

    [Fact]
    public void Long_Add_WorksCorrectly()
    {
        // Arrange
        var domain = new LongFixedStepDomain();

        // Act
        var result = domain.Add(100L, 50);

        // Assert
        Assert.Equal(150L, result);
    }

    [Fact]
    public void Long_Ceiling_ReturnsUnchanged()
    {
        // Arrange
        var domain = new LongFixedStepDomain();

        // Act
        var result = domain.Ceiling(42L);

        // Assert
        Assert.Equal(42L, result);
    }

    [Fact]
    public void Long_Distance_CalculatesCorrectly()
    {
        // Arrange
        var domain = new LongFixedStepDomain();

        // Act
        var result = domain.Distance(100L, 250L);

        // Assert
        Assert.Equal(150L, result);
    }
}
