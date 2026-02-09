using Intervals.NET.Domain.Default.Calendar;
using Intervals.NET.Domain.Extensions.Variable;
using RangeFactory = Intervals.NET.Factories.Range;

namespace Intervals.NET.Domain.Extensions.Tests.Variable;

/// <summary>
/// Tests for Variable-step domain extension methods (potentially O(N) operations).
/// Tests the extension methods in Intervals.NET.Domain.Extensions.Variable.RangeDomainExtensions
/// using StandardDateTimeBusinessDaysVariableStepDomain and StandardDateOnlyBusinessDaysVariableStepDomain.
/// </summary>
public class RangeDomainExtensionsTests
{
    #region Span Tests - StandardDateTimeBusinessDaysVariableStepDomain

    [Fact]
    public void Span_DateTime_BusinessDaysOneWeek_ReturnsCorrectCount()
    {
        // Arrange - Monday Jan 1, 2024 to Friday Jan 5, 2024
        var start = new DateTime(2024, 1, 1); // Monday
        var end = new DateTime(2024, 1, 5);   // Friday
        var range = RangeFactory.Closed<DateTime>(start, end);
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        // Monday through Friday = 5 business days
        Assert.Equal(5.0, span.Value);
    }

    [Fact]
    public void Span_DateTime_BusinessDaysIncludingWeekend_SkipsWeekend()
    {
        // Arrange - Friday Jan 5 to Monday Jan 8, 2024
        var start = new DateTime(2024, 1, 5);  // Friday
        var end = new DateTime(2024, 1, 8);    // Monday
        var range = RangeFactory.Closed<DateTime>(start, end);
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        // Friday and Monday = 2 business days (weekend skipped)
        Assert.Equal(2.0, span.Value);
    }

    [Fact]
    public void Span_DateTime_BusinessDaysUnboundedEnd_ReturnsPositiveInfinity()
    {
        // Arrange
        var start = new DateTime(2024, 1, 1);
        var range = RangeFactory.Closed(start, RangeValue<DateTime>.PositiveInfinity);
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.False(span.IsFinite);
        Assert.True(span.IsPositiveInfinity);
    }

    [Fact]
    public void Span_DateTime_BusinessDaysWithTimeComponent_IgnoresTime()
    {
        // Arrange - Monday at 9 AM to Friday at 5 PM
        var start = new DateTime(2024, 1, 1, 9, 0, 0);
        var end = new DateTime(2024, 1, 5, 17, 0, 0);
        var range = RangeFactory.Closed<DateTime>(start, end);
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        Assert.Equal(5.0, span.Value);
    }

    #endregion

    #region Span Tests - StandardDateOnlyBusinessDaysVariableStepDomain

    [Fact]
    public void Span_DateOnly_BusinessDaysOneWeek_ReturnsCorrectCount()
    {
        // Arrange - Monday Jan 1, 2024 to Friday Jan 5, 2024
        var start = new DateOnly(2024, 1, 1); // Monday
        var end = new DateOnly(2024, 1, 5);   // Friday
        var range = RangeFactory.Closed<DateOnly>(start, end);
        var domain = new StandardDateOnlyBusinessDaysVariableStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        // Monday through Friday = 5 business days
        Assert.Equal(5.0, span.Value);
    }

    [Fact]
    public void Span_DateOnly_BusinessDaysIncludingWeekend_SkipsWeekend()
    {
        // Arrange - Friday Jan 5 to Monday Jan 8, 2024
        var start = new DateOnly(2024, 1, 5);  // Friday
        var end = new DateOnly(2024, 1, 8);    // Monday
        var range = RangeFactory.Closed<DateOnly>(start, end);
        var domain = new StandardDateOnlyBusinessDaysVariableStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        // Friday and Monday = 2 business days (weekend skipped)
        Assert.Equal(2.0, span.Value);
    }

    [Fact]
    public void Span_DateOnly_BusinessDaysUnboundedEnd_ReturnsPositiveInfinity()
    {
        // Arrange
        var start = new DateOnly(2024, 1, 1);
        var range = RangeFactory.Closed(start, RangeValue<DateOnly>.PositiveInfinity);
        var domain = new StandardDateOnlyBusinessDaysVariableStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.False(span.IsFinite);
        Assert.True(span.IsPositiveInfinity);
    }

