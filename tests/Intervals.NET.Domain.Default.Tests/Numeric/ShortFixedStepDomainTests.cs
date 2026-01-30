using Intervals.NET.Domain.Default.Numeric;

namespace Intervals.NET.Domain.Default.Tests.Numeric;

public class ShortFixedStepDomainTests
{
    private readonly ShortFixedStepDomain _domain = new();

    [Fact]
    public void Floor_ReturnsValueItself_BecauseShortIsDiscrete()
    {
        // Arrange
        short value = 42;

        // Act
        var result = _domain.Floor(value);

        // Assert
        Assert.Equal(42, result);
    }

    [Fact]
    public void Ceiling_ReturnsValueItself_BecauseShortIsDiscrete()
    {
        // Arrange
        short value = 42;

        // Act
        var result = _domain.Ceiling(value);

        // Assert
        Assert.Equal(42, result);
    }

    [Theory]
    [InlineData((short)10, (short)20, 10L)]
    [InlineData((short)0, (short)100, 100L)]
    [InlineData((short)-50, (short)50, 100L)]
    [InlineData((short)20, (short)10, -10L)]
    public void Distance_CalculatesCorrectly(short start, short end, long expected)
    {
        // Act
        var result = _domain.Distance(start, end);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData((short)10, 5L, (short)15)]
    [InlineData((short)10, -5L, (short)5)]
    [InlineData((short)0, 100L, (short)100)]
    public void Add_AddsOffsetCorrectly(short value, long offset, short expected)
    {
        // Act
        var result = _domain.Add(value, offset);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Add_ThrowsOverflowException_WhenResultExceedsMaxValue()
    {
        // Arrange
        short value = short.MaxValue;
        long offset = 1;

        // Act & Assert
        Assert.Throws<OverflowException>(() => _domain.Add(value, offset));
    }

    [Fact]
    public void Add_ThrowsOverflowException_WhenResultBelowMinValue()
    {
        // Arrange
        short value = short.MinValue;
        long offset = -1;

        // Act & Assert
        Assert.Throws<OverflowException>(() => _domain.Add(value, offset));
    }

    [Theory]
    [InlineData((short)20, 5L, (short)15)]
    [InlineData((short)10, -5L, (short)15)]
    public void Subtract_SubtractsOffsetCorrectly(short value, long offset, short expected)
    {
        // Act
        var result = _domain.Subtract(value, offset);

        // Assert
        Assert.Equal(expected, result);
    }
}
