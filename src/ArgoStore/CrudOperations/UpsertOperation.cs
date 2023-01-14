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

        if (Metadata.IsKeyPropertyInt)
        {
            SqliteCommand cmdSerial = CreateForSerialId(jsonSerializerOptions);
            cmdSerial.EnsureNoGuidParams();
            return cmdSerial;
        }

        SqliteCommand cmdString = CreateForStringId(jsonSerializerOptions);
        cmdString.EnsureNoGuidParams();
        return cmdString;
    }

    private SqliteCommand CreateForStringId(JsonSerializerOptions jsonSerializerOptions)
    {
        string? key = Metadata.GetPrimaryKeyValue(Document!, out _)?.ToString();

        string sql;

        if (key == null || key == Guid.Empty.ToString())
        {
            Guid keyGuid = Guid.NewGuid();
            key = keyGuid.ToString();

            if (Metadata.IsKeyPropertyGuid)
            {
                Metadata.SetKey(Document!, keyGuid);
            }
            else
            {
               Metadata.SetKey(Document!, key);
            }

            sql = $@"
                INSERT INTO {Metadata.DocumentName} (stringId, tenantId, jsonData, createdAt)
                VALUES (@key, @tenantId, json(@jsonData), @updatedAt)
            ";
        }
        else
        {
            sql = $@"
                INSERT INTO {Metadata.DocumentName} (stringId, tenantId, jsonData, createdAt)
                VALUES (@key, @tenantId, json(@jsonData), @updatedAt)
                ON CONFLICT DO UPDATE SET jsonData = json(@jsonData), updatedAt = @updatedAt
                WHERE stringId = @key AND tenantId = @tenantId
            ";
        }

        long updatedAt = Clock.Default.GetCurrentUtcMilliseconds();
        string jsonData = JsonSerializer.Serialize(Document, jsonSerializerOptions);

        SqliteCommand cmd = new SqliteCommand(sql);

        cmd.Parameters.AddWithValue("jsonData", jsonData);
        cmd.Parameters.AddWithValue("updatedAt", updatedAt);
        cmd.Parameters.AddWithValue("key", key);
        cmd.Parameters.AddWithValue("tenantId", TenantId);

        return cmd;
    }

    private SqliteCommand CreateForSerialId(JsonSerializerOptions jsonSerializerOptions)
    {
        object key = Metadata.GetPrimaryKeyValue(Document!, out bool isDefaultValue)!;

        string sql;

        if (isDefaultValue)
        {
            sql = $@"
                INSERT INTO {Metadata.DocumentName} (tenantId, stringId, jsonData, createdAt)
                VALUES (@tenantId, @guidId, json(@jsonData), @updatedAt)
            ";
        }
        else
        {
            sql = $@"
                INSERT INTO {Metadata.DocumentName} (serialId, tenantId, stringId, jsonData, createdAt)
                VALUES (@id, @tenantId, @guidId, json(@jsonData), @updatedAt)
                ON CONFLICT DO UPDATE SET jsonData = json(@jsonData), updatedAt = @updatedAt
                WHERE serialId = @id
            ";
        }

        long updatedAt = Clock.Default.GetCurrentUtcMilliseconds();
        string jsonData = JsonSerializer.Serialize(Document, jsonSerializerOptions);

        SqliteCommand cmd = new SqliteCommand(sql);

        if (!isDefaultValue)
        {
            cmd.Parameters.AddWithValue("id", key);
        }
        
        cmd.Parameters.AddWithValue("jsonData", jsonData);
        cmd.Parameters.AddWithValue("updatedAt", updatedAt);
        cmd.Parameters.AddWithValue("key", key);
        cmd.Parameters.AddWithValue("tenantId", TenantId);
        cmd.Parameters.AddWithValue("guidId", Guid.NewGuid().ToString().ToLower());

        cmd.EnsureNoGuidParams();

        return cmd;
    }
}