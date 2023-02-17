using System.Text.Json;
using ArgoStore.Config;
using ArgoStore.Helpers;
using Microsoft.Data.Sqlite;

namespace ArgoStore.CrudOperations;

internal class UpsertOperation : CrudOperation
{
    public UpsertOperation(DocumentMetadata metadata, object document, string tenantId)
        : base(metadata, document, tenantId, null)
    {
    }

    public override SqliteCommand CreateCommand(JsonSerializerOptions jsonSerializerOptions)
    {
        if (jsonSerializerOptions == null) throw new ArgumentNullException(nameof(jsonSerializerOptions));
        
        SqliteCommand cmdString = CreateForStringId(jsonSerializerOptions);
        cmdString.EnsureNoGuidParams();
        return cmdString;
    }

    private SqliteCommand CreateForStringId(JsonSerializerOptions jsonSerializerOptions)
    {
        string key = Metadata.SetIfNeededAndGetPrimaryKeyValue(Document!).ToString()!;

        string sql = $@"
                INSERT INTO {Metadata.DocumentName} (stringId, tenantId, jsonData, createdAt)
                VALUES (@key, @tenantId, json(@jsonData), @updatedAt)
                ON CONFLICT DO UPDATE SET jsonData = json(@jsonData), updatedAt = @updatedAt
                WHERE stringId = @key AND tenantId = @tenantId
            ";

        long updatedAt = Clock.Default.GetCurrentUtcMilliseconds();
        string jsonData = JsonSerializer.Serialize(Document, jsonSerializerOptions);

        SqliteCommand cmd = new SqliteCommand(sql);

        cmd.Parameters.AddWithValue("jsonData", jsonData);
        cmd.Parameters.AddWithValue("updatedAt", updatedAt);
        cmd.Parameters.AddWithValue("key", key);
        cmd.Parameters.AddWithValue("tenantId", TenantId);
        cmd.EnsureNoGuidParams();

        return cmd;
    }
}