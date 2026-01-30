using Intervals.NET.Domain.Default.DateTime;

namespace Intervals.NET.Domain.Default.Tests.DateTime;

/// <summary>
/// Tests for DateTimeYearFixedStepDomain validating year-based operations.
/// </summary>
public class DateTimeYearFixedStepDomainTests
{
    private readonly DateTimeYearFixedStepDomain _domain = new();

    #region Floor Tests

    [Fact]
    public void Floor_RoundsDownToJanuaryFirst()
    {
        // Arrange
        var input = new System.DateTime(2024, 6, 15, 14, 30, 45);

        // Act
        var result = _domain.Floor(input);

        // Assert
        Assert.Equal(new System.DateTime(2024, 1, 1), result);
    }

    [Fact]
    public void Floor_OnBoundary_ReturnsUnchanged()
    {
        // Arrange
        var input = new System.DateTime(2024, 1, 1, 0, 0, 0);

        // Act
        var result = _domain.Floor(input);

        // Assert
        Assert.Equal(input, result);
    }

    #endregion

    #region Ceiling Tests

    [Fact]
    public void Ceiling_OnBoundary_ReturnsUnchanged()
    {
        // Arrange
        var input = new System.DateTime(2024, 1, 1, 0, 0, 0);

        // Act
        var result = _domain.Ceiling(input);

        // Assert
        Assert.Equal(input, result);
    }

    [Fact]
    public void Ceiling_OffBoundary_ReturnsNextYear()
    {
        // Arrange
        var input = new System.DateTime(2024, 6, 15, 14, 30, 0);

        // Act
        var result = _domain.Ceiling(input);

        // Assert
        Assert.Equal(new System.DateTime(2025, 1, 1), result);
    }

    [Fact]
    public void Ceiling_LastDayOfYear_ReturnsNextYear()
    {
        // Arrange
        var input = new System.DateTime(2024, 12, 31, 23, 59, 59);

        // Act
        var result = _domain.Ceiling(input);

        // Assert
        Assert.Equal(new System.DateTime(2025, 1, 1), result);
    }

    #endregion

    #region Distance Tests

    [Fact]
    public void Distance_ForwardRange_ReturnsPositive()
    {
        // Arrange
        var start = new System.DateTime(2020, 6, 15);
        var end = new System.DateTime(2024, 3, 20);

        // Act
        var distance = _domain.Distance(start, end);

        // Assert
        Assert.Equal(4, distance);
    }

    [Fact]
    public void Distance_ReverseRange_ReturnsNegative()
    {
        // Arrange
        var start = new System.DateTime(2024, 3, 20);
        var end = new System.DateTime(2020, 6, 15);

        // Act
        var distance = _domain.Distance(start, end);

        // Assert
        Assert.Equal(-4, distance);
    }

    [Fact]
    public void Distance_SameYear_ReturnsZero()
    {
        // Arrange
        var start = new System.DateTime(2024, 3, 10);
        var end = new System.DateTime(2024, 9, 25);

        // Act
        var distance = _domain.Distance(start, end);

        // Assert
        Assert.Equal(0, distance);
    }

    [Fact]
    public void Distance_SymmetryProperty_Holds()
    {
        // Property: Distance(a,b) = -Distance(b,a)
        // Arrange
        var date1 = new System.DateTime(2020, 1, 15);
        var date2 = new System.DateTime(2025, 5, 20);

        // Act
        var forward = _domain.Distance(date1, date2);
        var backward = _domain.Distance(date2, date1);

        // Assert
        Assert.Equal(-forward, backward);
    }

    [Fact]
    public void Distance_OneYearApart_ReturnsOne()
    {
        // Arrange
        var start = new System.DateTime(2023, 5, 15);
        var end = new System.DateTime(2024, 7, 20);

        // Act
        var distance = _domain.Distance(start, end);

        // Assert
        Assert.Equal(1, distance);
    }

    #endregion

    #region Add Tests

