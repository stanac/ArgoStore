using ArgoStore.Helpers;
using System;

namespace ArgoStore
{
    internal class StatementToSqlTranslator
    {
        private readonly IArgoStoreSerializer _serialzier;

        public StatementToSqlTranslator(IArgoStoreSerializer serialzier)
        {
            _serialzier = serialzier ?? throw new ArgumentNullException(nameof(serialzier));
        }

        public string ToSql(TopStatement statement)
        {
            if (statement is null) throw new ArgumentNullException(nameof(statement));

            string elementsToSelect = "json_data";

            if (!statement.SelectStatement.SelectStar)
            {
                throw new NotImplementedException();
            }

            string sql = $@"SELECT {elementsToSelect}
FROM {EntityTableHelper.GetTableName(statement.TargetType)}";

            if (statement.SelectStatement.WhereStatement != null)
            {
                sql += $@"
WHERE {ToSqlInternal(statement.SelectStatement.WhereStatement.Statement)}
";
            }

            return sql;

        }

        private string ToSqlInternal(Statement statement)
        {
            if (statement is null) throw new ArgumentNullException(nameof(statement));

            switch (statement)
            {
                case BinaryStatement s1: return ToSqlInternal(s1);
                case PropertyAccessStatement s3: return ToSqlInternal(s3);
                case ConstantStatement s4: return ToSqlInternal(s4);
                case MethodCallStatement s5: return ToSqlInternal(s5);
            }

            throw new ArgumentOutOfRangeException($"Missing implementation for \"{statement.GetType().FullName}\"");
        }

        private string ToSqlInternal(BinaryStatement statement)
        {
            string left = ToSqlInternal(statement.Left);

            if (statement.Left is BinaryStatement)
            {
                left = "(" + left + " )";
            }

            string right = ToSqlInternal(statement.Right);

            if (statement.Right is BinaryStatement)
            {
                right = "(" + right + " )";
            }

            return $"{left} {statement.OperatorString} {right}";
        }

        private string ToSqlInternal(PropertyAccessStatement statement)
        {
            return $"json_extract(json_data, '$.{_serialzier.ConvertPropertyNameToCorrectCase(statement.Name)}')";
        }

        private string ToSqlInternal(ConstantStatement statement)
        {
            if (statement.IsString)
            {
                return "'" + statement.Value.Replace("'", "''") + "'";
            }

            return statement.Value;
        }

        private string ToSqlInternal(MethodCallStatement statement)
        {
            throw new NotImplementedException();
        }
    }
}
