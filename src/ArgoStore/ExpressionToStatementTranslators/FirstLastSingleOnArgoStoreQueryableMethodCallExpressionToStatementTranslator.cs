using ArgoStore.Helpers;
using System.Linq.Expressions;
using ArgoStore.Statements;

namespace ArgoStore.ExpressionToStatementTranslators;

internal class FirstLastSingleOnArgoStoreQueryableMethodCallExpressionToStatementTranslator : IExpressionToStatementTranslator
{
    private static readonly string[] _supportedMethodNames = new[] { "First", "FirstOrDefault", "Single", "SingleOrDefault", "Last", "LastOrDefault" };

    public bool CanTranslate(Expression expression)
    {
        bool result = false;

        if (expression is MethodCallExpression m)
        {
            result = _supportedMethodNames.Contains(m.Method.Name)
                     && !(ExpressionHelpers.IsWhereCall(m.Arguments[0]))
                     && !(ExpressionHelpers.IsSelectCall(m.Arguments[0]))
                     && TypeHelpers.ImplementsIQueryableGenericInterface(m.Arguments[0].Type);
        }

        return result;
    }

    public Statement Translate(Expression expression)
    {
        MethodCallExpression me = expression as MethodCallExpression;

        CalledByMethods method = (CalledByMethods)Enum.Parse(typeof(CalledByMethods), me.Method.Name);

        Type targetType = GetTargetType(me.Arguments[0]);

        WhereStatement whereStatement = null;

        if (me.Arguments.Count == 2)
        {
            Statement whereCondition = ExpressionToStatementTranslatorStrategy.Translate(me.Arguments[1]);
            whereStatement = new WhereStatement(whereCondition, targetType);
        }

        List<SelectStatementElement> selectElements = new List<SelectStatementElement>
        {
            SelectStatementElement.CreateWithStar(targetType)
        };

        if (whereStatement != null)
        {
            return new SelectStatement(whereStatement, targetType, targetType, selectElements, 1, method);
        }

        return new SelectStatement(targetType, targetType, selectElements, 1, method);
    }

    private Type GetTargetType(Expression expression)
    {
        if (TypeHelpers.ImplementsIQueryableGenericInterface(expression.Type))
        {
            return expression.Type.GetGenericArguments()[0];
        }

        throw new ArgumentException($"Cannot get target type for select from \"{expression.NodeType}\", \"{expression.Type.FullName}\", \"{expression}\"");
    }
}