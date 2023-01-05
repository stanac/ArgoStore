using Microsoft.Data.Sqlite;

namespace ArgoStore;

public class ArgoCommand
{
    private static readonly ArgoCommandParameterCollection _noParameters = new();

    public string Sql { get; }
    public ArgoCommandParameterCollection Parameters { get; }
    public ArgoCommandTypes CommandType { get; }
    public Type ResultingType { get; }
    
    public ArgoCommand(string sql, ArgoCommandParameterCollection parameters, ArgoCommandTypes commandType, Type resultingType)
    {
        Sql = sql;
        Parameters = parameters;
        CommandType = commandType;
        ResultingType = resultingType;
    }
    
    public SqliteCommand ToSqliteCommand()
    {
        var cmd = new SqliteCommand(Sql);

        foreach (ArgoCommandParameter p in Parameters)
        {
            cmd.Parameters.AddWithValue(p.Name, p.Value);
        }

        return cmd;
    }
}