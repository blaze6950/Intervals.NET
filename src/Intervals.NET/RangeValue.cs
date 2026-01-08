using System.Diagnostics.CodeAnalysis;
using Intervals.NET.Enums;

namespace Intervals.NET;

/// <summary>
/// Represents a boundary value of a range.
/// A range value can be finite or represent positive or negative infinity.
/// </summary>
/// <typeparam name="T">The underlying comparable type.</typeparam>
public readonly struct RangeValue<T> :
    IComparable<RangeValue<T>>,
    IEquatable<RangeValue<T>>
    where T : IComparable<T>
{
    private readonly T? _value;
    private readonly RangeValueKind _kind;

    #region Constructors

    private RangeValue(T? value, RangeValueKind kind)
    {
        _value = value;
        _kind = kind;
    }

    /// <summary>
    /// Creates a finite range value.
    /// </summary>
    public RangeValue(T value)
        : this(value, RangeValueKind.Finite)
    {
    }

    #endregion

    #region Static factories

    /// <summary>
    /// Gets a range value representing negative infinity.
    /// </summary>
    public static RangeValue<T> NegativeInfinity { get; }
        = new(default, RangeValueKind.NegativeInfinity);

    /// <summary>
    /// Gets a range value representing positive infinity.
    /// </summary>
    public static RangeValue<T> PositiveInfinity { get; }
        = new(default, RangeValueKind.PositiveInfinity);

    #endregion

    #region Properties

    /// <summary>
    /// Gets whether this range value is finite.
    /// </summary>
    public bool IsFinite => _kind == RangeValueKind.Finite;

    /// <summary>
    /// Gets whether this range value represents positive infinity.
    /// </summary>
    public bool IsPositiveInfinity => _kind == RangeValueKind.PositiveInfinity;

    /// <summary>
    /// Gets whether this range value represents negative infinity.
    /// </summary>
    public bool IsNegativeInfinity => _kind == RangeValueKind.NegativeInfinity;

    #endregion

    #region Value access

    /// <summary>
    /// Attempts to get the finite value.
    /// Returns false if the value is infinite.
    /// </summary>
    public bool TryGetValue([MaybeNullWhen(false)] out T value)
    {
        if (_kind == RangeValueKind.Finite)
        {
            value = _value!;
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Gets the finite value or throws if the value is infinite.
    /// </summary>
    public T Value =>
        _kind == RangeValueKind.Finite
            ? _value!
            : throw new InvalidOperationException("Infinite range value has no finite value.");

    #endregion

    #region Comparison

    /// <inheritdoc />
    public int CompareTo(RangeValue<T> other)
    {
        if (_kind == other._kind)
        {
            if (_kind != RangeValueKind.Finite)
            {
                return 0;
            }

            return _value!.CompareTo(other._value!);
        }

        return _kind switch
        {
            RangeValueKind.NegativeInfinity => -1,
            RangeValueKind.PositiveInfinity => 1,
            _ => other._kind == RangeValueKind.NegativeInfinity ? 1 : -1
        };
    }

    /// <summary>
    /// Compares two range values.
    /// </summary>
    public static int Compare(RangeValue<T> left, RangeValue<T> right)
        => left.CompareTo(right);

    #endregion

    #region Equality

    /// <inheritdoc />
    public bool Equals(RangeValue<T> other)
    {
        if (_kind != other._kind)
        {
            return false;
        }

        if (_kind != RangeValueKind.Finite)
        {
            return true;
        }

        return EqualityComparer<T>.Default.Equals(_value, other._value);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is RangeValue<T> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            return _kind switch
            {
                RangeValueKind.PositiveInfinity => int.MaxValue,
                RangeValueKind.NegativeInfinity => int.MinValue,
                _ => (EqualityComparer<T>.Default.GetHashCode(_value!) * 397) ^ (int)_kind
            };
        }
    }

    #endregion

    #region Operators

    /// <summary>
    /// Checks whether two range values are equal.
    /// </summary>
    /// <param name="left">The left range value.</param>
    /// <param name="right">The right range value.</param>
    /// <returns>True if the two range values are equal; otherwise, false.</returns>
    public static bool operator ==(RangeValue<T> left, RangeValue<T> right)
        => left.Equals(right);

    /// <summary>
    /// Checks whether two range values are not equal.
    /// </summary>
    /// <param name="left">The left range value.</param>
    /// <param name="right">The right range value.</param>
    /// <returns>True if the two range values are not equal; otherwise, false.</returns>
    public static bool operator !=(RangeValue<T> left, RangeValue<T> right)
        => !left.Equals(right);

    /// <summary>
    /// Checks whether the left range value is less than the right range value.
    /// </summary>
    /// <param name="left">The left range value.</param>
    /// <param name="right">The right range value.</param>
    /// <returns>True if the left range value is less than the right range value; otherwise, false.</returns>
    public static bool operator <(RangeValue<T> left, RangeValue<T> right)
        => Compare(left, right) < 0;

    /// <summary>
    /// Checks whether the left range value is greater than the right range value.
    /// </summary>
    /// <param name="left">The left range value.</param>
    /// <param name="right">The right range value.</param>
    /// <returns>True if the left range value is greater than the right range value; otherwise, false.</returns>
    public static bool operator >(RangeValue<T> left, RangeValue<T> right)
        => Compare(left, right) > 0;

    /// <summary>
    /// Checks whether the left range value is less than or equal to the right range value.
    /// </summary>
    /// <param name="left">The left range value.</param>
    /// <param name="right">The right range value.</param>
    /// <returns>True if the left range value is less than or equal to the right range value; otherwise, false.</returns>
    public static bool operator <=(RangeValue<T> left, RangeValue<T> right)
        => Compare(left, right) <= 0;

    /// <summary>
    /// Checks whether the left range value is greater than or equal to the right range value.
    /// </summary>
    /// <param name="left">The left range value.</param>
    /// <param name="right">The right range value.</param>
    /// <returns>True if the left range value is greater than or equal to the right range value; otherwise, false.</returns>
    public static bool operator >=(RangeValue<T> left, RangeValue<T> right)
        => Compare(left, right) >= 0;

    /// <summary>
    /// Implicitly converts a finite value to a range value.
    /// </summary>
    /// <param name="value">The finite value to convert.</param>
    /// <returns>A finite range value containing the specified value.</returns>
    /// <example>
    /// <code>
    /// RangeValue&lt;int&gt; rv = 42; // Implicit conversion
    /// var range = Range.Closed(10, 20); // Uses implicit conversion
    /// </code>
    /// </example>
    public static implicit operator RangeValue<T>(T value)
        => new(value);

    /// <summary>
    /// Explicitly converts a range value to its underlying finite value.
    /// </summary>
    /// <param name="rangeValue">The range value to convert.</param>
    /// <returns>The underlying finite value.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the range value represents positive or negative infinity.
    /// </exception>
    /// <example>
    /// <code>
    /// RangeValue&lt;int&gt; rv = 42;
    /// int value = (int)rv; // Explicit cast required
    /// 
    /// // This would throw InvalidOperationException:
    /// // int invalid = (int)RangeValue&lt;int&gt;.PositiveInfinity;
    /// </code>
    /// </example>
    public static explicit operator T(RangeValue<T> rangeValue)
        => rangeValue.Value; // Uses the Value property which already throws for infinity

    #endregion

    #region Formatting

    /// <inheritdoc />
    public override string ToString() => _kind switch
    {
        RangeValueKind.PositiveInfinity => "∞",
        RangeValueKind.NegativeInfinity => "-∞",
        _ => _value?.ToString() ?? string.Empty
    };

    #endregion
}