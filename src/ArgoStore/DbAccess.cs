using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArgoStore
{
    internal class DbAccess : IDbAccess
    {
        private readonly string _connectionString;

        public DbAccess(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException($"'{nameof(connectionString)}' cannot be null or whitespace", nameof(connectionString));
            }

            _connectionString = connectionString;
        }

        public IReadOnlyList<string> QueryJsonField(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql)) throw new ArgumentException($"'{nameof(sql)}' cannot be null or whitespace", nameof(sql));

            List<string> result = new List<string>();

            using (var con = new SqliteConnection(_connectionString))
            {
                con.Open();

                var cmd = con.CreateCommand();
                cmd.CommandText = sql;

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.FieldCount != 1)
                    {
                        throw new InvalidOperationException("Reader returned more than 1 column for query:\n" + sql);
                    }

                    result.Add((string)reader[0]);
                }

                return result;
            }
        }

        public async Task<IReadOnlyList<string>> QueryJsonFieldAsync(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql)) throw new ArgumentException($"'{nameof(sql)}' cannot be null or whitespace", nameof(sql));

            List<string> result = new List<string>();

            using (var con = new SqliteConnection(_connectionString))
            {
                await con.OpenAsync();

                var cmd = con.CreateCommand();
                cmd.CommandText = sql;

                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    if (reader.FieldCount != 1)
                    {
                        throw new InvalidOperationException("Reader returned more than 1 column for query:\n" + sql);
                    }

                    result.Add((string)reader[0]);
                }

                return result;
            }
        }
    }
}
