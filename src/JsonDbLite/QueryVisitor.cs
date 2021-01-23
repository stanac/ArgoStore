using JsonDbLite.Expressions;
using System;
using System.Linq.Expressions;

namespace JsonDbLite
{
    internal class QueryVisitor : ExpressionVisitor
    {
        ExpressionData _data = new ExpressionData();

        private const string MethodWhere = "Where";
        private const string MethodSelect = "Select";


        public string Translate(Expression expression)
        {
            if (expression is null) throw new ArgumentNullException(nameof(expression));

            Visit(expression);

            return ExpressionTransator.Translate(_data);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            
            throw new NotSupportedException($"The method \"{node.Method.Name}\" is not supported"));
        }
    }
}
