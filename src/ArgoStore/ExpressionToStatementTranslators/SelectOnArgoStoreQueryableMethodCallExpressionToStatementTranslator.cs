using ArgoStore.Helpers;
using System.Linq.Expressions;
using ArgoStore.Statements;

namespace ArgoStore.ExpressionToStatementTranslators;

internal class SelectOnArgoStoreQueryableMethodCallExpressionToStatementTranslator : IExpressionToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        if (expression is MethodCallExpression m)
        {
            return m.Method.Name == "Select" && !(m.Arguments[0] is MethodCallExpression) && TypeHelpers.ImplementsIQueryableGenericInterface(m.Arguments[0].Type);
        }

        return false;
    }

    public Statement Translate(Expression expression)
    {
        MethodCallExpression me = expression as MethodCallExpression;

        Type fromType = GetTargetType(me.Arguments[0]);

        LambdaExpression lambda = GetSelectLambda(me);

        return SelectLambdaTranslator.Translate(lambda, fromType, null, CalledByMethods.Select);
    }

    private Type GetTargetType(Expression expression)
    {
        if (ExpressionHelpers.IsLambda(expression))
        {
            expression = ExpressionHelpers.RemoveQuotes(expression);
        }

        if (expression is LambdaExpression le)
        {
            return le.ReturnType;
        }

        if (expression is ConstantExpression ce)
        {
            if (ce.Type.IsGenericType && TypeHelpers.ImplementsIQueryableGenericInterface(ce.Type))
            {
                return ce.Type.GetGenericArguments()[0];
            }
        }

        if (expression is ParameterExpression pe)
        {
            if (pe.Type.IsGenericType && TypeHelpers.ImplementsIQueryableGenericInterface(pe.Type))
            {
                return pe.Type.GetGenericArguments()[0];
            }
        }

        throw new ArgumentException($"Cannot get target type for select from \"{expression.NodeType}\", \"{expression.Type.FullName}\", \"{expression}\"");
    }

    private LambdaExpression GetSelectLambda(MethodCallExpression ex)
    {
        var e = ex.Arguments[1];

        while (e.NodeType == ExpressionType.Quote)
        {
            e = (e as UnaryExpression).Operand;
        }

        if (e is LambdaExpression le)
        {
            return le;
        }

        throw new InvalidOperationException($"Expected lambda in Select \"{ex.NodeType}\", \"{ex.Type.FullName}\", \"{ex}\"");
    }
}