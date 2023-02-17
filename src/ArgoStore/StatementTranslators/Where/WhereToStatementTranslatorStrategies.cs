using System.Linq.Expressions;
using ArgoStore.Helpers;
using ArgoStore.Statements;
using ArgoStore.Statements.Where;

namespace ArgoStore.StatementTranslators.Where;

internal static class WhereToStatementTranslatorStrategies
{
    private static readonly IReadOnlyList<IWhereToStatementTranslator> _translators = GetTranslators();

    public static WhereStatementBase Translate(Expression expression, FromAlias alias, ArgoActivity? activity)
    {
        ArgoActivity? ca = activity?.CreateChild("WhereStrategies.Translate");
        ArgoActivity? ca2 = ca?.CreateChild("FindTranslator");

        IWhereToStatementTranslator? t = _translators.FirstOrDefault(x => x.CanTranslate(expression));

        ca2?.Stop();

        if (t == null)
        {
            throw new NotSupportedException($"Failed to translate to statement: {expression.Describe()}");
        }

        return t.Translate(expression, alias, ca);
    }

    private static IReadOnlyList<IWhereToStatementTranslator> GetTranslators()
    {
        return typeof(IWhereToStatementTranslator)
            .Assembly
            .GetTypes()
            .Where(IsTypeTranslator)
            .Select(Activator.CreateInstance)
            .Cast<IWhereToStatementTranslator>()
            .ToList();
    }

    private static bool IsTypeTranslator(Type type)
    {
        return type.IsClass
               && !type.IsAbstract
               && typeof(IWhereToStatementTranslator).IsAssignableFrom(type);
    }
}