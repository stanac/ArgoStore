using JsonDbLite.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace JsonDbLite.WhereTranslators
{
    internal static class WhereTranslatorStrategy
    {
        private static IReadOnlyList<IWhereTranslator> _translators = GetAllTranslators();

        public static WhereClauseExpressionData Translate(Expression expression)
        {
            if (expression is null) throw new ArgumentNullException(nameof(expression));

            IWhereTranslator translator = _translators.FirstOrDefault(x => x.CanTranslate(expression));

            if (translator == null)
            {
                throw new InvalidOperationException($"No where translator for expression with node type {expression.NodeType} {expression}");
            }

            return translator.Translate(expression);
        }

        private static IReadOnlyList<IWhereTranslator> GetAllTranslators() =>
            typeof(IWhereTranslator)
            .Assembly
            .GetTypes()
            .Where(x => x.IsClass && !x.IsAbstract && typeof(IWhereTranslator).IsAssignableFrom(x))
            .Select(x => Activator.CreateInstance(x))
            .Cast<IWhereTranslator>()
            .ToList();
    }
}
