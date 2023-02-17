using Microsoft.Data.Sqlite;

namespace ArgoStore.IntegrationTests.TestInfra;

internal class OnDiskTestDb : TestDb
{
    public string FileName { get; }
    public string FilePath { get; }
    public string DirectoryPath { get; }
    public override string ConnectionString { get; }

    public OnDiskTestDb()
    {
        DirectoryPath = TestDir.Instance.DirectoryPath;
        FileName = $"argo-test-{Guid.NewGuid():N}.sqlite";
        FilePath = Path.Combine(DirectoryPath, FileName);
        ConnectionString = $"Data Source={FilePath}";
    }

    public override SqliteConnection GetAndOpenConnection()
    {
        SqliteConnection c = new SqliteConnection(ConnectionString);
        c.Open();
        return c;
    }

    public override void Dispose()
    {
        for (int i = 0; i < 10; i++)
        {
            if (File.Exists(FilePath))
            {
                SqliteConnection.ClearAllPools();
                try
                {
                    File.Delete(FilePath);
                }
                catch
                {
                    // no op
                }
            }
        }
    }
}