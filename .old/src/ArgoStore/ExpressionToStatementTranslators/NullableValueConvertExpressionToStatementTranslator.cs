using System.Linq.Expressions;
using ArgoStore.Statements;

namespace ArgoStore.ExpressionToStatementTranslators;

internal class NullableValueConvertExpressionToStatementTranslator : IExpressionToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        if (expression is UnaryExpression ex && ex.NodeType == ExpressionType.Convert
            && ex.Type.IsGenericType && ex.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return true;
        }

        return false;
    }

    public Statement Translate(Expression expression)
    {
        UnaryExpression ex = (UnaryExpression)expression;

        return ExpressionToStatementTranslatorStrategy.Translate(ex.Operand);
    }
}