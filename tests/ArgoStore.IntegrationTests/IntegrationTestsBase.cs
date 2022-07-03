using System.Diagnostics;
using System.Reflection;
using ArgoStore.Configurations;
using ArgoStore.Helpers;
using Dapper;
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
    internal EntityTableHelper EntityTableHelper { get; }

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

            EntityTableHelper = new EntityTableHelper(new Configuration(
                TestDbConnectionString, false, new Dictionary<Type, EntityMetadata>(), TenantIdDefault.DefaultValue
            ));
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
        return new DocumentSession(new Configuration(
            TestDbConnectionString, createEntitiesOnTheFly, new Dictionary<Type, EntityMetadata>(), tenantId
            ));
    }

    protected List<string> ListNonUniqueIndexes(Type entityType)
    {
        return GetIndexes(entityType)
            .Where(x => x.unique == 0)
            .Select(x => x.name)
            .ToList();
    }

    protected List<string> ListUniqueIndexes(Type entityType)
    {
        return GetIndexes(entityType)
            .Where(x => x.unique == 1)
            .Select(x => x.name)
            .ToList();
    }

    protected string GetIndexDefinition(string indexName)
    {
        string sql = $@"
            SELECT sql FROM sqlite_master
            WHERE type = 'index' AND name = @name";

        using SqliteConnection c = new SqliteConnection(TestDbConnectionString);

        return c.QueryFirstOrDefault<string>(sql, new {name = indexName});
    }

    private IEnumerable<IndexItem> GetIndexes(Type entityType)
    {
        string tableName = EntityTableHelper.GetTableName(entityType);
        string sql = $"PRAGMA index_list('{tableName}')";

        using SqliteConnection c = new SqliteConnection(TestDbConnectionString);

        return c.Query<IndexItem>(sql)
            .Where(x => !x.name.StartsWith("sqlite_autoindex"));
    }

    private class IndexItem
    {
        public int seq { get; set; }
        public string name { get; set; }
        public int unique { get; set; }
        public string origin { get; set; }
        public int partial { get; set; }
    }
}