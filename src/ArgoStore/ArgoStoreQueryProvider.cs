using ArgoStore.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using ArgoStore.Statements;
using Microsoft.Data.Sqlite;

namespace ArgoStore
{
    internal class ArgoStoreQueryProvider : IQueryProvider
    {
        private readonly Configuration _config;
        private readonly EntityTableHelper _entityTableHelper;
        private readonly IDbAccess _dbAccess;
        private readonly Func<IStatementToSqlTranslator> _statementToSqlTranslatorFactory;

        public ArgoStoreQueryProvider(Configuration config)
            : this (config, new DbAccess(config?.ConnectionString), () => new StatementToSqlTranslator(config?.Serializer))
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
            object result = Execute(expression, out TopStatement statement);

            if (result == null)
            {
                return default;
            }

            Type resultType = result.GetType();

            if (resultType == typeof(long) && statement.IsCountQuery)
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

            if (resultType == typeof(bool) && statement.IsAnyQuery)
            {
                return (TResult)result;
            }
            
            if (TypeHelpers.IsCollectionType(resultType))
            {
                IEnumerable resultCollection = result as IEnumerable;

                Debug.Assert(resultCollection != null, nameof(resultCollection) + " != null");

                var list = resultCollection.Cast<object>().ToList();

                if (list.Any()) return (TResult)list.First();

                GuardEmptyCollectionLinqCall(expression);

                return default(TResult);
            }
            
            return (TResult)result;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotSupportedException("Non generic method CreateQuery isn't supported");
        }

        public object Execute(Expression expression)
        {
            return Execute(expression, out _);
        }

        public object Execute(Expression expression, out TopStatement topStatement)
        {
            if (expression is null) throw new ArgumentNullException(nameof(expression));

            Statement statement = ExpressionToStatementTranslators.ExpressionToStatementTranslatorStrategy.Translate(expression);

            TopStatement ts = TopStatement.Create(statement, _config.TenantId);
            topStatement = ts;

            ArgoSqlCommand argoCmd = _statementToSqlTranslatorFactory().CreateCommand(ts);
            SqliteCommand cmd = argoCmd.CreateCommand(_config.TenantId);

            _entityTableHelper.EnsureEntityTableExists(ts.TypeFrom);

            if (ts.IsCountQuery)
            {
                return _dbAccess.QueryField(cmd).ToList().Single();
            }

            if (ts.IsAnyQuery)
            {
                List<object> rows = _dbAccess.QueryField(cmd).ToList();

                return rows.Any();
            }

            bool isSingleSelect = ts.SelectStatement.CalledByMethod.ItSelectsOnlyOne();
            bool selectsJson = ts.SelectStatement.SelectElements[0].SelectsJson;
            
            if (isSingleSelect)
            {
                object result = _dbAccess.QueryField(cmd).ToList().FirstOrDefault();

                if (result == null)
                {
                    return null;
                }

                string json = selectsJson
                    ? (string) result
                    : "";

                if (selectsJson)
                {
                    return _config.Serializer.Deserialize(json, topStatement.TypeTo);
                }

                return result;
            }

            if (selectsJson)
            {
                List<string> result = _dbAccess.QueryStringField(cmd).ToList();

                return result.Select(jsonRow => _config.Serializer.Deserialize(jsonRow, ts.TypeTo));
            }

            Type[] propTypes = ts.SelectStatement.SelectElements.Select(x => x.ReturnType).ToArray();
            IEnumerable<object[]> rows2 = _dbAccess.QueryFields(cmd, propTypes);

            return CreateResultObjects(rows2, ts.SelectStatement);
        }

        private static void GuardEmptyCollectionLinqCall(Expression e)
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
                foreach (var row in rows)
                {
                    yield return CreateResultAnonymousObject(row, selectStatement);
                }
            }
            else if (TypeHelpers.IsPrimitiveType(selectStatement.TypeTo))
            {
                Type dbObjectType = null;

                foreach (object[] row in rows)
                {
                    Debug.Assert(row.Length == 1);

                    if (dbObjectType == null)
                    {
                        dbObjectType = row[0].GetType();
                    }

                    yield return CastResultingType(row[0], dbObjectType, selectStatement.TypeTo);
                }
            }
            else
            {
                foreach (object[] row in rows)
                {
                    yield return CreateResultObject(row, selectStatement);
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

        private static object CastResultingType(object dbObject, Type dbObjectType, Type typeTo)
        {
            if (dbObject == DBNull.Value)
            {
                return null;
            }

            if (dbObjectType == typeTo)
            {
                return dbObject;
            }

            throw new NotImplementedException();
        }
    }
}
