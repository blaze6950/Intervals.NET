namespace Intervals.NET.Tests;

public class RangeValueTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithFiniteValue_CreatesFiniteRangeValue()
    {
        // Arrange & Act
        var rangeValue = new RangeValue<int>(42);

        // Assert
        Assert.True(rangeValue.IsFinite);
        Assert.False(rangeValue.IsPositiveInfinity);
        Assert.False(rangeValue.IsNegativeInfinity);
        Assert.Equal(42, rangeValue.Value);
    }

    [Fact]
    public void Constructor_WithNullableValue_CreatesFiniteRangeValue()
    {
        // Arrange & Act
        var rangeValue = new RangeValue<string>("test");

        // Assert
        Assert.True(rangeValue.IsFinite);
        Assert.Equal("test", rangeValue.Value);
    }

    #endregion

    #region Static Factory Tests

    [Fact]
    public void NegativeInfinity_ReturnsNegativeInfinityRangeValue()
    {
        // Arrange & Act
        var rangeValue = RangeValue<int>.NegativeInfinity;

        // Assert
        Assert.True(rangeValue.IsNegativeInfinity);
        Assert.False(rangeValue.IsFinite);
        Assert.False(rangeValue.IsPositiveInfinity);
    }

    [Fact]
    public void PositiveInfinity_ReturnsPositiveInfinityRangeValue()
    {
        // Arrange & Act
        var rangeValue = RangeValue<int>.PositiveInfinity;

        // Assert
        Assert.True(rangeValue.IsPositiveInfinity);
        Assert.False(rangeValue.IsFinite);
        Assert.False(rangeValue.IsNegativeInfinity);
    }

    #endregion

    #region TryGetValue Tests

    [Fact]
    public void TryGetValue_WithFiniteValue_ReturnsTrueAndValue()
    {
        // Arrange
        var rangeValue = new RangeValue<int>(42);

        // Act
        var result = rangeValue.TryGetValue(out var value);

        // Assert
        Assert.True(result);
        Assert.Equal(42, value);
    }

    [Fact]
    public void TryGetValue_WithPositiveInfinity_ReturnsFalseAndDefault()
    {
        // Arrange
        var rangeValue = RangeValue<int>.PositiveInfinity;

        // Act
        var result = rangeValue.TryGetValue(out var value);

        // Assert
        Assert.False(result);
        Assert.Equal(default, value);
    }

    [Fact]
    public void TryGetValue_WithNegativeInfinity_ReturnsFalseAndDefault()
    {
        // Arrange
        var rangeValue = RangeValue<int>.NegativeInfinity;

        // Act
        var result = rangeValue.TryGetValue(out var value);

        // Assert
        Assert.False(result);
        Assert.Equal(default, value);
    }

    #endregion

    #region Value Property Tests

    [Fact]
    public void Value_WithFiniteValue_ReturnsValue()
    {
        // Arrange
        var rangeValue = new RangeValue<int>(42);

        // Act
        var value = rangeValue.Value;

        // Assert
        Assert.Equal(42, value);
    }

    [Fact]
    public void Value_WithPositiveInfinity_ThrowsInvalidOperationException()
    {
        // Arrange
        var rangeValue = RangeValue<int>.PositiveInfinity;

        // Act
        var exception = Record.Exception(() => rangeValue.Value);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
        Assert.Contains("Infinite range value has no finite value", exception.Message);
    }

    [Fact]
    public void Value_WithNegativeInfinity_ThrowsInvalidOperationException()
    {
        // Arrange
        var rangeValue = RangeValue<int>.NegativeInfinity;

        // Act
        var exception = Record.Exception(() => rangeValue.Value);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
        Assert.Contains("Infinite range value has no finite value", exception.Message);
    }

    #endregion

    #region CompareTo Tests

    [Fact]
    public void CompareTo_BothFiniteAndEqual_ReturnsZero()
    {
        // Arrange
        var left = new RangeValue<int>(42);
        var right = new RangeValue<int>(42);

        // Act
        var result = left.CompareTo(right);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CompareTo_BothFiniteLeftLessThanRight_ReturnsNegative()
    {
        // Arrange
        var left = new RangeValue<int>(10);
        var right = new RangeValue<int>(42);

        // Act
        var result = left.CompareTo(right);

        // Assert
        Assert.True(result < 0);
    }

    [Fact]
    public void CompareTo_BothFiniteLeftGreaterThanRight_ReturnsPositive()
    {
        // Arrange
        var left = new RangeValue<int>(100);
        var right = new RangeValue<int>(42);

        // Act
        var result = left.CompareTo(right);

        // Assert
        Assert.True(result > 0);
    }

    [Fact]
    public void CompareTo_BothNegativeInfinity_ReturnsZero()
    {
        // Arrange
        var left = RangeValue<int>.NegativeInfinity;
        var right = RangeValue<int>.NegativeInfinity;

        // Act
        var result = left.CompareTo(right);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CompareTo_BothPositiveInfinity_ReturnsZero()
    {
        // Arrange
        var left = RangeValue<int>.PositiveInfinity;
        var right = RangeValue<int>.PositiveInfinity;

        // Act
        var result = left.CompareTo(right);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CompareTo_NegativeInfinityAndFinite_ReturnsNegative()
    {
        // Arrange
        var left = RangeValue<int>.NegativeInfinity;
        var right = new RangeValue<int>(42);

        // Act
        var result = left.CompareTo(right);

        // Assert
        Assert.True(result < 0);
    }

    [Fact]
    public void CompareTo_PositiveInfinityAndFinite_ReturnsPositive()
    {
        // Arrange
        var left = RangeValue<int>.PositiveInfinity;
        var right = new RangeValue<int>(42);

        // Act
        var result = left.CompareTo(right);

        // Assert
        Assert.True(result > 0);
    }

    [Fact]
    public void CompareTo_FiniteAndNegativeInfinity_ReturnsPositive()
    {
        // Arrange
        var left = new RangeValue<int>(42);
        var right = RangeValue<int>.NegativeInfinity;

        // Act
        var result = left.CompareTo(right);

        // Assert
        Assert.True(result > 0);
    }

    [Fact]
    public void CompareTo_FiniteAndPositiveInfinity_ReturnsNegative()
    {
        // Arrange
        var left = new RangeValue<int>(42);
        var right = RangeValue<int>.PositiveInfinity;

        // Act
        var result = left.CompareTo(right);

        // Assert
        Assert.True(result < 0);
    }

    [Fact]
    public void CompareTo_NegativeInfinityAndPositiveInfinity_ReturnsNegative()
    {
        // Arrange
        var left = RangeValue<int>.NegativeInfinity;
        var right = RangeValue<int>.PositiveInfinity;

        // Act
        var result = left.CompareTo(right);

        // Assert
        Assert.True(result < 0);
    }

    [Fact]
    public void CompareTo_PositiveInfinityAndNegativeInfinity_ReturnsPositive()
    {
        // Arrange
        var left = RangeValue<int>.PositiveInfinity;
        var right = RangeValue<int>.NegativeInfinity;

        // Act
        var result = left.CompareTo(right);

        // Assert
        Assert.True(result > 0);
    }

    [Fact]
    public void Compare_StaticMethod_ReturnsSameAsCompareTo()
    {
        // Arrange
        var left = new RangeValue<int>(10);
        var right = new RangeValue<int>(42);

        // Act
        var compareToResult = left.CompareTo(right);
        var compareResult = RangeValue<int>.Compare(left, right);

        // Assert
        Assert.Equal(compareToResult, compareResult);
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void Equals_BothFiniteAndEqual_ReturnsTrue()
    {
        // Arrange
        var left = new RangeValue<int>(42);
        var right = new RangeValue<int>(42);

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_BothFiniteAndNotEqual_ReturnsFalse()
    {
        // Arrange
        var left = new RangeValue<int>(42);
        var right = new RangeValue<int>(100);

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_BothNegativeInfinity_ReturnsTrue()
    {
        // Arrange
        var left = RangeValue<int>.NegativeInfinity;
        var right = RangeValue<int>.NegativeInfinity;

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_BothPositiveInfinity_ReturnsTrue()
    {
        // Arrange
        var left = RangeValue<int>.PositiveInfinity;
        var right = RangeValue<int>.PositiveInfinity;

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_DifferentKinds_ReturnsFalse()
    {
        // Arrange
        var left = new RangeValue<int>(42);
        var right = RangeValue<int>.PositiveInfinity;

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_NegativeAndPositiveInfinity_ReturnsFalse()
    {
        // Arrange
        var left = RangeValue<int>.NegativeInfinity;
        var right = RangeValue<int>.PositiveInfinity;

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_WithObject_SameValue_ReturnsTrue()
    {
        // Arrange
        var left = new RangeValue<int>(42);
        object right = new RangeValue<int>(42);

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_WithObject_DifferentValue_ReturnsFalse()
    {
        // Arrange
        var left = new RangeValue<int>(42);
        object right = new RangeValue<int>(100);

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_WithObject_DifferentType_ReturnsFalse()
    {
        // Arrange
        var left = new RangeValue<int>(42);
        object right = 42;

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_WithObject_Null_ReturnsFalse()
    {
        // Arrange
        var left = new RangeValue<int>(42);
        object? right = null;

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region GetHashCode Tests

    [Fact]
    public void GetHashCode_EqualFiniteValues_ReturnsSameHashCode()
    {
        // Arrange
        var left = new RangeValue<int>(42);
        var right = new RangeValue<int>(42);

        // Act
        var leftHash = left.GetHashCode();
        var rightHash = right.GetHashCode();

        // Assert
        Assert.Equal(leftHash, rightHash);
    }

    [Fact]
    public void GetHashCode_PositiveInfinity_ReturnsIntMaxValue()
    {
        // Arrange
        var rangeValue = RangeValue<int>.PositiveInfinity;

        // Act
        var hash = rangeValue.GetHashCode();

        // Assert
        Assert.Equal(int.MaxValue, hash);
    }

    [Fact]
    public void GetHashCode_NegativeInfinity_ReturnsIntMinValue()
    {
        // Arrange
        var rangeValue = RangeValue<int>.NegativeInfinity;

        // Act
        var hash = rangeValue.GetHashCode();

        // Assert
        Assert.Equal(int.MinValue, hash);
    }

    [Fact]
    public void GetHashCode_DifferentFiniteValues_ReturnsDifferentHashCodes()
    {
        // Arrange
        var left = new RangeValue<int>(42);
        var right = new RangeValue<int>(100);

        // Act
        var leftHash = left.GetHashCode();
        var rightHash = right.GetHashCode();

        // Assert
        Assert.NotEqual(leftHash, rightHash);
    }

    #endregion

    #region Operator Tests

    [Fact]
    public void OperatorEquals_EqualValues_ReturnsTrue()
    {
        // Arrange
        var left = new RangeValue<int>(42);
        var right = new RangeValue<int>(42);

        // Act
        var result = left == right;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void OperatorEquals_DifferentValues_ReturnsFalse()
    {
        // Arrange
        var left = new RangeValue<int>(42);
        var right = new RangeValue<int>(100);

        // Act
        var result = left == right;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void OperatorNotEquals_EqualValues_ReturnsFalse()
    {
        // Arrange
        var left = new RangeValue<int>(42);
        var right = new RangeValue<int>(42);

        // Act
        var result = left != right;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void OperatorNotEquals_DifferentValues_ReturnsTrue()
    {
        // Arrange
        var left = new RangeValue<int>(42);
        var right = new RangeValue<int>(100);

        // Act
        var result = left != right;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void OperatorLessThan_LeftLessThanRight_ReturnsTrue()
    {
        // Arrange
        var left = new RangeValue<int>(10);
        var right = new RangeValue<int>(42);

        // Act
        var result = left < right;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void OperatorLessThan_LeftGreaterThanRight_ReturnsFalse()
    {
        // Arrange
        var left = new RangeValue<int>(100);
        var right = new RangeValue<int>(42);

        // Act
        var result = left < right;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void OperatorLessThan_EqualValues_ReturnsFalse()
    {
        // Arrange
        var left = new RangeValue<int>(42);
        var right = new RangeValue<int>(42);

        // Act
        var result = left < right;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void OperatorGreaterThan_LeftGreaterThanRight_ReturnsTrue()
    {
        // Arrange
        var left = new RangeValue<int>(100);
        var right = new RangeValue<int>(42);

        // Act
        var result = left > right;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void OperatorGreaterThan_LeftLessThanRight_ReturnsFalse()
    {
        // Arrange
        var left = new RangeValue<int>(10);
        var right = new RangeValue<int>(42);

        // Act
        var result = left > right;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void OperatorGreaterThan_EqualValues_ReturnsFalse()
    {
        // Arrange
        var left = new RangeValue<int>(42);
        var right = new RangeValue<int>(42);

        // Act
        var result = left > right;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void OperatorLessThanOrEqual_LeftLessThanRight_ReturnsTrue()
    {
        // Arrange
        var left = new RangeValue<int>(10);
        var right = new RangeValue<int>(42);

        // Act
        var result = left <= right;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void OperatorLessThanOrEqual_EqualValues_ReturnsTrue()
    {
        // Arrange
        var left = new RangeValue<int>(42);
        var right = new RangeValue<int>(42);

        // Act
        var result = left <= right;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void OperatorLessThanOrEqual_LeftGreaterThanRight_ReturnsFalse()
    {
        // Arrange
        var left = new RangeValue<int>(100);
        var right = new RangeValue<int>(42);

        // Act
        var result = left <= right;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void OperatorGreaterThanOrEqual_LeftGreaterThanRight_ReturnsTrue()
    {
        // Arrange
        var left = new RangeValue<int>(100);
        var right = new RangeValue<int>(42);

        // Act
        var result = left >= right;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void OperatorGreaterThanOrEqual_EqualValues_ReturnsTrue()
    {
        // Arrange
        var left = new RangeValue<int>(42);
        var right = new RangeValue<int>(42);

        // Act
        var result = left >= right;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void OperatorGreaterThanOrEqual_LeftLessThanRight_ReturnsFalse()
    {
        // Arrange
        var left = new RangeValue<int>(10);
        var right = new RangeValue<int>(42);

        // Act
        var result = left >= right;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ImplicitConversion_FromValue_CreatesFiniteRangeValue()
    {
        // Arrange & Act
        RangeValue<int> rangeValue = 42;

        // Assert
        Assert.True(rangeValue.IsFinite);
        Assert.Equal(42, rangeValue.Value);
    }

    [Fact]
    public void ImplicitConversion_FromString_CreatesFiniteRangeValue()
    {
        // Arrange & Act
        RangeValue<string> rangeValue = "test";

        // Assert
        Assert.True(rangeValue.IsFinite);
        Assert.Equal("test", rangeValue.Value);
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_FiniteValue_ReturnsValueString()
    {
        // Arrange
        var rangeValue = new RangeValue<int>(42);

        // Act
        var result = rangeValue.ToString();

        // Assert
        Assert.Equal("42", result);
    }

    [Fact]
    public void ToString_PositiveInfinity_ReturnsInfinitySymbol()
    {
        // Arrange
        var rangeValue = RangeValue<int>.PositiveInfinity;

        // Act
        var result = rangeValue.ToString();

        // Assert
        Assert.Equal("∞", result);
    }

    [Fact]
    public void ToString_NegativeInfinity_ReturnsNegativeInfinitySymbol()
    {
        // Arrange
        var rangeValue = RangeValue<int>.NegativeInfinity;

        // Act
        var result = rangeValue.ToString();

        // Assert
        Assert.Equal("-∞", result);
    }

    [Fact]
    public void ToString_StringValue_ReturnsString()
    {
        // Arrange
        var rangeValue = new RangeValue<string>("hello");

        // Act
        var result = rangeValue.ToString();

        // Assert
        Assert.Equal("hello", result);
    }

    #endregion

    #region Reference Type Tests

    [Fact]
    public void RangeValue_WithReferenceType_WorksCorrectly()
    {
        // Arrange
        var value1 = new RangeValue<string>("alpha");
        var value2 = new RangeValue<string>("beta");

        // Act
        var comparison = value1.CompareTo(value2);

        // Assert
        Assert.True(comparison < 0);
    }

    [Fact]
    public void RangeValue_WithDateTime_WorksCorrectly()
    {
        // Arrange
        var value1 = new RangeValue<DateTime>(new DateTime(2020, 1, 1));
        var value2 = new RangeValue<DateTime>(new DateTime(2021, 1, 1));

        // Act
        var comparison = value1.CompareTo(value2);

        // Assert
        Assert.True(comparison < 0);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void RangeValue_WithDefaultValue_WorksCorrectly()
    {
        // Arrange & Act
        var rangeValue = new RangeValue<int>(0);

        // Assert
        Assert.True(rangeValue.IsFinite);
        Assert.Equal(0, rangeValue.Value);
    }

    [Fact]
    public void RangeValue_WithNegativeValue_WorksCorrectly()
    {
        // Arrange & Act
        var rangeValue = new RangeValue<int>(-42);

        // Assert
        Assert.True(rangeValue.IsFinite);
        Assert.Equal(-42, rangeValue.Value);
    }

    [Fact]
    public void RangeValue_ComparePositiveInfinities_AreEqual()
    {
        // Arrange
        var value1 = RangeValue<int>.PositiveInfinity;
        var value2 = RangeValue<int>.PositiveInfinity;

        // Act & Assert
        Assert.True(value1 == value2);
        Assert.Equal(0, value1.CompareTo(value2));
    }

    [Fact]
    public void RangeValue_CompareNegativeInfinities_AreEqual()
    {
        // Arrange
        var value1 = RangeValue<int>.NegativeInfinity;
        var value2 = RangeValue<int>.NegativeInfinity;

        // Act & Assert
        Assert.True(value1 == value2);
        Assert.Equal(0, value1.CompareTo(value2));
    }

    #endregion

    #region Explicit Cast Operator Tests

    [Fact]
    public void ExplicitCast_WithFiniteValue_ReturnsUnderlyingValue()
    {
        // Arrange
        var rangeValue = new RangeValue<int>(42);

        // Act
        var value = (int)rangeValue;

        // Assert
        Assert.Equal(42, value);
    }

    [Fact]
    public void ExplicitCast_WithPositiveInfinity_ThrowsInvalidOperationException()
    {
        // Arrange
        var rangeValue = RangeValue<int>.PositiveInfinity;

        // Act
        var exception = Record.Exception(() => (int)rangeValue);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
        Assert.Contains("Infinite range value has no finite value", exception.Message);
    }

    [Fact]
    public void ExplicitCast_WithNegativeInfinity_ThrowsInvalidOperationException()
    {
        // Arrange
        var rangeValue = RangeValue<int>.NegativeInfinity;

        // Act
        var exception = Record.Exception(() => (int)rangeValue);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
        Assert.Contains("Infinite range value has no finite value", exception.Message);
    }

    [Fact]
    public void ExplicitCast_WithStringValue_ReturnsUnderlyingString()
    {
        // Arrange
        var rangeValue = new RangeValue<string>("test");

        // Act
        var value = (string)rangeValue;

        // Assert
        Assert.Equal("test", value);
    }

    [Fact]
    public void ExplicitCast_WithDateTimeValue_ReturnsUnderlyingDateTime()
    {
        // Arrange
        var dateTime = new DateTime(2020, 1, 1);
        var rangeValue = new RangeValue<DateTime>(dateTime);

        // Act
        var value = (DateTime)rangeValue;

        // Assert
        Assert.Equal(dateTime, value);
    }

    [Fact]
    public void ExplicitCast_WithDoubleValue_ReturnsUnderlyingDouble()
    {
        // Arrange
        var rangeValue = new RangeValue<double>(3.14159);

        // Act
        var value = (double)rangeValue;

        // Assert
        Assert.Equal(3.14159, value);
    }

    [Fact]
    public void ExplicitCast_WithZeroValue_ReturnsZero()
    {
        // Arrange
        var rangeValue = new RangeValue<int>(0);

        // Act
        var value = (int)rangeValue;

        // Assert
        Assert.Equal(0, value);
    }

    [Fact]
    public void ExplicitCast_WithNegativeValue_ReturnsNegativeValue()
    {
        // Arrange
        var rangeValue = new RangeValue<int>(-100);

        // Act
        var value = (int)rangeValue;

        // Assert
        Assert.Equal(-100, value);
    }

    [Fact]
    public void ExplicitCast_WithMinMaxValues_WorksCorrectly()
    {
        // Arrange
        var minValue = new RangeValue<int>(int.MinValue);
        var maxValue = new RangeValue<int>(int.MaxValue);

        // Act
        var min = (int)minValue;
        var max = (int)maxValue;

        // Assert
        Assert.Equal(int.MinValue, min);
        Assert.Equal(int.MaxValue, max);
    }

    [Fact]
    public void ExplicitCast_RoundTrip_PreservesValue()
    {
        // Arrange
        var original = 42;

        // Act - Round trip: T -> RangeValue<T> -> T
        RangeValue<int> rangeValue = original;  // Implicit conversion
        var result = (int)rangeValue;            // Explicit conversion

        // Assert
        Assert.Equal(original, result);
    }

    [Fact]
    public void ExplicitCast_WithStringRoundTrip_PreservesValue()
    {
        // Arrange
        var original = "Hello, World!";

        // Act
        RangeValue<string> rangeValue = original;
        var result = (string)rangeValue;

        // Assert
        Assert.Equal(original, result);
    }

    [Fact]
    public void ExplicitCast_WithCheckBeforeCast_DoesNotThrow()
    {
        // Arrange
        var rangeValue = new RangeValue<int>(42);

        // Act & Assert
        if (rangeValue.IsFinite)
        {
            var value = (int)rangeValue;
            Assert.Equal(42, value);
        }
    }

    [Fact]
    public void ExplicitCast_GuardAgainstInfinity_WorksCorrectly()
    {
        // Arrange
        var finiteValue = new RangeValue<int>(42);
        var infinityValue = RangeValue<int>.PositiveInfinity;

        // Act
        int? extractedValue = finiteValue.IsFinite ? (int)finiteValue : null;
        int? infinityExtracted = infinityValue.IsFinite ? (int)infinityValue : null;

        // Assert
        Assert.Equal(42, extractedValue);
        Assert.Null(infinityExtracted);
    }

    [Fact]
    public void ExplicitCast_ComparingWithImplicitConversion_ShowsAsymmetry()
    {
        // Arrange
        var value = 42;

        // Act
        RangeValue<int> implicitConversion = value;      // Implicit: easy
        var explicitConversion = (int)implicitConversion; // Explicit: requires cast

        // Assert - Demonstrates the asymmetry in design
        Assert.Equal(value, explicitConversion);
    }

    #endregion

    #region Nullable Reference Type Tests

    [Fact]
    public void RangeValue_NullableString_WithNullValue_GetHashCodeHandlesCorrectly()
    {
        // Arrange
        string? nullValue = null;
        var rangeValue = new RangeValue<string?>(nullValue);

        // Act
        var hashCode = rangeValue.GetHashCode();

        // Assert - Should not throw, uses EqualityComparer<T>.Default
        // Hash codes are allowed to be any value including 0
        _ = hashCode; // Verify no exception thrown
    }

    [Fact]
    public void RangeValue_NullableString_WithNullValue_ToStringHandlesCorrectly()
    {
        // Arrange
        string? nullValue = null;
        var rangeValue = new RangeValue<string?>(nullValue);

        // Act
        var result = rangeValue.ToString();

        // Assert - Should return empty string or null representation
        Assert.NotNull(result);
    }

    [Fact]
    public void RangeValue_NullableString_WithNullValue_AllOperationsWorkCorrectly()
    {
        // Arrange
        string? nullValue1 = null;
        string? nullValue2 = null;
        var rangeValue1 = new RangeValue<string?>(nullValue1);
        var rangeValue2 = new RangeValue<string?>(nullValue2);

        // Act & Assert - Equality works
        Assert.True(rangeValue1.Equals(rangeValue2));
        Assert.True(rangeValue1 == rangeValue2);

        // CompareTo works
        var comparison = rangeValue1.CompareTo(rangeValue2);
        Assert.Equal(0, comparison);

        // GetHashCode doesn't throw
        var hash1 = rangeValue1.GetHashCode();
        var hash2 = rangeValue2.GetHashCode();
        Assert.Equal(hash1, hash2);
    }

    #endregion
}
