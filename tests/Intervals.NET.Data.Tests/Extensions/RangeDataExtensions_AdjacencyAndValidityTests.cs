using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intervals.NET.Data.Extensions;
using Intervals.NET;
using Intervals.NET.Data;
using Intervals.NET.Domain.Default.Numeric;
using Xunit;
using Range = Intervals.NET.Factories.Range;

namespace Intervals.NET.Data.Tests.Extensions;

public class RangeDataExtensions_AdjacencyAndValidityTests
{
    private readonly IntegerFixedStepDomain _domain = new();

    [Fact]
    public void IsValid_WithCorrectLength_ReturnsTrue()
    {
        // Arrange
        var data = Enumerable.Range(1, 11).ToArray(); // [10,20] -> 11 elements
        var rd = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data, _domain);

        // Act
        var ok = rd.IsValid(out var message);

        // Assert
        Assert.True(ok);
        Assert.Null(message);
    }

    [Fact]
    public void IsValid_WithFewerElements_ReturnsFalseAndMessageContainsFewer()
    {
        // Arrange
        var data = Enumerable.Range(1, 10).ToArray(); // one element short
        var rd = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data, _domain);

        // Act
        var ok = rd.IsValid(out var message);

        // Assert
        Assert.False(ok);
        Assert.NotNull(message);
        Assert.Contains("fewer elements", message);
    }

    [Fact]
    public void IsValid_WithMoreElements_ReturnsFalseAndMessageContainsMore()
    {
        // Arrange
        var data = Enumerable.Range(1, 12).ToArray(); // one element too many
        var rd = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data, _domain);

        // Act
        var ok = rd.IsValid(out var message);

        // Assert
        Assert.False(ok);
        Assert.NotNull(message);
        Assert.Contains("more elements", message);
    }

    [Fact]
    public void IsValid_WithThrowingEnumerator_ReturnsFalseAndReportsException()
    {
        // Arrange
        var data = new ThrowingEnumerable();
        var rd = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), data, _domain);

        // Act
        var ok = rd.IsValid(out var message);

        // Assert
        Assert.False(ok);
        Assert.NotNull(message);
        Assert.Contains("Exception while enumerating data sequence", message);
        Assert.Contains("InvalidOperationException", message);
    }

    [Fact]
    public void IsBeforeAndAdjacentTo_WithOneInclusiveOtherExclusive_ReturnsTrue()
    {
        // Arrange
        var leftData = Enumerable.Range(1, 11).ToArray(); // [10,20]
        var rightData = Enumerable.Range(21, 10).ToArray(); // (20,29] but we'll use OpenClosed(20,30)

        var left = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), leftData, _domain);
        var right = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.OpenClosed<int>(20, 30), rightData, _domain);

        // Act
        var result = left.IsBeforeAndAdjacentTo(right);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsBeforeAndAdjacentTo_WithBothInclusive_ReturnsFalse()
    {
        // Arrange
        var left = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), Enumerable.Range(1, 11), _domain);
        var right = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(20, 30), Enumerable.Range(1, 11), _domain);

        // Act
        var result = left.IsBeforeAndAdjacentTo(right);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsBeforeAndAdjacentTo_WithDifferentBoundaries_ReturnsFalse()
    {
        // Arrange
        var left = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), Enumerable.Range(1, 11), _domain);
        var right = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(21, 30), Enumerable.Range(1, 10), _domain);

        // Act
        var result = left.IsBeforeAndAdjacentTo(right);

        // Assert
        Assert.False(result);
    }

    // NOTE: RangeData constructor requires finite ranges; constructing RangeData with infinite
    // boundaries throws ArgumentException. Therefore testing the infinite-boundary branch in
    // IsBeforeAndAdjacentTo is not possible via RangeData instances and is intentionally omitted.

    [Fact]
    public void IsAfterAndAdjacentTo_DelegatesToIsBeforeAndAdjacentTo()
    {
        // Arrange
        var left = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.Closed<int>(10, 20), Enumerable.Range(1, 11), _domain);
        var right = new RangeData<int, int, IntegerFixedStepDomain>(
            Range.OpenClosed<int>(20, 30), Enumerable.Range(21, 10), _domain);

        // Act & Assert
        Assert.True(right.IsAfterAndAdjacentTo(left));
        Assert.False(left.IsAfterAndAdjacentTo(right));
    }

    // Helper throwing enumerable used to simulate enumeration errors
    private sealed class ThrowingEnumerable : IEnumerable<int>
    {
        public IEnumerator<int> GetEnumerator() => new ThrowingEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private sealed class ThrowingEnumerator : IEnumerator<int>
        {
            public int Current => throw new System.NotSupportedException();
            object IEnumerator.Current => Current;
            public void Dispose() { }
            public bool MoveNext() => throw new InvalidOperationException("boom");
            public void Reset() => throw new System.NotSupportedException();
        }
    }
}