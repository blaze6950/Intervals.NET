using System.Runtime.CompilerServices;
using Intervals.NET.Domain.Abstractions;
using Intervals.NET.Extensions;

namespace Intervals.NET.Data.Extensions;

/// <summary>
/// Extension methods for <see cref="RangeData{TRangeType, TDataType, TRangeDomain}"/>.
/// <para>
/// These extensions mirror logical range operations from Intervals.NET while correctly
/// propagating associated data sequences and maintaining strict invariants.
/// </para>
/// </summary>
/// <remarks>
/// <para><strong>Strict Invariant:</strong></para>
/// <para>
/// The logical length of the range (as defined by domain distance) MUST exactly match
/// the number of elements in the associated data sequence. All operations preserve this invariant.
/// Any operation that would break this invariant is not implemented.
/// </para>
/// 
/// <para><strong>Design Principles:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Immutability:</strong> All operations return new instances; no mutation occurs.</description></item>
/// <item><description><strong>Domain-Agnostic:</strong> Operations work with any <see cref="IRangeDomain{T}"/> implementation.</description></item>
/// <item><description><strong>Consistency Guarantee:</strong> Extensions never create RangeData with mismatched range/data lengths.</description></item>
/// <item><description><strong>Lazy Evaluation:</strong> Data sequences use LINQ operators; materialization is deferred.</description></item>
/// </list>
/// 
/// <para><strong>Performance:</strong></para>
/// <para>
/// Operations over <see cref="IEnumerable{T}"/> may be O(n). For repeated access,
/// consider materializing data sequences (e.g., ToList(), ToArray()) before creating RangeData.
/// </para>
/// </remarks>
public static class RangeDataExtensions
{
    #region Domain Validation

