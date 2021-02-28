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

            string sql = ToSqlInternal(statement.SelectStatement);

            return sql;
        }

        private string ToSqlInternal(SelectStatement select)
        {
            string sql = SelectElementsToSql(select);

            string from = $@"
FROM {EntityTableHelper.GetTableName(select.TypeFrom)} {select.Alias}";

            if (select.SubQueryStatement != null)
            {
                from = "\nFROM (" + ToSqlInternal(select.SubQueryStatement) + ") " + select.Alias;
            }

            sql += from;

            if (select.WhereStatement != null)
            {
                sql += $@"
WHERE {ToSqlInternal(select.WhereStatement.Statement, select.Alias)}
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
            }

            throw new ArgumentOutOfRangeException($"Missing implementation for \"{statement.GetType().FullName}\"");
        }

        private string ToSqlInternal(BinaryStatement statement, string alias)
        {
            string left = ToSqlInternal(statement.Left, alias);

            if (statement.Left is BinaryStatement)
            {
                left = "(" + left + " )";
            }

            string right = ToSqlInternal(statement.Right, alias);

            if (statement.Right is BinaryStatement)
            {
                right = "(" + right + " )";
            }

            return $"{left} {statement.OperatorString} {right}";
        }

        private string ToSqlInternal(PropertyAccessStatement statement, string alias)
        {
            return $"json_extract({alias}.json_data, '$.{_serialzier.ConvertPropertyNameToCorrectCase(statement.Name)}')";
        }

        private string ToSqlInternal(ConstantStatement statement, string alias)
        {
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

        private string SelectElementsToSql(SelectStatement statement)
        {
            if (statement.SubQueryStatement != null)
            {
                string sql = "SELECT ";

                for (int i = 0; i < statement.SelectElements.Count; i++)
                {
                    if (i != statement.SelectElements.Count - 1)
                    {
                        sql += ", ";
                    }
                }

                throw new NotImplementedException();
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
