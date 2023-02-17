using ArgoStore.Benchmarks.Models;
using BenchmarkDotNet.Attributes;

namespace ArgoStore.Benchmarks.ArgoStoreBenchmarks;

public class ArgoBenchmarksInsert
{
    private readonly Person[] _testPersons = Person.GetTestData();
    private readonly string _singleEmail;
    private readonly string _multipleName;

    private ArgoDocumentStore _store;

    public ArgoBenchmarksInsert()
    {
        _singleEmail = _testPersons.Skip(_testPersons.Length / 2)
            .First().EmailAddress;

        _multipleName = _testPersons[0].Name;
    }
    
    [IterationSetup]
    public void Setup()
    {
        string filePath = @"c:\temp\benchmarks\argo-" + Guid.NewGuid().ToString("N") + ".sqlite";
        string connStr = "Data Source=" + filePath;
        _store = new ArgoDocumentStore(connStr);
        _store.RegisterDocument<Person>();
    }

    [Benchmark]
    public void ArgoInsertTests()
    {
        using var s = _store.OpenSession();
        s.Insert(_testPersons);
        s.SaveChanges();
    }
}