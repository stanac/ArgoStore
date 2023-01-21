using System.Linq.Expressions;
using ArgoStore.Statements.Where;

namespace ArgoStore.StatementTranslators.Where;

internal class WhereLogicalAndOrStatementTranslator : IWhereToStatementTranslator
{
    private readonly ExpressionType[] _supportedOperators =
    {
        ExpressionType.And,
        ExpressionType.AndAlso,
        ExpressionType.Or,
        ExpressionType.OrElse
    };

    public bool CanTranslate(Expression expression)
    {
        return expression is BinaryExpression && _supportedOperators.Contains(expression.NodeType);
    }

    public WhereStatementBase Translate(Expression expression)
    {
        BinaryExpression be = (BinaryExpression)expression;

        WhereStatementBase left = WhereToStatementTranslatorStrategies.Translate(be.Left);
        WhereStatementBase right = WhereToStatementTranslatorStrategies.Translate(be.Right);

        bool isAnd = be.NodeType is ExpressionType.And or ExpressionType.AndAlso;

        return new WhereLogicalAndOrStatement(isAnd, left, right);
    }
}