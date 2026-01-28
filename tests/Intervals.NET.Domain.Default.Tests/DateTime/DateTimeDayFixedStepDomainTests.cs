using Intervals.NET.Domain.Default.DateTime;

namespace Intervals.NET.Domain.Default.Tests.DateTime;

/// <summary>
/// Tests for DateTimeDayFixedStepDomain validating day-based operations.
/// </summary>
public class DateTimeDayFixedStepDomainTests
{
    private readonly DateTimeDayFixedStepDomain _domain = new();

    #region Floor Tests

    [Fact]
    public void Floor_RoundsDownToMidnight()
    {
        // Arrange
        var input = new System.DateTime(2024, 3, 15, 14, 30, 45);

        // Act
        var result = _domain.Floor(input);

        // Assert
        Assert.Equal(new System.DateTime(2024, 3, 15, 0, 0, 0), result);
    }

    [Fact]
    public void Floor_AlreadyAtMidnight_ReturnsUnchanged()
    {
        // Arrange
        var input = new System.DateTime(2024, 3, 15, 0, 0, 0);

        // Act
        var result = _domain.Floor(input);

        // Assert
        Assert.Equal(input, result);
    }

    [Fact]
    public void Floor_OneSecondBeforeMidnight_ReturnsCurrentDay()
    {
        // Arrange
        var input = new System.DateTime(2024, 3, 15, 23, 59, 59);

        // Act
        var result = _domain.Floor(input);

        // Assert
        Assert.Equal(new System.DateTime(2024, 3, 15, 0, 0, 0), result);
    }

    #endregion

    #region Ceiling Tests

    [Fact]
    public void Ceiling_AtMidnight_ReturnsUnchanged()
    {
        // Arrange
        var input = new System.DateTime(2024, 3, 15, 0, 0, 0);

        // Act
        var result = _domain.Ceiling(input);

        // Assert
        Assert.Equal(input, result);
    }

    [Fact]
    public void Ceiling_WithTimeComponent_ReturnsNextDayMidnight()
    {
        // Arrange
        var input = new System.DateTime(2024, 3, 15, 14, 30, 0);

        // Act
        var result = _domain.Ceiling(input);

        // Assert
        Assert.Equal(new System.DateTime(2024, 3, 16, 0, 0, 0), result);
    }

    [Fact]
    public void Ceiling_OneSecondAfterMidnight_ReturnsNextDay()
    {
        // Arrange
        var input = new System.DateTime(2024, 3, 15, 0, 0, 1);

        // Act
        var result = _domain.Ceiling(input);

        // Assert
        Assert.Equal(new System.DateTime(2024, 3, 16, 0, 0, 0), result);
    }

    #endregion

    #region Distance Tests

    [Fact]
    public void Distance_SameDay_ReturnsZero()
    {
        // Arrange
        var start = new System.DateTime(2024, 3, 15, 10, 0, 0);
        var end = new System.DateTime(2024, 3, 15, 20, 0, 0);

        // Act
        var distance = _domain.Distance(start, end);

        // Assert
        Assert.Equal(0, distance);
    }

    [Fact]
    public void Distance_ConsecutiveDays_ReturnsOne()
    {
        // Arrange
        var start = new System.DateTime(2024, 3, 15, 10, 0, 0);
        var end = new System.DateTime(2024, 3, 16, 20, 0, 0);

        // Act
        var distance = _domain.Distance(start, end);

        // Assert
        Assert.Equal(1, distance);
    }

    [Fact]
    public void Distance_MultipleDays_ReturnsCorrectCount()
    {
        // Arrange
        var start = new System.DateTime(2024, 3, 10, 14, 30, 0);
        var end = new System.DateTime(2024, 3, 15, 10, 15, 0);

        // Act
        var distance = _domain.Distance(start, end);

        // Assert
        Assert.Equal(5, distance);
    }

    [Fact]
    public void Distance_ReverseRange_ReturnsNegative()
    {
        // Arrange
        var start = new System.DateTime(2024, 3, 20);
        var end = new System.DateTime(2024, 3, 15);

        // Act
        var distance = _domain.Distance(start, end);

        // Assert
        Assert.Equal(-5, distance);
    }

