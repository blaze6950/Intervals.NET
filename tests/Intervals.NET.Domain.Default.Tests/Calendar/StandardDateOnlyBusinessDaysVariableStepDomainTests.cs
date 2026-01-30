using Intervals.NET.Domain.Default.Calendar;

namespace Intervals.NET.Domain.Default.Tests.Calendar;

/// <summary>
/// Tests for StandardDateOnlyBusinessDaysVariableStepDomain.
/// Covers the standard business week (Monday-Friday) variable-step domain.
/// </summary>
public class StandardDateOnlyBusinessDaysVariableStepDomainTests
{
    private readonly StandardDateOnlyBusinessDaysVariableStepDomain _domain = new();

    #region Floor Tests

    [Fact]
    public void Floor_BusinessDay_ReturnsUnchanged()
    {
        // Arrange - Monday, January 6, 2025
        var monday = new DateOnly(2025, 1, 6);

        // Act
        var result = _domain.Floor(monday);

        // Assert
        Assert.Equal(monday, result);
    }

    [Fact]
    public void Floor_Saturday_ReturnsPreviousFriday()
    {
        // Arrange - Saturday, January 4, 2025
        var saturday = new DateOnly(2025, 1, 4);
        var expectedFriday = new DateOnly(2025, 1, 3);

        // Act
        var result = _domain.Floor(saturday);

        // Assert
        Assert.Equal(expectedFriday, result);
    }

    [Fact]
    public void Floor_Sunday_ReturnsPreviousFriday()
    {
        // Arrange - Sunday, January 5, 2025
        var sunday = new DateOnly(2025, 1, 5);
        var expectedFriday = new DateOnly(2025, 1, 3);

        // Act
        var result = _domain.Floor(sunday);

        // Assert
        Assert.Equal(expectedFriday, result);
    }

    [Theory]
    [InlineData(2025, 1, 6, DayOfWeek.Monday)]    // Monday
    [InlineData(2025, 1, 7, DayOfWeek.Tuesday)]   // Tuesday
    [InlineData(2025, 1, 8, DayOfWeek.Wednesday)] // Wednesday
    [InlineData(2025, 1, 9, DayOfWeek.Thursday)]  // Thursday
    [InlineData(2025, 1, 10, DayOfWeek.Friday)]   // Friday
    public void Floor_AllBusinessDays_ReturnsUnchanged(int year, int month, int day, DayOfWeek expectedDayOfWeek)
    {
        // Arrange
        var date = new DateOnly(year, month, day);
        Assert.Equal(expectedDayOfWeek, date.DayOfWeek); // Verify test data

        // Act
        var result = _domain.Floor(date);

        // Assert
        Assert.Equal(date, result);
    }

    #endregion

    #region Ceiling Tests

    [Fact]
    public void Ceiling_BusinessDay_ReturnsUnchanged()
    {
        // Arrange - Monday, January 6, 2025
        var monday = new DateOnly(2025, 1, 6);

        // Act
        var result = _domain.Ceiling(monday);

        // Assert
        Assert.Equal(monday, result);
    }

    [Fact]
    public void Ceiling_Saturday_ReturnsNextMonday()
    {
        // Arrange - Saturday, January 4, 2025
        var saturday = new DateOnly(2025, 1, 4);
        var expectedMonday = new DateOnly(2025, 1, 6);

        // Act
        var result = _domain.Ceiling(saturday);

        // Assert
        Assert.Equal(expectedMonday, result);
    }

    [Fact]
    public void Ceiling_Sunday_ReturnsNextMonday()
    {
        // Arrange - Sunday, January 5, 2025
        var sunday = new DateOnly(2025, 1, 5);
        var expectedMonday = new DateOnly(2025, 1, 6);

        // Act
        var result = _domain.Ceiling(sunday);

        // Assert
        Assert.Equal(expectedMonday, result);
    }

