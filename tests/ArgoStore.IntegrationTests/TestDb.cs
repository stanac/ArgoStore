using Microsoft.Data.Sqlite;

namespace ArgoStore.IntegrationTests;

public abstract class TestDb
{
    public abstract SqliteConnection GetAndOpenConnection();

    public static TestDb CreateNew() => new OnDiskTestDb();
}