using System.Runtime.CompilerServices;

namespace Intervals.NET.Parsers;

/// <summary>
/// Custom interpolated string handler for parsing ranges in the format: {bracket}{start}, {end}{bracket}
/// where bracket is '[' or '(' and the closing bracket is ']' or ')'.
/// Parses values immediately without buffering.
/// </summary>
[InterpolatedStringHandler]
public ref struct RangeInterpolatedStringHandler<T> where T : IComparable<T>, ISpanParsable<T>
{
    private enum ParseState
    {
        ExpectingOpenBracket,
        ExpectingStartValue,
        ExpectingComma,
        ExpectingEndValue,
        ExpectingCloseBracket,
        Complete
    }

    private ParseState _state;
    private bool _isStartInclusive;
    private bool _isEndInclusive;
    private RangeValue<T> _start;
    private RangeValue<T> _end;
    private readonly IFormatProvider? _formatProvider;
    private bool _hasError;
    private string? _errorMessage;

    /// <summary>
    /// Initializes the handler.
    /// </summary>
    /// <param name="literalLength">The number of constant characters in the interpolated string.</param>
    /// <param name="formattedCount">The number of interpolated values.</param>
    /// <param name="formatProvider">Optional format provider for parsing values.</param>
    public RangeInterpolatedStringHandler(
        int literalLength,
        int formattedCount,
        IFormatProvider? formatProvider = null)
    {
        _state = ParseState.ExpectingOpenBracket;
        _isStartInclusive = false;
        _isEndInclusive = false;
        _start = default;
        _end = default;
        _formatProvider = formatProvider;
        _hasError = false;
        _errorMessage = null;

        // Expected formats:
        // 1. With char brackets: $"{openChar}{start}, {end}{closeChar}" = 4 interpolated values
        // 2. With string literal brackets: $"[{start}, {end}]" = 2 interpolated values
        if (formattedCount != 4 && formattedCount != 2)
        {
            _hasError = true;
            _errorMessage =
                $"Invalid interpolated range format. Expected 2 (with literal brackets) or 4 (with char brackets) interpolated values, but got {formattedCount}.";
        }
    }

    /// <summary>
    /// Appends a literal string to the handler (used for brackets and ", " separator).
    /// </summary>
    public bool AppendLiteral(string value)
    {
        if (_hasError)
        {
            return false;
        }

        return _state switch
        {
            ParseState.ExpectingOpenBracket => ProcessLiteralOpenBracket(value),
            ParseState.ExpectingComma => ProcessLiteralComma(value),
            ParseState.ExpectingCloseBracket => ProcessLiteralCloseBracket(value),
            _ => true // Ignore other literals (whitespace between values)
        };
    }

    /// <summary>
    /// Processes opening bracket from literal.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ProcessLiteralOpenBracket(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return true; // Empty literal - brackets will come as chars
        }

        if (value.Length == 0 || (value[0] != '[' && value[0] != '('))
        {
            return SetError($"Invalid opening bracket in literal '{value}'. Expected '[' or '('.");
        }

        _isStartInclusive = value[0] == '[';
        _state = ParseState.ExpectingStartValue;
        return true;
    }

    /// <summary>
    /// Processes comma separator from literal.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ProcessLiteralComma(string value)
    {
        var trimmed = value.Trim();
        if (!trimmed.StartsWith(','))
        {
            return SetError($"Expected comma separator, but got '{value}'.");
        }

        _state = ParseState.ExpectingEndValue;
        return true;
    }

    /// <summary>
    /// Processes closing bracket from literal.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ProcessLiteralCloseBracket(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return true; // Empty literal - bracket will come as char
        }

        var trimmed = value.TrimStart();
        if (trimmed.Length == 0 || (trimmed[0] != ']' && trimmed[0] != ')'))
        {
            return SetError($"Invalid closing bracket in literal '{value}'. Expected ']' or ')'.");
        }

        _isEndInclusive = trimmed[0] == ']';
        _state = ParseState.Complete;
        return true;
    }

    /// <summary>
    /// Appends a formatted char value (for brackets).
    /// </summary>
    public bool AppendFormatted(char value)
    {
        if (_hasError)
        {
            return false;
        }

        return _state switch
        {
            ParseState.ExpectingOpenBracket => ProcessBracket(value, true),
            ParseState.ExpectingCloseBracket => ProcessBracket(value, false),
            _ => SetError($"Unexpected char value '{value}' in state {_state}.")
        };
    }

    /// <summary>
    /// Processes a bracket character and transitions state.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ProcessBracket(char bracket, bool isOpenBracket)
    {
        if (bracket != '[' && bracket != '(' && bracket != ']' && bracket != ')')
        {
            return SetError(
                $"Invalid {(isOpenBracket ? "opening" : "closing")} bracket '{bracket}'. Expected '[' or '(' {(isOpenBracket ? "" : "or ']' or ')'")}.");
        }

        if (isOpenBracket)
        {
            _isStartInclusive = bracket == '[';
            _state = ParseState.ExpectingStartValue;
        }
        else
        {
            _isEndInclusive = bracket == ']';
            _state = ParseState.Complete;
        }

        return true;
    }

    /// <summary>
    /// Sets error state and returns false.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool SetError(string message)
    {
        _hasError = true;
        _errorMessage = message;
        return false;
    }

    /// <summary>
    /// Appends a formatted T value (for start and end boundaries).
    /// </summary>
    public bool AppendFormatted(T value) => AppendRangeValue(new RangeValue<T>(value));

    /// <summary>
    /// Appends a formatted RangeValue (for infinity values).
    /// </summary>
    public bool AppendFormatted(RangeValue<T> value) => AppendRangeValue(value);

    /// <summary>
    /// Appends a formatted string value (for parsing string representations).
    /// </summary>
    public bool AppendFormatted(string? value)
    {
        if (_hasError)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            // Empty value represents infinity based on position
            return AppendRangeValue(_state == ParseState.ExpectingStartValue
                ? RangeValue<T>.NegativeInfinity
                : RangeValue<T>.PositiveInfinity);
        }

        // Try to parse the string as T
        if (!T.TryParse(value, _formatProvider, out var parsedValue))
        {
            _hasError = true;
            _errorMessage = $"Failed to parse '{value}' as {typeof(T).Name}.";
            return false;
        }

        return AppendRangeValue(new RangeValue<T>(parsedValue));
    }

    /// <summary>
    /// Core method to append a range value and transition state.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool AppendRangeValue(RangeValue<T> value)
    {
        if (_hasError)
        {
            return false;
        }

        switch (_state)
        {
            case ParseState.ExpectingStartValue:
                _start = value;
                _state = ParseState.ExpectingComma;
                return true;

            case ParseState.ExpectingEndValue:
                _end = value;
                _state = ParseState.ExpectingCloseBracket;
                return true;

            default:
                _hasError = true;
                _errorMessage = $"Unexpected value in state {_state}.";
                return false;
        }
    }

    /// <summary>
    /// Gets the parsed range.
    /// </summary>
    /// <returns>The parsed Range instance.</returns>
    /// <exception cref="FormatException">Thrown if the format is invalid or parsing incomplete.</exception>
    public readonly Range<T> GetRange()
    {
        if (_hasError)
        {
            throw new FormatException(_errorMessage ?? "Failed to parse range from interpolated string.");
        }

        if (_state != ParseState.Complete)
        {
            throw new FormatException($"Incomplete range format. Parser stopped at state: {_state}.");
        }

        return new Range<T>(_start, _end, _isStartInclusive, _isEndInclusive);
    }

    /// <summary>
    /// Tries to get the parsed range.
    /// </summary>
    /// <param name="range">The parsed range if successful.</param>
    /// <returns>True if parsing succeeded; otherwise false.</returns>
    public readonly bool TryGetRange(out Range<T> range)
    {
        if (_hasError || _state != ParseState.Complete)
        {
            range = default;
            return false;
        }

        range = new Range<T>(_start, _end, _isStartInclusive, _isEndInclusive);
        return true;
    }
}