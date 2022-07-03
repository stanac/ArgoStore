using ArgoStore.Helpers;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
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
        MemberExpression me = (MemberExpression) expression;

        object value;

        // don't throw exception when calling Nullable<>.Value
        if (IsAccessOnNullable(me))
        {
            if (me.Member.Name == "Value")
            {
                value = GetValueFromNullable(me);
            }
            else if (me.Member.Name == "HasValue")
            {
                return GetNullableHasValueStatement(me);
            }
            else
            {
                throw new NotSupportedException($"MemberExpression, access on Nullable<> {me.Member.Name} not supported.");
            }
        }
        else
        {
            UnaryExpression ex = Expression.Convert(me, typeof(object));

            Func<object> compiled = Expression.Lambda<Func<object>>(ex).Compile();
            value = compiled.Invoke();
        }

        if (value is null)
        {
            return new ConstantStatement(false, false, null as string);
        }

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

    private static Statement GetNullableHasValueStatement(MemberExpression me)
    {
        UnaryExpression ex = Expression.Convert(me, typeof(object));

        Func<object> compiled = Expression.Lambda<Func<object>>(ex).Compile();
        bool value = (bool)compiled.Invoke();
        string sValue = (value ? 1 : 0).ToString();

        return new BinaryComparisonStatement(
            new ConstantStatement(false, false, sValue),
            new ConstantStatement(false, false, "1"),
            BinaryComparisonStatement.Operators.Equal
        );
    }

    private static object GetValueFromNullable(MemberExpression me)
    {
        // TODO: optimize without catch
        try
        {
            UnaryExpression ex = Expression.Convert(me, typeof(object));

            Func<object> compiled = Expression.Lambda<Func<object>>(ex).Compile();
            return compiled.Invoke();
        }
        catch
        {
            return null;
        }
    }

    private static bool IsConstant(MemberExpression ex)
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

    private static bool IsAccessOnNullable(MemberExpression me)
    {
        return me.Member.ReflectedType != null
               && me.Member.ReflectedType.IsGenericType
               && me.Member.ReflectedType.GetGenericTypeDefinition() == typeof(Nullable<>);
    }
}