using ArgoStore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArgoStore
{
    internal class StatementToSqlTranslator : IStatementToSqlTranslator
    {
        private readonly IArgoStoreSerializer _serializer;

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

            return CreateSelectCommand(statement.SelectStatement, statement.TenantId, false);
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

            ArgoSqlCommand selectCommand = CreateSelectCommand(statement.SelectStatement, statement.TenantId, false);
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
            
            ArgoSqlCommand selectCommand = CreateSelectCommand(statement.SelectStatement, statement.TenantId, true);

            selectCommand.CommandText = $"SELECT COUNT (*) FROM ({selectCommand.CommandText}) WHERE tenant_id = $__tenant_id__";

            return selectCommand;
        }

        private ArgoSqlCommand CreateSelectCommand(SelectStatement select, string tenantId, bool isSubQuery)
        {
            if (select.CalledByMethod == SelectStatement.CalledByMethods.Last || select.CalledByMethod == SelectStatement.CalledByMethods.LastOrDefault)
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
                ArgoSqlCommand subCommand = CreateSelectCommand(select.SubQueryStatement, tenantId, true);

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
                case ConstantStatement s4: return GetConstantStatementSql(s4, alias, cmd);
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

        private string GetConstantStatementSql(ConstantStatement statement, string alias, ArgoSqlCommand cmd)
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

            switch (statement.MethodName)
            {
                case MethodCallStatement.SupportedMethodNames.StringToUpper:
                    return $"upper({args[0]})";
                    
                case MethodCallStatement.SupportedMethodNames.StringToLower:
                    return $"lower({args[0]})";

                case MethodCallStatement.SupportedMethodNames.StringTrim:
                    return $"trim({args[0]})";

                case MethodCallStatement.SupportedMethodNames.StringTrimStart:
                    return $"ltrim({args[0]})";

                case MethodCallStatement.SupportedMethodNames.StringTrimEnd:
                    return $"rtrim({args[0]})";

                case MethodCallStatement.SupportedMethodNames.StringIsNullOrEmpty:
                    return $"({args[0]} IS NULL OR {args[0]} = '')";

                case MethodCallStatement.SupportedMethodNames.StringIsNullOrWhiteSpace:
                    return $"({args[0]} IS NULL OR trim({args[0]}) = '')";

                case MethodCallStatement.SupportedMethodNames.StringEquals:
                    return $"{args[0]} == {args[1]}";

                case MethodCallStatement.SupportedMethodNames.StringEqualsIgnoreCase:
                    return $"upper({args[0]}) == upper({args[1]})";

                case MethodCallStatement.SupportedMethodNames.StringContains:
                    cmd.Parameters.AddWildcard(args[1], true, true);
                    return $"{args[0]} LIKE {args[1]} ESCAPE('\\')";
                    
                case MethodCallStatement.SupportedMethodNames.StringContainsIgnoreCase:
                    break;

                case MethodCallStatement.SupportedMethodNames.StringStartsWith:
                    cmd.Parameters.AddWildcard(args[1], false, true);
                    return $"{args[0]} LIKE {args[1]} ESCAPE('\\')";
                    
                case MethodCallStatement.SupportedMethodNames.StringEndsWith:
                    cmd.Parameters.AddWildcard(args[1], true, false);
                    return $"{args[0]} LIKE {args[1]} ESCAPE('\\')";

                case MethodCallStatement.SupportedMethodNames.StringStartsWithIgnoreCase:
                    break;

                case MethodCallStatement.SupportedMethodNames.StringEndsWithIgnoreCase:
                    break;

                case MethodCallStatement.SupportedMethodNames.EnumerableContains:
                    break;
            }
            
            throw new ArgumentOutOfRangeException(nameof(statement));
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
}
