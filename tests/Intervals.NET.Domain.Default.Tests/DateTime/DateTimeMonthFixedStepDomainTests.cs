using Intervals.NET.Domain.Default.DateTime;

namespace Intervals.NET.Domain.Default.Tests.DateTime;

/// <summary>
/// Tests for DateTimeMonthFixedStepDomain validating month-based operations.
/// </summary>
public class DateTimeMonthFixedStepDomainTests
{
    private readonly DateTimeMonthFixedStepDomain _domain = new();

    #region Floor Tests

    [Fact]
    public void Floor_RoundsDownToFirstOfMonth()
    {
        // Arrange
        var input = new System.DateTime(2024, 3, 15, 14, 30, 45);

        // Act
        var result = _domain.Floor(input);

        // Assert
        Assert.Equal(new System.DateTime(2024, 3, 1), result);
    }

    [Fact]
    public void Floor_OnBoundary_ReturnsUnchanged()
    {
        // Arrange
        var input = new System.DateTime(2024, 3, 1, 0, 0, 0);

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
        var input = new System.DateTime(2024, 3, 1, 0, 0, 0);

        // Act
        var result = _domain.Ceiling(input);

        // Assert
        Assert.Equal(input, result);
    }

    [Fact]
    public void Ceiling_OffBoundary_ReturnsNextMonth()
    {
        // Arrange
        var input = new System.DateTime(2024, 3, 15, 14, 30, 0);

        // Act
        var result = _domain.Ceiling(input);

        // Assert
        Assert.Equal(new System.DateTime(2024, 4, 1), result);
    }

    [Fact]
    public void Ceiling_LastDayOfMonth_ReturnsNextMonth()
    {
        // Arrange
        var input = new System.DateTime(2024, 3, 31, 23, 59, 59);

        // Act
        var result = _domain.Ceiling(input);

        // Assert
        Assert.Equal(new System.DateTime(2024, 4, 1), result);
    }

    #endregion

    #region Distance Tests

    [Fact]
    public void Distance_ForwardRange_ReturnsPositive()
    {
        // Arrange
        var start = new System.DateTime(2024, 1, 15);
        var end = new System.DateTime(2024, 4, 20);

        // Act
        var distance = _domain.Distance(start, end);

        // Assert
        Assert.Equal(3, distance);
    }

    [Fact]
    public void Distance_ReverseRange_ReturnsNegative()
    {
        // This test validates the bug fix - previously returned 0
        // Arrange
        var start = new System.DateTime(2024, 4, 20);
        var end = new System.DateTime(2024, 1, 15);

        // Act
        var distance = _domain.Distance(start, end);

        // Assert
        Assert.Equal(-3, distance); // Was returning 0 before fix
    }

    [Fact]
    public void Distance_SameMonth_ReturnsZero()
    {
        // Arrange
        var start = new System.DateTime(2024, 3, 10);
        var end = new System.DateTime(2024, 3, 25);

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
        var date1 = new System.DateTime(2024, 1, 15);
        var date2 = new System.DateTime(2024, 5, 20);

        // Act
        var forward = _domain.Distance(date1, date2);
        var backward = _domain.Distance(date2, date1);

        // Assert
        Assert.Equal(-forward, backward);
    }

    [Fact]
    public void Distance_AcrossYears_CalculatesCorrectly()
    {
        // Arrange
        var start = new System.DateTime(2023, 10, 15);
        var end = new System.DateTime(2024, 3, 20);

        // Act
        var distance = _domain.Distance(start, end);

        // Assert
        Assert.Equal(5, distance); // Oct, Nov, Dec (2023), Jan, Feb, Mar (2024) = 5 months
    }

    [Fact]
    public void Distance_MultipleYears_CalculatesCorrectly()
    {
        // Arrange
        var start = new System.DateTime(2022, 3, 1);
        var end = new System.DateTime(2024, 6, 1);

        // Act
        var distance = _domain.Distance(start, end);

        // Assert
        Assert.Equal(27, distance); // 2 years + 3 months = 27 months
    }

    #endregion

    #region Add Tests

    [Fact]
    public void Add_PositiveOffset_AddsMonths()
    {
        // Arrange
        var date = new System.DateTime(2024, 1, 15, 10, 30, 0);

        // Act
        var result = _domain.Add(date, 3);

        // Assert
        Assert.Equal(new System.DateTime(2024, 4, 15, 10, 30, 0), result);
    }

    [Fact]
    public void Add_NegativeOffset_SubtractsMonths()
    {
        // Arrange
        var date = new System.DateTime(2024, 5, 15, 10, 30, 0);

        // Act
        var result = _domain.Add(date, -2);

        // Assert
        Assert.Equal(new System.DateTime(2024, 3, 15, 10, 30, 0), result);
    }

    [Fact]
    public void Add_AcrossYears_Works()
    {
        // Arrange
        var date = new System.DateTime(2023, 11, 15);

        // Act
        var result = _domain.Add(date, 4);

        // Assert
        Assert.Equal(new System.DateTime(2024, 3, 15), result);
    }

    [Fact]
    public void Add_LargeValidOffset_Works()
    {
        // DateTime.AddMonths has a limit - test with a reasonably large value
        // Arrange
        var date = new System.DateTime(2024, 1, 1);
        long offset = 1000; // 1000 months is safe

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
    public void Subtract_PositiveOffset_SubtractsMonths()
    {
        // Arrange
        var date = new System.DateTime(2024, 5, 15, 10, 30, 0);

        // Act
        var result = _domain.Subtract(date, 2);

        // Assert
        Assert.Equal(new System.DateTime(2024, 3, 15, 10, 30, 0), result);
    }

    [Fact]
    public void Subtract_NegativeOffset_AddsMonths()
    {
        // Arrange
        var date = new System.DateTime(2024, 1, 15, 10, 30, 0);

        // Act
        var result = _domain.Subtract(date, -3);

        // Assert
        Assert.Equal(new System.DateTime(2024, 4, 15, 10, 30, 0), result);
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
    public void Add_ToFebruary29LeapYear_HandlesCorrectly()
    {
        // Arrange
        var date = new System.DateTime(2024, 1, 31); // Jan 31 (leap year)

        // Act
        var result = _domain.Add(date, 1); // Add 1 month

        // Assert
        Assert.Equal(new System.DateTime(2024, 2, 29), result); // Feb 29 (leap year)
    }

    [Fact]
    public void Add_ToFebruaryNonLeapYear_HandlesCorrectly()
    {
        // Arrange
        var date = new System.DateTime(2023, 1, 31); // Jan 31 (non-leap year)

        // Act
        var result = _domain.Add(date, 1); // Add 1 month

        // Assert
        Assert.Equal(new System.DateTime(2023, 2, 28), result); // Feb 28 (non-leap year)
    }

    [Fact]
    public void Distance_WithTimeComponents_IgnoresTime()
    {
        // Arrange
        var start = new System.DateTime(2024, 1, 15, 23, 59, 59);
        var end = new System.DateTime(2024, 4, 1, 0, 0, 1);

        // Act
        var distance = _domain.Distance(start, end);

        // Assert
        Assert.Equal(3, distance); // Only month difference matters
    }

    #endregion
}
