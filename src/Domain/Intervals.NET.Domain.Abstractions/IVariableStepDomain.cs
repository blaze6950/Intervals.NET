namespace Intervals.NET.Domain.Abstractions;

/// <summary>
/// Represents a domain with variable step size between values.
/// Operations may be O(N) as step size can vary depending on position in the domain.
/// Examples include months (28-31 days) or business days calendars.
/// </summary>
/// <typeparam name="T">
/// The type of the values in the domain. Must implement IComparable&lt;T&gt;.
/// </typeparam>
public interface IVariableStepDomain<T> : IRangeDomain<T> where T : IComparable<T>
{
    /// <summary>
    /// Calculates the distance between two values in domain-specific units.
    /// May return fractional values to account for partial steps.
    /// ⚠️ Warning: This operation may be O(N) depending on the domain implementation.
    /// </summary>
    /// <param name="start">The starting value.</param>
    /// <param name="end">The ending value.</param>
    /// <returns>The distance from start to end, potentially including fractional steps.</returns>
    double Distance(T start, T end);
}