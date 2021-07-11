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
        private readonly Func<IStatementToSqlTranslator> _statementToSqlTranslatorFactory;

        public ArgoStoreQueryProvider(Configuration config)
            : this (config, new DbAccess(config?.ConnectionString), () => new StatementToSqlTranslator(config.Serializer))
        {
        }

        public ArgoStoreQueryProvider(Configuration config, IDbAccess dbAccess, Func<IStatementToSqlTranslator> statementToSqlTranslatorFactory)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _entityTableHelper = new EntityTableHelper(config);
            _dbAccess = dbAccess ?? throw new ArgumentNullException(nameof(dbAccess));
            _statementToSqlTranslatorFactory = statementToSqlTranslatorFactory ?? throw new ArgumentNullException(nameof(statementToSqlTranslatorFactory));
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

            // ReSharper disable once AssignNullToNotNullAttribute
            return Activator.CreateInstance(queryType, this, expression) as IQueryable<TElement>;
        }

        public TResult Execute<TResult>(Expression expression)
        {
            object result = Execute(expression);

            Type resultType = result.GetType();

            if (resultType == typeof(long) && IsCountExpression(expression))
            {
                if (expression.Type == typeof(long))
                {
                    return (TResult)result;
                }

                if (expression.Type == typeof(int))
                {
                    int intResult = (int) (long)result;

                    return (TResult) (object)intResult;
                }
            }

            if (resultType == typeof(bool) && IsAnyExpression(expression))
            {
                return (TResult)result;
            }
            
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

            string sql = _statementToSqlTranslatorFactory().ToSql(ts);

            _entityTableHelper.EnsureEntityTableExists(ts.TypeFrom);

            if (ts.IsCountQuery)
            {
                return _dbAccess.QueryField(sql).ToList().Single();
            }

            if (ts.IsAnyQuery)
            {
                List<object> rows = _dbAccess.QueryField(sql).ToList();

                return rows.Any();
            }

            if (ts.SelectStatement.SelectElements.Count == 1)
            {
                if (ts.SelectStatement.SelectElements[0].SelectsJson)
                {
                    IEnumerable<string> result = _dbAccess.QueryJsonField(sql);

                    return result.Select(x => _config.Serializer.Deserialize(x, ts.TypeFrom));
                }
                else
                {
                    return _dbAccess.QueryField(sql);
                }
            }
            else
            {
                Type[] propTypes = ts.SelectStatement.SelectElements.Select(x => x.ReturnType).ToArray();
                IEnumerable<object[]> rows = _dbAccess.QueryFields(sql, propTypes);

                return CreateResultObjects(rows, ts.SelectStatement);
            }
        }

        private static void GuardEmptyCallectionLinqCall(Expression e)
        {
            if (e is MethodCallExpression me && (me.Method.Name == "First" || me.Method.Name == "Single" || me.Method.Name == "Last"))
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }
        }

        private static IEnumerable<object> CreateResultObjects(IEnumerable<object[]> rows, SelectStatement selectStatement)
        {
            if (TypeHelpers.IsAnonymousType(selectStatement.TypeTo))
            {
                foreach (var r in rows)
                {
                    yield return CreateResultAnonymousObject(r, selectStatement);
                }
            }
            else
            {
                foreach (var r in rows)
                {
                    yield return CreateResultObject(r, selectStatement);
                }
            }
        }

        private static object CreateResultObject(object[] row, SelectStatement selectStatement)
        {
            // todo: optimize, remove reflection
            object result = Activator.CreateInstance(selectStatement.TypeTo);

            for (int i = 0; i < selectStatement.SelectElements.Count; i++)
            {
                selectStatement.TypeTo.GetProperty(selectStatement.SelectElements[i].OutputProperty).SetValue(result, row[i]);
            }

            return result;
        }

        private static object CreateResultAnonymousObject(object[] row, SelectStatement selectStatement)
        {
            var ctors = selectStatement.TypeTo.GetConstructors();
            var parameters = ctors[0].GetParameters();

            // todo: optimize if possible
            return Activator.CreateInstance(selectStatement.TypeTo, row);
        }

        private static bool IsCountExpression(Expression e)
        {
            if (e is MethodCallExpression mce)
            {
                return mce.Method.DeclaringType == typeof(Queryable) 
                       && (mce.Method.Name == "Count" || mce.Method.Name == "LongCount");
            }

            return false;
        }

        private static bool IsAnyExpression(Expression e)
        {
            if (e is MethodCallExpression mce)
            {
                return mce.Method.DeclaringType == typeof(Queryable)
                       && mce.Method.Name == "Any";
            }

            return false;
        }
    }
}
