using ArgoStore.Helpers;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ArgoStore.ExpressionToStatementTranslators
{
    internal class CountAndAnyExpressionToStatementTranslator : IExpressionToStatementTranslator
    {
        private readonly string[] _supportedMethodNames = new[] { "Count", "LongCount", "Any" };

        public bool CanTranslate(Expression expression)
        {
            if (expression is UnaryExpression ue && ue.NodeType == ExpressionType.Convert && ue.Operand is MethodCallExpression mc)
            {
                return _supportedMethodNames.Contains(mc.Method.Name);
            }

            return false;
        }

        public Statement Translate(Expression expression)
        {
            UnaryExpression ue = expression as UnaryExpression;
            MethodCallExpression methodCall = ue.Operand as MethodCallExpression;

            if (methodCall == null)
                throw new InvalidOperationException("Provided expression in not method call expression");

            bool isAny = methodCall.Method.Name == "Any";
            bool isCount = methodCall.Method.Name == "Count" || methodCall.Method.Name == "LongCount";

            bool isLongCount = methodCall.Method.Name == "LongCount";

            if (!isAny && !isCount) throw new NotSupportedException($"{nameof(CountAndAnyExpressionToStatementTranslator)} doesn't support operand {ue.Operand}");
            
            WhereStatement where = null;

            Type type = GetEntityType(methodCall);

            if (methodCall.Arguments.Count == 2)
            {
                 Statement condition = ExpressionToStatementTranslatorStrategy.Translate(methodCall.Arguments[1]);
                 where = new WhereStatement(condition, type);
            }
            
            if (isAny)
            {
                return CreateSelectExistsStatement(@where, type);
            }
            else
            {
                return CreateSelectCountStatement(@where, type, isLongCount);
            }
        }

        private static Type GetEntityType(MethodCallExpression methodCall)
        {
            ParameterInfo[] methodParams = methodCall.Method.GetParameters();

            Type calledOn = methodParams[0].ParameterType;

            if (TypeHelpers.IsCollectionType(calledOn))
            {
                return TypeHelpers.GetCollectionElementType(calledOn);
            }

            throw new NotSupportedException("Unknown type in CountAndAnyExpressionToStatementTranslator");
        }

        private static Statement CreateSelectExistsStatement(WhereStatement where, Type type)
        {
            if (where == null)
            {
                return new SelectExistsStatement(type);
            }

            return new SelectExistsStatement(type, where);
        }

        private static Statement CreateSelectCountStatement(WhereStatement where, Type type, bool isLongCount)
        {
            if (where != null)
            {
                return new SelectCountStatement(type, where, isLongCount);
            }

            return new SelectCountStatement(type, isLongCount);
        }
    }
}
