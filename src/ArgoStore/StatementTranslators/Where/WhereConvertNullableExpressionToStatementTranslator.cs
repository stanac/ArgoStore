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

    public WhereStatementBase Translate(Expression expression, FromAlias alias, ArgoActivity? activity)
    {
        ArgoActivity? ca = activity?.CreateChild("Nullable");

        UnaryExpression ue = (UnaryExpression) expression;

        WhereStatementBase result = WhereToStatementTranslatorStrategies.Translate(ue.Operand, alias, ca);

        ca?.Stop();

        return result;
    }
}