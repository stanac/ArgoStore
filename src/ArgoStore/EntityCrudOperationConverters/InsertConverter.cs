﻿using System;
using ArgoStore.Helpers;
using Microsoft.Data.Sqlite;

namespace ArgoStore.EntityCrudOperationConverters
{
    internal class InsertConverter : IEntityCrudOperationConverter
    {
        public bool CanConvert(EntityCrudOperation op) => op != null && op.CrudOperation == CrudOperations.Insert;

        public SqliteCommand ConvertToCommand(EntityCrudOperation op, SqliteConnection connection, IArgoStoreSerializer serializer)
        {
            SqliteCommand cmd = connection.CreateCommand();
            string json = serializer.Serialize(op.Entity);
            cmd.CommandText = $"INSERT INTO {EntityTableHelper.GetTableName(op.EntityMeta.EntityType)} " +
                            "(string_id, json_data, created_at)\n" +
                            "VALUES($id, json($jsonData), $createdTime)";
            cmd.Parameters.AddWithValue("$id", op.Id);
            cmd.Parameters.AddWithValue("$jsonData", json);
            cmd.Parameters.AddWithValue("$createdTime", DateTimeFormatter.ToUtcFormat(DateTime.UtcNow));

            return cmd;
        }
    }
}
