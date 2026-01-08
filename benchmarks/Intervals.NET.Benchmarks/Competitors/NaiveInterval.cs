using System.Runtime.CompilerServices;

namespace Intervals.NET.Benchmarks.Competitors;

/// <summary>
/// Naive class-based interval implementation representing typical pre-modern .NET approach.
/// Characteristics:
/// - Heap-allocated (class)
/// - Nullable bounds for infinity representation
/// - No span-based parsing
/// - Defensive copying
/// - GC pressure
/// 
/// This represents the baseline most developers would write without performance considerations.
/// </summary>
public sealed class NaiveInterval
{
    public int? Start { get; }
    public int? End { get; }
    public bool IsStartInclusive { get; }
    public bool IsEndInclusive { get; }

    public NaiveInterval(int? start, int? end, bool isStartInclusive, bool isEndInclusive)
    {
        if (start.HasValue && end.HasValue && start.Value > end.Value)
        {
            throw new ArgumentException("Start must be <= End");
        }

        Start = start;
        End = end;
        IsStartInclusive = isStartInclusive;
        IsEndInclusive = isEndInclusive;
    }

    public static NaiveInterval Closed(int start, int end)
        => new(start, end, true, true);

    public static NaiveInterval Open(int start, int end)
        => new(start, end, false, false);

    public static NaiveInterval ClosedOpen(int start, int end)
        => new(start, end, true, false);

    public static NaiveInterval OpenClosed(int start, int end)
        => new(start, end, false, true);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool Contains(int value)
    {
        if (Start.HasValue)
        {
            var startComparison = value.CompareTo(Start.Value);
            if (startComparison < 0 || (startComparison == 0 && !IsStartInclusive))
            {
                return false;
            }
        }

        if (End.HasValue)
        {
            var endComparison = value.CompareTo(End.Value);
            if (endComparison > 0 || (endComparison == 0 && !IsEndInclusive))
            {
                return false;
            }
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool Overlaps(NaiveInterval other)
    {
        // Check if ranges don't overlap (easier logic)
        if (End.HasValue && other.Start.HasValue)
        {
            var cmp = End.Value.CompareTo(other.Start.Value);
            if (cmp < 0 || (cmp == 0 && !(IsEndInclusive && other.IsStartInclusive)))
            {
                return false;
            }
        }

        if (Start.HasValue && other.End.HasValue)
        {
            var cmp = Start.Value.CompareTo(other.End.Value);
            if (cmp > 0 || (cmp == 0 && !(IsStartInclusive && other.IsEndInclusive)))
            {
                return false;
            }
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public NaiveInterval? Intersect(NaiveInterval other)
    {
        if (!Overlaps(other))
        {
            return null;
        }

        int? newStart;
        bool newStartInclusive;

        if (!Start.HasValue)
        {
            newStart = other.Start;
            newStartInclusive = other.IsStartInclusive;
        }
        else if (!other.Start.HasValue)
        {
            newStart = Start;
            newStartInclusive = IsStartInclusive;
        }
        else
        {
            var cmp = Start.Value.CompareTo(other.Start.Value);
            if (cmp > 0)
            {
                newStart = Start;
                newStartInclusive = IsStartInclusive;
            }
            else if (cmp < 0)
            {
                newStart = other.Start;
                newStartInclusive = other.IsStartInclusive;
            }
            else
            {
                newStart = Start;
                newStartInclusive = IsStartInclusive && other.IsStartInclusive;
            }
        }

        int? newEnd;
        bool newEndInclusive;

        if (!End.HasValue)
        {
            newEnd = other.End;
            newEndInclusive = other.IsEndInclusive;
        }
        else if (!other.End.HasValue)
        {
            newEnd = End;
            newEndInclusive = IsEndInclusive;
        }
        else
        {
            var cmp = End.Value.CompareTo(other.End.Value);
            if (cmp < 0)
            {
                newEnd = End;
                newEndInclusive = IsEndInclusive;
            }
            else if (cmp > 0)
            {
                newEnd = other.End;
                newEndInclusive = other.IsEndInclusive;
            }
            else
            {
                newEnd = End;
                newEndInclusive = IsEndInclusive && other.IsEndInclusive;
            }
        }

        return new NaiveInterval(newStart, newEnd, newStartInclusive, newEndInclusive);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static NaiveInterval Parse(string input)
    {
        // Naive string parsing - allocates intermediate strings
        input = input.Trim();

        if (input.Length < 3)
        {
            throw new FormatException("Invalid format");
        }

        var isStartInclusive = input[0] == '[';
        var isEndInclusive = input[^1] == ']';

        var content = input.Substring(1, input.Length - 2);
        var parts = content.Split(',');

        if (parts.Length != 2)
        {
            throw new FormatException("Invalid format");
        }

        var startStr = parts[0].Trim();
        var endStr = parts[1].Trim();

        int? start = string.IsNullOrEmpty(startStr) ? null : int.Parse(startStr);
        int? end = string.IsNullOrEmpty(endStr) ? null : int.Parse(endStr);

        return new NaiveInterval(start, end, isStartInclusive, isEndInclusive);
    }

    public override string ToString()
    {
        var startBracket = IsStartInclusive ? '[' : '(';
        var endBracket = IsEndInclusive ? ']' : ')';
        var startStr = Start?.ToString() ?? "";
        var endStr = End?.ToString() ?? "";
        return $"{startBracket}{startStr}, {endStr}{endBracket}";
    }
}