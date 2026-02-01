using Intervals.NET.Domain.Default.DateTime;

namespace Intervals.NET.Domain.Default.Tests.DateTime;

public class DateTimeSubSecondFixedStepDomainTests
{
    [Fact]
    public void DateTimeHour_Add_AddsHoursCorrectly()
    {
        // Arrange
        var domain = new DateTimeHourFixedStepDomain();
        var start = new System.DateTime(2024, 1, 1, 10, 30, 0);

        // Act
        var result = domain.Add(start, 5);

        // Assert
        Assert.Equal(new System.DateTime(2024, 1, 1, 15, 30, 0), result);
    }

    [Fact]
    public void DateTimeHour_Ceiling_RoundsUpToNextHour()
    {
        // Arrange
        var domain = new DateTimeHourFixedStepDomain();
        var value = new System.DateTime(2024, 1, 1, 10, 30, 0);

        // Act
        var result = domain.Ceiling(value);

        // Assert
        Assert.Equal(new System.DateTime(2024, 1, 1, 11, 0, 0), result);
    }

    [Fact]
    public void DateTimeHour_Distance_CalculatesHoursCorrectly()
    {
        // Arrange
        var domain = new DateTimeHourFixedStepDomain();
        var start = new System.DateTime(2024, 1, 1, 10, 0, 0);
        var end = new System.DateTime(2024, 1, 1, 15, 0, 0);

        // Act
        var result = domain.Distance(start, end);

        // Assert
        Assert.Equal(5, result);
    }

    [Fact]
    public void DateTimeMinute_Add_AddsMinutesCorrectly()
    {
        // Arrange
        var domain = new DateTimeMinuteFixedStepDomain();
        var start = new System.DateTime(2024, 1, 1, 10, 30, 45);

        // Act
        var result = domain.Add(start, 15);

        // Assert
        Assert.Equal(new System.DateTime(2024, 1, 1, 10, 45, 45), result);
    }

    [Fact]
    public void DateTimeMinute_Ceiling_RoundsUpToNextMinute()
    {
        // Arrange
        var domain = new DateTimeMinuteFixedStepDomain();
        var value = new System.DateTime(2024, 1, 1, 10, 30, 45);

        // Act
        var result = domain.Ceiling(value);

        // Assert
        Assert.Equal(new System.DateTime(2024, 1, 1, 10, 31, 0), result);
    }

    [Fact]
    public void DateTimeMinute_Distance_CalculatesMinutesCorrectly()
    {
        // Arrange
        var domain = new DateTimeMinuteFixedStepDomain();
        var start = new System.DateTime(2024, 1, 1, 10, 30, 0);
        var end = new System.DateTime(2024, 1, 1, 10, 45, 0);

        // Act
        var result = domain.Distance(start, end);

        // Assert
        Assert.Equal(15, result);
    }

    [Fact]
    public void DateTimeSecond_Add_AddsSecondsCorrectly()
    {
        // Arrange
        var domain = new DateTimeSecondFixedStepDomain();
        var start = new System.DateTime(2024, 1, 1, 10, 30, 45);

        // Act
        var result = domain.Add(start, 30);

        // Assert
        Assert.Equal(new System.DateTime(2024, 1, 1, 10, 31, 15), result);
    }

    [Fact]
    public void DateTimeSecond_Ceiling_RoundsUpToNextSecond()
    {
        // Arrange
        var domain = new DateTimeSecondFixedStepDomain();
        var value = new System.DateTime(2024, 1, 1, 10, 30, 45, 500);

        // Act
        var result = domain.Ceiling(value);

        // Assert
        Assert.Equal(new System.DateTime(2024, 1, 1, 10, 30, 46), result);
    }

    [Fact]
    public void DateTimeSecond_Distance_CalculatesSecondsCorrectly()
    {
        // Arrange
        var domain = new DateTimeSecondFixedStepDomain();
        var start = new System.DateTime(2024, 1, 1, 10, 30, 15);
        var end = new System.DateTime(2024, 1, 1, 10, 30, 45);

        // Act
        var result = domain.Distance(start, end);

        // Assert
        Assert.Equal(30, result);
    }

