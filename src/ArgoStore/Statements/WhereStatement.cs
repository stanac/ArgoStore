using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace ArgoStore.Statements;

internal class WhereStatement
{
    private WhereComparisonStatement ComparisonStatement { get; }
    private LogicalAndOrStatement LogicalAndOrStatement { get; }

    public WhereStatement(WhereClause clause, QueryModel model)
    {
        if (clause.Predicate is BinaryExpression be)
        {
            var left = ValueStatement.From(be.Left);
            var right = ValueStatement.From(be.Right);
            ComparisonOperators op;

            if (be.NodeType is ExpressionType.NotEqual)
            {
                op = ComparisonOperators.NotEqual;
            }
            else
            {
                throw new NotSupportedException();
            }

            ComparisonStatement = new WhereComparisonStatement(left, op, right);
        }
    }
}