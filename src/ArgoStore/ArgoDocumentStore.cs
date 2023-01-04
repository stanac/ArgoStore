using Microsoft.Data.Sqlite;

namespace ArgoStore;

public class ArgoDocumentStore
{
    private readonly string _connectionString;
    
    public ArgoDocumentStore(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

        _connectionString = connectionString;
    }

    public IArgoQueryDocumentSession CreateQuerySession()
    {
        return new ArgoSession(_connectionString);
    }

    public IArgoQueryDocumentSession CreateQuerySession(string tenantId)
    {
        return new ArgoSession(_connectionString, tenantId);
    }

    public void RegisterDocumentType<TDocument>()
        where TDocument : class, new()
    {
        RegisterDocumentType<TDocument>(typeof(TDocument).Name);
    }

    public void RegisterDocumentType<TDocument>(string documentName)
        where TDocument : class, new()
    {
        RegisterDocumentType(typeof(TDocument), documentName);
    }

    public void RegisterDocumentType(Type documentType)
    {
        RegisterDocumentType(documentType, documentType.Name);
    }

    public void RegisterDocumentType(Type documentType, string documentName)
    {
        CreateTableAsync(documentName).GetAwaiter().GetResult();
    }

    private async Task CreateTableAsync(string documentName)
    {
        using SqliteConnection c = await GetAndOpenConnectionAsync();

        string sql = $"""
            CREATE TABLE IF NOT EXISTS as_{documentName} (
                serialId BIGINT NOT NULL PRIMARY KEY AUTOINCREMENT,
                guidId TEXT NOT NULL UNIQUE,
                jsonData JSON NOT NULL,
                createdAt BIGINT NOT NULL,
                updatedAt BIGINT NULL
            )
        """;
    }

    private async Task<SqliteConnection> GetAndOpenConnectionAsync()
    {
        SqliteConnection c = new SqliteConnection(_connectionString);
        await c.OpenAsync();
        return c;
    }
}