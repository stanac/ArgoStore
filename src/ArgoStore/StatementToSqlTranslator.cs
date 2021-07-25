using ArgoStore.Helpers;
using System;
using System.Linq;
using Microsoft.Data.Sqlite;

namespace ArgoStore
{
    internal class StatementToSqlTranslator : IStatementToSqlTranslator
    {
        private readonly IArgoStoreSerializer _serializer;

        public StatementToSqlTranslator(IArgoStoreSerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public void SetSqlCommand(TopStatement statement, SqliteCommand cmd)
        {
            if (statement is null) throw new ArgumentNullException(nameof(statement));

            statement.SetAliases();

            if (statement.IsCountQuery)
            {
                SetCountSql(statement, cmd);
            }

            if (statement.IsAnyQuery)
            {
                SetAnySql(statement, cmd);
                return;
            }

            SetSql(statement.SelectStatement);
        }

        // todo: optimize sql generation, use string builder

        private void SetAnySql(TopStatement statement, SqliteCommand cmd)
        {
            // any statement only returns one row or zero rows
            // if one row is returned result is True, otherwise is False

            if (statement.SelectStatement == null)
            {
                cmd.CommandText = $"SELECT 1 FROM {EntityTableHelper.GetTableName(statement.TypeFrom)} LIMIT 1";
                return;
            }

            string innerSql = SetSql(statement.SelectStatement);

            return $"SELECT 1 FROM ({innerSql}) LIMIT 1";
        }

        private void SetCountSql(TopStatement statement, SqliteCommand cmd)
        {
            if (statement.SelectStatement == null)
            {
                return $"SELECT COUNT(*) FROM {EntityTableHelper.GetTableName(statement.TypeFrom)}";
            }
            
            string innerSql = SetSql(statement.SelectStatement);

            return $"SELECT COUNT (*) FROM ({innerSql})";
        }

        private string SetSql(SelectStatement select)
        {
            if (select.CalledByMethod == SelectStatement.CalledByMethods.Last || select.CalledByMethod == SelectStatement.CalledByMethods.LastOrDefault)
            {
                throw new NotSupportedException($"Method {select.CalledByMethod} is not supported. Use OrderBy/OrderByDescending and First/FirstOrDefault");
            }

            string sql = SelectElementsToSql(select);

            string from;

            if (select.SubQueryStatement != null)
            {
                from = "\nFROM (" + SetSql(select.SubQueryStatement) + ") " + select.Alias;
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
WHERE {SetSql(select.WhereStatement.Statement, select.Alias)}
";
            }

            if (select.OrderByStatement != null && select.OrderByStatement.Elements.Any())
            {
                sql += $@"
{SetSql(select.OrderByStatement, select.Alias)}
";
            }

            return sql;
        }

        private string SetSql(Statement statement, string alias)
        {
            if (statement is null) throw new ArgumentNullException(nameof(statement));

            switch (statement)
            {
                case BinaryStatement s1: return SetSql(s1, alias);
                case PropertyAccessStatement s3: return SetSql(s3, alias);
                case ConstantStatement s4: return SetSql(s4, alias);
                case MethodCallStatement s5: return SetSql(s5, alias);
                case OrderByStatement s6: return SetSql(s6, alias);
            }

            throw new ArgumentOutOfRangeException($"Missing implementation for \"{statement.GetType().FullName}\"");
        }

        private string SetSql(BinaryStatement statement, string alias)
        {
            string left = SetSql(statement.Left, alias);

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

            string right = SetSql(statement.Right, alias);

            if (statement.Right is BinaryStatement)
            {
                right = "(" + right + " )";
            }

            return $"{left} {op} {right}";
        }

        private string SetSql(PropertyAccessStatement statement, string alias)
        {
            return $"json_extract({alias}.json_data, '$.{_serializer.ConvertPropertyNameToCorrectCase(statement.Name)}')";
        }

        private string SetSql(ConstantStatement statement, string alias)
        {
            if (statement.IsNull)
            {
                return "NULL";
            }
            if (statement.IsString)
            {
                return "'" + statement.Value.Replace("'", "''") + "'";
            }

            return statement.Value;
        }

        private string SetSql(MethodCallStatement statement, string alias)
        {
            throw new NotImplementedException();
        }
        
        private string SetSql(OrderByStatement statement, string alias)
        {
            string sql = $"ORDER BY ";

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

        private string SelectElementsToSql(SelectStatement statement)
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

                return "SELECT " + string.Join(", ", statement.SelectElements.Select(x => SetSql(x.Statement, statement.Alias) + $" {x.Alias}"));
            }
        }
    }
}
