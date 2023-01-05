﻿using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace ArgoStore.CrudOperations;

internal class UpdateOperation : CrudOperation
{
    public UpdateOperation(DocumentMetadata metadata, object document, string tenantId) 
        : base(metadata, document, tenantId)
    {
    }

    public override SqliteCommand CreateCommand(JsonSerializerOptions jsonSerializerOptions)
    {
        object key = Metadata.GetPrimaryKeyValue(Document, out _);

        string pkName = Metadata.IsKeyPropertyInt
            ? "serialId"
            : "stringId";

        long updatedAt = Clock.Default.GetCurrentUtcMilliseconds();

        string jsonData = JsonSerializer.Serialize(Document, jsonSerializerOptions);

        string sql = $"""
                UPDATE {Metadata.DocumentName}
                SET
                    jsonData = @jsonData,
                    updatedAt = @updatedAt
                WHERE 
                    {pkName} = @key
                    AND tenantId = @tenantId
            """ ;

        SqliteCommand cmd = new SqliteCommand(sql);

        cmd.Parameters.AddWithValue("jsonData", jsonData);
        cmd.Parameters.AddWithValue("updatedAt", updatedAt);
        cmd.Parameters.AddWithValue("key", key);
        cmd.Parameters.AddWithValue("tenantId", TenantId);

        return cmd;
    }
}