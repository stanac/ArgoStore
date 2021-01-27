using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonDbLite.Expressions
{
    internal class ExpressionData
    {
        public Type EntityType { get; set; }
        public SelectClauseExpressionData Select { get; set; } = SelectClauseExpressionData.CreateWithStar();
        public List<WhereClauseExpressionData> Where { get; set; } = new List<WhereClauseExpressionData>();
    }
}
