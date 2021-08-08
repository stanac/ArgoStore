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

        public IEnumerable<object[]> QueryFields(SqliteCommand cmd, Type[] expectedResultTypes)
        {
            if (cmd == null) throw new ArgumentNullException(nameof(cmd));
            if (expectedResultTypes == null) throw new ArgumentNullException(nameof(expectedResultTypes));
            
            using var con = new SqliteConnection(_connectionString);

            con.Open();

            ExecutePragmaCaseSensitiveLike(con);

            cmd.Connection = con;
                
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                object[] row = new object[reader.FieldCount];

                for (int i = 0; i < row.Length; i++)
                {
                    row[i] = reader[i];
                    if (row[i] is DBNull)
                    {
                        row[i] = null;
                    }

                    row[i] = ArgoStoreConvert.To(expectedResultTypes[i], row[i]);
                }

                yield return row;
            }
        }

        public IEnumerable<object> QueryField(SqliteCommand cmd)
        {
            if (cmd == null) throw new ArgumentNullException(nameof(cmd));
            
            using var con = new SqliteConnection(_connectionString);

            con.Open();

            ExecutePragmaCaseSensitiveLike(con);

            cmd.Connection = con;

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                if (reader.FieldCount != 1)
                {
                    throw new InvalidOperationException("Reader returned more than 1 column for query:\n" + cmd.CommandText);
                }

                if (reader[0] is DBNull) yield return null;
                else yield return reader[0];
            }
        }

        public IEnumerable<string> QueryJsonField(SqliteCommand cmd)
        {
            if (cmd == null) throw new ArgumentNullException(nameof(cmd));
            
            using var con = new SqliteConnection(_connectionString);

            con.Open();

            ExecutePragmaCaseSensitiveLike(con);
                
            cmd.Connection = con;

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                if (reader.FieldCount != 1)
                {
                    throw new InvalidOperationException("Reader returned more than 1 column for query:\n" + cmd.CommandText);
                }

                yield return (string)reader[0];
            }
        }

        public Task<IEnumerable<string>> QueryJsonFieldAsync(SqliteCommand cmd)
        {
            throw new NotImplementedException();
        }

        private void ExecutePragmaCaseSensitiveLike(SqliteConnection c)
        {
            SqliteCommand cmd = c.CreateCommand();

            cmd.CommandText = "PRAGMA case_sensitive_like = true;";

            cmd.ExecuteNonQuery();
        }
    }
}
