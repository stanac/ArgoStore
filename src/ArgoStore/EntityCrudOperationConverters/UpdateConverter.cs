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

            string whereCondition;
            object id;

            if (op.EntityMeta.PrimaryKeyProperty.PropertyType == typeof(Guid) ||
                op.EntityMeta.PrimaryKeyProperty.PropertyType == typeof(string))
            {
                whereCondition = "string_id = $id";
                id = op.PkValue.StringKey;
            }
            else
            {
                whereCondition = "id = $id";
                id = op.PkValue.LongKey;
            }

            cmd.CommandText = $"UPDATE {EntityTableHelper.GetTableName(op.EntityMeta.EntityType)}\n" +
                               "  SET json_data = json($json),\n" +
                               "      updated_at = $updatedAt\n" +
                              $"WHERE {whereCondition}";

            string json = serializer.Serialize(op.Entity);

            cmd.Parameters.AddWithValue("$json", json);
            cmd.Parameters.AddWithValue("$updatedAt", DateTimeFormatter.ToUtcFormat(DateTime.UtcNow));
            cmd.Parameters.AddWithValue("$id", id);

            return cmd;
        }
    }
}
