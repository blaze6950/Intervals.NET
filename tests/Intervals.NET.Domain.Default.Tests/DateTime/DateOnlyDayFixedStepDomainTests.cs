using Intervals.NET.Domain.Default.DateTime;

namespace Intervals.NET.Domain.Default.Tests.DateTime;

public class DateOnlyDayFixedStepDomainTests
{
    private readonly DateOnlyDayFixedStepDomain _domain = new();

    [Fact]
    public void Floor_ReturnsValueItself_BecauseDateOnlyIsAlreadyDayAligned()
    {
        // Arrange
        var value = new DateOnly(2024, 6, 15);

        // Act
        var result = _domain.Floor(value);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void Ceiling_ReturnsValueItself_BecauseDateOnlyIsAlreadyDayAligned()
    {
        // Arrange
        var value = new DateOnly(2024, 6, 15);

        // Act
        var result = _domain.Ceiling(value);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void Distance_CalculatesDaysBetweenDates()
    {
        // Arrange
        var start = new DateOnly(2024, 1, 1);
        var end = new DateOnly(2024, 1, 11);

        // Act
        var result = _domain.Distance(start, end);

        // Assert
        Assert.Equal(10L, result);
    }

    [Fact]
    public void Distance_ReturnsNegative_WhenEndIsBeforeStart()
    {
        // Arrange
        var start = new DateOnly(2024, 1, 11);
        var end = new DateOnly(2024, 1, 1);

        // Act
        var result = _domain.Distance(start, end);

        // Assert
        Assert.Equal(-10L, result);
    }

    [Fact]
    public void Add_AddsDaysCorrectly()
    {
        // Arrange
        var value = new DateOnly(2024, 1, 1);

        // Act
        var result = _domain.Add(value, 10);

        // Assert
        Assert.Equal(new DateOnly(2024, 1, 11), result);
    }

    [Fact]
    public void Add_HandlesNegativeOffset()
    {
        // Arrange
        var value = new DateOnly(2024, 1, 11);

        // Act
        var result = _domain.Add(value, -10);

        // Assert
        Assert.Equal(new DateOnly(2024, 1, 1), result);
    }

    [Fact]
    public void Subtract_SubtractsDaysCorrectly()
    {
        // Arrange
        var value = new DateOnly(2024, 1, 11);

        // Act
        var result = _domain.Subtract(value, 10);

        // Assert
        Assert.Equal(new DateOnly(2024, 1, 1), result);
    }

    [Fact]
    public void Distance_HandlesMonthBoundaries()
    {
        // Arrange
        var start = new DateOnly(2024, 1, 31);
        var end = new DateOnly(2024, 2, 1);

        // Act
        var result = _domain.Distance(start, end);

        // Assert
        Assert.Equal(1L, result);
    }

    [Fact]
    public void Distance_HandlesYearBoundaries()
    {
        // Arrange
        var start = new DateOnly(2023, 12, 31);
        var end = new DateOnly(2024, 1, 1);

        // Act
        var result = _domain.Distance(start, end);

        // Assert
        Assert.Equal(1L, result);
    }

    [Fact]
    public void Add_ExceedingMaxDate_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var date = DateOnly.MaxValue.AddDays(-5);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _domain.Add(date, 10));
    }

    [Fact]
    public void Subtract_BelowMinDate_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var date = DateOnly.MinValue.AddDays(3);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _domain.Subtract(date, 10));
    }
}
