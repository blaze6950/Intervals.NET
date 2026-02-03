using Intervals.NET.Domain.Default.Calendar;

namespace Intervals.NET.Domain.Default.Tests.Calendar;

/// <summary>
/// Tests for StandardDateTimeBusinessDaysVariableStepDomain.
/// Covers the standard business week (Monday-Friday) variable-step domain for DateTime.
/// </summary>
public class StandardDateTimeBusinessDaysVariableStepDomainTests
{
    private readonly StandardDateTimeBusinessDaysVariableStepDomain _domain = new();

    #region Floor Tests

    [Fact]
    public void Floor_BusinessDayAtMidnight_ReturnsUnchanged()
    {
        // Arrange - Monday, January 6, 2025 at midnight
        var monday = new System.DateTime(2025, 1, 6, 0, 0, 0);

        // Act
        var result = _domain.Floor(monday);

        // Assert
        Assert.Equal(monday, result);
    }

    [Fact]
    public void Floor_BusinessDayWithTime_ReturnsDateAtMidnight()
    {
        // Arrange - Monday, January 6, 2025 at 10:30 AM
        var monday = new System.DateTime(2025, 1, 6, 10, 30, 0);
        var expectedMidnight = new System.DateTime(2025, 1, 6, 0, 0, 0);

        // Act
        var result = _domain.Floor(monday);

        // Assert
        Assert.Equal(expectedMidnight, result);
    }

    [Fact]
    public void Floor_Saturday_ReturnsPreviousFridayAtMidnight()
    {
        // Arrange - Saturday, January 4, 2025 at 3:45 PM
        var saturday = new System.DateTime(2025, 1, 4, 15, 45, 0);
        var expectedFriday = new System.DateTime(2025, 1, 3, 0, 0, 0);

        // Act
        var result = _domain.Floor(saturday);

        // Assert
        Assert.Equal(expectedFriday, result);
    }

    [Fact]
    public void Floor_Sunday_ReturnsPreviousFridayAtMidnight()
    {
        // Arrange - Sunday, January 5, 2025 at 11:59 PM
        var sunday = new System.DateTime(2025, 1, 5, 23, 59, 59);
        var expectedFriday = new System.DateTime(2025, 1, 3, 0, 0, 0);

        // Act
        var result = _domain.Floor(sunday);

        // Assert
        Assert.Equal(expectedFriday, result);
    }

    #endregion

    #region Ceiling Tests

    [Fact]
    public void Ceiling_BusinessDayAtMidnight_ReturnsUnchanged()
    {
        // Arrange - Monday, January 6, 2025 at midnight
        var monday = new System.DateTime(2025, 1, 6, 0, 0, 0);

        // Act
        var result = _domain.Ceiling(monday);

        // Assert
        Assert.Equal(monday, result);
    }

    [Fact]
    public void Ceiling_BusinessDayWithTime_ReturnsNextDayAtMidnight()
    {
        // Arrange - Monday, January 6, 2025 at 10:30 AM
        var monday = new System.DateTime(2025, 1, 6, 10, 30, 0);
        var expectedNextDay = new System.DateTime(2025, 1, 7, 0, 0, 0);

        // Act
        var result = _domain.Ceiling(monday);

        // Assert
        Assert.Equal(expectedNextDay, result);
    }

    [Fact]
    public void Ceiling_Saturday_ReturnsNextMondayAtMidnight()
    {
        // Arrange - Saturday, January 4, 2025
        var saturday = new System.DateTime(2025, 1, 4, 15, 45, 0);
        var expectedMonday = new System.DateTime(2025, 1, 6, 0, 0, 0);

        // Act
        var result = _domain.Ceiling(saturday);

        // Assert
        Assert.Equal(expectedMonday, result);
    }

    [Fact]
    public void Ceiling_Sunday_ReturnsNextMondayAtMidnight()
    {
        // Arrange - Sunday, January 5, 2025
        var sunday = new System.DateTime(2025, 1, 5, 8, 0, 0);
        var expectedMonday = new System.DateTime(2025, 1, 6, 0, 0, 0);

        // Act
        var result = _domain.Ceiling(sunday);

        // Assert
        Assert.Equal(expectedMonday, result);
    }

    #endregion

    #region Add Tests

    [Fact]
    public void Add_OneBusinessDay_FromMonday_ReturnsTuesday()
    {
        // Arrange - Monday, January 6, 2025 at 9:00 AM
        var monday = new System.DateTime(2025, 1, 6, 9, 0, 0);
        var expectedTuesday = new System.DateTime(2025, 1, 7, 9, 0, 0);

        // Act
        var result = _domain.Add(monday, 1);

        // Assert
        Assert.Equal(expectedTuesday, result);
    }

