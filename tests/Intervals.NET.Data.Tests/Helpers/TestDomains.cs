using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Data.Tests.Helpers;

public sealed class NonEqualDomainStub : IRangeDomain<int>
{
    public int Add(int value, long steps) => throw new NotSupportedException();
    public int Subtract(int value, long steps) => throw new NotSupportedException();
    public int Floor(int value) => throw new NotSupportedException();
    public int Ceiling(int value) => throw new NotSupportedException();
    public long Distance(int start, int end) => throw new NotSupportedException();

    public override bool Equals(object? obj) => false; // never equal to any other domain
    public override int GetHashCode() => 42;
}

public sealed class HugeDistanceDomainStub : IRangeDomain<int>
{
    private readonly long _distance;
    public HugeDistanceDomainStub(long distance) => _distance = distance;

    public int Add(int value, long steps) => (int)(value + steps);
    public int Subtract(int value, long steps) => (int)(value - steps);
    public int Floor(int value) => value;
    public int Ceiling(int value) => value;

    public long Distance(int start, int end)
    {
        // Return configured huge distance regardless of inputs to trigger overflow guards
        return _distance;
    }

    public override bool Equals(object? obj)
    {
        if (obj is HugeDistanceDomainStub other)
            return other._distance == _distance;
        return false;
    }

    public override int GetHashCode() => _distance.GetHashCode();
}
