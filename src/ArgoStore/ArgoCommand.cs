using System.Text;
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

        return new ArgoCommand(sql, Parameters, ArgoCommandTypes.LongCount, typeof(long));
    }
}