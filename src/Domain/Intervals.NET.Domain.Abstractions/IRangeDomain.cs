namespace Intervals.NET.Domain.Abstractions;

/// <summary>
/// Represents a domain that supports range operations for type T.
/// Provides step-based navigation and boundary alignment operations.
/// </summary>
/// <typeparam name="T">The type of the values in the domain. Must implement IComparable&lt;T&gt;.</typeparam>
public interface IRangeDomain<T> where T : IComparable<T>
{
    /// <summary>
    /// Adds a specified number of steps to the given value.
    /// </summary>
    /// <param name="value">The value to which steps will be added.</param>
    /// <param name="steps">The number of steps to add. Can be positive or negative.</param>
    /// <returns>The resulting value after adding the specified number of steps.</returns>
    T Add(T value, long steps);

    /// <summary>
    /// Subtracts a specified number of steps from the given value.
    /// </summary>
    /// <param name="value">The value from which steps will be subtracted.</param>
    /// <param name="steps">The number of steps to subtract. Can be positive or negative.</param>
    /// <returns>The resulting value after subtracting the specified number of steps.</returns>
    T Subtract(T value, long steps);

    /// <summary>
    /// Rounds the given value down to the nearest domain boundary (step boundary).
    /// </summary>
    /// <param name="value">The value to be floored.</param>
    /// <returns>The largest domain boundary that is less than or equal to the specified value.</returns>
    T Floor(T value);

    /// <summary>
    /// Rounds the given value up to the nearest domain boundary (step boundary).
    /// </summary>
    /// <param name="value">The value to be ceiled.</param>
    /// <returns>The smallest domain boundary that is greater than or equal to the specified value.</returns>
    T Ceiling(T value);

    /// <summary>
    /// Calculates the distance in discrete steps between two values.
    /// This operation is O(1) and returns an exact integer count.
    /// </summary>
    /// <param name="start">The starting value.</param>
    /// <param name="end">The ending value.</param>
    /// <returns>The number of complete steps from start to end.</returns>
    long Distance(T start, T end);
}