using System.Linq.Expressions;
using ArgoStore.Statements;
using ArgoStore.Statements.Where;

namespace ArgoStore.StatementTranslators.Where;

internal class WhereConvertNullableExpressionToStatementTranslator : IWhereToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        return expression is UnaryExpression ue && expression.NodeType == ExpressionType.Convert;
    }

    public WhereStatementBase Translate(Expression expression, FromAlias alias)
    {
        UnaryExpression ue = (UnaryExpression) expression;

        return WhereToStatementTranslatorStrategies.Translate(ue.Operand, alias);
    }
}