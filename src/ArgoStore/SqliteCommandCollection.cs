using Microsoft.Data.Sqlite;

namespace ArgoStore;

public class SqliteCommandCollection : IDisposable
{
    public SqliteCommand[] Commands { get; }

    public SqliteCommandCollection(SqliteCommand[] commands)
    {
        Commands = commands;
    }

    public SqliteConnection Connection
    {
        set
        {
            foreach (SqliteCommand c in Commands)
            {
                c.Connection = value;
            }
        }
    }

    public void Dispose()
    {
        foreach (SqliteCommand c in Commands)
        {
            c.Dispose();
        }
    }
}