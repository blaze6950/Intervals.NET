using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Validators;

namespace Intervals.NET.Benchmarks;

/// <summary>
/// Global BenchmarkDotNet configuration for Intervals.NET benchmarks.
/// Enforces consistent settings across all benchmark classes.
/// </summary>
public class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        // Add memory diagnoser to track allocations
        AddDiagnoser(MemoryDiagnoser.Default);

        // Add threading diagnoser to track thread pool usage
        AddDiagnoser(ThreadingDiagnoser.Default);

        // Add validators to catch common mistakes
        AddValidator(JitOptimizationsValidator.FailOnError);
        AddValidator(ExecutionValidator.FailOnError);

        // Export results in multiple formats
        AddExporter(MarkdownExporter.GitHub);
        AddExporter(HtmlExporter.Default);
        AddExporter(CsvExporter.Default);

        // Configure job
        AddJob(Job.Default
            .WithGcServer(true) // Server GC for more realistic throughput
            .WithGcConcurrent(true) // Concurrent GC (default for most apps)
            .WithGcForce(false)); // Don't force GC between iterations

        // Display configuration
        WithOptions(ConfigOptions.DisableOptimizationsValidator);
    }
}

/// <summary>
/// Configuration for detailed performance analysis.
/// Includes disassembly viewer (Windows only).
/// 
/// Usage: [Config(typeof(DetailedConfig))]
/// </summary>
public class DetailedConfig : ManualConfig
{
    public DetailedConfig()
    {
        AddDiagnoser(MemoryDiagnoser.Default);
        AddDiagnoser(ThreadingDiagnoser.Default);

        // Uncomment for disassembly view (Windows only, requires elevated permissions)
        // AddDiagnoser(new DisassemblyDiagnoser(new DisassemblyDiagnoserConfig(
        //     maxDepth: 3,
        //     printSource: true,
        //     printInstructionAddresses: true,
        //     exportHtml: true)));

        AddValidator(JitOptimizationsValidator.FailOnError);
        AddExporter(MarkdownExporter.GitHub);
        AddExporter(HtmlExporter.Default);

        AddJob(Job.Default
            .WithGcServer(true)
            .WithGcConcurrent(true));
    }
}