    [Fact]
    public void DateTimeMillisecond_Add_AddsMillisecondsCorrectly()
    {
        // Arrange
        var domain = new DateTimeMillisecondFixedStepDomain();
        var start = new System.DateTime(2024, 1, 1, 10, 0, 0, 100);

        // Act
        var result = domain.Add(start, 250);

        // Assert
        Assert.Equal(new System.DateTime(2024, 1, 1, 10, 0, 0, 350), result);
    }

    [Fact]
    public void DateTimeMillisecond_Ceiling_RoundsUpCorrectly()
    {
        // Arrange
        var domain = new DateTimeMillisecondFixedStepDomain();
        var value = new System.DateTime(2024, 1, 1, 10, 0, 0, 100).AddTicks(5000);

        // Act
        var result = domain.Ceiling(value);

        // Assert
        Assert.Equal(new System.DateTime(2024, 1, 1, 10, 0, 0, 101), result);
    }

    [Fact]
    public void DateTimeMillisecond_Distance_CalculatesCorrectly()
    {
        // Arrange
        var domain = new DateTimeMillisecondFixedStepDomain();
        var start = new System.DateTime(2024, 1, 1, 10, 0, 0, 100);
        var end = new System.DateTime(2024, 1, 1, 10, 0, 0, 350);

        // Act
        var result = domain.Distance(start, end);

        // Assert
        Assert.Equal(250, result);
    }

    [Fact]
    public void DateTimeMillisecond_Floor_RoundsDownCorrectly()
    {
        // Arrange
        var domain = new DateTimeMillisecondFixedStepDomain();
        var value = new System.DateTime(2024, 1, 1, 10, 0, 0, 100).AddTicks(5000);

        // Act
        var result = domain.Floor(value);

        // Assert
        Assert.Equal(new System.DateTime(2024, 1, 1, 10, 0, 0, 100), result);
    }

    [Fact]
    public void DateTimeMillisecond_Subtract_SubtractsMillisecondsCorrectly()
    {
        // Arrange
        var domain = new DateTimeMillisecondFixedStepDomain();
        var value = new System.DateTime(2024, 1, 1, 10, 0, 0, 350);

        // Act
        var result = domain.Subtract(value, 250);

        // Assert
        Assert.Equal(new System.DateTime(2024, 1, 1, 10, 0, 0, 100), result);
    }

    [Fact]
    public void DateTimeMicrosecond_Add_AddsMicrosecondsCorrectly()
    {
        // Arrange
        var domain = new DateTimeMicrosecondFixedStepDomain();
        var start = new System.DateTime(2024, 1, 1, 10, 0, 0).AddTicks(100);

        // Act
        var result = domain.Add(start, 50);

        // Assert
        Assert.Equal(new System.DateTime(2024, 1, 1, 10, 0, 0).AddTicks(600), result);
    }

    [Fact]
    public void DateTimeMicrosecond_Ceiling_RoundsUpCorrectly()
    {
        // Arrange
        var domain = new DateTimeMicrosecondFixedStepDomain();
        var value = new System.DateTime(2024, 1, 1, 10, 0, 0).AddTicks(15);

        // Act
        var result = domain.Ceiling(value);

        // Assert
        Assert.Equal(new System.DateTime(2024, 1, 1, 10, 0, 0).AddTicks(20), result);
    }

    [Fact]
    public void DateTimeMicrosecond_Distance_CalculatesCorrectly()
    {
        // Arrange
        var domain = new DateTimeMicrosecondFixedStepDomain();
        var start = new System.DateTime(2024, 1, 1, 10, 0, 0).AddTicks(100);
        var end = new System.DateTime(2024, 1, 1, 10, 0, 0).AddTicks(600);

        // Act
        var result = domain.Distance(start, end);

        // Assert
        Assert.Equal(50, result); // (600-100)/10 = 50 microseconds
    }

