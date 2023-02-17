using System.Linq.Expressions;
using ArgoStore.Helpers;
using ArgoStore.Statements;
using ArgoStore.Statements.Where;

namespace ArgoStore.StatementTranslators.Where;

internal class WhereStringBoolMethodCallStatementTranslator : IWhereToStatementTranslator
{
    private static readonly IReadOnlyList<string> _supportedMethods = new List<string>
    {
        "Contains", "StartsWith", "EndsWith"
    };

    public bool CanTranslate(Expression expression)
    {
        return expression is MethodCallExpression mce
               && mce.Object?.Type == typeof(string)
               && _supportedMethods.Contains(mce.Method.Name);
    }

    public WhereStatementBase Translate(Expression expression, FromAlias alias, ArgoActivity? activity)
    {
        ArgoActivity? ca = activity?.CreateChild("StringBoolMethod");

        MethodCallExpression e = (MethodCallExpression)expression;

        if (e.Arguments.Count == 1)
        {
            WhereStatementBase left = WhereToStatementTranslatorStrategies.Translate(e.Object!, alias, ca);
            WhereStatementBase right = WhereToStatementTranslatorStrategies.Translate(e.Arguments[0], alias, ca);

            StringMethods method = (StringMethods)Enum.Parse(typeof(StringMethods), e.Method.Name);

            WhereStringContainsMethodCallStatement r = new WhereStringContainsMethodCallStatement(left, right, method, false);
            ca?.Stop();
            return r;
        }

        if (e.Arguments.Count == 2 && e.Arguments[1] is ConstantExpression ce && ce.Value is StringComparison sc)
        {
            WhereStatementBase left = WhereToStatementTranslatorStrategies.Translate(e.Object!, alias, ca);
            WhereStatementBase right = WhereToStatementTranslatorStrategies.Translate(e.Arguments[0], alias, ca);

            StringMethods method = (StringMethods)Enum.Parse(typeof(StringMethods), e.Method.Name);

            bool ignoreCase = !sc.IsCaseSensitive();

            WhereStringContainsMethodCallStatement r = new WhereStringContainsMethodCallStatement(left, right, method, ignoreCase);
            ca?.Stop();
            return r;
        }

        throw new NotSupportedException("a26f50f816da");
    }
}