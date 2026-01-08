using System.Globalization;
using System.Runtime.CompilerServices;

namespace Intervals.NET.Parsers;

/// <summary>
/// Provides methods to parse range strings into <see cref="Range{T}"/> instances.
/// </summary>
public static class RangeStringParser
{
    /// <summary>
    /// Parses a range from the given input string. Throws an exception if parsing fails.
    /// The expected format is:
    /// [start,end], (start,end), [start,end), or (start,end]
    /// where start and end are values of type T, or empty for infinite bounds (+infinity/-infinity).
    /// </summary>
    /// <param name="input">
    /// The input string representing the range.
    /// </param>
    /// <param name="formatProvider">
    /// An optional format provider for parsing the boundary values.
    /// The default is CultureInfo.InvariantCulture.
    /// </param>
    /// <typeparam name="T">
    /// The type of the values in the range. Must implement IComparable&lt;T&gt; and ISpanParsable&lt;T&gt;.
    /// </typeparam>
    /// <returns>
    /// A new instance of <see cref="Range{T}"/> representing the parsed range.
    /// </returns>
    public static Range<T> Parse<T>(
        ReadOnlySpan<char> input,
        IFormatProvider? formatProvider = null
    ) where T : IComparable<T>, ISpanParsable<T>
    {
        if (!TryParseCore<T>(input, out var range, formatProvider, throwOnError: true))
        {
            // This should never be reached since throwOnError=true
            throw new InvalidOperationException("Unexpected parse failure.");
        }

        return range;
    }

    /// <summary>
    /// Attempts to parse a range from the given input string.
    /// The expected format is:
    /// [start,end], (start,end), [start,end), or (start,end]
    /// where start and end are values of type T, or empty for infinite bounds (+infinity/-infinity).
    /// </summary>
    /// <param name="input">
    /// The input string representing the range.
    /// </param>
    /// <param name="range">
    /// When this method returns, contains the parsed range if the parsing succeeded,
    /// or the default value if the parsing failed.
    /// </param>
    /// <param name="formatProvider">
    /// An optional format provider for parsing the boundary values.
    /// The default is CultureInfo.InvariantCulture.
    /// </param>
    /// <typeparam name="T">
    /// The type of the values in the range. Must implement IComparable&lt;T&gt; and ISpanParsable&lt;T&gt;.
    /// </typeparam>
    /// <returns>
    /// True if the range was successfully parsed; otherwise, false.
    /// </returns>
    public static bool TryParse<T>(
        ReadOnlySpan<char> input,
        out Range<T> range,
        IFormatProvider? formatProvider = null
    ) where T : IComparable<T>, ISpanParsable<T> => TryParseCore(input, out range, formatProvider, throwOnError: false);

    private static bool TryParseCore<T>(
        ReadOnlySpan<char> input,
        out Range<T> range,
        IFormatProvider? formatProvider,
        bool throwOnError
    ) where T : IComparable<T>, ISpanParsable<T>
    {
        range = default;

        // Validate minimum length
        if (input.Length < 3)
        {
            if (throwOnError)
            {
                ThrowInvalidInputLength();
            }

            return false;
        }

        // Validate and extract bracket types
        var firstChar = input[0];
        var lastChar = input[^1];

        if (firstChar != '[' && firstChar != '(')
        {
            if (throwOnError)
            {
                ThrowInvalidStartBracket();
            }

            return false;
        }

        if (lastChar != ']' && lastChar != ')')
        {
            if (throwOnError)
            {
                ThrowInvalidEndBracket();
            }

            return false;
        }

        var isStartInclusive = firstChar == '[';
        var isEndInclusive = lastChar == ']';

        formatProvider ??= CultureInfo.InvariantCulture;

        // Find the separator comma by trying to parse from each comma position
        // This handles cases where the format provider uses comma as decimal separator
        var contentSpan = input.Slice(1, input.Length - 2); // Remove brackets
        var commaIndex = FindSeparatorComma<T>(contentSpan, formatProvider);

        if (commaIndex < 0)
        {
            if (throwOnError)
            {
                ThrowMissingComma();
            }

            return false;
        }

        // Extract boundary spans (commaIndex is relative to contentSpan, so add 1 for original input)
        var startSpan = contentSpan.Slice(0, commaIndex).Trim();
        var endSpan = contentSpan.Slice(commaIndex + 1).Trim();

        // Parse boundaries
        RangeValue<T> start;
        if (startSpan.IsEmpty || IsNegativeInfinitySymbol(startSpan))
        {
            start = RangeValue<T>.NegativeInfinity;
        }
        else if (IsPositiveInfinitySymbol(startSpan))
        {
            start = RangeValue<T>.PositiveInfinity;
        }
        else if (T.TryParse(startSpan, formatProvider, out var startValue))
        {
            start = startValue;
        }
        else
        {
            if (throwOnError)
            {
                throw new FormatException($"Failed to parse start value: '{startSpan.ToString()}'.");
            }

            return false;
        }

        RangeValue<T> end;
        if (endSpan.IsEmpty || IsPositiveInfinitySymbol(endSpan))
        {
            end = RangeValue<T>.PositiveInfinity;
        }
        else if (IsNegativeInfinitySymbol(endSpan))
        {
            end = RangeValue<T>.NegativeInfinity;
        }
        else if (T.TryParse(endSpan, formatProvider, out var endValue))
        {
            end = endValue;
        }
        else
        {
            if (throwOnError)
            {
                throw new FormatException($"Failed to parse end value: '{endSpan.ToString()}'.");
            }

            return false;
        }

        // Use fast constructor - parser already validated the input
        range = new Range<T>(start, end, isStartInclusive, isEndInclusive, skipValidation: true);
        return true;
    }