    [Theory]
    [InlineData(2025, 1, 6, DayOfWeek.Monday)]    // Monday
    [InlineData(2025, 1, 7, DayOfWeek.Tuesday)]   // Tuesday
    [InlineData(2025, 1, 8, DayOfWeek.Wednesday)] // Wednesday
    [InlineData(2025, 1, 9, DayOfWeek.Thursday)]  // Thursday
    [InlineData(2025, 1, 10, DayOfWeek.Friday)]   // Friday
    public void Ceiling_AllBusinessDays_ReturnsUnchanged(int year, int month, int day, DayOfWeek expectedDayOfWeek)
    {
        // Arrange
        var date = new DateOnly(year, month, day);
        Assert.Equal(expectedDayOfWeek, date.DayOfWeek); // Verify test data

        // Act
        var result = _domain.Ceiling(date);

        // Assert
        Assert.Equal(date, result);
    }

    #endregion

    #region Add Tests

    [Fact]
    public void Add_OneBusinessDay_FromMonday_ReturnsTuesday()
    {
        // Arrange - Monday, January 6, 2025
        var monday = new DateOnly(2025, 1, 6);
        var expectedTuesday = new DateOnly(2025, 1, 7);

        // Act
        var result = _domain.Add(monday, 1);

        // Assert
        Assert.Equal(expectedTuesday, result);
    }

    [Fact]
    public void Add_OneBusinessDay_FromFriday_SkipsWeekendReturnsMonday()
    {
        // Arrange - Friday, January 3, 2025
        var friday = new DateOnly(2025, 1, 3);
        var expectedMonday = new DateOnly(2025, 1, 6);

        // Act
        var result = _domain.Add(friday, 1);

        // Assert
        Assert.Equal(expectedMonday, result);
    }

    [Fact]
    public void Add_FiveBusinessDays_SkipsWeekend()
    {
        // Arrange - Monday, January 6, 2025
        var monday = new DateOnly(2025, 1, 6);
        var expectedNextMonday = new DateOnly(2025, 1, 13);

        // Act
        var result = _domain.Add(monday, 5);

        // Assert
        Assert.Equal(expectedNextMonday, result);
    }

    [Fact]
    public void Add_ZeroSteps_ReturnsUnchanged()
    {
        // Arrange - Wednesday, January 8, 2025
        var wednesday = new DateOnly(2025, 1, 8);

        // Act
        var result = _domain.Add(wednesday, 0);

        // Assert
        Assert.Equal(wednesday, result);
    }

    [Fact]
    public void Add_NegativeSteps_MovesBackward()
    {
        // Arrange - Friday, January 10, 2025
        var friday = new DateOnly(2025, 1, 10);
        var expectedMonday = new DateOnly(2025, 1, 6);

        // Act
        var result = _domain.Add(friday, -4);

        // Assert
        Assert.Equal(expectedMonday, result);
    }

    [Fact]
    public void Add_FromSaturday_SkipsWeekend()
    {
        // Arrange - Saturday, January 4, 2025
        var saturday = new DateOnly(2025, 1, 4);
        var expectedTuesday = new DateOnly(2025, 1, 7); // Skip weekend, Monday is step 1, Tuesday is step 2

        // Act
        var result = _domain.Add(saturday, 2);

        // Assert
        Assert.Equal(expectedTuesday, result);
    }

    #endregion

    #region Subtract Tests

    [Fact]
    public void Subtract_OneBusinessDay_FromTuesday_ReturnsMonday()
    {
        // Arrange - Tuesday, January 7, 2025
        var tuesday = new DateOnly(2025, 1, 7);
        var expectedMonday = new DateOnly(2025, 1, 6);

        // Act
        var result = _domain.Subtract(tuesday, 1);

        // Assert
        Assert.Equal(expectedMonday, result);
    }

    [Fact]
    public void Subtract_OneBusinessDay_FromMonday_SkipsWeekendReturnsFriday()
    {
        // Arrange - Monday, January 6, 2025
        var monday = new DateOnly(2025, 1, 6);
        var expectedFriday = new DateOnly(2025, 1, 3);

        // Act
        var result = _domain.Subtract(monday, 1);

        // Assert
        Assert.Equal(expectedFriday, result);
    }

