using Microsoft.Data.Sqlite;

namespace ArgoStore;

public class ArgoCommand
{
    public string Sql { get; }
    public IReadOnlyDictionary<string, object> Parameters { get; }
    private static readonly IReadOnlyDictionary<string, object> _noParameters = new Dictionary<string, object>();

    public ArgoCommand(string sql)
        : this (sql, _noParameters)
    {
    }

    public ArgoCommand(string sql, IReadOnlyDictionary<string, object> parameters)
    {
        Sql = sql;
        Parameters = parameters;
    }

    public SqliteCommand ToSqliteCommand()
    {
        var cmd = new SqliteCommand(Sql);

        throw new NotSupportedException();
    }
}