using System;

namespace JsonDbLite.Expressions
{
    internal class SqlWhereTranslator
    {
        private readonly IJsonDbLiteSerializer _serializer;

        public SqlWhereTranslator(IJsonDbLiteSerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public string Translate (WhereClauseExpressionData w)
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

        private string Translate (WhereNotExpressionData w)
        {
            return Translate(w.Negate());
        }

        private string Translate(WhereBinaryLogicalExpressionData w)
        {
            return $"{Translate(w.Left)} {(w.IsAnd ? "AND" : "OR")} {Translate(w.Right)}";
        }

        private string Translate(WhereBinaryComparisonExpressionData w)
        {
            return $"{Translate(w.Left)} {w.OperatorString} {Translate(w.Right)}";
        }

        private string Translate(WherePropertyExpressionData w)
        {
            return $"json_extract(json_data, '$.{_serializer.ConvertPropertyNameToCorrectCase(w.Name)}')";
        }

        private string Translate(WhereConstantExpressionData w)
        {
            if (w.IsString)
            {
                return $"'{EscapeParam(w.Value)}'";
            }

            return w.Value;
        }

        private string Translate(WhereMethodCallExpressionData w)
        {
            throw new NotImplementedException();
        }

        private string EscapeParam(string s) => s.Replace("'", "''");
    }
}
