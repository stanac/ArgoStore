using ArgoStore.Helpers;
using System.Linq.Expressions;
using ArgoStore.Statements;

namespace ArgoStore.ExpressionToStatementTranslators;

internal class NewArrayInitExpressionToStatementTranslator : IExpressionToStatementTranslator
{
    public bool CanTranslate(Expression expression) => expression is NewArrayExpression;

    public Statement Translate(Expression expression)
    {
        var exp = expression as NewArrayExpression;

        List<ConstantStatement> constants = exp.Expressions.Select(x => ExpressionToStatementTranslatorStrategy.Translate(x) as ConstantStatement).ToList();
        List<string> translated = constants.Select(x => x.Value).ToList();

        Type elementType = TypeHelpers.GetCollectionElementType(exp.Type);

        bool isString = elementType == typeof(string) || elementType == typeof(char) || elementType.IsEnum;
        bool isBool = elementType == typeof(bool);

        return new ConstantStatement(isString, isBool, translated);
    }
}