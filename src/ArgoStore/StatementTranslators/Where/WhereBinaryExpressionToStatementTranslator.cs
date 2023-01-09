using System.Diagnostics;
using System.Linq.Expressions;
using ArgoStore.Statements.Where;

namespace ArgoStore.StatementTranslators.Where;

internal class WhereBinaryExpressionToStatementTranslator : IWhereToStatementTranslator
{
    private static readonly ExpressionType[] _supportedExTypes =
    {
        ExpressionType.Equal,
        ExpressionType.NotEqual,
        ExpressionType.GreaterThan,
        ExpressionType.GreaterThanOrEqual,
        ExpressionType.LessThan,
        ExpressionType.LessThanOrEqual
    };

    public bool CanTranslate(Expression expression)
    {
        return expression is BinaryExpression && _supportedExTypes.Contains(expression.NodeType);
    }

    public WhereStatementBase Translate(Expression expression)
    {
        BinaryExpression be = expression as BinaryExpression;
        Debug.Assert(be != null, "BinaryExpression cast in " + nameof(WhereBinaryExpressionToStatementTranslator));

        
        WhereStatementBase left = WhereToStatementTranslatorStrategies.Translate(be.Left);
        WhereStatementBase right = WhereToStatementTranslatorStrategies.Translate(be.Right);
        ComparisonOperators op;
        
        if (_supportedExTypes.Contains(be.NodeType))
        {
            op = (ComparisonOperators)Enum.Parse(typeof(ComparisonOperators), be.NodeType.ToString());
        }
        else
        {
            throw new NotSupportedException("3f173ca71821");
        }

        return new WhereComparisonStatement(left, op, right);
    }
}