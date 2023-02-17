using ArgoStore.Benchmarks.ArgoStoreBenchmarks;
using ArgoStore.Benchmarks.EntityFrameworkBenchmarks;
using BenchmarkDotNet.Running;

namespace ArgoStore.Benchmarks;

internal class Program
{
    static void Main()
    {
        // BenchmarkRunner.Run<ArgoBenchmarksInsert>();
        // BenchmarkRunner.Run<EfBenchmarksInsert>();

        BenchmarkRunner.Run<ArgoBenchmarksQuery>();
        BenchmarkRunner.Run<EfBenchmarksQuery>();
    }
}

/*
BenchmarkDotNet=v0.13.4, OS=Windows 11 (10.0.22621.1265)
AMD Ryzen 5 5600X, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.103
  [Host]     : .NET 7.0.3 (7.0.323.6910), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.3 (7.0.323.6910), X64 RyuJIT AVX2

    Insert test 10,000 models:
    ArgoStore (pre-release): 297.9 ms
    EF Core (v 7.0.3):       269.6 ms

    Query First with single matching (100 times with different parameter):
    ArgoStore (pre-release): 380.1 ms
    EF Core (v 7.0.3):       023.0 ms

    ToList 100 rows with where condition:
    ArgoStore (pre-release): 008.8 ms
    EF Core (v 7.0.3):       000.5 ms
 */