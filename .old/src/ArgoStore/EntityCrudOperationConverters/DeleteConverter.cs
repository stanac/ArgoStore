﻿using ArgoStore.Helpers;
using Microsoft.Data.Sqlite;

namespace ArgoStore.EntityCrudOperationConverters;

internal class DeleteConverter : IEntityCrudOperationConverter
{
    public bool CanConvert(EntityCrudOperation op) => op != null && op.CrudOperation == CrudOperations.Delete;

    public SqliteCommand ConvertToCommand(EntityCrudOperation op, SqliteConnection connection, IArgoStoreSerializer serializer, string tenantId)
    {
        if (string.IsNullOrWhiteSpace(tenantId)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(tenantId));

        string condition = op.PkValue.IsLongKey ? "id = $id" : "string_id = $id";

        object key = op.PkValue.GetValue();
        if (key is Guid)
        {
            key = key.ToString();
        }

        SqliteCommand cmd = connection.CreateCommand();
        cmd.CommandText = $"DELETE FROM {EntityTableHelper.GetTableName(op.EntityMeta.EntityType)} WHERE tenant_id = $tenantId AND {condition}";
        cmd.Parameters.AddWithValue("$id", key);
        cmd.Parameters.AddWithValue("$tenantId", tenantId);
            
        return cmd;
    }
}