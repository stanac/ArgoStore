using System.Linq.Expressions;
using ArgoStore.Helpers;
using ArgoStore.Statements;
using ArgoStore.Statements.Where;

namespace ArgoStore.StatementTranslators.Where;

internal class ConvertNotNullableExpressionToStatementTranslator : IWhereToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        return expression is UnaryExpression ue
               && expression.NodeType == ExpressionType.Convert
               && ue.Operand.Type.IsNullableType(out _);
    }

    public WhereStatementBase Translate(Expression expression, FromAlias alias)
    {
        UnaryExpression ue = (UnaryExpression)expression;

        return WhereToStatementTranslatorStrategies.Translate(ue.Operand, alias);
    }
}