    /// <summary>
    /// Validates that two RangeData objects have equal domains.
    /// </summary>
    /// <remarks>
    /// While the generic type constraint ensures both operands have the same TRangeDomain type,
    /// custom domain implementations may have instance-specific state. This validation ensures
    /// that operations are performed on compatible domain instances.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ValidateDomainEquality<TRangeType, TDataType, TRangeDomain>(
        RangeData<TRangeType, TDataType, TRangeDomain> left,
        RangeData<TRangeType, TDataType, TRangeDomain> right,
        string operationName)
        where TRangeType : IComparable<TRangeType>
        where TRangeDomain : IRangeDomain<TRangeType>
    {
        if (!EqualityComparer<TRangeDomain>.Default.Equals(left.Domain, right.Domain))
        {
            throw new ArgumentException(
                $"Cannot {operationName} RangeData objects with different domain instances. " +
                "Both operands must use the same domain instance or equivalent domains.",
                nameof(right));
        }
    }

    #endregion

    #region Set Operations

    /// <summary>
    /// Computes the intersection of two <see cref="RangeData{TRangeType, TDataType, TRangeDomain}"/> objects.
    /// <para>
    /// Returns a new RangeData containing the overlapping range with data sliced from the
    /// <strong>right operand</strong>. If ranges do not overlap, returns null.
    /// </para>
    /// <para>
    /// ⚡ <strong>Performance:</strong> O(n) where n is the number of elements to skip/take from the data sequence.
    /// </para>
    /// </summary>
    /// <param name="left">The left RangeData object (older/stale data).</param>
    /// <param name="right">The right RangeData object (newer/fresh data - used as data source for intersection).</param>
    /// <typeparam name="TRangeType">
    /// The type of the range values. Must implement IComparable&lt;TRangeType&gt;.
    /// </typeparam>
    /// <typeparam name="TDataType">The type of the associated data.</typeparam>
    /// <typeparam name="TRangeDomain">
    /// The type of the range domain that implements IRangeDomain&lt;TRangeType&gt;.
    /// </typeparam>
    /// <returns>
    /// A new RangeData object with the intersected range and sliced data from the right operand,
    /// or null if the ranges do not overlap.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when domains are not equal.</exception>
    /// <remarks>
    /// <para><strong>Right-Biased Behavior:</strong></para>
    /// <para>
    /// The intersection always uses data from the <strong>right operand</strong> (fresh data).
    /// This ensures consistency with <see cref="Union{TRangeType, TDataType, TRangeDomain}"/> 
    /// and follows the principle that the right operand represents newer/fresher data.
    /// </para>
    /// 
    /// <para><strong>Invariant Preservation:</strong></para>
    /// <para>
    /// The resulting RangeData has data length exactly matching the intersection range length.
    /// </para>
    /// 
    /// <para><strong>Example (Right-Biased):</strong></para>
    /// <code>
    /// var domain = new IntegerFixedStepDomain();
    /// var oldData = new RangeData(Range.Closed(10, 30), staleValues, domain);
    /// var newData = new RangeData(Range.Closed(20, 40), freshValues, domain);
    /// 
    /// var intersection = oldData.Intersect(newData);  
    /// // Range [20, 30], data from newData (fresh), not oldData (stale)
    /// </code>
    /// 
    /// <para><strong>Use Cases:</strong></para>
    /// <list type="bullet">
    /// <item><description>Cache queries: get the fresh overlapping portion</description></item>
    /// <item><description>Data validation: compare with latest values</description></item>
    /// <item><description>Time-series: extract recent measurements in overlapping period</description></item>
    /// </list>
    /// </remarks>
    public static RangeData<TRangeType, TDataType, TRangeDomain>? Intersect<TRangeType, TDataType, TRangeDomain>(
        this RangeData<TRangeType, TDataType, TRangeDomain> left,
        RangeData<TRangeType, TDataType, TRangeDomain> right)
        where TRangeType : IComparable<TRangeType>
        where TRangeDomain : IRangeDomain<TRangeType>
    {
        ValidateDomainEquality(left, right, "intersect");

        // Compute range intersection
        var intersectedRange = left.Range.Intersect(right.Range);

        if (!intersectedRange.HasValue)
        {
            return null;
        }

        // Slice data from RIGHT operand (fresh data) to match the intersection
        return right[intersectedRange.Value];
    }

    /// <summary>
    /// Computes the union of two <see cref="RangeData{TRangeType, TDataType, TRangeDomain}"/> objects
    /// if they are contiguous (overlapping or adjacent).
    /// <para>
    /// Returns a new RangeData with the combined range and <strong>distinct</strong> data.
    /// Overlapping data appears only once, with the <strong>right operand taking priority</strong>.
    /// </para>
    /// <para>
    /// ⚡ <strong>Performance:</strong> O(n + m) where n and m are the data sequence lengths.
    /// </para>
    /// </summary>
    /// <param name="left">The left RangeData object (older/stale data).</param>
    /// <param name="right">The right RangeData object (newer/fresh data - takes priority in overlapping regions).</param>
    /// <typeparam name="TRangeType">
    /// The type of the range values. Must implement IComparable&lt;TRangeType&gt;.
    /// </typeparam>
    /// <typeparam name="TDataType">The type of the associated data.</typeparam>
    /// <typeparam name="TRangeDomain">
    /// The type of the range domain that implements IRangeDomain&lt;TRangeType&gt;.
    /// </typeparam>
    /// <returns>
    /// A new RangeData object with the union range and distinct data,
    /// or null if the ranges are disjoint (neither overlapping nor adjacent).
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when domains are not equal.</exception>
    /// <remarks>
    /// <para><strong>Union Distinct Semantics:</strong></para>
    /// <para>
    /// This is NOT a "union all" operation. Overlapping data appears only once.
    /// The resulting RangeData maintains the strict invariant: range length equals data length.
    /// </para>
    /// 
    /// <para><strong>Conflict Resolution (Right-Biased):</strong></para>
    /// <para>
    /// When ranges overlap, data from the <strong>right operand</strong> is used for the intersection region.
    /// This follows the principle that the right operand typically represents newer/fresher data
    /// (e.g., cache updates, incremental data loads, time-series updates).
    /// Data from the left operand is included only for the non-overlapping portion.
    /// </para>
    /// 
    /// <para><strong>Data Construction Algorithm:</strong></para>
    /// <list type="number">
    /// <item><description>Compute union range and intersection range (if any).</description></item>
    /// <item><description>If no overlap: concatenate left + right data in proper order.</description></item>
    /// <item><description>If overlap exists:
    ///   <list type="bullet">
    ///   <item><description>Take all data from right operand (covers its entire range with fresh data).</description></item>
    ///   <item><description>Take only non-overlapping data from left operand.</description></item>
    ///   <item><description>Result: distinct data matching union range exactly, with right's data preferred.</description></item>
    ///   </list>
    /// </description></item>
    /// </list>
    /// 
    /// <para><strong>Invariant Preservation:</strong></para>
    /// <para>
    /// The resulting data sequence length exactly matches the union range's domain distance.
    /// </para>
    /// 
    /// <para><strong>Example (Right-Biased):</strong></para>
    /// <code>
    /// var domain = new IntegerFixedStepDomain();
    /// var oldData = new RangeData(Range.Closed(10, 20), staleValues, domain);   // 11 elements (stale)
    /// var newData = new RangeData(Range.Closed(18, 30), freshValues, domain);   // 13 elements (fresh)
    /// 
    /// var union = oldData.Union(newData);  
    /// // Range [10, 30], 21 elements total
    /// // staleValues[0..7] (indices 10-17, non-overlapping left part) 
    /// // + freshValues[0..12] (indices 18-30, all fresh data)
    /// // The overlap [18, 20] uses freshValues (fresh), not staleValues (stale)
    /// </code>
    /// 
    /// <para><strong>Use Cases:</strong></para>
    /// <list type="bullet">
    /// <item><description>Cache updates: merging old cached data with fresh updates</description></item>
    /// <item><description>Time-series: combining historical data with recent measurements</description></item>
    /// <item><description>Incremental loads: adding new data to existing dataset</description></item>
    /// </list>
    /// </remarks>
    public static RangeData<TRangeType, TDataType, TRangeDomain>? Union<TRangeType, TDataType, TRangeDomain>(
        this RangeData<TRangeType, TDataType, TRangeDomain> left,
        RangeData<TRangeType, TDataType, TRangeDomain> right)
        where TRangeType : IComparable<TRangeType>
        where TRangeDomain : IRangeDomain<TRangeType>
    {
        ValidateDomainEquality(left, right, "union");

        // Check if ranges can be merged (overlapping or adjacent)
        var unionRange = left.Range.Union(right.Range);

        if (!unionRange.HasValue)
        {
            // Ranges are disjoint - cannot form union
            return null;
        }

        // Compute intersection to determine overlap
        var intersectionRange = left.Range.Intersect(right.Range);

        var unionData = intersectionRange.HasValue
            // Ranges overlap - need to deduplicate (RIGHT-BIASED: fresh data wins)
            ? MergeOverlappingRanges(left, right)
            // No overlap - ranges are adjacent
            : ConcatenateAdjacentRanges(left, right);

        return new RangeData<TRangeType, TDataType, TRangeDomain>(
            unionRange.Value,
            unionData,
            left.Domain);

        // Local functions with aggressive inlining for hot-path performance

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<TDataType> ConcatenateAdjacentRanges(
            RangeData<TRangeType, TDataType, TRangeDomain> leftRange,
            RangeData<TRangeType, TDataType, TRangeDomain> rightRange)
        {
            // Determine ordering and concatenate in correct sequence
            return leftRange.Range.IsBefore(rightRange.Range)
                ? leftRange.Data.Concat(rightRange.Data)
                : rightRange.Data.Concat(leftRange.Data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<TDataType> MergeOverlappingRanges(
            RangeData<TRangeType, TDataType, TRangeDomain> leftRange,
            RangeData<TRangeType, TDataType, TRangeDomain> rightRange)
        {
            // RIGHT-BIASED: Always prioritize right's data (fresh) over left's data (stale)
            // Get the portions of LEFT that don't overlap with RIGHT
            // Note: We avoid ToList() to prevent allocation and GC pressure
            var leftOnlyRanges = leftRange.Range.Except(rightRange.Range);

            return CombineDataWithFreshPrimary(
                freshData: rightRange.Data,
                freshRange: rightRange.Range,
                staleRangeData: leftRange,
                staleOnlyRanges: leftOnlyRanges);
        }

        static IEnumerable<TDataType> CombineDataWithFreshPrimary(
            IEnumerable<TDataType> freshData,
            Range<TRangeType> freshRange,
            RangeData<TRangeType, TDataType, TRangeDomain> staleRangeData,
            IEnumerable<Range<TRangeType>> staleOnlyRanges)
        {
            // Handle three topological cases by manually iterating the enumerable:
            // - Count == 0: stale completely contained in fresh [F...S...F] → use only fresh
            // - Count == 1: stale extends beyond fresh on one side [S..S]F...F] or [F...F]S..S]
            // - Count == 2: stale wraps around fresh [S..S]F...F[S..S] (fresh is contained)

            // Manually iterate to avoid materializing the entire collection
            using var enumerator = staleOnlyRanges.GetEnumerator();

            // Try to get the first range
            if (!enumerator.MoveNext())
            {
                // Count == 0: No exclusive ranges, stale is completely contained in fresh
                return HandleStaleContainedInFresh(freshData);
            }

            var firstRange = enumerator.Current;

            // Try to get the second range
            if (!enumerator.MoveNext())
            {
                // Count == 1: Single exclusive range, stale extends on one side
                return HandleStaleExtendsOneSide(freshData, freshRange, staleRangeData, firstRange);
            }

            var secondRange = enumerator.Current;

            // Check if there's a third range (error condition)
            if (enumerator.MoveNext())
            {
                // Count > 2: This should never happen with Range.Except
                throw new InvalidOperationException(
                    "Range.Except returned more than 2 ranges, which indicates an invalid state.");
            }

            // Count == 2: Two exclusive ranges, stale wraps around fresh
            return HandleStaleWrapsFresh(freshData, staleRangeData, firstRange, secondRange);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<TDataType> HandleStaleContainedInFresh(
            IEnumerable<TDataType> freshData)
        {
            // Stale is completely covered by fresh: [F....S....F]
            // Use only fresh data (no stale data needed)
            return freshData;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<TDataType> HandleStaleExtendsOneSide(
            IEnumerable<TDataType> freshData,
            Range<TRangeType> freshRange,
            RangeData<TRangeType, TDataType, TRangeDomain> staleRangeData,
            Range<TRangeType> staleOnlyRange)
        {
            // Stale extends beyond fresh on one side
            // Determine if it extends to the left or right
            var staleExclusivePart = staleRangeData[staleOnlyRange];

            // Check if stale extends to the left of fresh
            var staleExtendsLeft = RangeValue<TRangeType>.Compare(
                staleOnlyRange.Start,
                freshRange.Start) < 0;

            return staleExtendsLeft
                ? staleExclusivePart.Data.Concat(freshData) // [S..S]F...F]
                : freshData.Concat(staleExclusivePart.Data); // [F...F]S..S]
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<TDataType> HandleStaleWrapsFresh(
            IEnumerable<TDataType> freshData,
            RangeData<TRangeType, TDataType, TRangeDomain> staleRangeData,
            Range<TRangeType> leftStaleRange,
            Range<TRangeType> rightStaleRange)
        {
            // Stale wraps around fresh: [S..S]F....F[S..S]
            // Fresh is completely contained within stale
            // Combine: left stale part + fresh (priority) + right stale part
            var leftStalePart = staleRangeData[leftStaleRange];
            var rightStalePart = staleRangeData[rightStaleRange];
            return leftStalePart.Data.Concat(freshData).Concat(rightStalePart.Data);
        }
    }

    #endregion

    #region Trimming Operations

    /// <summary>
    /// Trims the start of the range to a new start value, adjusting data accordingly.
    /// <para>
    /// Returns a new RangeData with the trimmed range and sliced data.
    /// If trimming removes the entire range, returns null.
    /// </para>
    /// <para>
    /// ⚡ <strong>Performance:</strong> O(n) where n is the number of elements to skip.
    /// </para>
    /// </summary>
    /// <param name="source">The source RangeData object.</param>
    /// <param name="newStart">The new start value for the range.</param>
    /// <typeparam name="TRangeType">
    /// The type of the range values. Must implement IComparable&lt;TRangeType&gt;.
    /// </typeparam>
    /// <typeparam name="TDataType">The type of the associated data.</typeparam>
    /// <typeparam name="TRangeDomain">
    /// The type of the range domain that implements IRangeDomain&lt;TRangeType&gt;.
    /// </typeparam>
    /// <returns>
    /// A new RangeData with the trimmed range and sliced data,
    /// or null if the new start is beyond the current end.
    /// </returns>
    /// <remarks>
    /// <para><strong>Example:</strong></para>
    /// <code>
    /// var domain = new IntegerFixedStepDomain();
    /// var rd = new RangeData(Range.Closed(10, 30), data, domain);
    /// 
    /// var trimmed = rd.TrimStart(15);  // Range [15, 30], data from index 5 onward
    /// var invalid = rd.TrimStart(40);  // null (new start beyond end)
    /// </code>
    /// </remarks>
    public static RangeData<TRangeType, TDataType, TRangeDomain>? TrimStart<TRangeType, TDataType, TRangeDomain>(
        this RangeData<TRangeType, TDataType, TRangeDomain> source,
        TRangeType newStart)
        where TRangeType : IComparable<TRangeType>
        where TRangeDomain : IRangeDomain<TRangeType>
    {
        if (!Factories.Range.TryCreate(
                new RangeValue<TRangeType>(newStart),
                source.Range.End,
                source.Range.IsStartInclusive,
                source.Range.IsEndInclusive,
                out var trimmedRange,
                out _))
        {
            return null;
        }

        // Check if the new range is valid (has any values)
        if (!trimmedRange.Overlaps(source.Range))
        {
            return null;
        }

        // Slice data to match the trimmed range
        if (!source.TryGet(trimmedRange, out var result))
        {
            return null;
        }

        return result;
    }

    /// <summary>
    /// Trims the end of the range to a new end value, adjusting data accordingly.
    /// <para>
    /// Returns a new RangeData with the trimmed range and sliced data.
    /// If trimming removes the entire range, returns null.
    /// </para>
    /// <para>
    /// ⚡ <strong>Performance:</strong> O(n) where n is the number of elements to take.
    /// </para>
    /// </summary>
    /// <param name="source">The source RangeData object.</param>
    /// <param name="newEnd">The new end value for the range.</param>
    /// <typeparam name="TRangeType">
    /// The type of the range values. Must implement IComparable&lt;TRangeType&gt;.
    /// </typeparam>
    /// <typeparam name="TDataType">The type of the associated data.</typeparam>
    /// <typeparam name="TRangeDomain">
    /// The type of the range domain that implements IRangeDomain&lt;TRangeType&gt;.
    /// </typeparam>
    /// <returns>
    /// A new RangeData with the trimmed range and sliced data,
    /// or null if the new end is before the current start.
    /// </returns>
    /// <remarks>
    /// <para><strong>Example:</strong></para>
    /// <code>
    /// var domain = new IntegerFixedStepDomain();
    /// var rd = new RangeData(Range.Closed(10, 30), data, domain);
    /// 
    /// var trimmed = rd.TrimEnd(25);  // Range [10, 25], first 16 elements
    /// var invalid = rd.TrimEnd(5);   // null (new end before start)
    /// </code>
    /// </remarks>
    public static RangeData<TRangeType, TDataType, TRangeDomain>? TrimEnd<TRangeType, TDataType, TRangeDomain>(
        this RangeData<TRangeType, TDataType, TRangeDomain> source,
        TRangeType newEnd)
        where TRangeType : IComparable<TRangeType>
        where TRangeDomain : IRangeDomain<TRangeType>
    {
        if (!Factories.Range.TryCreate(
                source.Range.Start,
                new RangeValue<TRangeType>(newEnd),
                source.Range.IsStartInclusive,
                source.Range.IsEndInclusive,
                out var trimmedRange,
                out _))
        {
            return null;
        }

        // Check if the new range is valid (has any values)
        if (!trimmedRange.Overlaps(source.Range))
        {
            return null;
        }

        // Slice data to match the trimmed range
        if (!source.TryGet(trimmedRange, out var result))
        {
            return null;
        }

        return result;
    }

    #endregion

    #region Containment Checks

    /// <summary>
    /// Determines whether the range contains the specified value.
    /// <para>
    /// Delegates to <see cref="RangeExtensions.Contains{T}(Range{T}, T)"/>.
    /// Does NOT inspect or validate data.
    /// </para>
    /// </summary>
    /// <param name="source">The source RangeData object.</param>
    /// <param name="value">The value to check for containment.</param>
    /// <typeparam name="TRangeType">
    /// The type of the range values. Must implement IComparable&lt;TRangeType&gt;.
    /// </typeparam>
    /// <typeparam name="TDataType">The type of the associated data.</typeparam>
    /// <typeparam name="TRangeDomain">
    /// The type of the range domain that implements IRangeDomain&lt;TRangeType&gt;.
    /// </typeparam>
    /// <returns>True if the range contains the value; otherwise, false.</returns>
    /// <remarks>
    /// <para>
    /// This is a pure range operation; data is not inspected.
    /// </para>
    /// </remarks>
    public static bool Contains<TRangeType, TDataType, TRangeDomain>(
        this RangeData<TRangeType, TDataType, TRangeDomain> source,
        TRangeType value)
        where TRangeType : IComparable<TRangeType>
        where TRangeDomain : IRangeDomain<TRangeType> => source.Range.Contains(value);

    /// <summary>
    /// Determines whether the range completely contains another range.
    /// <para>
    /// Delegates to <see cref="RangeExtensions.Contains{T}(Range{T}, Range{T})"/>.
    /// Does NOT inspect or validate data.
    /// </para>
    /// </summary>
    /// <param name="source">The source RangeData object.</param>
    /// <param name="range">The range to check for containment.</param>
    /// <typeparam name="TRangeType">
    /// The type of the range values. Must implement IComparable&lt;TRangeType&gt;.
    /// </typeparam>
    /// <typeparam name="TDataType">The type of the associated data.</typeparam>
    /// <typeparam name="TRangeDomain">
    /// The type of the range domain that implements IRangeDomain&lt;TRangeType&gt;.
    /// </typeparam>
    /// <returns>True if the source range fully contains the specified range; otherwise, false.</returns>
    /// <remarks>
    /// <para>
    /// This is a pure range operation; data is not inspected.
    /// </para>
    /// </remarks>
    public static bool Contains<TRangeType, TDataType, TRangeDomain>(
        this RangeData<TRangeType, TDataType, TRangeDomain> source,
        Range<TRangeType> range)
        where TRangeType : IComparable<TRangeType>
        where TRangeDomain : IRangeDomain<TRangeType> => source.Range.Contains(range);

    #endregion

    /// <summary>
    /// Validates that the <see cref="RangeData{TRangeType,TDataType,TRangeDomain}.Data"/> sequence
    /// exactly matches the number of logical elements implied by the <see cref="Range{TRangeType}"/>
    /// and the associated domain instance.
    /// <para>
    /// IMPORTANT: This method enumerates the underlying <see cref="IEnumerable{T}"/> returned by
    /// <c>RangeData.Data</c>. Enumeration may force deferred execution, be expensive, or produce
    /// side-effects (for example if the sequence is generated on-the-fly). Callers should be
    /// aware that calling this method will iterate the entire data sequence (up to the expected
    /// count + 1 to detect oversize sequences).
    /// </para>
    /// </summary>
    /// <param name="source">The RangeData instance to validate.</param>
    /// <param name="message">When validation fails contains a descriptive message; null on success.</param>
    /// <returns>True if the data sequence length matches the logical range length; otherwise false.</returns>
    public static bool IsValid<TRangeType, TDataType, TRangeDomain>(
        this RangeData<TRangeType, TDataType, TRangeDomain> source,
        out string? message)
        where TRangeType : IComparable<TRangeType>
        where TRangeDomain : IRangeDomain<TRangeType>
    {
        // Calculate expected element count using the same index arithmetic as RangeData.TryGet
        var startIndex = 0L;
        var endIndex = source.Domain.Distance(source.Range.Start.Value, source.Range.End.Value);

        if (!source.Range.IsStartInclusive)
        {
            startIndex++;
        }

        if (!source.Range.IsEndInclusive)
        {
            endIndex--;
        }

        long expectedCount = endIndex - startIndex + 1;
        if (expectedCount <= 0)
        {
            expectedCount = 0;
        }

        try
        {
            using var e = source.Data.GetEnumerator();
            long actualCount = 0;

            // Enumerate but bail out early if we detect more elements than expected
            while (e.MoveNext())
            {
                actualCount++;
                if (actualCount > expectedCount)
                {
                    message =
                        $"Data sequence contains more elements ({actualCount}) than expected ({expectedCount}) for range {source.Range}.";
                    return false;
                }
            }

            if (actualCount < expectedCount)
            {
                message =
                    $"Data sequence contains fewer elements ({actualCount}) than expected ({expectedCount}) for range {source.Range}.";
                return false;
            }

            message = null;
            return true;
        }
        catch (Exception ex)
        {
            // If enumeration throws, surface a helpful message
            message = $"Exception while enumerating data sequence: {ex.GetType().Name} - {ex.Message}";
            return false;
        }
    }

    #region Relationship Checks

    /// <summary>
    /// Determines whether this RangeData touches another RangeData through overlap or adjacency.
    /// <para>
    /// Two RangeData objects are touching if their ranges either overlap or are adjacent
    /// (touch at exactly one boundary). This is a symmetric relationship.
    /// </para>
    /// </summary>
    /// <param name="source">The source RangeData object.</param>
    /// <param name="other">The other RangeData object.</param>
    /// <typeparam name="TRangeType">
    /// The type of the range values. Must implement IComparable&lt;TRangeType&gt;.
    /// </typeparam>
    /// <typeparam name="TDataType">The type of the associated data.</typeparam>
    /// <typeparam name="TRangeDomain">
    /// The type of the range domain that implements IRangeDomain&lt;TRangeType&gt;.
    /// </typeparam>
    /// <returns>True if the ranges are touching (overlap or adjacent); otherwise, false.</returns>
    /// <exception cref="ArgumentException">Thrown when domains are not equal.</exception>
    /// <remarks>
    /// <para><strong>Symmetric Relationship:</strong></para>
    /// <para>
    /// <c>a.IsTouching(b)</c> is always equivalent to <c>b.IsTouching(a)</c>.
    /// </para>
    /// 
    /// <para><strong>Use Case:</strong></para>
    /// <para>
    /// Use this method to determine if two RangeData objects can be merged using
    /// <see cref="Union{TRangeType, TDataType, TRangeDomain}"/>.
    /// </para>
    /// 
    /// <para><strong>Example:</strong></para>
    /// <code>
    /// var domain = new IntegerFixedStepDomain();
    /// var rd1 = new RangeData(Range.Closed(10, 20), data1, domain);
    /// var rd2 = new RangeData(Range.Open(20, 30), data2, domain);
    /// 
    /// bool touching = rd1.IsTouching(rd2);  // true (adjacent at 20)
    /// </code>
    /// </remarks>
    public static bool IsTouching<TRangeType, TDataType, TRangeDomain>(
        this RangeData<TRangeType, TDataType, TRangeDomain> source,
        RangeData<TRangeType, TDataType, TRangeDomain> other)
        where TRangeType : IComparable<TRangeType>
        where TRangeDomain : IRangeDomain<TRangeType>
    {
        ValidateDomainEquality(source, other, "check relationship of");

        // Touching = overlapping or adjacent (symmetric)
        return source.Range.Overlaps(other.Range) || source.Range.IsAdjacent(other.Range);
    }

    /// <summary>
    /// Determines whether this RangeData is positioned before and adjacent to another RangeData.
    /// <para>
    /// Returns true if this range ends exactly where the other range starts, with no gap or overlap.
    /// This is a directional (non-symmetric) relationship.
    /// </para>
    /// </summary>
    /// <param name="source">The source RangeData object (expected to come first).</param>
    /// <param name="other">The other RangeData object (expected to come second).</param>
    /// <typeparam name="TRangeType">
    /// The type of the range values. Must implement IComparable&lt;TRangeType&gt;.
    /// </typeparam>
    /// <typeparam name="TDataType">The type of the associated data.</typeparam>
    /// <typeparam name="TRangeDomain">
    /// The type of the range domain that implements IRangeDomain&lt;TRangeType&gt;.
    /// </typeparam>
    /// <returns>
    /// True if source.Range ends exactly where other.Range starts (adjacent, source before other);
    /// otherwise, false.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when domains are not equal.</exception>
    /// <remarks>
    /// <para><strong>Directional Relationship:</strong></para>
    /// <para>
    /// <c>a.IsBeforeAndAdjacentTo(b)</c> implies <c>b.IsAfterAndAdjacentTo(a)</c>.
    /// </para>
    /// 
    /// <para><strong>Use Case:</strong></para>
    /// <para>
    /// Use this method when you need to verify ordered, non-overlapping sequences.
    /// </para>
    /// 
    /// <para><strong>Example:</strong></para>
    /// <code>
    /// var domain = new IntegerFixedStepDomain();
    /// var rd1 = new RangeData(Range.Closed(10, 20), data1, domain);
    /// var rd2 = new RangeData(Range.Open(20, 30), data2, domain);
    /// 
    /// bool adjacent = rd1.IsBeforeAndAdjacentTo(rd2);  // true
    /// bool reverse = rd2.IsBeforeAndAdjacentTo(rd1);   // false (wrong direction)
    /// </code>
    /// </remarks>
    public static bool IsBeforeAndAdjacentTo<TRangeType, TDataType, TRangeDomain>(
        this RangeData<TRangeType, TDataType, TRangeDomain> source,
        RangeData<TRangeType, TDataType, TRangeDomain> other)
        where TRangeType : IComparable<TRangeType>
        where TRangeDomain : IRangeDomain<TRangeType>
    {
        ValidateDomainEquality(source, other, "check relationship of");

        // Check if source.end touches other.start
        if (!source.Range.End.IsFinite || !other.Range.Start.IsFinite)
        {
            return false;
        }

        var comparison = RangeValue<TRangeType>.Compare(source.Range.End, other.Range.Start);
        if (comparison != 0)
        {
            return false;
        }

        // Boundaries are equal - adjacent if exactly one is inclusive
        return source.Range.IsEndInclusive != other.Range.IsStartInclusive;
    }

    /// <summary>
    /// Determines whether this RangeData is positioned after and adjacent to another RangeData.
    /// <para>
    /// Returns true if this range starts exactly where the other range ends, with no gap or overlap.
    /// This is a directional (non-symmetric) relationship.
    /// </para>
    /// </summary>
    /// <param name="source">The source RangeData object (expected to come second).</param>
    /// <param name="other">The other RangeData object (expected to come first).</param>
    /// <typeparam name="TRangeType">
    /// The type of the range values. Must implement IComparable&lt;TRangeType&gt;.
    /// </typeparam>
    /// <typeparam name="TDataType">The type of the associated data.</typeparam>
    /// <typeparam name="TRangeDomain">
    /// The type of the range domain that implements IRangeDomain&lt;TRangeType&gt;.
    /// </typeparam>
    /// <returns>
    /// True if source.Range starts exactly where other.Range ends (adjacent, source after other);
    /// otherwise, false.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when domains are not equal.</exception>
    /// <remarks>
    /// <para><strong>Directional Relationship:</strong></para>
    /// <para>
    /// <c>a.IsAfterAndAdjacentTo(b)</c> implies <c>b.IsBeforeAndAdjacentTo(a)</c>.
    /// </para>
    /// 
    /// <para><strong>Use Case:</strong></para>
    /// <para>
    /// Use this method when you need to verify ordered, non-overlapping sequences.
    /// </para>
    /// 
    /// <para><strong>Example:</strong></para>
    /// <code>
    /// var domain = new IntegerFixedStepDomain();
    /// var rd1 = new RangeData(Range.Closed(10, 20), data1, domain);
    /// var rd2 = new RangeData(Range.Open(20, 30), data2, domain);
    /// 
    /// bool adjacent = rd2.IsAfterAndAdjacentTo(rd1);  // true
    /// bool reverse = rd1.IsAfterAndAdjacentTo(rd2);   // false (wrong direction)
    /// </code>
    /// </remarks>
    public static bool IsAfterAndAdjacentTo<TRangeType, TDataType, TRangeDomain>(
        this RangeData<TRangeType, TDataType, TRangeDomain> source,
        RangeData<TRangeType, TDataType, TRangeDomain> other)
        where TRangeType : IComparable<TRangeType>
        where TRangeDomain : IRangeDomain<TRangeType> =>
        // This is simply the inverse of IsBeforeAndAdjacentTo
        other.IsBeforeAndAdjacentTo(source);

    #endregion
}