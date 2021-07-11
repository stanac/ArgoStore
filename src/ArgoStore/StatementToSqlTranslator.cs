using ArgoStore.Helpers;
using System;
using System.Linq;

namespace ArgoStore
{
    internal class StatementToSqlTranslator : IStatementToSqlTranslator
    {
        private readonly IArgoStoreSerializer _serialzier;

        public StatementToSqlTranslator(IArgoStoreSerializer serialzier)
        {
            _serialzier = serialzier ?? throw new ArgumentNullException(nameof(serialzier));
        }

        public string ToSql(TopStatement statement)
        {
            if (statement is null) throw new ArgumentNullException(nameof(statement));

            statement.SetAliases();

            if (statement.IsCountQuery)
            {
                string countSql = ToCountSql(statement);

                return countSql;
            }

            string sql = ToSqlInternal(statement.SelectStatement);

            return sql;
        }

        // todo: optimize sql generation, use string builder

        private string ToCountSql(TopStatement statement)
        {
            if (statement.SelectStatement == null)
            {
                return $"SELECT COUNT(*) FROM {EntityTableHelper.GetTableName(statement.TypeFrom)}";
            }

            // else select statement is not null
            throw new NotImplementedException();
        }

        private string ToSqlInternal(SelectStatement select)
        {
            if (select.CalledByMethod == SelectStatement.CalledByMethods.Last || select.CalledByMethod == SelectStatement.CalledByMethods.LastOrDefault)
            {
                throw new NotSupportedException($"Method {select.CalledByMethod} is not supported use OrderBy/OrderByDescending and First/FirstOrDefault");
            }

            string sql = SelectElementsToSql(select);

            string from;

            if (select.SubQueryStatement != null)
            {
                from = "\nFROM (" + ToSqlInternal(select.SubQueryStatement) + ") " + select.Alias;
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
WHERE {ToSqlInternal(select.WhereStatement.Statement, select.Alias)}
";
            }

            if (select.OrderByStatement != null && select.OrderByStatement.Elements.Any())
            {
                sql += $@"
{ToSqlInternal(select.OrderByStatement, select.Alias)}
";
            }

            return sql;
        }

        private string ToSqlInternal(Statement statement, string alias)
        {
            if (statement is null) throw new ArgumentNullException(nameof(statement));

            switch (statement)
            {
                case BinaryStatement s1: return ToSqlInternal(s1, alias);
                case PropertyAccessStatement s3: return ToSqlInternal(s3, alias);
                case ConstantStatement s4: return ToSqlInternal(s4, alias);
                case MethodCallStatement s5: return ToSqlInternal(s5, alias);
                case OrderByStatement s6: return ToSqlInternal(s6, alias);
            }

            throw new ArgumentOutOfRangeException($"Missing implementation for \"{statement.GetType().FullName}\"");
        }

        private string ToSqlInternal(BinaryStatement statement, string alias)
        {
            string left = ToSqlInternal(statement.Left, alias);

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

            string right = ToSqlInternal(statement.Right, alias);

            if (statement.Right is BinaryStatement)
            {
                right = "(" + right + " )";
            }

            return $"{left} {op} {right}";
        }

        private string ToSqlInternal(PropertyAccessStatement statement, string alias)
        {
            return $"json_extract({alias}.json_data, '$.{_serialzier.ConvertPropertyNameToCorrectCase(statement.Name)}')";
        }

        private string ToSqlInternal(ConstantStatement statement, string alias)
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

        private string ToSqlInternal(MethodCallStatement statement, string alias)
        {
            throw new NotImplementedException();
        }
        
        private string ToSqlInternal(OrderByStatement statement, string alias)
        {
            string sql = $"ORDER BY ";

            for (int i = 0; i < statement.Elements.Count; i++)
            {
                sql += $"json_extract({alias}.json_data, '$.{_serialzier.ConvertPropertyNameToCorrectCase(statement.Elements[i].PropertyName)}') {(statement.Elements[i].Ascending ? "ASC" : "DESC")}";

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

                return "SELECT " + string.Join(", ", statement.SelectElements.Select(x => ToSqlInternal(x.Statement, statement.Alias) + $" {x.Alias}"));
            }
        }
    }
}
