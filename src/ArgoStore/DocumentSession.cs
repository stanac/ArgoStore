using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ArgoStore.Helpers;
using Microsoft.Data.Sqlite;

namespace ArgoStore
{
    internal class DocumentSession : IDocumentSession
    {
        private readonly Configuration _config;
        private readonly EntityTableHelper _entityTableHelper;
        private readonly SqliteConnection _connection;
        private readonly List<EntityCrudOperation> _operations = new List<EntityCrudOperation>();
        private bool _disposed;

        public DocumentSession(Configuration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _config.EnsureValid();
            _entityTableHelper = new EntityTableHelper(config);
            _connection = new SqliteConnection(config.ConnectionString);
        }

        public IQueryable<T> Query<T>() where T : class, new()
        {
            return new ArgoStoreQueryable<T>(new ArgoStoreQueryProvider(_config));
        }

        public void Insert<T>(params T[] entities)
        {
            EntityMetadata meta = ValidateEntityAndGetMeta(entities);

            foreach (T entity in entities)
            {
                PrimaryKeySetter.SetPrimaryKey(meta, entity);
            }

            _operations.AddRange(entities.Select(x => new EntityCrudOperation(x, CrudOperations.Insert)));
        }

        public void Update<T>(params T[] entities)
        {
            EntityMetadata meta = ValidateEntityAndGetMeta(entities);

            foreach (T entity in entities)
            {
                if (!PrimaryKeySetter.DoesPrimaryKeyHaveDefaultValue(meta, entity))
                {
                    throw new InvalidOperationException("At least one of the entities doesn't have PK set.");
                }
            }

            _operations.AddRange(entities.Select(x => new EntityCrudOperation(x, CrudOperations.Update)));
        }

        public void Delete<T>(params T[] entities)
        {
            EntityMetadata meta = ValidateEntityAndGetMeta(entities);

            foreach (T entity in entities)
            {
                if (!PrimaryKeySetter.DoesPrimaryKeyHaveDefaultValue(meta, entity))
                {
                    throw new InvalidOperationException("At least one of the entities doesn't have PK set.");
                }
            }

            _operations.AddRange(entities.Select(x => new EntityCrudOperation(x, CrudOperations.Delete)));
        }

        public void Store<T>(params T[] entities)
        {
            EntityMetadata meta = ValidateEntityAndGetMeta(entities);
            
            _operations.AddRange(entities.Select(x => new EntityCrudOperation(x, CrudOperations.Upsert)));
        }

        public void DeleteWhere<T>(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            _operations.Add(new EntityCrudOperation(predicate));
        }

        private EntityMetadata ValidateEntityAndGetMeta<T>(T[] entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            if (entities.Any(x => x == null))
            {
                throw new ArgumentException("Collection contains null", nameof(entities));
            }

            Type entityType = typeof(T);

            _entityTableHelper.EnsureEntityTableExists(entityType);

            return _config.GetOrCreateEntityMetadata(entityType);
        }

        public void Dispose()
        {
            if (_disposed) throw new InvalidOperationException("Session already disposed");

            if (_connection != null && _connection.State != ConnectionState.Closed)
            {
                _connection.Close();
                _connection.Dispose();
            }

            _disposed = true;
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