    [Fact]
    public void Subtract_FiveBusinessDays_SkipsWeekend()
    {
        // Arrange - Monday, January 13, 2025
        var monday = new DateOnly(2025, 1, 13);
        var expectedPreviousMonday = new DateOnly(2025, 1, 6);

        // Act
        var result = _domain.Subtract(monday, 5);

        // Assert
        Assert.Equal(expectedPreviousMonday, result);
    }

    [Fact]
    public void Subtract_NegativeSteps_MovesForward()
    {
        // Arrange - Monday, January 6, 2025
        var monday = new DateOnly(2025, 1, 6);
        var expectedFriday = new DateOnly(2025, 1, 10);

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
        // Arrange - Monday, January 6, 2025 to Friday, January 10, 2025
        var monday = new DateOnly(2025, 1, 6);
        var friday = new DateOnly(2025, 1, 10);

        // Act
        var result = _domain.Distance(monday, friday);

        // Assert
        Assert.Equal(4.0, result); // 4 steps: Mon->Tue->Wed->Thu->Fri
    }

    [Fact]
    public void Distance_FridayToMonday_SkipsWeekend_ReturnsOne()
    {
        // Arrange - Friday, January 3, 2025 to Monday, January 6, 2025
        var friday = new DateOnly(2025, 1, 3);
        var monday = new DateOnly(2025, 1, 6);

        // Act
        var result = _domain.Distance(friday, monday);

        // Assert
        Assert.Equal(1.0, result); // 1 step: Fri->Mon (skips weekend)
    }

    [Fact]
    public void Distance_SameDate_ReturnsZero()
    {
        // Arrange - Monday, January 6, 2025
        var date = new DateOnly(2025, 1, 6);

        // Act
        var result = _domain.Distance(date, date);

        // Assert
        Assert.Equal(0.0, result); // Same date = 0 steps needed
    }

    [Fact]
    public void Distance_EndBeforeStart_ReturnsNegative()
    {
        // Arrange - Monday, January 13, 2025 to Monday, January 6, 2025
        var laterDate = new DateOnly(2025, 1, 13);
        var earlierDate = new DateOnly(2025, 1, 6);

        // Act
        var result = _domain.Distance(laterDate, earlierDate);

        // Assert
        Assert.Equal(-5.0, result); // 5 business days backward
    }

