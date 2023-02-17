using System.Linq.Expressions;
using ArgoStore.Statements;
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

    public WhereStatementBase Translate(Expression expression, FromAlias alias, ArgoActivity? activity)
    {
        ArgoActivity? ca = activity?.CreateChild("Binary");

        BinaryExpression be = (BinaryExpression) expression;
        
        WhereStatementBase left = WhereToStatementTranslatorStrategies.Translate(be.Left, alias, ca);
        WhereStatementBase right = WhereToStatementTranslatorStrategies.Translate(be.Right, alias, ca);
        ComparisonOperators op;
        
        if (_supportedExTypes.Contains(be.NodeType))
        {
            op = (ComparisonOperators)Enum.Parse(typeof(ComparisonOperators), be.NodeType.ToString());
        }
        else
        {
            throw new NotSupportedException("3f173ca71821");
        }

        WhereComparisonStatement result = new WhereComparisonStatement(left, op, right);

        ca?.Stop();

        return result;
    }
}