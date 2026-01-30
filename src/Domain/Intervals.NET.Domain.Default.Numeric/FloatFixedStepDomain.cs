using Intervals.NET.Domain.Abstractions;

namespace Intervals.NET.Domain.Default.Numeric;

/// <summary>
/// Provides a fixed-step domain implementation for <see cref="float"/> (Single) values with a step size of 1.0f.
/// </summary>
/// <remarks>
/// <para>
/// This domain treats float values as having discrete steps of 1.0f.
/// Due to floating-point precision limitations, results may not be exact for very large values.
/// </para>
/// <para><strong>Note:</strong> Step size is 1.0f, not epsilon. This is intentional for practical range operations.</para>
/// </remarks>
public readonly struct FloatFixedStepDomain : IFixedStepDomain<float>
{
    public float Floor(float value) => MathF.Floor(value);
    public float Ceiling(float value) => MathF.Ceiling(value);
    public long Distance(float start, float end) => (long)MathF.Floor(end - start);
    
    public float Add(float value, long offset) => value + (offset * 1.0f);
    public float Subtract(float value, long offset) => Add(value, -offset);
}
