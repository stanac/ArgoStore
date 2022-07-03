using ArgoStore.Helpers;
using ArgoStore.Statements;

namespace ArgoStore;

internal class StatementToSqlTranslator : IStatementToSqlTranslator
{
    private readonly IArgoStoreSerializer _serializer;

    private static readonly MethodCallStatement.SupportedMethodNames[] _stringContainsMethods =
    {
        MethodCallStatement.SupportedMethodNames.StringContains,
        MethodCallStatement.SupportedMethodNames.StringContainsIgnoreCase,
        MethodCallStatement.SupportedMethodNames.StringStartsWith,
        MethodCallStatement.SupportedMethodNames.StringStartsWithIgnoreCase,
        MethodCallStatement.SupportedMethodNames.StringEndsWith,
        MethodCallStatement.SupportedMethodNames.StringEndsWithIgnoreCase
    };

    public StatementToSqlTranslator()
        : this (new ArgoStoreSerializer())
    {
    }

    public StatementToSqlTranslator(IArgoStoreSerializer serializer)
    {
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    }

    public ArgoSqlCommand CreateCommand(TopStatement statement)
    {
        if (statement is null) throw new ArgumentNullException(nameof(statement));
            
        statement.SetAliases();

        if (statement.IsCountQuery)
        {
            return CreateCountCommand(statement);
        }

        if (statement.IsAnyQuery)
        {
            return CreateAnyCommand(statement);
        }

        ArgoSqlCommand selectStatement = CreateSelectCommand(statement.SelectStatement, false);

        if (statement.SelectStatement.Top.HasValue)
        {
            string sql = selectStatement.CommandText + " LIMIT " + statement.SelectStatement.Top.Value;
            ArgoSqlParameterCollection parameters = selectStatement.Parameters;

            selectStatement = new ArgoSqlCommand
            {
                CommandText = sql
            };
            selectStatement.SetParameters(parameters);
        }

        return selectStatement;
    }

    // todo: optimize sql generation, use string builder

    private ArgoSqlCommand CreateAnyCommand(TopStatement statement)
    {
        // any statement only returns one row or zero rows
        // if one row is returned result is True, otherwise is False
            
        if (statement.SelectStatement == null)
        {
            return new ArgoSqlCommand
            {
                CommandText = $"SELECT 1 FROM {EntityTableHelper.GetTableName(statement.TypeFrom)} WHERE tenant_id = $__tenant_id__ LIMIT 1"
            };
        }

        ArgoSqlCommand selectCommand = CreateSelectCommand(statement.SelectStatement, false);
        selectCommand.CommandText = $"SELECT 1 FROM ({selectCommand.CommandText}) LIMIT 1";
        return selectCommand;
    }

    private ArgoSqlCommand CreateCountCommand(TopStatement statement)
    {
        if (statement.SelectStatement == null)
        {
            return new ArgoSqlCommand
            {
                CommandText = $"SELECT COUNT(*) FROM {EntityTableHelper.GetTableName(statement.TypeFrom)} WHERE tenant_id = $__tenant_id__"
            };
        }
            
        ArgoSqlCommand selectCommand = CreateSelectCommand(statement.SelectStatement, true);

        selectCommand.CommandText = $"SELECT COUNT (*) FROM ({selectCommand.CommandText}) WHERE tenant_id = $__tenant_id__";

        return selectCommand;
    }

