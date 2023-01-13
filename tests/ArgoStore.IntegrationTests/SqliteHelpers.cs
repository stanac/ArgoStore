using System.Data;
using Microsoft.Data.Sqlite;

namespace ArgoStore.IntegrationTests;

internal static class SqliteHelpers
{
    public static List<IndexInfo> ListIndexes(SqliteConnection connection)
    {
        if (connection == null) throw new ArgumentNullException(nameof(connection));

        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }

        SqliteCommand cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT type, name, tbl_name, sql
            FROM sqlite_master
            WHERE type= 'index';";

        List<IndexInfo> ret = new();

        SqliteDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            IndexInfo info = new IndexInfo
            {
                Type = reader.GetString(0),
                Name = reader.GetString(1),
                Table = reader.GetString(2),
                Sql = reader.GetString(3)
            };

            ret.Add(info);
        }

        return ret;
    }

    public class IndexInfo
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Table { get; set; }
        public string Sql { get; set; }
    }

}