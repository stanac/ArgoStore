using Microsoft.Data.Sqlite;

namespace ArgoStore.IntegrationTests;

internal class OnDiskTestDb : TestDb
{
    public string FileName { get; }
    public string FilePath { get; }
    public string DirectoryPath { get; }
    public string ConnectionString { get; }

    public OnDiskTestDb()
    {
        DirectoryPath = TestDir.Instance.DirectoryPath;
        FileName = $"argo-test-{Guid.NewGuid():N}.sqlite";
        FilePath = Path.Combine(DirectoryPath, FileName);
        ConnectionString = $"Data Source={FilePath};Version=3";
    }
    
    public override SqliteConnection GetAndOpenConnection()
    {
        SqliteConnection c = new SqliteConnection(ConnectionString);
        c.Open();
        return c;
    }
}