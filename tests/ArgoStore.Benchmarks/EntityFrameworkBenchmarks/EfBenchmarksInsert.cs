using ArgoStore.Benchmarks.Models;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;

namespace ArgoStore.Benchmarks.EntityFrameworkBenchmarks;

public class EfBenchmarksInsert
{
    private readonly Person[] _testPersons = Person.GetTestData();
    private EfDbContext _db;
    
    [IterationSetup]
    public void Setup()
    {
        string filePath = @"c:\temp\benchmarks\ef-" + Guid.NewGuid().ToString("N") + ".sqlite";

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        string connStr = "Data Source=" + filePath;
        _db = new EfDbContext(connStr);
        _db.Database.EnsureCreated();
        _db.Database.Migrate();
    }

    [Benchmark]
    public void InsertTest()
    {
        _db.Persons.AddRange(_testPersons);
        _db.SaveChanges();
    }
}