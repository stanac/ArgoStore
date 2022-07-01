using ArgoStore.Helpers;
using Microsoft.Data.Sqlite;

namespace ArgoStore.EntityCrudOperationConverters;

internal class InsertConverter : IEntityCrudOperationConverter
{
    public bool CanConvert(EntityCrudOperation op) => op != null && op.CrudOperation == CrudOperations.Insert;

    public SqliteCommand ConvertToCommand(EntityCrudOperation op, SqliteConnection connection, IArgoStoreSerializer serializer, string tenantId)
    {
        if (string.IsNullOrWhiteSpace(tenantId)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(tenantId));

        string stringId = null;

        if (op.PkValue.IsStringKey)
        {
            stringId = op.PkValue.StringKey;
        }
        else
        {
            stringId = Guid.NewGuid().ToString();
        }

        SqliteCommand cmd = connection.CreateCommand();
        string json = serializer.Serialize(op.Entity);
        cmd.CommandText = $"INSERT INTO {EntityTableHelper.GetTableName(op.EntityMeta.EntityType)} " +
                          "(string_id, json_data, created_at, tenant_id)\n" +
                          "VALUES($id, json($jsonData), $createdTime, $tenantId)";

        cmd.Parameters.AddWithValue("$id", stringId);
        cmd.Parameters.AddWithValue("$jsonData", json);
        cmd.Parameters.AddWithValue("$createdTime", DateTimeFormatter.ToUtcFormat(DateTime.UtcNow));
        cmd.Parameters.AddWithValue("$tenantId", tenantId);

        return cmd;
    }
}