    [Fact]
    public void Distance_AcrossMonths_CalculatesCorrectly()
    {
        // Arrange
        var start = new System.DateTime(2024, 2, 28);
        var end = new System.DateTime(2024, 3, 2);

        // Act
        var distance = _domain.Distance(start, end);

        // Assert
        Assert.Equal(3, distance); // Feb 28, 29 (leap year), Mar 1, Mar 2
    }

    [Fact]
    public void Distance_SymmetryProperty_Holds()
    {
        // Property: Distance(a,b) = -Distance(b,a)
        // Arrange
        var date1 = new System.DateTime(2024, 3, 10);
        var date2 = new System.DateTime(2024, 3, 20);

        // Act
        var forward = _domain.Distance(date1, date2);
        var backward = _domain.Distance(date2, date1);

        // Assert
        Assert.Equal(-forward, backward);
    }

    #endregion

    #region Add Tests

    [Fact]
    public void Add_PositiveOffset_AddsDays()
    {
        // Arrange
        var date = new System.DateTime(2024, 3, 15, 10, 30, 0);

        // Act
        var result = _domain.Add(date, 5);

        // Assert
        Assert.Equal(new System.DateTime(2024, 3, 20, 10, 30, 0), result);
    }

    [Fact]
    public void Add_NegativeOffset_SubtractsDays()
    {
        // Arrange
        var date = new System.DateTime(2024, 3, 15, 10, 30, 0);

        // Act
        var result = _domain.Add(date, -5);

        // Assert
        Assert.Equal(new System.DateTime(2024, 3, 10, 10, 30, 0), result);
    }

    [Fact]
    public void Add_PreservesTimeComponent()
    {
        // Arrange
        var date = new System.DateTime(2024, 3, 15, 14, 30, 45);

        // Act
        var result = _domain.Add(date, 3);

        // Assert
        Assert.Equal(new System.DateTime(2024, 3, 18, 14, 30, 45), result);
    }

    [Fact]
    public void Add_AcrossMonthBoundary_Works()
    {
        // Arrange
        var date = new System.DateTime(2024, 2, 28);

        // Act
        var result = _domain.Add(date, 2);

        // Assert
        Assert.Equal(new System.DateTime(2024, 3, 1), result);
    }

    #endregion

    #region Subtract Tests

    [Fact]
    public void Subtract_PositiveOffset_SubtractsDays()
    {
        // Arrange
        var date = new System.DateTime(2024, 3, 15, 10, 30, 0);

        // Act
        var result = _domain.Subtract(date, 5);

        // Assert
        Assert.Equal(new System.DateTime(2024, 3, 10, 10, 30, 0), result);
    }

    [Fact]
    public void Subtract_NegativeOffset_AddsDays()
    {
        // Arrange
        var date = new System.DateTime(2024, 3, 10, 10, 30, 0);

        // Act
        var result = _domain.Subtract(date, -5);

        // Assert
        Assert.Equal(new System.DateTime(2024, 3, 15, 10, 30, 0), result);
    }

    [Fact]
    public void Subtract_PreservesTimeComponent()
    {
        // Arrange
        var date = new System.DateTime(2024, 3, 18, 14, 30, 45);

        // Act
        var result = _domain.Subtract(date, 3);

        // Assert
        Assert.Equal(new System.DateTime(2024, 3, 15, 14, 30, 45), result);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public void Distance_LeapYearFebruary29_CalculatesCorrectly()
    {
        // Arrange
        var start = new System.DateTime(2024, 2, 28);
        var end = new System.DateTime(2024, 3, 1);

        // Act
        var distance = _domain.Distance(start, end);

        // Assert
        Assert.Equal(2, distance); // Feb 28 → Feb 29 → Mar 1
    }

    [Fact]
    public void Add_ToFebruary29_HandlesLeapYear()
    {
        // Arrange
        var date = new System.DateTime(2024, 2, 28);

        // Act
        var result = _domain.Add(date, 1);

        // Assert
        Assert.Equal(new System.DateTime(2024, 2, 29), result);
    }

    [Fact]
    public void Floor_Midnight_UsesEqualityWithTimeSpanZero()
    {
        // This specifically tests the fix for System.TimeSpan.Zero → TimeSpan.Zero
        // Arrange
        var input = new System.DateTime(2024, 3, 15, 0, 0, 0);

        // Act
        var ceiling = _domain.Ceiling(input);

        // Assert
        Assert.Equal(input, ceiling); // Should return unchanged for midnight
    }

    #endregion
}
