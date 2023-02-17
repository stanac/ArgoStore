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

    public WhereStatementBase Translate(Expression expression, FromAlias alias, ArgoActivity? activity)
    {
        ArgoActivity? ca = activity?.CreateChild("StringStringMethod");

        MethodCallExpression mce = (MethodCallExpression)expression;

        StringTransformTypes type = (StringTransformTypes)Enum.Parse(typeof(StringTransformTypes), mce.Method.Name);

        WhereStatementBase onObject = WhereToStatementTranslatorStrategies.Translate(mce.Object!, alias, ca);
        WhereStringTransformStatement r = new WhereStringTransformStatement(onObject, type);

        ca?.Stop();
        return r;
    }
}