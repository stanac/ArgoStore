using Microsoft.Data.Sqlite;
using System;

namespace ArgoStore.Helpers
{
    internal class EntityTableHelper
    {
        private readonly Configuration _config;
        private readonly CacheHashSet<Type> _createdTables = new();
        
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

            string tableName = GetTableName(t);

            bool tableExists = TableExists(tableName);

            if (!tableExists)
            {
                if (_config.CreateEntitiesOnTheFly)
                {
                    CreateTableIfNotExists(tableName);

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

            using (var c = new SqliteConnection(_config.ConnectionString))
            {
                c.Open();

                var cmd = c.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("tableName", tableName);
                long count = (long)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        private void CreateTableIfNotExists(string tableName)
        {
            tableName = EscapeSqliteParameter(tableName);

            string sql = $@"
                CREATE TABLE IF NOT EXISTS {tableName} (
                    id INTEGER NOT NULL PRIMARY KEY,
                    string_id TEXT NOT NULL UNIQUE,
                    json_data JSON NOT NULL,
                    create_by TEXT NULL,
                    created_at TEXT NOT NULL,
                    updated_by TEXT NULL,
                    updated_at TEXT NULL
                )
            ";

            using (var c = new SqliteConnection(_config.ConnectionString))
            {
                c.Open();

                var cmd = c.CreateCommand();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

        private string EscapeSqliteParameter(string parameter)
        {
            return parameter.Replace("'", "''");
        }
    }
}
