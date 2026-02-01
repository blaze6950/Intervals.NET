using Intervals.NET.Domain.Abstractions;
using Intervals.NET.Extensions;

namespace Intervals.NET.Data;

/// <summary>
/// Represents a finite range associated with a collection of data.
/// </summary>
/// <typeparam name="TRangeType">
/// The type of the range boundaries. Must implement IComparable&lt;TRangeType&gt;.
/// </typeparam>
/// <typeparam name="TDataType">
/// The type of the data associated with the range.
/// </typeparam>
/// <typeparam name="TRangeDomain">
/// The type of the domain used for range calculations. Must implement IRangeDomain&lt;TRangeType&gt;.
/// </typeparam>
/// <remarks>
/// <para>
/// <strong>Caller Responsibility:</strong> The caller is responsible for ensuring that the provided data sequence
/// corresponds exactly to the specified range and domain semantics.
/// No validation of data length or consistency is performed.
/// </para>
/// <para>
/// <strong>Performance:</strong> Operations over <see cref="IEnumerable{T}"/> may be O(n).
/// For better performance with repeated access, consider materializing the data sequence (e.g., using ToList() or ToArray())
/// before passing it to RangeData.
/// </para>
/// </remarks>
public record RangeData<TRangeType, TDataType, TRangeDomain> where TRangeType : IComparable<TRangeType>
    where TRangeDomain : IRangeDomain<TRangeType>
{
    public RangeData(Range<TRangeType> range, IEnumerable<TDataType> data, TRangeDomain domain)
    {
        if (!range.Start.IsFinite || !range.End.IsFinite)
        {
            throw new ArgumentException("Range must be finite.", nameof(range));
        }

        ArgumentNullException.ThrowIfNull(data);
        // Use pattern matching null-check to avoid boxing when TRangeDomain is a value type (struct).
        // ArgumentNullException.ThrowIfNull(domain) would box the value-type generic, causing an allocation.
        if (domain is null)
        {
            throw new ArgumentNullException(nameof(domain));
        }

        Range = range;
        Data = data;
        Domain = domain;
    }

    /// <summary>
    /// The finite range associated with the data.
    /// </summary>
    public Range<TRangeType> Range { get; }

    /// <summary>
    /// The data associated with the range.
    /// </summary>
    public IEnumerable<TDataType> Data { get; }

    /// <summary>
    /// The domain used for range calculations.
    /// </summary>
    public TRangeDomain Domain { get; }

    /// <summary>
    /// Gets the data element corresponding to the specified point within the range,
    /// using the provided domain to calculate the index.
    /// </summary>
    /// <param name="point">
    /// The point within the range for which to retrieve the data element.
    /// </param>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown if the calculated index is outside the bounds of the data collection.
    /// </exception>
    public TDataType this[TRangeType point] => TryGet(point, out var data)
        ? data!
        : throw new IndexOutOfRangeException($"The point {point} is outside the bounds of the range data.");

    /// <summary>
    /// Gets the data elements corresponding to the specified sub-range within the range,
    /// using the provided domain to calculate the indices.
    /// </summary>
    /// <param name="subRange">
    /// The sub-range within the range for which to retrieve the data elements.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown if the sub-range is not finite.
    /// </exception>
    public RangeData<TRangeType, TDataType, TRangeDomain> this[Range<TRangeType> subRange]
    {
        get
        {
            if (!subRange.Start.IsFinite || !subRange.End.IsFinite)
            {
                throw new ArgumentException("Sub-range must be finite.", nameof(subRange));
            }

            if (!Range.Contains(subRange))
            {
                throw new ArgumentOutOfRangeException(nameof(subRange), subRange, "Sub-range is outside the bounds of the source range.");
            }

            if (TryGet(subRange, out var data))
            {
                return data!;
            }

            // Fallback: if TryGet failed for other reasons (overflow), provide a generic exception
            throw new InvalidOperationException($"Unable to retrieve sub-range {subRange} from RangeData.");
        }
    }

    /// <summary>
    /// Tries to get the data element corresponding to the specified point within the range.
    /// </summary>
    /// <param name="point">The point within the range for which to retrieve the data element.</param>
    /// <param name="data">
    /// When this method returns, contains the data element associated with the specified point,
    /// if the point is within the range; otherwise, the default value for TDataType.
    /// </param>
    /// <returns>
    /// <c>true</c> if the data element was found; otherwise, <c>false</c>.
    /// </returns>
    public bool TryGet(TRangeType point, out TDataType? data)
    {
        // Ensure the requested point is logically contained in the parent range (respects inclusive/exclusive bounds)
        if (!Range.Contains(point))
        {
            data = default;
            return false;
        }

        // Align baseStart to the first included element of the parent range. This ensures indices map
        // to the actual first element of Data even when the parent range is exclusive at the start.
        var baseStart = Range.IsStartInclusive ? Range.Start.Value : Domain.Add(Range.Start.Value, 1);

        var index = Domain.Distance(baseStart, point);

        // Guard against index overflow (long → int cast)
        if (index < 0 || index > int.MaxValue)
        {
            data = default;
            return false;
        }

        var intIndex = (int)index;

        // Skip to the target index and check if an element exists
        // This supports null values as valid data (unlike ElementAtOrDefault)
        // and avoids exceptions (unlike ElementAt)
        var remaining = Data.Skip(intIndex);
        using var enumerator = remaining.GetEnumerator();

        if (enumerator.MoveNext())
        {
            data = enumerator.Current;
            return true;
        }

        data = default;
        return false;
    }

    /// <summary>
    /// Tries to get the data elements corresponding to the specified sub-range within the range.
    /// </summary>
    /// <param name="subRange">The sub-range within the range for which to retrieve the data elements.</param>
    /// <param name="data">
    /// When this method returns, contains the RangeData associated with the specified sub-range,
    /// if the sub-range is finite; otherwise, null.
    /// </param>
    /// <returns>
    /// <c>true</c> if the sub-range is finite and data was retrieved; otherwise, <c>false</c>.
    /// </returns>
    public bool TryGet(Range<TRangeType> subRange, out RangeData<TRangeType, TDataType, TRangeDomain>? data)
    {
        if (!subRange.Start.IsFinite || !subRange.End.IsFinite)
        {
            data = null;
            return false;
        }

        // Ensure the requested subRange is fully contained within this.Range (respects inclusive/exclusive bounds)
        if (!Range.Contains(subRange))
        {
            data = null;
            return false;
        }

        // Align baseStart to the first included element of the parent range so indices are computed
        // relative to the first element held by Data.
        var baseStart = Range.IsStartInclusive ? Range.Start.Value : Domain.Add(Range.Start.Value, 1);

        var startIndex = Domain.Distance(baseStart, subRange.Start.Value);
        var endIndex = Domain.Distance(baseStart, subRange.End.Value);

        // Adjust indices based on inclusiveness
        // If start is exclusive, skip the boundary element
        if (!subRange.IsStartInclusive)
        {
            startIndex++;
        }

        // If end is exclusive, don't include the boundary element
        if (!subRange.IsEndInclusive)
        {
            endIndex--;
        }

        // Guard against index overflow (long → int cast)
        if (startIndex < 0 || startIndex > int.MaxValue || endIndex < 0 || endIndex > int.MaxValue)
        {
            data = null;
            return false;
        }

        // Calculate the count of elements to take
        // If adjusted indices result in startIndex > endIndex, the range is empty
        var count = endIndex - startIndex + 1;
        if (count <= 0)
        {
            // Return empty RangeData for empty ranges
            data = Empty(subRange, Domain);
            return true;
        }

        // Guard against count overflow
        if (count > int.MaxValue)
        {
            data = null;
            return false;
        }

        var subset = Data.Skip((int)startIndex).Take((int)count);

        data = new RangeData<TRangeType, TDataType, TRangeDomain>(subRange, subset, Domain);
        return true;
    }

    /// <summary>
    /// Returns a new <see cref="RangeData{TRangeType, TDataType, TRangeDomain}"/> instance
    /// containing the data elements corresponding to the specified sub-range.
    /// </summary>
    /// <param name="subRange">The sub-range within the range for which to retrieve the data elements.</param>
    /// <returns>A new RangeData instance containing the sliced data.</returns>
    /// <exception cref="ArgumentException">Thrown if the sub-range is not finite.</exception>
    /// <remarks>
    /// This is a named alternative to the indexer syntax for improved readability in fluent APIs.
    /// </remarks>
    public RangeData<TRangeType, TDataType, TRangeDomain> Slice(Range<TRangeType> subRange)
        => this[subRange];

    /// <summary>
    /// Creates an empty <see cref="RangeData{TRangeType, TDataType, TRangeDomain}"/> instance
    /// with the specified range and domain.
    /// </summary>
    /// <param name="range">The finite range associated with the empty data.</param>
    /// <param name="domain">The domain used for range calculations.</param>
    /// <returns>A new RangeData instance with an empty data sequence.</returns>
    /// <exception cref="ArgumentException">Thrown if the range is not finite.</exception>
    /// <exception cref="ArgumentNullException">Thrown if domain is null.</exception>
    public static RangeData<TRangeType, TDataType, TRangeDomain> Empty(
        Range<TRangeType> range,
        TRangeDomain domain)
        => new(range, Enumerable.Empty<TDataType>(), domain);

    /// <summary>
    /// Determines whether the specified RangeData is equal to the current RangeData.
    /// </summary>
    /// <param name="other">The RangeData to compare with the current RangeData.</param>
    /// <returns>
    /// <c>true</c> if the specified RangeData is equal to the current RangeData; otherwise, <c>false</c>.
    /// </returns>
    public virtual bool Equals(RangeData<TRangeType, TDataType, TRangeDomain>? other)
    {
        if (other is null)
        {
            return false;
        }

        return Range.Equals(other.Range)
               && Domain.Equals(other.Domain);
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>
    /// A hash code for the current RangeData.
    /// </returns>
    public override int GetHashCode() => HashCode.Combine(Range, Domain);
}