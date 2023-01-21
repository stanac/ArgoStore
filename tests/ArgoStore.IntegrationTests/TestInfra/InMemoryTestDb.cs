using Microsoft.Data.Sqlite;

namespace ArgoStore.IntegrationTests.TestInfra;

public class InMemoryTestDb : TestDb
{
    public string ConnectionName { get; }
    public override string ConnectionString { get; }

    private readonly SqliteConnection _connection;

    public InMemoryTestDb()
    {
        ConnectionName = "inMemoryTest-" + Guid.NewGuid().ToString("N");
        string conStr = $"Data Source={ConnectionName};Mode=Memory;Cache=Shared";
        ConnectionString = conStr;
        _connection = new SqliteConnection(conStr);
        _connection.Open();
    }

    public override SqliteConnection GetAndOpenConnection()
    {
        SqliteConnection c = new SqliteConnection(ConnectionString);
        c.Open();
        return c;
    }

    public override void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }
}