using System.Linq.Expressions;
using System.Reflection;
using ArgoStore.Helpers;
using Remotion.Linq;

namespace ArgoStore.StatementTranslators.From;

internal class FromProperty : FromStatementBase
{
    public int ParentAliasCounter { get; }
    public int CurrentAliasCounter { get; }
    public PropertyInfo Property { get; }
    public string ItemName { get; }
    
    public FromProperty(QueryModel model, int parentAliasCounter, int currentAliasCounter)
    {
        ParentAliasCounter = parentAliasCounter;
        CurrentAliasCounter = currentAliasCounter;
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