using System.Linq.Expressions;
using ArgoStore.Helpers;
using ArgoStore.Statements.Select;

namespace ArgoStore.StatementTranslators.Select;

internal class ConvertNotNullableExpressionToStatementTranslator : ISelectStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        return expression is UnaryExpression ue
               && expression.NodeType == ExpressionType.Convert
               && ue.Operand.Type.IsNullableType();
    }

    public SelectStatementBase Translate(Expression expression)
    {
        UnaryExpression ue = (UnaryExpression)expression;

        return SelectToStatementTranslatorStrategies.Translate(ue.Operand);
    }
}