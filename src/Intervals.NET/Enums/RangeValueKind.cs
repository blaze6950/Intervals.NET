namespace Intervals.NET.Enums;

/// <summary>
/// Specifies the kind of a range value.
/// </summary>
internal enum RangeValueKind : byte
{
    /// <summary>A finite value.</summary>
    Finite = 0,

    /// <summary>Positive infinity.</summary>
    PositiveInfinity = 1,

    /// <summary>Negative infinity.</summary>
    NegativeInfinity = 2
}