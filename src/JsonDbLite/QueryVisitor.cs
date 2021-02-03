using JsonDbLite.Expressions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace JsonDbLite
{
    internal class QueryVisitor : ExpressionVisitor
    {
        internal ExpressionData ExpData { get; } = new ExpressionData();
        
        private const string MethodWhere = "Where";
        private const string MethodSelect = "Select";

        private const string MethodIsNullOrEmpty = "IsNullOrEmpty";
        private const string MethodIsNullOrWhiteSpace = "IsNullOrWhiteSpace";

        public string Translate(Expression expression, IJsonDbLiteSerializer serializer)
        {
            if (expression is null) throw new ArgumentNullException(nameof(expression));

            Visit(expression);

            return ExpressionTransator.Translate(ExpData, serializer);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable))
            {
                if (node.Method.Name == MethodWhere)
                {
                    var type = node.Arguments[0] as ConstantExpression;
                    ExpData.EntityType = type.Type.GenericTypeArguments[0];

                    LambdaExpression lambda = (LambdaExpression)RemoveQuotes(node.Arguments[1]);
                    WhereClauseExpressionData where = WhereTranslators.WhereTranslatorStrategy.Translate(lambda);
                    ExpData.Where.Add(where);

                    return node;
                }
            }
    
            throw new NotSupportedException($"The method \"{node.Method.Name}\" is not supported");
        }

        private static Expression RemoveQuotes(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Quote)
            {
                expression = (expression as UnaryExpression).Operand;
            }
            return expression;
        }
    }
}
