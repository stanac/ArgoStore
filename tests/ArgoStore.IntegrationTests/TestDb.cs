using Microsoft.Data.Sqlite;

namespace ArgoStore.IntegrationTests;

public abstract class TestDb : IDisposable
{
    public abstract SqliteConnection GetAndOpenConnection();

    public abstract string ConnectionString { get; }

    public static TestDb CreateNew() => 
        // new OnDiskTestDb();
        new InMemoryTestDb();

    public abstract void Dispose();
}