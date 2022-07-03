using System.Text;
using Microsoft.Data.Sqlite;
using ArgoStore.Configurations;

namespace ArgoStore.Helpers;

internal class EntityTableHelper
{
    private readonly Configuration _config;
    private readonly CacheHashSet<Type> _createdTables = new(); // make static to cache
        
    public EntityTableHelper(Configuration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public static string GetTableName<T>() => GetTableName(typeof(T));

    public static string GetTableName(Type t)
    {
        if (t is null) throw new ArgumentNullException(nameof(t));

        return $"{t.Name}_json_docs";
    }

    public void EnsureEntityTableExists<T>() => EnsureEntityTableExists(typeof(T));

    public void EnsureEntityTableExists(Type t)
    {
        if (t is null) throw new ArgumentNullException(nameof(t));

        if (_createdTables.Contains(t))
        {
            return;
        }

        EntityMetadata entityMeta = _config.GetOrCreateEntityMetadata(t);

        string tableName = GetTableName(t);

        bool tableExists = TableExists(tableName);

        if (!tableExists)
        {
            if (_config.CreateEntitiesOnTheFly)
            {
                CreateTableIfNotExists(tableName);
                CreateIndexesIfMissing(entityMeta);

                _createdTables.Add(t);
            }
            else
            {
                throw new InvalidOperationException(
                    $"Table for entity {t.FullName} doesn't exist and configuration " +
                    $"{nameof(Configuration.CreateEntitiesOnTheFly)} is set to False");
            }
        }
    }

    private bool TableExists(string tableName)
    {
        string sql = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=@tableName";

        using var c = new SqliteConnection(_config.ConnectionString);
        c.Open();

        var cmd = c.CreateCommand();
        cmd.CommandText = sql;
        cmd.Parameters.AddWithValue("tableName", tableName);
        long count = (long)cmd.ExecuteScalar()!;
        return count > 0;
    }

    private void CreateTableIfNotExists(string tableName)
    {
        tableName = EscapeSqliteParameter(tableName);

        string sql = $@"
                CREATE TABLE IF NOT EXISTS {tableName} (
                    tenant_id TEXT NOT NULL,
                    id INTEGER NOT NULL PRIMARY KEY,
                    string_id TEXT NOT NULL,
                    json_data JSON NOT NULL,
                    created_by TEXT NULL,
                    created_at TEXT NOT NULL,
                    updated_by TEXT NULL,
                    updated_at TEXT NULL,
                    audit_id TEXT NULL,
                    UNIQUE (tenant_id, string_id)
                )
            ";

        using SqliteConnection c = new SqliteConnection(_config.ConnectionString);
        c.Open();

        SqliteCommand cmd = c.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    private void CreateIndexesIfMissing(EntityMetadata entityMeta)
    {
        foreach (EntityIndexMetadata indexMeta in entityMeta.Indexes)
        {
            CreateIndexIfNotExists(indexMeta);
        }
    }

    private void CreateIndexIfNotExists(EntityIndexMetadata indexMeta)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("CREATE ");
        if (indexMeta.Unique) sb.Append("UNIQUE ");
        sb.Append("INDEX IF NOT EXISTS ").Append(indexMeta.GetIndexName())
            .Append(" ON ").Append(GetTableName(indexMeta.EntityType))
            .Append(" (");

        for (var i = 0; i < indexMeta.PropertyNames.Count; i++)
        {
            if (i > 0)
            {
                sb.Append(", ");
            }

            string s = indexMeta.PropertyNames[i];
            string columnName = _config.Serializer.ConvertPropertyNameToCorrectCase(s);
            string extract = $"json_extract(json_data, '$.{columnName}')";
            
            sb.Append(extract);
        }

        sb.Append(")");
        
        using SqliteConnection c = new SqliteConnection(_config.ConnectionString);
        c.Open();

        SqliteCommand cmd = c.CreateCommand();
        cmd.CommandText = sb.ToString();
        cmd.ExecuteNonQuery();
    }

    private string EscapeSqliteParameter(string parameter)
    {
        return parameter.Replace("'", "''");
    }
}