    [Fact]
    public void Add_PositiveOffset_AddsYears()
    {
        // Arrange
        var date = new System.DateTime(2020, 3, 15, 10, 30, 0);

        // Act
        var result = _domain.Add(date, 5);

        // Assert
        Assert.Equal(new System.DateTime(2025, 3, 15, 10, 30, 0), result);
    }

    [Fact]
    public void Add_NegativeOffset_SubtractsYears()
    {
        // Arrange
        var date = new System.DateTime(2024, 5, 15, 10, 30, 0);

        // Act
        var result = _domain.Add(date, -3);

        // Assert
        Assert.Equal(new System.DateTime(2021, 5, 15, 10, 30, 0), result);
    }

    [Fact]
    public void Add_LargeValidOffset_Works()
    {
        // DateTime.AddYears has a limit of +/-10000 years
        // Arrange
        var date = new System.DateTime(2024, 1, 1);
        long offset = 5000; // Well within the limit

        // Act & Assert - Should not throw
        var result = _domain.Add(date, offset);
        
        Assert.NotEqual(date, result);
    }

    [Fact]
    public void Add_ExceedsIntMax_ThrowsArgumentOutOfRangeException()
    {
        // This test validates the overflow protection fix
        // Arrange
        var date = new System.DateTime(2024, 1, 1);
        long hugeOffset = (long)int.MaxValue + 1;

        // Act & Assert
        var ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => _domain.Add(date, hugeOffset));

        Assert.Equal("offset", ex.ParamName);
        Assert.Contains("must be between", ex.Message);
    }

    [Fact]
    public void Add_BelowIntMin_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var date = new System.DateTime(2024, 1, 1);
        long hugeOffset = (long)int.MinValue - 1;

        // Act & Assert
        var ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => _domain.Add(date, hugeOffset));

        Assert.Equal("offset", ex.ParamName);
    }

    #endregion

    #region Subtract Tests

    [Fact]
    public void Subtract_PositiveOffset_SubtractsYears()
    {
        // Arrange
        var date = new System.DateTime(2024, 5, 15, 10, 30, 0);

        // Act
        var result = _domain.Subtract(date, 3);

        // Assert
        Assert.Equal(new System.DateTime(2021, 5, 15, 10, 30, 0), result);
    }

    [Fact]
    public void Subtract_NegativeOffset_AddsYears()
    {
        // Arrange
        var date = new System.DateTime(2020, 1, 15, 10, 30, 0);

        // Act
        var result = _domain.Subtract(date, -5);

        // Assert
        Assert.Equal(new System.DateTime(2025, 1, 15, 10, 30, 0), result);
    }

    [Fact]
    public void Subtract_ExceedsIntMax_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var date = new System.DateTime(2024, 1, 1);
        long hugeOffset = (long)int.MaxValue + 1;

        // Act & Assert
        var ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => _domain.Subtract(date, hugeOffset));

        Assert.Equal("offset", ex.ParamName);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public void Add_FromLeapYearToNonLeapYear_HandlesCorrectly()
    {
        // Arrange
        var date = new System.DateTime(2024, 2, 29); // Leap year

        // Act
        var result = _domain.Add(date, 1); // Add 1 year to non-leap year

        // Assert
        Assert.Equal(new System.DateTime(2025, 2, 28), result); // Adjusts to Feb 28
    }

    [Fact]
    public void Distance_WithTimeComponents_IgnoresTime()
    {
        // Arrange
        var start = new System.DateTime(2020, 12, 31, 23, 59, 59);
        var end = new System.DateTime(2024, 1, 1, 0, 0, 1);

        // Act
        var distance = _domain.Distance(start, end);

        // Assert
        Assert.Equal(4, distance); // Only year difference matters
    }

    [Fact]
    public void Distance_ZeroYears_ReturnsZero()
    {
        // Arrange
        var start = new System.DateTime(2024, 1, 1);
        var end = new System.DateTime(2024, 12, 31);

        // Act
        var distance = _domain.Distance(start, end);

        // Assert
        Assert.Equal(0, distance);
    }

    #endregion
}
