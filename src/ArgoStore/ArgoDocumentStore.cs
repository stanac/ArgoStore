using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace ArgoStore;

public class ArgoDocumentStore
{
    private readonly string _connectionString;
    private readonly JsonSerializerOptions _serializerOptions;

    private readonly ConcurrentDictionary<string, DocumentMetadata> _documents = new();

    public ArgoDocumentStore(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

        _connectionString = connectionString;
        
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public IArgoDocumentSession CreateSession() => CreateSession(ArgoSession.DefaultTenant);

    public IArgoDocumentSession CreateSession(string tenantId)
    {
        return new ArgoSession(_connectionString, tenantId, _documents, _serializerOptions);
    }

    public IArgoQueryDocumentSession CreateQuerySession() => CreateQuerySession(ArgoSession.DefaultTenant);

    public IArgoQueryDocumentSession CreateQuerySession(string tenantId)
    {
        return new ArgoSession(_connectionString, tenantId, _documents, _serializerOptions);
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
        DocumentMetadata meta = new DocumentMetadata(documentType, documentName);
        _documents[meta.DocumentName] = meta;
        CreateTableAsync(meta.DocumentName).GetAwaiter().GetResult();
    }

    private async Task CreateTableAsync(string documentName)
    {
        string[] sql =
        {
            $"""
            CREATE TABLE IF NOT EXISTS {documentName}   (
                serialId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                stringId TEXT NOT NULL UNIQUE,
                jsonData JSON NOT NULL,
                tenantId TEXT NOT NULL,
                createdAt BIGINT NOT NULL,
                updatedAt BIGINT NULL
            )
            """,
            $"""
            CREATE INDEX IF NOT EXISTS ix_{documentName}_tenant 
            ON {documentName} (tenantId)
            """ /*,
            $"""
            CREATE INDEX IF NOT EXISTS ix_{documentName}_createdAt
            ON {documentName} (createdAt)
            """,
            $"""
            CREATE INDEX IF NOT EXISTS ix_{documentName}_updatedAt
            ON {documentName} (updatedAt)
            """*/
        };

        using SqliteConnection c = await GetAndOpenConnectionAsync();

        foreach (string s in sql)
        {
            SqliteCommand cmd = c.CreateCommand();
            cmd.CommandText = s;
            await cmd.ExecuteNonQueryAsync();
        }
    }

    private async Task<SqliteConnection> GetAndOpenConnectionAsync()
    {
        SqliteConnection c = new SqliteConnection(_connectionString);
        await c.OpenAsync();
        return c;
    }
}