    private ArgoSqlCommand CreateSelectCommand(SelectStatement select, bool isSubQuery)
    {
        if (select.CalledByMethod == CalledByMethods.Last || select.CalledByMethod == CalledByMethods.LastOrDefault)
        {
            throw new NotSupportedException($"Method {select.CalledByMethod} is not supported. Use OrderBy/OrderByDescending and First/FirstOrDefault");
        }

        ArgoSqlCommand cmd = new ArgoSqlCommand();

        string sql = SelectElementsToSql(select, cmd);

        if (isSubQuery)
        {
            sql += $", {select.Alias}.tenant_id";
        }

        string from;

        if (select.SubQueryStatement != null)
        {
            ArgoSqlCommand subCommand = CreateSelectCommand(select.SubQueryStatement, true);

            if (!subCommand.ArePrefixLocked())
            {
                subCommand.SetRandomParametersPrefix();
                subCommand.LockPrefix();
            }

            foreach (ArgoSqlParameter p in subCommand.Parameters)
            {
                cmd.Parameters.Add(p);
            }
                
            from = "\nFROM (" + subCommand.CommandText + ") " + select.Alias;
        }
        else
        {
            from = $@"
FROM {EntityTableHelper.GetTableName(select.TypeFrom)} {select.Alias}";
        }

        sql += from;

        if (select.WhereStatement != null)
        {
            sql += $@"
WHERE ({GetSql(select.WhereStatement.Statement, select.Alias, cmd)})
";
            if (!isSubQuery)
            {
                sql += "AND tenant_id = $__tenant_id__";
            }
        }
        else if (!isSubQuery)
        {
            sql += $@"
WHERE {select.Alias}.tenant_id = $__tenant_id__
";
        }

        if (select.OrderByStatement != null && select.OrderByStatement.Elements.Any())
        {
            sql += $@"
{GetSql(select.OrderByStatement, select.Alias, cmd)}
";
        }

        cmd.CommandText = sql;
        return cmd;
    }

    private string GetSql(Statement statement, string alias, ArgoSqlCommand cmd)
    {
        if (statement is null) throw new ArgumentNullException(nameof(statement));

        switch (statement)
        {
            case BinaryStatement s1: return GetBinaryStatementSql(s1, alias, cmd);
            case PropertyAccessStatement s3: return GetPropertyAccessStatementSql(s3, alias);
            case ConstantStatement s4: return GetConstantStatementSql(s4, cmd);
            case MethodCallStatement s5: return GetMethodCallStatementSql(s5, alias, cmd);
            case OrderByStatement s6: return GetOrderByStatementSql(s6, alias);
        }

        throw new ArgumentOutOfRangeException($"Missing implementation for \"{statement.GetType().FullName}\"");
    }

    private string GetBinaryStatementSql(BinaryStatement statement, string alias, ArgoSqlCommand cmd)
    {
        string left = GetSql(statement.Left, alias, cmd);

        string op = statement.OperatorString;
            
        if (statement is BinaryComparisonStatement bcs && (statement.Left is ConstantStatement c1 && c1.IsNull || statement.Right is ConstantStatement c2 && c2.IsNull))
        {
            if (bcs.Operator == BinaryComparisonStatement.Operators.Equal) op = "IS";
            else if (bcs.Operator == BinaryComparisonStatement.Operators.NotEqual) op = "IS NOT";
        }

        if (statement.Left is BinaryStatement)
        {
            left = "(" + left + " )";
        }

        string right = GetSql(statement.Right, alias, cmd);

        if (statement.Right is BinaryStatement)
        {
            right = "(" + right + " )";
        }

        return $"{left} {op} {right}";
    }

    private string GetPropertyAccessStatementSql(PropertyAccessStatement statement, string alias)
    {
        return $"json_extract({alias}.json_data, '$.{_serializer.ConvertPropertyNameToCorrectCase(statement.Name)}')";
    }

    private string GetConstantStatementSql(ConstantStatement statement, ArgoSqlCommand cmd)
    {
        if (statement.IsNull)
        {
            return "NULL";
        }
        if (statement.IsString)
        {
            return cmd.AddParameterAndGetParameterName(statement.Value);
        }

        return statement.Value;
    }

