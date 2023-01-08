using System.Linq.Expressions;
using ArgoStore.Helpers;
using ArgoStore.Statements;
using ArgoStore.Statements.Where;

namespace ArgoStore.StatementTranslators.Where;

internal class WhereObjectEqualsMethodCallToStatementTranslator : IWhereToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        if (expression is MethodCallExpression e && e.Method.Name == "Equals")
        {
            return true;
        }

        return false;
    }

    public WhereStatementBase Translate(Expression expression)
    {
        MethodCallExpression mce = (MethodCallExpression)expression;

        if (mce.Arguments.Count == 1)
        {
            WhereStatementBase left = WhereToStatementTranslatorStrategies.Translate(mce.Object);
            WhereStatementBase right = WhereToStatementTranslatorStrategies.Translate(mce.Arguments[0]);

            return new WhereComparisonStatement(left, ComparisonOperators.Equal, right);
        }

        if (mce.Arguments.Count == 2 && mce.Arguments[1] is ConstantExpression ce && ce.Value is StringComparison sc)
        {
            bool ignoreCase = !sc.IsCaseSensitive();

            WhereStatementBase left = WhereToStatementTranslatorStrategies.Translate(mce.Object);
            WhereStatementBase right = WhereToStatementTranslatorStrategies.Translate(mce.Arguments[0]);

            if (ignoreCase)
            {
                if (left is WhereParameterStatement wps1 && wps1.Value is string s1)
                {
                    left = new WhereParameterStatement(s1.ToLower());
                }
                else
                {
                    left = new WhereStringTransformStatement(left, StringTransformTypes.ToLower);
                }

                if (right is WhereParameterStatement wps2 && wps2.Value is string s2)
                {
                    right = new WhereParameterStatement(s2.ToLower());
                }
                else
                {
                    right = new WhereStringTransformStatement(right, StringTransformTypes.ToLower);
                }
            }

            return new WhereComparisonStatement(left, ComparisonOperators.Equal, right);
        }
        
        throw new NotSupportedException("2528bb28e858");
    }
    
}