using System.Linq.Expressions;
using ArgoStore.Helpers;
using ArgoStore.Statements.Where;

namespace ArgoStore.StatementTranslators.Where;

internal static class WhereToStatementTranslatorStrategies
{
    private static readonly IReadOnlyList<IWhereToStatementTranslator> _translators = GetTranslators();

    public static WhereStatementBase Translate(Expression expression)
    {
        IWhereToStatementTranslator? t = _translators.FirstOrDefault(x => x.CanTranslate(expression));

        if (t == null)
        {
            throw new NotSupportedException($"Failed to translate to statement: {expression.Describe()}");
        }

        return t.Translate(expression);
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