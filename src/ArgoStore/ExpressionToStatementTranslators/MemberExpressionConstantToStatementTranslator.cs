using ArgoStore.Helpers;
using System.Collections;
using System.Linq.Expressions;
using ArgoStore.Statements;

namespace ArgoStore.ExpressionToStatementTranslators;

internal class MemberExpressionConstantToStatementTranslator : IExpressionToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        if (expression is MemberExpression me)
        {
            return IsConstant(me);
        }

        return false;
    }
    
    public Statement Translate(Expression expression)
    {
        UnaryExpression ex = Expression.Convert(expression as MemberExpression, typeof(object));
        object value = Expression.Lambda<Func<object>>(ex).Compile().Invoke();

        Type valueType = value.GetType();

        if (valueType.IsCollectionType())
        {
            Type collectionType = valueType.GetCollectionElementType();
            bool isString = collectionType == typeof(string);
            bool isBool = collectionType == typeof(bool);

            List<string> values = new List<string>();

            foreach (var v in value as IEnumerable)
            {
                values.Add(v.ToString());
            }

            return new ConstantStatement(isString, isBool, values);
        }
        else
        {
            bool isString = valueType == typeof(string) || valueType == typeof(Guid);
            return new ConstantStatement(isString, valueType == typeof(bool),
                value.ToString());
        }

        throw new NotSupportedException($"MemberExpression with member of type \"{(expression as MemberExpression).Member.GetType().FullName}\" isn't supported");
    }
        
    private bool IsConstant(MemberExpression ex)
    {
        if (ex.NodeType == ExpressionType.Constant)
        {
            return true;
        }

        while (ex.Expression is MemberExpression me)
        {
            ex = me;
        }

        return ex.Expression.NodeType == ExpressionType.Constant;
    }
}