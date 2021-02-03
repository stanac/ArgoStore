using JsonDbLite.Helpers;
using System;

namespace JsonDbLite.Expressions
{
    internal static class ExpressionTransator
    {
        public static string Translate(ExpressionData data, IJsonDbLiteSerializer serializer)
        {
            if (data is null) throw new ArgumentNullException(nameof(data));
            if (serializer is null) throw new ArgumentNullException(nameof(serializer));

            string sql = Translate(data.Select) + $"\nFROM {EntityTableHelper.GetTableName(data.EntityType)}";

            if (data.Where != null)
            {
                if (data.Where.Count == 1)
                {
                    var sqlWhereTranslator = new SqlWhereTranslator(serializer);
                    sql += "\nWHERE " + sqlWhereTranslator.Translate(data.Where[0]);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            return sql;
        }

        private static string Translate(SelectClauseExpressionData select)
        {
            if (select.IsStar)
            {
                return "SELECT json_data";
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
