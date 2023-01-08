using System.Linq.Expressions;
using ArgoStore.Helpers;
using ArgoStore.Statements.Where;
using ArgoStore.StatementTranslators.Select;

namespace ArgoStore.StatementTranslators.Where;

internal class ConvertNotNullableExpressionToStatementTranslator : IWhereToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        return expression is UnaryExpression ue
               && expression.NodeType == ExpressionType.Convert
               && ue.Operand.Type.IsNullableType();
    }

    public WhereStatementBase Translate(Expression expression)
    {
        UnaryExpression ue = (UnaryExpression)expression;

        return WhereToStatementTranslatorStrategies.Translate(ue.Operand);
    }
}