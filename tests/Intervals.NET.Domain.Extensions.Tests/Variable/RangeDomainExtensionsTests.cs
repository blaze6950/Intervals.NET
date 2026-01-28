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

    #endregion
}