    [Fact]
    public void Distance_AcrossTwoWeeks_CountsOnlyBusinessDays()
    {
        // Arrange - Monday, January 6, 2025 to Monday, January 20, 2025
        var startMonday = new DateOnly(2025, 1, 6);
        var endMonday = new DateOnly(2025, 1, 20);

        // Act
        var result = _domain.Distance(startMonday, endMonday);

        // Assert
        // Mon6 -> ... -> Fri10 (4 steps) -> Mon13 (1 step) -> ... -> Fri17 (4 steps) -> Mon20 (1 step)
        // Total: 10 steps
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void Distance_FromSaturday_FloorsToFriday()
    {
        // Arrange - Saturday, January 4, 2025 to Monday, January 6, 2025
        var saturday = new DateOnly(2025, 1, 4);
        var monday = new DateOnly(2025, 1, 6);

        // Act
        var result = _domain.Distance(saturday, monday);

        // Assert
        // Saturday floors to Friday(3), Friday to Monday = 1 step
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void Distance_ToSunday_FloorsToFriday()
    {
        // Arrange - Monday, January 6, 2025 to Sunday, January 12, 2025
        var monday = new DateOnly(2025, 1, 6);
        var sunday = new DateOnly(2025, 1, 12);

        // Act
        var result = _domain.Distance(monday, sunday);

        // Assert
        // Sunday floors to Friday(10), Mon(6) to Fri(10) = 4 steps
        Assert.Equal(4.0, result);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void AddAndSubtract_AreInverse()
    {
        // Arrange - Wednesday, January 8, 2025
        var original = new DateOnly(2025, 1, 8);

        // Act
        var added = _domain.Add(original, 7);
        var backToOriginal = _domain.Subtract(added, 7);

        // Assert
        Assert.Equal(original, backToOriginal);
    }

    [Fact]
    public void FloorAndCeiling_Weekend_ProduceDifferentResults()
    {
        // Arrange - Saturday, January 4, 2025
        var saturday = new DateOnly(2025, 1, 4);

        // Act
        var floored = _domain.Floor(saturday);
        var ceiled = _domain.Ceiling(saturday);

        // Assert
        Assert.Equal(new DateOnly(2025, 1, 3), floored);  // Friday
        Assert.Equal(new DateOnly(2025, 1, 6), ceiled);   // Monday
        Assert.NotEqual(floored, ceiled);
    }

    [Fact]
    public void FloorAndCeiling_BusinessDay_ProduceSameResult()
    {
        // Arrange - Wednesday, January 8, 2025
        var wednesday = new DateOnly(2025, 1, 8);

        // Act
        var floored = _domain.Floor(wednesday);
        var ceiled = _domain.Ceiling(wednesday);

        // Assert
        Assert.Equal(wednesday, floored);
        Assert.Equal(wednesday, ceiled);
        Assert.Equal(floored, ceiled);
    }

    [Fact]
    public void Distance_MatchesManualAddition()
    {
        // Arrange - Monday, January 6, 2025
        var start = new DateOnly(2025, 1, 6);
        var end = new DateOnly(2025, 1, 15); // Wednesday, next week

        // Act
        var distance = _domain.Distance(start, end);
        var calculatedEnd = _domain.Add(start, (long)distance);

        // Assert
        Assert.Equal(7.0, distance); // 7 steps: Mon6+1=Tue7, +1=Wed8, +1=Thu9, +1=Fri10, +1=Mon13, +1=Tue14, +1=Wed15
        Assert.Equal(end, calculatedEnd);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Add_LargeNumberOfDays_WorksCorrectly()
    {
        // Arrange - Monday, January 6, 2025
        var start = new DateOnly(2025, 1, 6);

        // Act - Add 100 business days
        var result = _domain.Add(start, 100);

        // Assert
        // Verify it's a business day
        Assert.NotEqual(DayOfWeek.Saturday, result.DayOfWeek);
        Assert.NotEqual(DayOfWeek.Sunday, result.DayOfWeek);
    }

    [Fact]
    public void Distance_LargeRange_CalculatesCorrectly()
    {
        // Arrange - 1 year span
        var start = new DateOnly(2025, 1, 6); // Monday
        var end = new DateOnly(2026, 1, 5);   // Monday, one year later

        // Act
        var distance = _domain.Distance(start, end);

        // Assert
        // Approximately 52 weeks * 5 business days = ~260 business days
        Assert.True(distance >= 250 && distance <= 265, $"Expected ~260 business days, got {distance}");
    }

    [Fact]
    public void Floor_FirstDayOfYear_Saturday_WorksCorrectly()
    {
        // Arrange - Saturday, January 1, 2022
        var newYearSaturday = new DateOnly(2022, 1, 1);
        var expectedFriday = new DateOnly(2021, 12, 31);

        // Act
        var result = _domain.Floor(newYearSaturday);

        // Assert
        Assert.Equal(expectedFriday, result);
    }

    [Fact]
    public void Ceiling_LastDayOfYear_Sunday_WorksCorrectly()
    {
        // Arrange - Sunday, December 31, 2023
        var newYearEveSunday = new DateOnly(2023, 12, 31);
        var expectedMonday = new DateOnly(2024, 1, 1);

        // Act
        var result = _domain.Ceiling(newYearEveSunday);

        // Assert
        Assert.Equal(expectedMonday, result);
    }

    [Fact]
    public void Ceiling_AlreadyOnBusinessDay_ReturnsUnchanged()
    {
        // Arrange - Monday, January 1, 2024 (a business day)
        var monday = new DateOnly(2024, 1, 1);

        // Act
        var result = _domain.Ceiling(monday);

        // Assert
        Assert.Equal(monday, result);
    }

    #endregion
}
