using System.Runtime.CompilerServices;

namespace Intervals.NET.Parsers;

/// <summary>
/// Provides methods to parse ranges using interpolated string syntax.
/// The interpolated string handler avoids allocating the final string - values are parsed on-the-fly.
/// </summary>
public static class RangeInterpolatedStringParser
{
    // ...existing code...

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Range<T> Parse<T>(
        RangeInterpolatedStringHandler<T> handler
    ) where T : IComparable<T>, ISpanParsable<T> => handler.GetRange();

    // ...existing code...

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse<T>(
        RangeInterpolatedStringHandler<T> handler,
        out Range<T> range
    ) where T : IComparable<T>, ISpanParsable<T> => handler.TryGetRange(out range);
}