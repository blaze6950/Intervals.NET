using Intervals.NET.Data.Extensions;
using Intervals.NET.Domain.Default.Numeric;
using Intervals.NET.Data.Tests.Helpers;
using Range = Intervals.NET.Factories.Range;

namespace Intervals.NET.Data.Tests.Extensions;

public class RangeDataExtensions_DomainValidationTests
{
    [Fact]
    public void Intersect_WithDifferentDomainInstanceThatIsNotEqual_ThrowsArgumentException()
    {
        // Arrange
        var domainA = new NonEqualDomainStub();
        var domainB = new NonEqualDomainStub(); // separate instance but Equals returns false

        var dataA = Enumerable.Range(1, 11);
        var dataB = Enumerable.Range(1, 11);

        var rdA = new RangeData<int, int, NonEqualDomainStub>(Range.Closed<int>(10, 20), dataA, domainA);
        var rdB = new RangeData<int, int, NonEqualDomainStub>(Range.Closed<int>(10, 20), dataB, domainB);

        // Act & Assert - domains are same compile-time type but instances are not equal
        Assert.Throws<ArgumentException>(() => RangeDataExtensions.Intersect(rdA, rdB));
    }

    [Fact]
    public void Intersect_WithDifferentInstancesButEqualValueDomain_DoesNotThrow()
    {
        // Arrange
        var domain1 = new IntegerFixedStepDomain();
        var domain2 = new IntegerFixedStepDomain(); // separate instance but value-equal (struct)

        var data1 = Enumerable.Range(1, 11);
        var data2 = Enumerable.Range(1, 11);

        var rd1 = new RangeData<int, int, IntegerFixedStepDomain>(Range.Closed<int>(10, 20), data1, domain1);
        var rd2 = new RangeData<int, int, IntegerFixedStepDomain>(Range.Closed<int>(10, 20), data2, domain2);

        // Act & Assert - should not throw
        var result = RangeDataExtensions.Intersect(rd1, rd2);
        Assert.NotNull(result);
    }
}