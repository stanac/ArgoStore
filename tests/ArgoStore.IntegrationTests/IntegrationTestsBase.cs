using System.Diagnostics;
using System.Reflection;
using Microsoft.Data.Sqlite;
using Xunit.Sdk;

namespace ArgoStore.IntegrationTests;

public abstract class IntegrationTestsBase : IDisposable
{
    private static readonly IntegrationTestsConfiguration _config = new IntegrationTestsConfiguration();

    private readonly XunitTest _test;
    private readonly ITestOutputHelper _output;
    protected string TestDbFilePath { get; }
    protected string TestDbConnectionString { get; }

    protected IntegrationTestsBase(ITestOutputHelper output)
    {
        _output = output;
        FieldInfo field = output.GetType().GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);
        // ReSharper disable once PossibleNullReferenceException
        _test = (XunitTest)field.GetValue(output);

        Skip.IfNot(_config.RunIntegrationTests,
            "Integration tests not enabled, one of the following environment variables needs to point to a directory: \"ram_disk_db_test_dir\", \"db_tests_dir\". " +
            "Tests will try to use \"ram_disk_db_test_dir\" and if that is not possible or variable isn't set tests will try to use \"db_tests_dir\""
        );

        if (_config.RunIntegrationTests)
        {
            TestDbFilePath = Path.Combine(_config.WorkingDir, "test-db-" + Guid.NewGuid().ToString("N") + ".sqlite");
            TestDbConnectionString = $"Data Source={TestDbFilePath};";
        }
    }

    public void Dispose()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(TestDbFilePath) && File.Exists(TestDbFilePath))
            {
                using (SqliteConnection c = new SqliteConnection(TestDbConnectionString))
                {
                    SqliteConnection.ClearPool(c);
                }

                File.Delete(TestDbFilePath);
            }
        }
        catch
        {
            string msg = $"Failed to delete test db \"{TestDbFilePath}\" while running text {_test.DisplayName}";
            Trace.TraceWarning(msg);
            _output.WriteLine(msg);
        }
    }

    protected IDocumentSession GetNewDocumentSession(bool createEntitiesOnTheFly = true, string tenantId = TenantIdDefault.DefaultValue)
    {
        return new DocumentSession(new Configuration
        {
            ConnectionString = TestDbConnectionString,
            CreateEntitiesOnTheFly = createEntitiesOnTheFly,
            TenantId = tenantId,
            Serializer = new ArgoStoreSerializer()
        });
    }
}