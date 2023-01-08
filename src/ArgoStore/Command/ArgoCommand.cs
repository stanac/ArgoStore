using System.Text;
using ArgoStore.Helpers;
using Microsoft.Data.Sqlite;

namespace ArgoStore.Command;

public class ArgoCommand
{
    private static readonly ArgoCommandParameterCollection _noParameters = new();

    public string Sql { get; }
    public ArgoCommandParameterCollection Parameters { get; }
    public ArgoCommandTypes CommandType { get; }
    public Type ResultingType { get; }
    public bool IsResultingTypeJson { get; }
    public bool ContainsLikeOperator { get; }

    public ArgoCommand(string sql, ArgoCommandParameterCollection parameters, ArgoCommandTypes commandType, Type resultingType,
        bool isResultingTypeJson, bool containsLikeOperator)
    {
        Sql = sql;
        Parameters = parameters;
        CommandType = commandType;
        ResultingType = resultingType;
        IsResultingTypeJson = isResultingTypeJson;
        ContainsLikeOperator = containsLikeOperator;
    }

    public SqliteCommandCollection ToSqliteCommands()
    {
        SqliteCommand[] cmds = ContainsLikeOperator
            ? new SqliteCommand[2]
            : new SqliteCommand[1];

        SqliteCommand cmd = new SqliteCommand(Sql);

        foreach (ArgoCommandParameter p in Parameters)
        {
            cmd.Parameters.AddWithValue(p.Name, p.Value);
        }

        if (ContainsLikeOperator)
        {
            cmds[0] = new SqliteCommand("PRAGMA case_sensitive_like=ON");
            cmds[1] = cmd;
        }
        else
        {
            cmds[0] = cmd;
        }

        return new SqliteCommandCollection(cmds);
    }

    public ArgoCommand ConvertToLongCount(int? maxCount)
    {
        string[] lines = Sql.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        StringBuilder sb = StringBuilderBag.Default.Get();

        sb.AppendLine("SELECT COUNT(1) FROM ( ");

        foreach (string line in lines)
        {
            sb.Append("    ");

            if (line.Trim().StartsWith("LIMIT"))
            {
                if (maxCount.HasValue)
                {
                    sb.AppendLine("LIMIT ").Append(maxCount);
                }
            }
            else
            {
                sb.AppendLine(line);
            }
        }

        sb.AppendLine(")");

        string sql = sb.ToString();

        StringBuilderBag.Default.Return(sb);

        return new ArgoCommand(sql, Parameters, ArgoCommandTypes.LongCount, typeof(long), false, false);
    }
}