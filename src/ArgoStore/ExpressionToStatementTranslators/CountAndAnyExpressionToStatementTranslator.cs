using ArgoStore.Helpers;
using System;
using System.Linq;
using System.Linq.Expressions;

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

            bool isAny = methodCall.Method.Name == "Any";
            bool isCount = methodCall.Method.Name == "Count" || methodCall.Method.Name == "LongCount";

            bool isLongCount = methodCall.Method.Name == "LongCount";

            if (!isAny && !isCount) throw new NotSupportedException($"{nameof(CountAndAnyExpressionToStatementTranslator)} doesn't support operand {ue.Operand}");

            Type resultType = isAny ? typeof(bool) : typeof(int);

            if (methodCall.Arguments.Count == 1) // no lambda inside method call
            {
                var methodParams = methodCall.Method.GetParameters();

                Type calledOn = methodParams[0].ParameterType;

                if (TypeHelpers.IsCollectionType(calledOn))
                {
                    Type type = TypeHelpers.GetCollectionElementType(calledOn);

                    if (isAny)
                    {
                        return new SelectExistsStatement(type);
                    }
                    else
                    {
                        return new SelectCountStatement(type, isLongCount);
                    }
                }
            }

            throw new NotImplementedException();
        }

    }
}
