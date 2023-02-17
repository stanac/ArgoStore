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

    public WhereStatementBase Translate(Expression expression, FromAlias alias, ArgoActivity? activity)
    {
        ArgoActivity? ca = activity?.CreateChild("Equals");

        MethodCallExpression mce = (MethodCallExpression)expression;

        if (mce.Arguments.Count == 1)
        {
            WhereStatementBase left = WhereToStatementTranslatorStrategies.Translate(mce.Object!, alias, ca);
            WhereStatementBase right = WhereToStatementTranslatorStrategies.Translate(mce.Arguments[0], alias, ca);

            WhereComparisonStatement r = new WhereComparisonStatement(left, ComparisonOperators.Equal, right);

            ca?.Stop();

            return r;
        }

        if (mce.Arguments.Count == 2 && mce.Arguments[1] is ConstantExpression ce && ce.Value is StringComparison sc)
        {
            bool ignoreCase = !sc.IsCaseSensitive();

            WhereStatementBase left = WhereToStatementTranslatorStrategies.Translate(mce.Object!, alias, ca);
            WhereStatementBase right = WhereToStatementTranslatorStrategies.Translate(mce.Arguments[0], alias, ca);

            if (ignoreCase)
            {
                if (left is WhereParameterStatement wps1 && wps1.Value is string s1)
                {
                    left = new WhereParameterStatement(s1.ToLower(), typeof(string));
                }
                else
                {
                    left = new WhereStringTransformStatement(left, StringTransformTypes.ToLower);
                }

                if (right is WhereParameterStatement wps2 && wps2.Value is string s2)
                {
                    right = new WhereParameterStatement(s2.ToLower(), typeof(string));
                }
                else
                {
                    right = new WhereStringTransformStatement(right, StringTransformTypes.ToLower);
                }
            }

            WhereComparisonStatement r = new WhereComparisonStatement(left, ComparisonOperators.Equal, right);
            
            ca?.Stop();

            return r;
        }
        
        throw new NotSupportedException("2528bb28e858");
    }
    
}