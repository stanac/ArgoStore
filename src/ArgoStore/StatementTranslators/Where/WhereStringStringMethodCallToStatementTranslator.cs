using System.Linq.Expressions;
using ArgoStore.Statements;
using ArgoStore.Statements.Where;

namespace ArgoStore.StatementTranslators.Where;

internal class WhereStringStringMethodCallToStatementTranslator : IWhereToStatementTranslator
{
    private static readonly string[] _supportedMethods =
    {
        "Trim", "TrimStart", "TrimEnd", "ToUpper", "ToLower"
    };

    public bool CanTranslate(Expression expression)
    {
        return expression is MethodCallExpression mce
               && mce.Method.DeclaringType == typeof(string)
               && _supportedMethods.Contains(mce.Method.Name);
    }

    public WhereStatementBase Translate(Expression expression)
    {
        MethodCallExpression mce = (MethodCallExpression)expression;

        StringTransformTypes type = (StringTransformTypes)Enum.Parse(typeof(StringTransformTypes), mce.Method.Name);

        WhereStatementBase onObject = WhereToStatementTranslatorStrategies.Translate(mce.Object!);
        return new WhereStringTransformStatement(onObject, type);
    }
}