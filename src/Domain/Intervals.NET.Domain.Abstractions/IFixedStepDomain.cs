namespace Intervals.NET.Domain.Abstractions;

/// <summary>
/// Represents a domain of values with fixed steps between them.
/// All operations are O(1) with constant-time performance.
/// </summary>
/// <typeparam name="T">
/// The type of the values in the domain. Must implement IComparable&lt;T&gt;.
/// </typeparam>
public interface IFixedStepDomain<T> : IRangeDomain<T> where T : IComparable<T>;