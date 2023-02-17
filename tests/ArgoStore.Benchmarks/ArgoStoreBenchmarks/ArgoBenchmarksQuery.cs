using BenchmarkDotNet.Attributes;
using System.Diagnostics;
using ArgoStore.Benchmarks.Models;

namespace ArgoStore.Benchmarks.ArgoStoreBenchmarks;

public class ArgoBenchmarksQuery
{
    private readonly Person[] _testPersons = Person.GetTestData();
    private readonly string[] _emails;
    private readonly string _multipleName;
    
    private ArgoDocumentStore _queryStore;
    private IArgoQueryDocumentSession _querySession;

    public ArgoBenchmarksQuery()
    {
        _emails = _testPersons.Skip(_testPersons.Length / 2)
            .Take(100).Select(x => x.EmailAddress).ToArray();

        _multipleName = _testPersons[0].Name;
    }

    [GlobalSetup]
    public void Setup()
    {
        string filePath = @"c:\temp\benchmarks\argo-query.sqlite";

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        string connStr = "Data Source=" + filePath;
        _queryStore = new ArgoDocumentStore(connStr);
        _queryStore.RegisterDocument<Person>();
        using IArgoDocumentSession s = _queryStore.OpenSession();
        s.Insert(_testPersons);
        s.SaveChanges();

        _querySession = _queryStore.OpenQuerySession();
    }
    
    [Benchmark]
    public void ArgoQueryFirstResult()
    {
        foreach (string e in _emails)
        {
            int cc = _querySession.Query<Person>().First(x => x.EmailAddress == e).CookiesCount;
            Trace.Write(cc);
        }
    }

    [Benchmark]
    public void ArgoToList100Rows()
    {
        List<Person> items = _querySession.Query<Person>().Where(x => x.Name == _multipleName).ToList();
        Trace.Write(items.Count);
    }
}