    [Fact]
    public void Span_DateOnly_AcrossMultipleWeeks_CountsOnlyBusinessDays()
    {
        // Arrange - 3 full weeks
        var start = new DateOnly(2024, 1, 1);  // Monday
        var end = new DateOnly(2024, 1, 21);   // Sunday (3 weeks later)
        var range = RangeFactory.Closed<DateOnly>(start, end);
        var domain = new StandardDateOnlyBusinessDaysVariableStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        // Week 1: Mon-Fri = 5, Week 2: Mon-Fri = 5, Week 3: Mon-Fri = 5
        // Total = 15 business days
        Assert.Equal(15.0, span.Value);
    }

    #endregion

    #region ExpandByRatio Tests - StandardDateTimeBusinessDaysVariableStepDomain

    [Fact]
    public void ExpandByRatio_DateTime_BusinessDaysRange_ExpandsCorrectly()
    {
        // Arrange - Mon-Fri (5 business days)
        var start = new DateTime(2024, 1, 1); // Monday
        var end = new DateTime(2024, 1, 5);   // Friday
        var range = RangeFactory.Closed<DateTime>(start, end);
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();

        // Act - Expand by 40% on each side (2 business days each)
        var expanded = range.ExpandByRatio(domain, leftRatio: 0.4, rightRatio: 0.4);

        // Assert
        // 2 business days before Monday = previous Thursday
        var expectedStart = new DateTime(2023, 12, 28);
        Assert.Equal(expectedStart, expanded.Start.Value);
        // 2 business days after Friday = next Tuesday
        var expectedEnd = new DateTime(2024, 1, 9);
        Assert.Equal(expectedEnd, expanded.End.Value);
    }

    [Fact]
    public void ExpandByRatio_DateTime_InfiniteRange_ThrowsArgumentException()
    {
        // Arrange
        var start = new DateTime(2024, 1, 1);
        var range = RangeFactory.Closed(start, RangeValue<DateTime>.PositiveInfinity);
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            range.ExpandByRatio(domain, leftRatio: 0.5, rightRatio: 0.5));