    private static void ThrowInvalidInputLength() =>
        throw new FormatException(
            "Input is too short to be a valid range. The minimum length is 3 characters: e.g., [,].");

    private static void ThrowInvalidStartBracket() =>
        throw new FormatException("Invalid range format. Must start with [ or (.");

    private static void ThrowInvalidEndBracket() =>
        throw new FormatException("Invalid range format. Must end with ] or ).");

    private static void ThrowMissingComma() =>
        throw new FormatException("Missing comma separator.");

    private static void ThrowMultipleCommas() =>
        throw new FormatException("Invalid range format. More than one comma found.");

    /// <summary>
    /// Checks if the span represents a positive infinity symbol.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsPositiveInfinitySymbol(ReadOnlySpan<char> span)
    {
        return span.Length == 1 && span[0] == '∞';
    }

    /// <summary>
    /// Checks if the span represents a negative infinity symbol.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsNegativeInfinitySymbol(ReadOnlySpan<char> span)
    {
        return span.Length == 2 && span[0] == '-' && span[1] == '∞';
    }

    /// <summary>
    /// Finds the index of the comma that separates the start and end values.
    /// Handles cases where the format provider uses comma as a decimal separator.
    /// </summary>
    private static int FindSeparatorComma<T>(ReadOnlySpan<char> span, IFormatProvider? formatProvider)
        where T : IComparable<T>, ISpanParsable<T>
    {
        var commaIndex = span.IndexOf(',');
        if (commaIndex < 0)
        {
            return -1; // No comma found
        }

        // FAST PATH: if only one comma, it must be the separator (99% of cases)
        var lastCommaIndex = span.LastIndexOf(',');
        if (lastCommaIndex == commaIndex)
        {
            return commaIndex;
        }

        // SLOW PATH: Multiple commas - need to validate with parsing
        // This only happens with decimal formats like "3,14" in some cultures
        // Try each comma position until we find one where both sides parse
        var searchStart = 0;
        var currentCommaIndex = commaIndex;
        
        while (currentCommaIndex >= 0)
        {
            var leftSpan = span.Slice(0, currentCommaIndex).Trim();
            var rightSpan = span.Slice(currentCommaIndex + 1).Trim();

            // Empty spans are valid (infinity), otherwise try to parse
            var leftValid = leftSpan.IsEmpty || 
                           IsNegativeInfinitySymbol(leftSpan) || 
                           IsPositiveInfinitySymbol(leftSpan) ||
                           T.TryParse(leftSpan, formatProvider, out _);
                           
            var rightValid = rightSpan.IsEmpty || 
                            IsNegativeInfinitySymbol(rightSpan) || 
                            IsPositiveInfinitySymbol(rightSpan) ||
                            T.TryParse(rightSpan, formatProvider, out _);

            if (leftValid && rightValid)
            {
                return currentCommaIndex;
            }

            // Find next comma
            searchStart = currentCommaIndex + 1;
            if (searchStart >= span.Length)
            {
                break;
            }

            var nextCommaOffset = span.Slice(searchStart).IndexOf(',');
            currentCommaIndex = nextCommaOffset >= 0 ? searchStart + nextCommaOffset : -1;
        }

        return -1; // No valid separator comma found
    }
}