using System;
using System.Diagnostics;
using System.IO;
using Xunit;

namespace ArgoStore.IntegrationTests
{
    public abstract class IntegrationTestsBase : IDisposable
    {
        private static readonly IntegrationTestsConfiguration _config = new IntegrationTestsConfiguration();
        
        protected string TestDbFilePath { get; }
        protected string TestDbConnectionString { get; }

        protected IntegrationTestsBase()
        {
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
                    File.Delete(TestDbFilePath);
                }
            }
            catch
            {
                Trace.TraceWarning($"Failed to delete test db \"{TestDbFilePath}\"");
            }
        }

        protected IDocumentSession GetNewDocumentSession(bool createEntitiesOnTheFly = true)
        {
            return new DocumentSession(new Configuration
            {
                ConnectionString = TestDbConnectionString,
                CreateEntitiesOnTheFly = createEntitiesOnTheFly
            });
        }
    }
}