        Assert.Contains("infinite", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ExpandByRatio_DateTime_PreservesTimeComponent()
    {
        // Arrange - Mon-Fri with time components
        var start = new DateTime(2024, 1, 1, 9, 0, 0); // Monday 9 AM
        var end = new DateTime(2024, 1, 5, 17, 0, 0);   // Friday 5 PM
        var range = RangeFactory.Closed<DateTime>(start, end);
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();

        // Act
        var expanded = range.ExpandByRatio(domain, leftRatio: 0.2, rightRatio: 0.2);

        // Assert - Time components preserved
        Assert.Equal(9, expanded.Start.Value.Hour);
        Assert.Equal(17, expanded.End.Value.Hour);
    }

    #endregion

    #region ExpandByRatio Tests - StandardDateOnlyBusinessDaysVariableStepDomain

    [Fact]
    public void ExpandByRatio_DateOnly_BusinessDaysRange_ExpandsCorrectly()
    {
        // Arrange - Mon-Fri (5 business days)
        var start = new DateOnly(2024, 1, 1); // Monday
        var end = new DateOnly(2024, 1, 5);   // Friday
        var range = RangeFactory.Closed<DateOnly>(start, end);
        var domain = new StandardDateOnlyBusinessDaysVariableStepDomain();

        // Act - Expand by 40% on each side (2 business days each)
        var expanded = range.ExpandByRatio(domain, leftRatio: 0.4, rightRatio: 0.4);

        // Assert
        // 2 business days before Monday = previous Thursday
        var expectedStart = new DateOnly(2023, 12, 28);
        Assert.Equal(expectedStart, expanded.Start.Value);
        // 2 business days after Friday = next Tuesday
        var expectedEnd = new DateOnly(2024, 1, 9);
        Assert.Equal(expectedEnd, expanded.End.Value);
    }

    [Fact]
    public void ExpandByRatio_DateOnly_InfiniteRange_ThrowsArgumentException()
    {
        // Arrange
        var start = new DateOnly(2024, 1, 1);
        var range = RangeFactory.Closed(start, RangeValue<DateOnly>.PositiveInfinity);
        var domain = new StandardDateOnlyBusinessDaysVariableStepDomain();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            range.ExpandByRatio(domain, leftRatio: 0.5, rightRatio: 0.5));

        Assert.Contains("infinite", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ExpandByRatio_DateOnly_AsymmetricExpansion_WorksCorrectly()
    {
        // Arrange - Mon-Fri
        var start = new DateOnly(2024, 1, 1); // Monday
        var end = new DateOnly(2024, 1, 5);   // Friday
        var range = RangeFactory.Closed<DateOnly>(start, end);
        var domain = new StandardDateOnlyBusinessDaysVariableStepDomain();

        // Act - Expand left by 20% (1 day), right by 60% (3 days)
        var expanded = range.ExpandByRatio(domain, leftRatio: 0.2, rightRatio: 0.6);

        // Assert
        // 1 business day before Monday = previous Friday
        var expectedStart = new DateOnly(2023, 12, 29);
        Assert.Equal(expectedStart, expanded.Start.Value);
        // 3 business days after Friday = next Wednesday
        var expectedEnd = new DateOnly(2024, 1, 10);
        Assert.Equal(expectedEnd, expanded.End.Value);
    }

    [Fact]
    public void Span_WithNegativeInfinityStart_ReturnsPositiveInfinity()
    {
        // Arrange
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();
        var range = RangeFactory.Closed(
            RangeValue<DateTime>.NegativeInfinity,
            new DateTime(2024, 12, 31));

        // Act
        var result = range.Span(domain);

        // Assert
        Assert.True(result.IsPositiveInfinity);
    }

    [Fact]
    public void Span_WithPositiveInfinityEnd_ReturnsPositiveInfinity()
    {
        // Arrange
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();
        var range = RangeFactory.Closed(
            new DateTime(2024, 1, 1),
            RangeValue<DateTime>.PositiveInfinity);

        // Act
        var result = range.Span(domain);

        // Assert
        Assert.True(result.IsPositiveInfinity);
    }

    [Fact]
    public void Span_WithBothInfinities_ReturnsPositiveInfinity()
    {
        // Arrange
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();
        var range = RangeFactory.Open(
            RangeValue<DateTime>.NegativeInfinity,
            RangeValue<DateTime>.PositiveInfinity);

        // Act
        var result = range.Span(domain);

        // Assert
        Assert.True(result.IsPositiveInfinity);
    }

    [Fact]
    public void Span_SingleBusinessDayRange_BothBoundariesOnBusinessDay_BothInclusive_ReturnsOne()
    {
        // Arrange - Monday with both boundaries on the day
        var date = new DateTime(2024, 1, 1); // Monday
        var range = RangeFactory.Closed<DateTime>(date, date);
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        Assert.Equal(1.0, span.Value);
    }

    [Fact]
    public void Span_SingleDayRange_BothBoundariesBetweenBusinessDays_ReturnsZero()
    {
        // Arrange - both times within the same business day, exclusive boundaries
        var start = new DateTime(2024, 1, 1, 10, 0, 0); // Monday 10 AM
        var end = new DateTime(2024, 1, 1, 15, 0, 0);   // Monday 3 PM
        var range = RangeFactory.Open(start, end);
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        // Both times are within the same business day, and boundaries are exclusive
        Assert.Equal(0.0, span.Value);
    }

    [Fact]
    public void Span_SingleBusinessDayRange_StartOnBoundary_EndWithinDay_ReturnsOne()
    {
        // Arrange - Monday midnight to Monday noon
        var start = new DateTime(2024, 1, 1, 0, 0, 0);  // Monday midnight (on boundary)
        var end = new DateTime(2024, 1, 1, 12, 0, 0);   // Monday noon
        var range = RangeFactory.Closed<DateTime>(start, end);
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        Assert.Equal(1.0, span.Value);
    }

    [Fact]
    public void Span_EmptyRange_ExclusiveBoundariesConsecutiveBusinessDays_ReturnsZero()
    {
        // Arrange - exclusive boundaries on consecutive business days
        var start = new DateTime(2024, 1, 1);  // Monday
        var end = new DateTime(2024, 1, 2);    // Tuesday
        var range = RangeFactory.Open(start, end);
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        // (Monday, Tuesday) excludes both days, no business days in between
        Assert.Equal(0.0, span.Value);
    }

    [Fact]
    public void Span_InvertedRange_StartGreaterThanEnd_ReturnsZero()
    {
        // Arrange - valid range that becomes empty after floor adjustments with exclusive boundaries
        var start = new DateTime(2024, 1, 1, 23, 0, 0);  // Monday, 11 PM
        var end = new DateTime(2024, 1, 2, 1, 0, 0);     // Tuesday, 1 AM
        var range = RangeFactory.Open(start, end);
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        // After flooring: both floor to their respective days
        // With both exclusive: firstStep = Tuesday, lastStep = Monday
        // firstStep > lastStep, so result should be 0
        Assert.Equal(0.0, span.Value);
    }

    [Fact]
    public void Span_StartExclusiveOnWeekend_SkipsToNextBusinessDay()
    {
        // Arrange - start on Saturday, end on Wednesday
        var start = new DateTime(2024, 1, 6);  // Saturday
        var end = new DateTime(2024, 1, 10);   // Wednesday
        var range = RangeFactory.Create<DateTime>(start, end, isStartInclusive: false, isEndInclusive: true);
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        // Excludes Saturday (weekend), includes Monday, Tuesday, Wednesday = 3 business days
        Assert.Equal(3.0, span.Value);
    }

    [Fact]
    public void Span_EndExclusiveOnBusinessDayBoundary_ExcludesThatDay()
    {
        // Arrange - Monday to Friday, end exclusive
        var start = new DateTime(2024, 1, 1, 0, 0, 0);  // Monday midnight
        var end = new DateTime(2024, 1, 5, 0, 0, 0);    // Friday midnight
        var range = RangeFactory.ClosedOpen(start, end);
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        // [Monday, Friday) includes Mon, Tue, Wed, Thu but not Fri = 4 business days
        Assert.Equal(4.0, span.Value);
    }

    [Fact]
    public void Span_DateOnly_SingleBusinessDayRange_BothInclusive_ReturnsOne()
    {
        // Arrange - single Monday
        var date = new DateOnly(2024, 1, 1); // Monday
        var range = RangeFactory.Closed<DateOnly>(date, date);
        var domain = new StandardDateOnlyBusinessDaysVariableStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        Assert.Equal(1.0, span.Value);
    }

    [Fact]
    public void Span_DateOnly_SingleDayInclusive_OnBusinessDay_ReturnsOne()
    {
        // Arrange - single Monday, both inclusive
        var date = new DateOnly(2024, 1, 1); // Monday
        var range = RangeFactory.Closed<DateOnly>(date, date);
        var domain = new StandardDateOnlyBusinessDaysVariableStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        Assert.Equal(1.0, span.Value);
    }

    [Fact]
    public void Span_DateOnly_WeekendOnly_ReturnsZero()
    {
        // Arrange - Saturday to Sunday inclusive
        var start = new DateOnly(2024, 1, 6);  // Saturday
        var end = new DateOnly(2024, 1, 7);    // Sunday
        var range = RangeFactory.Closed<DateOnly>(start, end);
        var domain = new StandardDateOnlyBusinessDaysVariableStepDomain();

        // Act
        var span = range.Span(domain);

        // Assert
        Assert.True(span.IsFinite);
        // Weekend days are not business days
        Assert.Equal(0.0, span.Value);
    }

    #endregion

    #region Shift Tests - StandardDateTimeBusinessDaysVariableStepDomain

    [Fact]
    public void Shift_DateTime_BusinessDaysRange_ShiftsCorrectly()
    {
        // Arrange - Monday to Friday
        var range = RangeFactory.Closed<DateTime>(new DateTime(2024, 1, 1), new DateTime(2024, 1, 5));
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();

        // Act - Shift forward by 3 business days
        var shifted = range.Shift(domain, 3);

        // Assert - Should shift to Thursday-Wednesday, skipping weekend
        Assert.Equal(new DateTime(2024, 1, 4).Date, shifted.Start.Value.Date);
        Assert.Equal(new DateTime(2024, 1, 10).Date, shifted.End.Value.Date);
        Assert.True(shifted.IsStartInclusive);
        Assert.True(shifted.IsEndInclusive);
    }

    [Fact]
    public void Shift_DateTime_BusinessDaysRangeNegativeOffset_ShiftsCorrectly()
    {
        // Arrange - Wednesday to Friday
        var range = RangeFactory.Closed<DateTime>(new DateTime(2024, 1, 10), new DateTime(2024, 1, 12));
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();

        // Act - Shift back by 3 business days
        var shifted = range.Shift(domain, -3);

        // Assert - Should shift back to Friday-Tuesday (previous week), skipping weekend
        Assert.Equal(new DateTime(2024, 1, 5).Date, shifted.Start.Value.Date);
        Assert.Equal(new DateTime(2024, 1, 9).Date, shifted.End.Value.Date);
    }

    [Fact]
    public void Shift_DateTime_UnboundedRange_PreservesInfinity()
    {
        // Arrange
        var range = RangeFactory.Closed<DateTime>(RangeValue<DateTime>.NegativeInfinity, new DateTime(2024, 1, 10));
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();

        // Act
        var shifted = range.Shift(domain, 5);

        // Assert
        Assert.False(shifted.Start.IsFinite);
        Assert.True(shifted.Start.IsNegativeInfinity);
        Assert.Equal(new DateTime(2024, 1, 17).Date, shifted.End.Value.Date);
    }

    [Fact]
    public void Shift_DateTime_PreservesInclusivity()
    {
        // Arrange
        var range = RangeFactory.Open<DateTime>(new DateTime(2024, 1, 1), new DateTime(2024, 1, 5));
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();

        // Act
        var shifted = range.Shift(domain, 2);

        // Assert
        Assert.Equal(new DateTime(2024, 1, 3).Date, shifted.Start.Value.Date);
        Assert.Equal(new DateTime(2024, 1, 9).Date, shifted.End.Value.Date);
        Assert.False(shifted.IsStartInclusive);
        Assert.False(shifted.IsEndInclusive);
    }

    [Fact]
    public void Shift_DateTime_PreservesTimeComponent()
    {
        // Arrange - Monday 9 AM to Friday 5 PM
        var range = RangeFactory.Closed<DateTime>(
            new DateTime(2024, 1, 1, 9, 0, 0),
            new DateTime(2024, 1, 5, 17, 0, 0));
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();

        // Act
        var shifted = range.Shift(domain, 3);

        // Assert - Time components preserved
        Assert.Equal(9, shifted.Start.Value.Hour);
        Assert.Equal(17, shifted.End.Value.Hour);
    }

    #endregion

    #region Shift Tests - StandardDateOnlyBusinessDaysVariableStepDomain

    [Fact]
    public void Shift_DateOnly_BusinessDaysRange_ShiftsCorrectly()
    {
        // Arrange - Monday to Friday
        var range = RangeFactory.Closed<DateOnly>(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 5));
        var domain = new StandardDateOnlyBusinessDaysVariableStepDomain();

        // Act - Shift forward by 3 business days
        var shifted = range.Shift(domain, 3);

        // Assert - Should shift to Thursday-Wednesday, skipping weekend
        Assert.Equal(new DateOnly(2024, 1, 4), shifted.Start.Value);
        Assert.Equal(new DateOnly(2024, 1, 10), shifted.End.Value);
    }

    [Fact]
    public void Shift_DateOnly_AcrossWeekend_SkipsWeekend()
    {
        // Arrange - Thursday to Friday
        var range = RangeFactory.Closed<DateOnly>(new DateOnly(2024, 1, 4), new DateOnly(2024, 1, 5));
        var domain = new StandardDateOnlyBusinessDaysVariableStepDomain();

        // Act - Shift forward by 2 business days
        var shifted = range.Shift(domain, 2);

        // Assert - Should shift to Monday-Tuesday, skipping weekend
        Assert.Equal(new DateOnly(2024, 1, 8), shifted.Start.Value);
        Assert.Equal(new DateOnly(2024, 1, 9), shifted.End.Value);
    }

    #endregion

    #region Expand Tests - StandardDateTimeBusinessDaysVariableStepDomain

    [Fact]
    public void Expand_DateTime_BusinessDaysRange_ExpandsCorrectly()
    {
        // Arrange - Tuesday to Thursday
        var range = RangeFactory.Closed<DateTime>(new DateTime(2024, 1, 2), new DateTime(2024, 1, 4));
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();

        // Act - Expand by 2 business days on left, 3 on right
        var expanded = range.Expand(domain, left: 2, right: 3);

        // Assert - Left: 2 days before Tuesday = previous Friday
        // Right: 3 days after Thursday = next Tuesday
        Assert.Equal(new DateTime(2023, 12, 29).Date, expanded.Start.Value.Date);
        Assert.Equal(new DateTime(2024, 1, 9).Date, expanded.End.Value.Date);
    }

    [Fact]
    public void Expand_DateTime_BusinessDaysRangeNegativeValues_ContractsRange()
    {
        // Arrange - Monday to Friday (5 business days)
        var range = RangeFactory.Closed<DateTime>(new DateTime(2024, 1, 1), new DateTime(2024, 1, 5));
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();

        // Act - Contract by 1 business day on each side
        var expanded = range.Expand(domain, left: -1, right: -1);

        // Assert - Should contract to Tuesday-Thursday
        Assert.Equal(new DateTime(2024, 1, 2).Date, expanded.Start.Value.Date);
        Assert.Equal(new DateTime(2024, 1, 4).Date, expanded.End.Value.Date);
    }

    [Fact]
    public void Expand_DateTime_UnboundedRange_PreservesInfinity()
    {
        // Arrange
        var range = RangeFactory.Closed<DateTime>(RangeValue<DateTime>.NegativeInfinity, new DateTime(2024, 1, 10));
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();

        // Act
        var expanded = range.Expand(domain, left: 5, right: 5);

        // Assert
        Assert.False(expanded.Start.IsFinite);
        Assert.True(expanded.Start.IsNegativeInfinity);
        Assert.Equal(new DateTime(2024, 1, 17).Date, expanded.End.Value.Date);
    }

    [Fact]
    public void Expand_DateTime_AsymmetricExpansion_WorksCorrectly()
    {
        // Arrange - Monday to Wednesday
        var range = RangeFactory.Closed<DateTime>(new DateTime(2024, 1, 1), new DateTime(2024, 1, 3));
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();

        // Act - Expand left by 3, right by 0
        var expanded = range.Expand(domain, left: 3, right: 0);

        // Assert - 3 business days before Monday = previous Wednesday
        Assert.Equal(new DateTime(2023, 12, 27).Date, expanded.Start.Value.Date);
        Assert.Equal(new DateTime(2024, 1, 3).Date, expanded.End.Value.Date);
    }

    [Fact]
    public void Expand_DateTime_PreservesInclusivity()
    {
        // Arrange
        var range = RangeFactory.OpenClosed<DateTime>(new DateTime(2024, 1, 2), new DateTime(2024, 1, 4));
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();

        // Act
        var expanded = range.Expand(domain, left: 1, right: 1);

        // Assert
        Assert.Equal(new DateTime(2024, 1, 1).Date, expanded.Start.Value.Date);
        Assert.Equal(new DateTime(2024, 1, 5).Date, expanded.End.Value.Date);
        Assert.False(expanded.IsStartInclusive);
        Assert.True(expanded.IsEndInclusive);
    }

    [Fact]
    public void Expand_DateTime_PreservesTimeComponent()
    {
        // Arrange - Tuesday 10 AM to Thursday 3 PM
        var range = RangeFactory.Closed<DateTime>(
            new DateTime(2024, 1, 2, 10, 0, 0),
            new DateTime(2024, 1, 4, 15, 0, 0));
        var domain = new StandardDateTimeBusinessDaysVariableStepDomain();

        // Act
        var expanded = range.Expand(domain, left: 1, right: 1);

        // Assert - Time components preserved
        Assert.Equal(10, expanded.Start.Value.Hour);
        Assert.Equal(15, expanded.End.Value.Hour);
    }

    #endregion

    #region Expand Tests - StandardDateOnlyBusinessDaysVariableStepDomain

    [Fact]
    public void Expand_DateOnly_BusinessDaysRange_ExpandsCorrectly()
    {
        // Arrange - Tuesday to Thursday
        var range = RangeFactory.Closed<DateOnly>(new DateOnly(2024, 1, 2), new DateOnly(2024, 1, 4));
        var domain = new StandardDateOnlyBusinessDaysVariableStepDomain();

        // Act - Expand by 2 business days on left, 3 on right
        var expanded = range.Expand(domain, left: 2, right: 3);

        // Assert - Left: 2 days before Tuesday = previous Friday
        // Right: 3 days after Thursday = next Tuesday
        Assert.Equal(new DateOnly(2023, 12, 29), expanded.Start.Value);
        Assert.Equal(new DateOnly(2024, 1, 9), expanded.End.Value);
    }

    [Fact]
    public void Expand_DateOnly_AcrossWeekend_SkipsWeekend()
    {
        // Arrange - Thursday to Friday
        var range = RangeFactory.Closed<DateOnly>(new DateOnly(2024, 1, 4), new DateOnly(2024, 1, 5));
        var domain = new StandardDateOnlyBusinessDaysVariableStepDomain();

        // Act - Expand right by 2 business days
        var expanded = range.Expand(domain, left: 0, right: 2);

        // Assert - 2 business days after Friday = next Tuesday
        Assert.Equal(new DateOnly(2024, 1, 4), expanded.Start.Value);
        Assert.Equal(new DateOnly(2024, 1, 9), expanded.End.Value);
    }

    #endregion
}
