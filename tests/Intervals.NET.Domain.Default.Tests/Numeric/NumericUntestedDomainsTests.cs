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
    public void Decimal_Add_WithNegativeOffset_WorksCorrectly()
    {
        // Arrange
        var domain = new DecimalFixedStepDomain();

        // Act
        var result = domain.Add(10.5m, -3);

        // Assert
        Assert.Equal(7.5m, result);
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
    public void Decimal_Ceiling_OnExactValue_ReturnsUnchanged()
    {
        // Arrange
        var domain = new DecimalFixedStepDomain();

        // Act
        var result = domain.Ceiling(10.0m);

        // Assert
        Assert.Equal(10.0m, result);
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
    public void Decimal_Distance_WithNegativeRange_ReturnsNegative()
    {
        // Arrange
        var domain = new DecimalFixedStepDomain();

        // Act
        var result = domain.Distance(20.5m, 10.5m);

        // Assert
        Assert.Equal(-10, result);
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
    public void Double_Add_WithNegativeOffset_WorksCorrectly()
    {
        // Arrange
        var domain = new DoubleFixedStepDomain();

        // Act
        var result = domain.Add(10.5, -3);

        // Assert
        Assert.Equal(7.5, result);
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
    public void Double_Ceiling_OnExactValue_ReturnsUnchanged()
    {
        // Arrange
        var domain = new DoubleFixedStepDomain();

        // Act
        var result = domain.Ceiling(10.0);

        // Assert
        Assert.Equal(10.0, result);
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
    public void Double_Distance_WithNegativeRange_ReturnsNegative()
    {
        // Arrange
        var domain = new DoubleFixedStepDomain();

        // Act
        var result = domain.Distance(20.5, 10.5);

        // Assert
        Assert.Equal(-10, result);
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
    public void Long_Add_WithNegativeOffset_WorksCorrectly()
    {
        // Arrange
        var domain = new LongFixedStepDomain();

        // Act
        var result = domain.Add(100L, -30);

        // Assert
        Assert.Equal(70L, result);
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

    [Fact]
    public void Long_Distance_WithNegativeRange_ReturnsNegative()
    {
        // Arrange
        var domain = new LongFixedStepDomain();

        // Act
        var result = domain.Distance(250L, 100L);

        // Assert
        Assert.Equal(-150L, result);
    }

    [Fact]
    public void Decimal_Floor_RoundsDownCorrectly()
    {
        // Arrange
        var domain = new DecimalFixedStepDomain();

        // Act
        var result = domain.Floor(10.8m);

        // Assert
        Assert.Equal(10.0m, result);
    }

    [Fact]
    public void Decimal_Subtract_SubtractsCorrectly()
    {
        // Arrange
        var domain = new DecimalFixedStepDomain();

        // Act
        var result = domain.Subtract(20.5m, 5);

        // Assert
        Assert.Equal(15.5m, result);
    }

    [Fact]
    public void Double_Floor_RoundsDownCorrectly()
    {
        // Arrange
        var domain = new DoubleFixedStepDomain();

        // Act
        var result = domain.Floor(10.8);

        // Assert
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void Double_Subtract_SubtractsCorrectly()
    {
        // Arrange
        var domain = new DoubleFixedStepDomain();

        // Act
        var result = domain.Subtract(20.5, 5);

        // Assert
        Assert.Equal(15.5, result);
    }

    [Fact]
    public void Long_Floor_ReturnsUnchanged()
    {
        // Arrange
        var domain = new LongFixedStepDomain();

        // Act
        var result = domain.Floor(42L);

        // Assert
        Assert.Equal(42L, result);
    }

    [Fact]
    public void Long_Subtract_SubtractsCorrectly()
    {
        // Arrange
        var domain = new LongFixedStepDomain();

        // Act
        var result = domain.Subtract(250L, 100);

        // Assert
        Assert.Equal(150L, result);
    }
}