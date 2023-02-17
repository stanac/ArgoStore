using System.Linq.Expressions;
using System.Reflection;
using ArgoStore.Helpers;
using ArgoStore.Statements;
using ArgoStore.Statements.Where;

namespace ArgoStore.StatementTranslators.Where;

internal class PropertyExpressionToStatementTranslator : IWhereToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        return expression is MemberExpression;
    }

    public WhereStatementBase Translate(Expression expression, FromAlias alias, ArgoActivity? activity)
    {
        var ca = activity?.CreateChild("Property");

        MemberExpression me = (MemberExpression)expression;

        if (me.Member is PropertyInfo pi)
        {
            if (pi.Name == "Length" && pi.DeclaringType == typeof(string))
            {
                WhereStatementBase prop = WhereToStatementTranslatorStrategies.Translate(me.Expression!, alias, ca);

                return new WhereStringLengthStatement(prop);
            }

            if (me.Expression is MemberExpression)
            {
                WhereStatementBase parent = WhereToStatementTranslatorStrategies.Translate(me.Expression, alias, ca);

                if (parent is WherePropertyStatement wps)
                {
                    return wps.AddChild(pi.Name, pi.PropertyType);
                }

                throw new NotSupportedException("Unexpected parent expression: " + me.Expression.Describe());
            }

            WherePropertyStatement result = new WherePropertyStatement(pi.Name, pi.PropertyType, alias.CurrentAliasName);

            ca?.Stop();

            return result;
        }

        throw new NotSupportedException("f1dde3f265cb");
    }
}