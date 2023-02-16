using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ArgoStore.Benchmarks.Models;

namespace ArgoStore.Benchmarks.EntityFrameworkBenchmarks;

public class EfBenchmarksQuery
{
    private readonly Person[] _testPersons = Person.GetTestData();
    private EfDbContext _queryDb;

    private readonly string[] _emails;
    private readonly string _multipleName;

    public EfBenchmarksQuery()
    {
        _emails = _testPersons.Skip(_testPersons.Length / 2)
            .Take(100).Select(x => x.EmailAddress).ToArray();

        _multipleName = _testPersons[0].Name;
    }

    [GlobalSetup]
    public void Setup()
    {
        string filePath = @"c:\temp\benchmarks\ef-query.sqlite";

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        string connStr = "Data Source=" + filePath;
        _queryDb = new EfDbContext(connStr);
        _queryDb.Database.EnsureCreated();
        _queryDb.Database.Migrate();

        _queryDb.Persons.AddRange(_testPersons);
        _queryDb.SaveChanges();
    }
    
    // [Benchmark]
    public void QueryFirstResult()
    {
        foreach (string e in _emails)
        {
            int cc = _queryDb.Persons.First(x => x.EmailAddress == e).CookiesCount;
            Trace.Write(cc);
        }
    }

    [Benchmark]
    public void ToList100Rows()
    {
        List<Person> items = _queryDb.Persons.Where(x => x.Name == _multipleName).ToList();
        Trace.Write(items.Count);
    }
}