    [Fact]
    public void Add_OneBusinessDay_FromFriday_SkipsWeekendReturnsMonday()
    {
        // Arrange - Friday, January 3, 2025 at 5:00 PM
        var friday = new System.DateTime(2025, 1, 3, 17, 0, 0);
        var expectedMonday = new System.DateTime(2025, 1, 6, 17, 0, 0);

        // Act
        var result = _domain.Add(friday, 1);

        // Assert
        Assert.Equal(expectedMonday, result);
    }

    [Fact]
    public void Add_FiveBusinessDays_SkipsWeekend()
    {
        // Arrange - Monday, January 6, 2025
        var monday = new System.DateTime(2025, 1, 6, 12, 0, 0);
        var expectedNextMonday = new System.DateTime(2025, 1, 13, 12, 0, 0);

        // Act
        var result = _domain.Add(monday, 5);

        // Assert
        Assert.Equal(expectedNextMonday, result);
    }

    [Fact]
    public void Add_ZeroSteps_ReturnsUnchanged()
    {
        // Arrange
        var wednesday = new System.DateTime(2025, 1, 8, 10, 30, 15);

        // Act
        var result = _domain.Add(wednesday, 0);

        // Assert
        Assert.Equal(wednesday, result);
    }

    [Fact]
    public void Add_NegativeSteps_MovesBackward()
    {
        // Arrange - Friday, January 10, 2025
        var friday = new System.DateTime(2025, 1, 10, 8, 0, 0);
        var expectedMonday = new System.DateTime(2025, 1, 6, 8, 0, 0);

        // Act
        var result = _domain.Add(friday, -4);

        // Assert
        Assert.Equal(expectedMonday, result);
    }

    [Fact]
    public void Add_PreservesTimeComponent()
    {
        // Arrange - Monday at 2:34:56 PM
        var monday = new System.DateTime(2025, 1, 6, 14, 34, 56);

        // Act
        var result = _domain.Add(monday, 3);

        // Assert
        Assert.Equal(14, result.Hour);
        Assert.Equal(34, result.Minute);
        Assert.Equal(56, result.Second);
    }

    #endregion

    #region Subtract Tests

    [Fact]
    public void Subtract_OneBusinessDay_FromTuesday_ReturnsMonday()
    {
        // Arrange - Tuesday, January 7, 2025
        var tuesday = new System.DateTime(2025, 1, 7, 11, 0, 0);
        var expectedMonday = new System.DateTime(2025, 1, 6, 11, 0, 0);

        // Act
        var result = _domain.Subtract(tuesday, 1);

        // Assert
        Assert.Equal(expectedMonday, result);
    }

    [Fact]
    public void Subtract_OneBusinessDay_FromMonday_SkipsWeekendReturnsFriday()
    {
        // Arrange - Monday, January 6, 2025
        var monday = new System.DateTime(2025, 1, 6, 16, 30, 0);
        var expectedFriday = new System.DateTime(2025, 1, 3, 16, 30, 0);

        // Act
        var result = _domain.Subtract(monday, 1);

        // Assert
        Assert.Equal(expectedFriday, result);
    }

    [Fact]
    public void Subtract_NegativeSteps_MovesForward()
    {
        // Arrange - Monday, January 6, 2025
        var monday = new System.DateTime(2025, 1, 6, 9, 0, 0);
        var expectedFriday = new System.DateTime(2025, 1, 10, 9, 0, 0);

        // Act
        var result = _domain.Subtract(monday, -4);

        // Assert
        Assert.Equal(expectedFriday, result);
    }

    #endregion

    #region Distance Tests

    [Fact]
    public void Distance_MondayToFriday_SameWeek_ReturnsFour()
    {
        // Arrange
        var monday = new System.DateTime(2025, 1, 6, 9, 0, 0);
        var friday = new System.DateTime(2025, 1, 10, 17, 0, 0);

        // Act
        var result = _domain.Distance(monday, friday);

        // Assert
        Assert.Equal(4.0, result); // 4 steps: Mon->Tue->Wed->Thu->Fri
    }

    [Fact]
    public void Distance_FridayToMonday_SkipsWeekend_ReturnsOne()
    {
        // Arrange
        var friday = new System.DateTime(2025, 1, 3, 17, 0, 0);
        var monday = new System.DateTime(2025, 1, 6, 9, 0, 0);

        // Act
        var result = _domain.Distance(friday, monday);

        // Assert
        Assert.Equal(1.0, result); // 1 step: Fri->Mon (skips weekend)
    }

