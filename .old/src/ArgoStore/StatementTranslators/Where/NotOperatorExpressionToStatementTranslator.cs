using System.Linq.Expressions;
using ArgoStore.Statements;
using ArgoStore.Statements.Where;

namespace ArgoStore.StatementTranslators.Where;

internal class NotOperatorExpressionToStatementTranslator : IWhereToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        return expression is UnaryExpression
               && expression.NodeType == ExpressionType.Not;
    }

    public WhereStatementBase Translate(Expression expression, FromAlias alias, ArgoActivity? activity)
    {
        ArgoActivity? ca = activity?.CreateChild("NotOperator");

        UnaryExpression e = (UnaryExpression)expression;

        WhereStatementBase statement = WhereToStatementTranslatorStrategies.Translate(e.Operand, alias, ca);

        WhereNotStatement ret = new WhereNotStatement(statement);

        ca?.Stop();

        return ret;
    }
}