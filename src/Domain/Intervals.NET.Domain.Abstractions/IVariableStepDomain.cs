namespace Intervals.NET.Domain.Abstractions;

/// <summary>
/// Represents a domain with variable step size between values.
/// Operations may be O(N) as step size can vary depending on position in the domain.
/// Examples include months (28-31 days) or business days calendars.
/// </summary>
/// <typeparam name="T">
/// The type of the values in the domain. Must implement IComparable&lt;T&gt;.
/// </typeparam>
public interface IVariableStepDomain<T> : IRangeDomain<T> where T : IComparable<T>;