    [Fact]
    public void Distance_SameDateTime_ReturnsZero()
    {
        // Arrange
        var date = new System.DateTime(2025, 1, 6, 14, 30, 45);

        // Act
        var result = _domain.Distance(date, date);

        // Assert
        Assert.Equal(0.0, result); // Same date = 0 steps needed
    }

    [Fact]
    public void Distance_EndBeforeStart_ReturnsNegative()
    {
        // Arrange
        var laterDate = new System.DateTime(2025, 1, 13);
        var earlierDate = new System.DateTime(2025, 1, 6);

        // Act
        var result = _domain.Distance(laterDate, earlierDate);

        // Assert
        Assert.Equal(-5.0, result);
    }

    [Fact]
    public void Distance_IgnoresTimeComponent()
    {
        // Arrange - Same date, different times
        var morning = new System.DateTime(2025, 1, 6, 8, 0, 0);
        var evening = new System.DateTime(2025, 1, 6, 20, 0, 0);

        // Act
        var result = _domain.Distance(morning, evening);

        // Assert
        Assert.Equal(0.0, result); // Same business day = 0 steps
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void AddAndSubtract_AreInverse()
    {
        // Arrange
        var original = new System.DateTime(2025, 1, 8, 14, 30, 45);

        // Act
        var added = _domain.Add(original, 7);
        var backToOriginal = _domain.Subtract(added, 7);

        // Assert
        Assert.Equal(original, backToOriginal);
    }

    [Fact]
    public void FloorAndCeiling_Weekend_ProduceDifferentResults()
    {
        // Arrange - Saturday
        var saturday = new System.DateTime(2025, 1, 4, 12, 0, 0);

        // Act
        var floored = _domain.Floor(saturday);
        var ceiled = _domain.Ceiling(saturday);

        // Assert
        Assert.Equal(new System.DateTime(2025, 1, 3, 0, 0, 0), floored);  // Friday midnight
        Assert.Equal(new System.DateTime(2025, 1, 6, 0, 0, 0), ceiled);   // Monday midnight
        Assert.NotEqual(floored, ceiled);
    }

    [Fact]
    public void Distance_MatchesManualAddition()
    {
        // Arrange
        var start = new System.DateTime(2025, 1, 6, 9, 0, 0);
        var end = new System.DateTime(2025, 1, 15, 9, 0, 0);

        // Act
        var distance = _domain.Distance(start, end);
        var calculatedEnd = _domain.Add(start, (long)distance);

        // Assert
        Assert.Equal(7.0, distance); // 7 steps needed
        Assert.Equal(end, calculatedEnd);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Add_LargeNumberOfDays_WorksCorrectly()
    {
        // Arrange
        var start = new System.DateTime(2025, 1, 6, 9, 0, 0);

        // Act - Add 100 business days
        var result = _domain.Add(start, 100);

        // Assert - Verify it's a business day
        Assert.NotEqual(DayOfWeek.Saturday, result.DayOfWeek);
        Assert.NotEqual(DayOfWeek.Sunday, result.DayOfWeek);
    }

    [Fact]
    public void Floor_MidnightBusinessDay_ReturnsUnchanged()
    {
        // Arrange - Tuesday at exactly midnight
        var tuesday = new System.DateTime(2025, 1, 7, 0, 0, 0);

        // Act
        var result = _domain.Floor(tuesday);

        // Assert
        Assert.Equal(tuesday, result);
    }

    [Fact]
    public void Floor_OneTickBeforeMidnight_ReturnsDateAtMidnight()
    {
        // Arrange - Monday at 23:59:59.9999999
        var almostMidnight = new System.DateTime(2025, 1, 6, 23, 59, 59, 999).AddTicks(9999);
        var expectedMidnight = new System.DateTime(2025, 1, 6, 0, 0, 0);

        // Act
        var result = _domain.Floor(almostMidnight);

        // Assert
        Assert.Equal(expectedMidnight, result);
    }

    [Fact]
    public void Ceiling_FridayWithTime_ReturnsNextMondayNotSaturday()
    {
        // Arrange - Friday at 11:30 AM
        var friday = new System.DateTime(2025, 1, 10, 11, 30, 0);
        var expectedMonday = new System.DateTime(2025, 1, 13, 0, 0, 0); // Next business day

        // Act
        var result = _domain.Ceiling(friday);

        // Assert
        Assert.Equal(expectedMonday, result);
    }

    [Fact]
    public void Ceiling_BusinessDayExactlyAtMidnight_ReturnsUnchanged()
    {
        // Arrange - Monday, January 6, 2025 at midnight (a business day)
        var monday = new System.DateTime(2025, 1, 6, 0, 0, 0);

        // Act
        var result = _domain.Ceiling(monday);

        // Assert
        Assert.Equal(monday, result);
    }

    #endregion
}
