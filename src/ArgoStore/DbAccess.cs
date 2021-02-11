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

        public IEnumerable<object[]> QueryFields(string sql)
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
                    object[] row = new object[reader.FieldCount];

                    for (int i = 0; i < row.Length; i++)
                    {
                        row[i] = reader[0];
                        if (row[i] is DBNull)
                        {
                            row[i] = null;
                        }
                    }

                    yield return row;
                }
            }
        }

        public IEnumerable<object> QueryField(string sql)
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

                    if (reader[0] is DBNull) yield return null;
                    else yield return reader[0];
                }
            }
        }
        public IEnumerable<string> QueryJsonField(string sql)
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

                    yield return (string)reader[0];
                }
            }
        }

        public Task<IEnumerable<string>> QueryJsonFieldAsync(string sql)
        {
            throw new NotImplementedException();
        }
    }
}
