using System.Linq.Expressions;
using System.Reflection;
using ArgoStore.Helpers;
using ArgoStore.Statements;
using Remotion.Linq;

namespace ArgoStore.StatementTranslators.From;

internal class FromProperty : FromStatementBase
{
    public FromAlias Alias { get; }
    public PropertyInfo Property { get; }
    public string ItemName { get; }
    
    public FromProperty(QueryModel model, FromAlias alias)
    {
        Alias = alias;
        ItemName = model.MainFromClause.ItemName;

        if (model.MainFromClause.FromExpression is MemberExpression me && me.Member is PropertyInfo pi)
        {
            Property = pi;
        }
        else
        {
            throw new InvalidOperationException(
                $"Unsupported SubQuery from {model.MainFromClause.FromExpression.Describe()}"
                );
        }
    }
}