    private string GetMethodCallStatementSql(MethodCallStatement statement, string alias, ArgoSqlCommand cmd)
    {
        List<string> args = statement.Arguments.Select(x => GetSql(x, alias, cmd)).ToList();

        string s = null;

        bool argsHasNull = statement.Arguments.Any(x => x is ConstantStatement cs && cs.IsNull);

        if (argsHasNull && _stringContainsMethods.Contains(statement.MethodName))
        {
            return $"{args[0]} IS {args[1]}";
        }

        switch (statement.MethodName)
        {
            case MethodCallStatement.SupportedMethodNames.StringToUpper:
                s = $"upper({args[0]})";
                break;
                    
            case MethodCallStatement.SupportedMethodNames.StringToLower:
                s = $"lower({args[0]})";
                break;
                    
            case MethodCallStatement.SupportedMethodNames.StringTrim:
                s = $"trim({args[0]})";
                break;

            case MethodCallStatement.SupportedMethodNames.StringTrimStart:
                s = $"ltrim({args[0]})";
                break;

            case MethodCallStatement.SupportedMethodNames.StringTrimEnd:
                s = $"rtrim({args[0]})";
                break;

            case MethodCallStatement.SupportedMethodNames.StringIsNullOrEmpty:
                s = $"({args[0]} IS NULL OR {args[0]} = '')";
                break;

            case MethodCallStatement.SupportedMethodNames.StringIsNullOrWhiteSpace:
                s = $"({args[0]} IS NULL OR trim({args[0]}) = '')";
                break;

            case MethodCallStatement.SupportedMethodNames.StringEquals:
                s = $"{args[0]} == {args[1]}";
                break;

            case MethodCallStatement.SupportedMethodNames.StringEqualsIgnoreCase:
                cmd.Parameters.ToUpper(args[1]);
                s = $"upper({args[0]}) == {args[1]}";
                break;

            case MethodCallStatement.SupportedMethodNames.StringContains:
                cmd.Parameters.AddWildcard(args[1], true, true);
                s = $"{args[0]} LIKE {args[1]} ESCAPE('\\')";
                break;
                    
            case MethodCallStatement.SupportedMethodNames.StringContainsIgnoreCase:
                cmd.Parameters.AddWildcard(args[1], true, true);
                cmd.Parameters.ToUpper(args[1]);
                s = $"upper({args[0]}) LIKE {args[1]} ESCAPE('\\')";
                break;
                    
            case MethodCallStatement.SupportedMethodNames.StringStartsWith:
                cmd.Parameters.AddWildcard(args[1], false, true);
                s = $"{args[0]} LIKE {args[1]} ESCAPE('\\')";
                break;

            case MethodCallStatement.SupportedMethodNames.StringStartsWithIgnoreCase:
                cmd.Parameters.AddWildcard(args[1], false, true);
                cmd.Parameters.ToUpper(args[1]);
                s = $"upper({args[0]}) LIKE {args[1]} ESCAPE('\\')";
                break;
                    
            case MethodCallStatement.SupportedMethodNames.StringEndsWith:
                cmd.Parameters.AddWildcard(args[1], true, false);
                s = $"{args[0]} LIKE {args[1]} ESCAPE('\\')";
                break;

            case MethodCallStatement.SupportedMethodNames.StringEndsWithIgnoreCase:
                cmd.Parameters.AddWildcard(args[1], true, false);
                cmd.Parameters.ToUpper(args[1]);
                s = $"upper({args[0]}) LIKE {args[1]} ESCAPE('\\')";
                break;
                    
            case MethodCallStatement.SupportedMethodNames.EnumerableContains:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(statement));
        }

        if (statement.Negated)
        {
            s = $"NOT ({s})";
        }

        return s;
    }
        
    private string GetOrderByStatementSql(OrderByStatement statement, string alias)
    {
        string sql = "ORDER BY ";

        for (int i = 0; i < statement.Elements.Count; i++)
        {
            sql += $"json_extract({alias}.json_data, '$.{_serializer.ConvertPropertyNameToCorrectCase(statement.Elements[i].PropertyName)}') {(statement.Elements[i].Ascending ? "ASC" : "DESC")}";

            if (i < statement.Elements.Count - 1)
            {
                sql += ", ";
            }
        }

        return sql;
    }

    private string SelectElementsToSql(SelectStatement statement, ArgoSqlCommand cmd)
    {
        if (statement.SubQueryStatement != null)
        {
            string sql = "SELECT ";

            for (int i = 0; i < statement.SelectElements.Count; i++)
            {
                sql += $"{statement.Alias}.{statement.SelectElements[i].BindsToSubQueryAlias}";

                if (i != statement.SelectElements.Count - 1)
                {
                    sql += ", ";
                }
            }

            return sql;
        }
        else
        {
            if (statement.SelectElements.Count == 1 && statement.SelectElements[0].Statement is SelectStarParameterStatement)
            {
                return "SELECT json_data";
            }

            return "SELECT " + string.Join(", ", statement.SelectElements.Select(x => GetSql(x.Statement, statement.Alias, cmd) + $" {x.Alias}"));
        }
    }
}