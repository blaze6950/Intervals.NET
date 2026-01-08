using BenchmarkDotNet.Running;

namespace Intervals.NET.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        // Allow BenchmarkSwitcher to parse command-line args
        // Usage: dotnet run -c Release -- --filter *Construction*
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}