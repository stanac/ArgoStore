using ArgoStore.StatementTranslators.Where;
using System.Linq.Expressions;
using ArgoStore.Statements.Select;

namespace ArgoStore.StatementTranslators.Select;

internal class SelectToStatementTranslatorStrategies
{
    private static readonly IReadOnlyList<ISelectStatementTranslator> _translators = GetTranslators();

    public static SelectStatementBase Translate(Expression expression)
    {
        ISelectStatementTranslator t = _translators.FirstOrDefault(x => x.CanTranslate(expression));

        if (t == null)
        {
            throw new NotSupportedException($"Failed to translate to statement: {expression.Describe()}");
        }

        return t.Translate(expression);
    }

    private static IReadOnlyList<ISelectStatementTranslator> GetTranslators()
    {
        return typeof(ISelectStatementTranslator)
            .Assembly
            .GetTypes()
            .Where(IsTypeTranslator)
            .Select(Activator.CreateInstance)
            .Cast<ISelectStatementTranslator>()
            .ToList();
    }

    private static bool IsTypeTranslator(Type type)
    {
        return type.IsClass
               && !type.IsAbstract
               && typeof(ISelectStatementTranslator).IsAssignableFrom(type);
    }
}