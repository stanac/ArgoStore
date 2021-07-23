using System;
using ArgoStore.Helpers;
using Microsoft.Data.Sqlite;

namespace ArgoStore.EntityCrudOperationConverters
{
    internal class UpdateConverter : IEntityCrudOperationConverter
    {
        public bool CanConvert(EntityCrudOperation op) => op != null && op.CrudOperation == CrudOperations.Update;

        public SqliteCommand ConvertToCommand(EntityCrudOperation op, SqliteConnection connection, IArgoStoreSerializer serializer)
        {
            SqliteCommand cmd = connection.CreateCommand();

            cmd.CommandText = $"UPDATE {EntityTableHelper.GetTableName(op.EntityMeta.EntityType)}\n" +
                               "  SET json_data = json($json)," +
                               "      updated_at = $updatedAt";

            string json = serializer.Serialize(op.Entity);

            cmd.Parameters.AddWithValue("$json", json);
            cmd.Parameters.AddWithValue("$updatedAt", DateTimeFormatter.ToUtcFormat(DateTime.UtcNow));

            return cmd;
        }
    }
}
