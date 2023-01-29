using System.Linq.Expressions;
using System.Reflection;
using ArgoStore.Helpers;
using ArgoStore.Statements;
using Remotion.Linq;

namespace ArgoStore.StatementTranslators.From;

internal class FromProperty : FromStatementBase
{
    public FromAlias Alias { get; }
    public string PropertyName { get; }
    public string ItemName { get; }
    
    public FromProperty(QueryModel model, FromAlias alias)
    {
        Alias = alias;
        ItemName = model.MainFromClause.ItemName;

        if (model.MainFromClause.FromExpression is MemberExpression me && me.Member is PropertyInfo pi)
        {
            if (me.Expression is MemberExpression)
            {
                PropertyName = ExtractPropertyName(me);
            }
            else
            {
                PropertyName = pi.Name;
            }
        }
        else
        {
            throw new InvalidOperationException(
                $"Unsupported SubQuery from {model.MainFromClause.FromExpression.Describe()}"
                );
        }
    }

    private static string ExtractPropertyName(MemberExpression me)
    {
        List<string> path = new();

        Expression? e = me;

        while (e is MemberExpression m2 && m2.Member is PropertyInfo pi)
        {
            path.Add(pi.Name);
            e = m2.Expression;
        }

        path.Reverse();

        return string.Join(".", path);
    }
}