    [Fact]
    public void DateTimeMicrosecond_Subtract_SubtractsMicrosecondsCorrectly()
    {
        // Arrange
        var domain = new DateTimeMicrosecondFixedStepDomain();
        var value = new System.DateTime(2024, 1, 1, 10, 0, 0).AddTicks(600);

        // Act
        var result = domain.Subtract(value, 50);

        // Assert
        Assert.Equal(new System.DateTime(2024, 1, 1, 10, 0, 0).AddTicks(100), result);
    }

    [Fact]
    public void DateTimeTicks_Add_AddsTicksCorrectly()
    {
        // Arrange
        var domain = new DateTimeTicksFixedStepDomain();
        var start = new System.DateTime(2024, 1, 1, 10, 0, 0, 0).AddTicks(100);

        // Act
        var result = domain.Add(start, 500);

        // Assert
        Assert.Equal(new System.DateTime(2024, 1, 1, 10, 0, 0, 0).AddTicks(600), result);
    }

    [Fact]
    public void DateTimeTicks_Ceiling_ReturnsUnchanged()
    {
        // Arrange
        var domain = new DateTimeTicksFixedStepDomain();
        var value = new System.DateTime(2024, 1, 1, 10, 0, 0, 0).AddTicks(123);

        // Act
        var result = domain.Ceiling(value);

        // Assert
        Assert.Equal(value, result); // Ticks is finest granularity
    }

    [Fact]
    public void DateTimeTicks_Distance_CalculatesTicksCorrectly()
    {
        // Arrange
        var domain = new DateTimeTicksFixedStepDomain();
        var start = new System.DateTime(2024, 1, 1, 10, 0, 0, 0).AddTicks(100);
        var end = new System.DateTime(2024, 1, 1, 10, 0, 0, 0).AddTicks(600);

        // Act
        var result = domain.Distance(start, end);

        // Assert
        Assert.Equal(500, result);
    }

    [Fact]
    public void DateTimeTicks_Floor_ReturnsUnchanged()
    {
        // Arrange
        var domain = new DateTimeTicksFixedStepDomain();
        var value = new System.DateTime(2024, 1, 1, 10, 0, 0, 0).AddTicks(123);

        // Act
        var result = domain.Floor(value);

        // Assert
        Assert.Equal(value, result); // Ticks is finest granularity
    }

    [Fact]
    public void DateTimeTicks_Subtract_SubtractsTicksCorrectly()
    {
        // Arrange
        var domain = new DateTimeTicksFixedStepDomain();
        var value = new System.DateTime(2024, 1, 1, 10, 0, 0, 0).AddTicks(1000);

        // Act
        var result = domain.Subtract(value, 250);

        // Assert
        Assert.Equal(new System.DateTime(2024, 1, 1, 10, 0, 0, 0).AddTicks(750), result);
    }

    [Fact]
    public void DateTimeHour_Subtract_SubtractsHoursCorrectly()
    {
        // Arrange
        var domain = new DateTimeHourFixedStepDomain();
        var value = new System.DateTime(2024, 1, 1, 15, 30, 0);

        // Act
        var result = domain.Subtract(value, 5);

        // Assert
        Assert.Equal(new System.DateTime(2024, 1, 1, 10, 30, 0), result);
    }

    [Fact]
    public void DateTimeMinute_Subtract_SubtractsMinutesCorrectly()
    {
        // Arrange
        var domain = new DateTimeMinuteFixedStepDomain();
        var value = new System.DateTime(2024, 1, 1, 10, 45, 0);

        // Act
        var result = domain.Subtract(value, 15);

        // Assert
        Assert.Equal(new System.DateTime(2024, 1, 1, 10, 30, 0), result);
    }

    [Fact]
    public void DateTimeSecond_Subtract_SubtractsSecondsCorrectly()
    {
        // Arrange
        var domain = new DateTimeSecondFixedStepDomain();
        var value = new System.DateTime(2024, 1, 1, 10, 30, 45);

        // Act
        var result = domain.Subtract(value, 30);

        // Assert
        Assert.Equal(new System.DateTime(2024, 1, 1, 10, 30, 15), result);
    }
}