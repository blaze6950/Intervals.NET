using Intervals.NET;
using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Data.Extensions;

/// <summary>
/// Extension methods for IEnumerable to create RangeData objects.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Converts an IEnumerable of data into a RangeData object associated with the specified finite range.
    /// </summary>
    /// <param name="data">
    /// The collection of data to associate with the range.
    /// </param>
    /// <param name="range">
    /// The finite range to associate with the data.
    /// </param>
    /// <param name="domain">
    /// The range domain that defines the behavior of the range.
    /// </param>
    /// <typeparam name="TRangeType">
    /// The type of the range boundaries. Must implement IComparable&lt;TRangeType&gt;.
    /// </typeparam>
    /// <typeparam name="TDataType">
    /// The type of the data associated with the range.
    /// </typeparam>
    /// <typeparam name="TRangeDomain">
    /// The type of the range domain. Must implement IRangeDomain&lt;TRangeType&gt;.
    /// </typeparam>
    /// <returns>
    /// A new RangeData object containing the specified range and data.
    /// </returns>
    public static RangeData<TRangeType, TDataType, TRangeDomain> ToRangeData<TRangeType, TDataType, TRangeDomain>(
        this IEnumerable<TDataType> data, Range<TRangeType> range, TRangeDomain domain)
        where TRangeType : IComparable<TRangeType>
        where TRangeDomain : IRangeDomain<TRangeType> => new(range, data, domain);
}