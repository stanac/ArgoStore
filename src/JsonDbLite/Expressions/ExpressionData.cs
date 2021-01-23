using System.Collections.Generic;

namespace JsonDbLite.Expressions
{
    internal class ExpressionData
    {
        public SelectClauseExpressionData Select { get; set; } = SelectClauseExpressionData.CreateWithStar();
        public List<WhereClauseExpressionData> Where { get; set; } = new List<WhereClauseExpressionData>();
    }
}
