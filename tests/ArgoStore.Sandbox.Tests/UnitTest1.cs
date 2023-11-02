using System.Diagnostics;
using ArgoStore.Benchmarks.Models;
using ArgoStore.Implementations;

namespace ArgoStore.Sandbox.Tests;

public class UnitTest1
{
    private const string FilePath = @"r:\test.sqlite";
    private const string ConnectionString = "Data source=" + FilePath;

    /*
QueryProvider.Execute 002.061 ms
    QueryProvider.Execute::VisitAndBuild 001.786 ms
    QueryProvider.Execute::WhereStrategies.Translate 000.298 ms
        QueryProvider.Execute::WhereStrategies.Translate::FindTranslator 000.004 ms
        QueryProvider.Execute::WhereStrategies.Translate::Binary 000.012 ms
            QueryProvider.Execute::WhereStrategies.Translate::Binary::WhereStrategies.Translate 000.012 ms
                QueryProvider.Execute::WhereStrategies.Translate::Binary::WhereStrategies.Translate::FindTranslator 000.002 ms
                QueryProvider.Execute::WhereStrategies.Translate::Binary::WhereStrategies.Translate::Property 000.003 ms
            QueryProvider.Execute::WhereStrategies.Translate::Binary::WhereStrategies.Translate 000.007 ms
                QueryProvider.Execute::WhereStrategies.Translate::Binary::WhereStrategies.Translate::FindTranslator 000.001 ms
                QueryProvider.Execute::WhereStrategies.Translate::Binary::WhereStrategies.Translate::Constant 000.000 ms
    QueryProvider.Execute::BuildCommand 000.024 ms
    QueryProvider.Execute::Execute 000.244 ms
        QueryProvider.Execute::Execute::ExecuteFirstOrDefault 000.235 ms
            QueryProvider.Execute::Execute::ExecuteFirstOrDefault::CreateAndOpenConnection 000.011 ms       
     */

    // [Fact]
    public void Test1()
    {
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
        }

        var testData = Person.GetTestData();

        ArgoDocumentStore store = new ArgoDocumentStore(ConnectionString);
        store.RegisterDocument<Person>();
        var s = store.OpenSession();
        s.Insert(testData);
        s.SaveChanges();

        ArgoStoreQueryProvider.MeasureExecutionTime = true;

        string? dump = "";

        foreach (string emailAddress in testData.Select(x => x.EmailAddress).Skip(10).Take(10))
        {
            var p = s.Query<Person>().FirstOrDefault(x => x.EmailAddress == emailAddress);
            Trace.WriteLine(p != null);

            dump = ArgoStoreQueryProvider.LastActivity?.Dump();
        }
    }
}