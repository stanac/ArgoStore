using System.Diagnostics;
using System.Linq.Expressions;
using ArgoStore.Statements.Where;

namespace ArgoStore.StatementTranslators.Where;

internal class BinaryExpressionToStatementTranslator : IWhereToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        return expression is BinaryExpression;
    }

    public WhereStatementBase Translate(Expression expression)
    {
        BinaryExpression be = expression as BinaryExpression;
        Debug.Assert(be != null, "BinaryExpression cast in " + nameof(BinaryExpressionToStatementTranslator));

        var left = WhereValueStatement.From(be.Left);
        var right = WhereValueStatement.From(be.Right);
        ComparisonOperators op;

        if (be.NodeType is ExpressionType.NotEqual)
        {
            op = ComparisonOperators.NotEqual;
        }
        else
        {
            throw new NotSupportedException();
        }

        return new WhereComparisonStatement(left, op, right);
    }
}