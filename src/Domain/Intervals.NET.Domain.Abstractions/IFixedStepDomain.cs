namespace Intervals.NET.Domain.Abstractions;

/// <summary>
/// Represents a domain of values with fixed steps between them.
/// All operations are O(1) with constant-time performance.
/// </summary>
/// <typeparam name="T">
/// The type of the values in the domain. Must implement IComparable&lt;T&gt;.
/// </typeparam>
public interface IFixedStepDomain<T> : IRangeDomain<T> where T : IComparable<T>
{
    /// <summary>
    /// Calculates the distance in discrete steps between two values.
    /// This operation is O(1) and returns an exact integer count.
    /// </summary>
    /// <param name="start">The starting value.</param>
    /// <param name="end">The ending value.</param>
    /// <returns>The number of complete steps from start to end.</returns>
    long Distance(T start, T end);
}