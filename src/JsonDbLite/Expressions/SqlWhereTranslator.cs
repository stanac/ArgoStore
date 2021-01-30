using System;

namespace JsonDbLite.Expressions
{
    internal static class SqlWhereTranslator
    {
        public static string Translate (WhereClauseExpressionData w)
        {
            if (w is null) throw new ArgumentNullException(nameof(w));

            switch (w)
            {
                case WhereNotExpressionData w1: return Translate(w1);
                case WhereBinaryLogicalExpressionData w2: return Translate(w2);
                case WhereBinaryComparisonExpressionData w3: return Translate(w3);
                case WherePropertyExpressionData w4: return Translate(w4);
                case WhereConstantExpressionData w5: return Translate(w5);
                case WhereMethodCallExpressionData w6: return Translate(w6);
            }

            throw new NotSupportedException($"{nameof(WhereClauseExpressionData)} Translate not supported for {w.GetType().Name}");
        }

        private static string Translate (WhereNotExpressionData w)
        {
            return Translate(w.Negate());
        }

        private static string Translate(WhereBinaryLogicalExpressionData w)
        {
            return $"{Translate(w.Left)} {(w.IsAnd ? "AND" : "OR")} {Translate(w.Right)}";
        }

        private static string Translate(WhereBinaryComparisonExpressionData w)
        {
            return $"{Translate(w.Left)} {w.OperatorString} {Translate(w.Right)}";
        }

        private static string Translate(WherePropertyExpressionData w)
        {
            return $"json_extract(json_data, $.{w.Name})";
        }

        private static string Translate(WhereConstantExpressionData w)
        {
            if (w.IsString)
            {
                return $"'{EscapeParam(w.Value)}'";
            }

            return w.Value;
        }

        private static string Translate(WhereMethodCallExpressionData w)
        {
            throw new NotImplementedException();
        }

        private static string EscapeParam(string s) => s.Replace("'", "''");
    }
}
