using Intervals.NET.Domain.Default.Numeric;
using RangeFactory = Intervals.NET.Factories.Range;

namespace Intervals.NET.Domain.Extensions.Tests;

/// <summary>
/// Tests for Common domain extension methods (performance-agnostic operations).
/// Tests the extension methods in Intervals.NET.Domain.Extensions.CommonRangeDomainExtensions.
/// </summary>
public class CommonRangeDomainExtensionsTests
{
    #region Shift Tests

    [Fact]
    public void Shift_IntegerRange_ShiftsCorrectly()
    {
        // Arrange
        var range = RangeFactory.Closed<int>(10, 20);
        var domain = new IntegerFixedStepDomain();

        // Act
        var shifted = range.Shift(domain, 5);

        // Assert
        Assert.Equal(15, shifted.Start.Value);
        Assert.Equal(25, shifted.End.Value);
        Assert.True(shifted.IsStartInclusive);
        Assert.True(shifted.IsEndInclusive);
    }

    [Fact]
    public void Shift_IntegerRangeNegativeOffset_ShiftsCorrectly()
    {
        // Arrange
        var range = RangeFactory.Closed<int>(10, 20);
        var domain = new IntegerFixedStepDomain();

        // Act
        var shifted = range.Shift(domain, -3);

        // Assert
        Assert.Equal(7, shifted.Start.Value);
        Assert.Equal(17, shifted.End.Value);
    }

    [Fact]
    public void Shift_UnboundedRange_PreservesInfinity()
    {
        // Arrange
        var range = RangeFactory.Closed(RangeValue<int>.NegativeInfinity, 10);
        var domain = new IntegerFixedStepDomain();

        // Act
        var shifted = range.Shift(domain, 5);

        // Assert
        Assert.False(shifted.Start.IsFinite);
        Assert.True(shifted.Start.IsNegativeInfinity);
        Assert.Equal(15, shifted.End.Value);
    }

    #endregion

    #region Expand Tests

    [Fact]
    public void Expand_IntegerRange_ExpandsCorrectly()
    {
        // Arrange
        var range = RangeFactory.Closed<int>(10, 20);
        var domain = new IntegerFixedStepDomain();

        // Act
        var expanded = range.Expand(domain, left: 2, right: 3);

        // Assert
        Assert.Equal(8, expanded.Start.Value);
        Assert.Equal(23, expanded.End.Value);
    }

    [Fact]
    public void Expand_IntegerRangeNegativeValues_ContractsRange()
    {
        // Arrange
        var range = RangeFactory.Closed<int>(10, 20);
        var domain = new IntegerFixedStepDomain();

        // Act
        var expanded = range.Expand(domain, left: -2, right: -3);

        // Assert
        Assert.Equal(12, expanded.Start.Value);
        Assert.Equal(17, expanded.End.Value);
    }

    [Fact]
    public void Expand_UnboundedRange_PreservesInfinity()
    {
        // Arrange
        var range = RangeFactory.Closed(RangeValue<int>.NegativeInfinity, 10);
        var domain = new IntegerFixedStepDomain();

        // Act
        var expanded = range.Expand(domain, left: 5, right: 5);

        // Assert
        Assert.False(expanded.Start.IsFinite);
        Assert.True(expanded.Start.IsNegativeInfinity);
        Assert.Equal(15, expanded.End.Value);
    }

    #endregion
}
