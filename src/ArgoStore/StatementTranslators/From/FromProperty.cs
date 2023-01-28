using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq;

namespace ArgoStore.StatementTranslators.From;

internal class FromProperty : FromStatementBase
{
    public PropertyInfo Property { get; }
    public string ItemName { get; }


    public FromProperty(QueryModel model)
    {
        ItemName = model.MainFromClause.ItemName;

        if (model.MainFromClause.FromExpression is MemberExpression me)
        {

        }
    }
}