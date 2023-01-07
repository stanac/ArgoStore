using System.Linq.Expressions;
using ArgoStore.Statements.Where;

namespace ArgoStore.StatementTranslators.Where;

internal class WhereStringContainsMethodCallStatementTranslator : IWhereToStatementTranslator
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

    public WhereStatementBase Translate(Expression expression)
    {
        MethodCallExpression e = (MethodCallExpression)expression;

        if (e.Arguments.Count == 1)
        {
            WhereStatementBase left = WhereToStatementTranslatorStrategies.Translate(e.Object);
            WhereStatementBase right = WhereToStatementTranslatorStrategies.Translate(e.Arguments[0]);

            StringMethods method = (StringMethods)Enum.Parse(typeof(StringMethods), e.Method.Name);

            return new WhereStringContainsMethodCallStatement(left, right, method, false);
        }

        if (e.Arguments.Count == 2 && e.Arguments[1] is ConstantExpression ce && ce.Value is StringComparison sc)
        {
            WhereStatementBase left = WhereToStatementTranslatorStrategies.Translate(e.Object);
            WhereStatementBase right = WhereToStatementTranslatorStrategies.Translate(e.Arguments[0]);

            StringMethods method = (StringMethods)Enum.Parse(typeof(StringMethods), e.Method.Name);

            bool ignoreCase = !sc.IsCaseSensitive();

            return new WhereStringContainsMethodCallStatement(left, right, method, ignoreCase);
        }

        throw new NotSupportedException();
    }
}