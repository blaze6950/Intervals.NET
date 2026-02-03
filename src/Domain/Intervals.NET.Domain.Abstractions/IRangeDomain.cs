namespace Intervals.NET.Domain.Abstractions;

/// <summary>
/// Represents a domain that supports range operations for type T.
/// Provides step-based navigation and boundary alignment operations.
/// </summary>
/// <typeparam name="T">The type of the values in the domain. Must implement IComparable&lt;T&gt;.</typeparam>
/// <remarks>
/// <para><strong>Implementation Guidance:</strong></para>
/// <list type="bullet">
/// <item><description>
/// <strong>Record structs (recommended):</strong> Implement as a readonly record struct with no state, only logic.
/// Record structs automatically implement value-based equality (IEquatable&lt;TSelf&gt;) without boxing, 
/// ensuring zero allocation and optimal performance. All built-in domains follow this pattern.
/// </description></item>
/// <item><description>
/// <strong>Regular structs:</strong> Can be used but must explicitly implement IEquatable&lt;TSelf&gt; 
/// to ensure allocation-free equality checks. Without IEquatable, struct equality will box.
/// </description></item>
/// <item><description>
/// <strong>Class domains:</strong> Can be used when domain logic requires state or shared resources 
/// (e.g., holiday calendars, configuration). Should implement IEquatable&lt;TClassName&gt; 
/// for better performance in equality comparisons.
/// </description></item>
/// </list>
/// <para><strong>Why not IEquatable&lt;IRangeDomain&lt;T&gt;&gt;?</strong></para>
/// <para>
/// This interface intentionally does NOT inherit IEquatable&lt;IRangeDomain&lt;T&gt;&gt; because that would
/// require implementing Equals(IRangeDomain&lt;T&gt;? other), which would box struct parameters when called.
/// Instead, record structs implement IEquatable&lt;ConcreteType&gt;, and EqualityComparer&lt;T&gt;.Default
/// will use that implementation without any boxing.
/// </para>
/// </remarks>
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
    /// Implementations may have different complexity characteristics: fixed-step domains typically
    /// compute this in O(1), while variable-step domains may iterate and therefore be O(N).
    /// </summary>
    /// <param name="start">The starting value.</param>
    /// <param name="end">The ending value.</param>
    /// <returns>The number of complete steps from start to end.</returns>
    long Distance(T start, T end);
}