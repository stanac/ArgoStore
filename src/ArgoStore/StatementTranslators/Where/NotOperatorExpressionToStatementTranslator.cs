using System.Linq.Expressions;
using ArgoStore.Statements.Where;

namespace ArgoStore.StatementTranslators.Where;

internal class NotOperatorExpressionToStatementTranslator : IWhereToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        return expression is UnaryExpression
               && expression.NodeType == ExpressionType.Not;
    }

    public WhereStatementBase Translate(Expression expression)
    {
        UnaryExpression e = (UnaryExpression)expression;

        WhereStatementBase statement = WhereToStatementTranslatorStrategies.Translate(e.Operand);

        return new WhereNotStatement(statement);
    }
}