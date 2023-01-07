using System.Diagnostics;
using System.Linq.Expressions;
using ArgoStore.Statements.Where;

namespace ArgoStore.StatementTranslators.Where;

internal class WhereBinaryExpressionToStatementTranslator : IWhereToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        return expression is BinaryExpression;
    }

    public WhereStatementBase Translate(Expression expression)
    {
        BinaryExpression be = expression as BinaryExpression;
        Debug.Assert(be != null, "BinaryExpression cast in " + nameof(WhereBinaryExpressionToStatementTranslator));

        
        WhereStatementBase left = WhereToStatementTranslatorStrategies.Translate(be.Left);
        WhereStatementBase right = WhereToStatementTranslatorStrategies.Translate(be.Right);
        ComparisonOperators op;

        if (be.NodeType is ExpressionType.NotEqual)
        {
            op = ComparisonOperators.NotEqual;
        }
        else if (be.NodeType is ExpressionType.Equal)
        {
            op = ComparisonOperators.Equal;
        }
        else
        {
            throw new NotSupportedException();
        }

        return new WhereComparisonStatement(left, op, right);
    }
}