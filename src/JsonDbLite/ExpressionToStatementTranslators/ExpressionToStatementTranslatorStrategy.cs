using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace JsonDbLite.ExpressionToStatementTranslators
{
    internal static class ExpressionToStatementTranslatorStrategy
    {
        private static IReadOnlyList<IExpressionToStatementTranslator> _translators = GetAllTranslators();

        public static Statement Translate(Expression expression)
        {
            if (expression is null) throw new ArgumentNullException(nameof(expression));

            IExpressionToStatementTranslator translator = _translators.FirstOrDefault(x => x.CanTranslate(expression));

            if (translator == null)
            {
                throw new InvalidOperationException($"No where translator found for expression with node type \"{expression.NodeType}\", type: \"{expression.GetType().Name}\" and expression \"{expression}\"");
            }

            return translator.Translate(expression).ReduceIfPossible();
        }

        private static IReadOnlyList<IExpressionToStatementTranslator> GetAllTranslators() =>
            typeof(IExpressionToStatementTranslator)
            .Assembly
            .GetTypes()
            .Where(x => x.IsClass && !x.IsAbstract && typeof(IExpressionToStatementTranslator).IsAssignableFrom(x))
            .Select(x => Activator.CreateInstance(x))
            .Cast<IExpressionToStatementTranslator>()
            .ToList();
    }
}
