using ArgoStore.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ArgoStore
{
    internal class ArgoStoreQueryProvider : IQueryProvider
    {
        private readonly Configuration _config;
        private readonly EntityTableHelper _entityTableHelper;
        private readonly IDbAccess _dbAccess;

        public ArgoStoreQueryProvider(Configuration config)
            : this (config, new DbAccess(config?.ConnectionString))
        {
        }

        public ArgoStoreQueryProvider(Configuration config, IDbAccess dbAccess)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _entityTableHelper = new EntityTableHelper(config);
            _dbAccess = dbAccess ?? throw new ArgumentNullException(nameof(dbAccess));
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (expression is null) throw new ArgumentNullException(nameof(expression));

            Type resultType = expression.Type;

            if (TypeHelpers.IsCollectionType(resultType))
            {
                resultType = TypeHelpers.GetCollectionElementType(resultType);
            }

            Type queryType = typeof(ArgoStoreQueryable<>).MakeGenericType(resultType);

            return Activator.CreateInstance(queryType, this, expression) as IQueryable<TElement>;
        }

        public TResult Execute<TResult>(Expression expression)
        {
            object result = Execute(expression);

            Type resultType = result.GetType();

            if (TypeHelpers.IsCollectionType(resultType))
            {
                IEnumerable resultCollection = result as IEnumerable;

                var list = resultCollection.Cast<object>().ToList();

                if (list.Any()) return (TResult)list.First();

                GuardEmptyCallectionLinqCall(expression);

                return default(TResult);
            }

            throw new NotSupportedException();
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotSupportedException("Non generic method CreateQuery isn't supported");
        }

        public object Execute(Expression expression)
        {
            if (expression is null) throw new ArgumentNullException(nameof(expression));

            Statement statement = ExpressionToStatementTranslators.ExpressionToStatementTranslatorStrategy.Translate(expression);

            TopStatement ts = TopStatement.Create(statement);

            StatementToSqlTranslator translator = new StatementToSqlTranslator(_config.Serializer);

            string sql = translator.ToSql(ts);

            _entityTableHelper.EnsureEntityTableExists(ts.TargetType);

            if (ts.SelectStatement.SelectElements.Count == 1)
            {
                if (ts.SelectStatement.SelectElements[0].SelectsJson)
                {
                    IEnumerable<string> result = _dbAccess.QueryJsonField(sql);

                    return result.Select(x => _config.Serializer.Deserialize(x, ts.TargetType));
                }
                else
                {
                    return _dbAccess.QueryField(sql);
                }
            }


            throw new NotImplementedException();
        }

        private static void GuardEmptyCallectionLinqCall(Expression e)
        {
            if (e is MethodCallExpression me && (me.Method.Name == "First" || me.Method.Name == "Single" || me.Method.Name == "Last"))
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }
        }
    }
}
