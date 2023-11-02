using System;
using System.Linq.Expressions;

namespace ArgoStore.TestsCommon.TestHelpers;

public static class FindExpressionVisitor
{
    public static Expression? Find(Expression expression, Func<Expression, bool> condition)
    {
        Visitor visitor = new Visitor(condition);
        visitor.Visit(expression);
        return visitor.FoundExpression;
    }
}

file class Visitor : ExpressionVisitor
{
    private readonly Func<Expression, bool> _condition;
    public Expression? FoundExpression { get; private set; }

    public Visitor(Func<Expression, bool> condition)
    {
        _condition = condition;
    }

    public override Expression Visit(Expression node)
    {
        if (_condition(node))
        {
            FoundExpression = node;
        }
        return base.Visit(node);
    }
}