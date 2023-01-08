using System.Linq.Expressions;
using System.Reflection;
using ArgoStore.Helpers;
using ArgoStore.Statements.Select;

namespace ArgoStore.StatementTranslators.Select;

internal class AnonymousObjectExpressionToStatementTranslator : ISelectStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        return expression is NewExpression ne && ne.Type.IsAnonymousType();
    }

    public SelectStatementBase Translate(Expression expression)
    {
        NewExpression ne = (NewExpression)expression;

        List<SelectPropertyStatement> selectStatementElements = new List<SelectPropertyStatement>();

        for (var i = 0; i < ne.Arguments.Count; i++)
        {
            Expression e = ne.Arguments[i];
            MemberInfo m = ne.Members[i];

            SelectStatementBase selectElement = SelectToStatementTranslatorStrategies.Translate(e);

            if (selectElement is SelectPropertyStatement sps)
            {
                if (sps.Name != m.Name)
                {
                    sps.ResultName = m.Name;
                }

                selectStatementElements.Add(sps);
            }
            else
            {
                throw new NotSupportedException("924812c5f715");
            }
        }

        return new SelectAnonymousType(ne.Type, selectStatementElements);
    }
}