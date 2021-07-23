using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ArgoStore.EntityCrudOperationConverters;
using ArgoStore.Helpers;
using Microsoft.Data.Sqlite;

namespace ArgoStore
{
    internal class DocumentSession : IDocumentSession
    {
        private readonly Configuration _config;
        private readonly EntityTableHelper _entityTableHelper;
        private readonly SqliteConnection _connection;
        private readonly Queue<IDbCommand> _commands = new Queue<IDbCommand>();
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
            EnsureNotDisposed();
            return new ArgoStoreQueryable<T>(new ArgoStoreQueryProvider(_config));
        }

        public void Insert<T>(params T[] entities)
        {
            EnsureNotDisposed();
            EntityMetadata meta = ValidateEntityAndGetMeta(entities);

            foreach (T entity in entities)
            {
                PrimaryKeySetter.SetPrimaryKey(meta, entity);
                EntityCrudOperation op = new EntityCrudOperation(entity, CrudOperations.Insert);
                _commands.Enqueue(EntityCrudOperationConverterStrategies.Convert(op, _connection));
            }
        }

        public void Update<T>(params T[] entities)
        {
            EnsureNotDisposed();
            EntityMetadata meta = ValidateEntityAndGetMeta(entities);

            foreach (T entity in entities)
            {
                if (!PrimaryKeySetter.DoesPrimaryKeyHaveDefaultValue(meta, entity))
                {
                    throw new InvalidOperationException("At least one of the entities doesn't have PK set.");
                }

                EntityCrudOperation op = new EntityCrudOperation(entity, CrudOperations.Update);
                _commands.Enqueue(EntityCrudOperationConverterStrategies.Convert(op, _connection));
            }
        }

        public void Delete<T>(params T[] entities)
        {
            EnsureNotDisposed();
            EntityMetadata meta = ValidateEntityAndGetMeta(entities);

            foreach (T entity in entities)
            {
                if (!PrimaryKeySetter.DoesPrimaryKeyHaveDefaultValue(meta, entity))
                {
                    throw new InvalidOperationException("At least one of the entities doesn't have PK set.");
                }

                EntityCrudOperation op = new EntityCrudOperation(entity, CrudOperations.Delete);
                _commands.Enqueue(EntityCrudOperationConverterStrategies.Convert(op, _connection));
            }
        }

        public void Store<T>(params T[] entities)
        {
            EnsureNotDisposed();
            ValidateEntityAndGetMeta(entities);

            foreach (T entity in entities)
            {
                EntityCrudOperation op = new EntityCrudOperation(entity, CrudOperations.Upsert);
                _commands.Enqueue(EntityCrudOperationConverterStrategies.Convert(op, _connection));
            }
        }

        public void DeleteWhere<T>(Expression<Func<T, bool>> predicate)
        {
            EnsureNotDisposed();
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            EntityCrudOperation op = new EntityCrudOperation(predicate, CrudOperations.Upsert);
            _commands.Enqueue(EntityCrudOperationConverterStrategies.Convert(op, _connection));
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

        public void SaveChanges() => SaveChangesAsync().GetAwaiter().GetResult();
        
        public async Task SaveChangesAsync()
        {
            EnsureNotDisposed();
            await OpenConnectionAsync();

            using SqliteTransaction tr = _connection.BeginTransaction();
            
            try
            {
                while (_commands.Any())
                {
                    IDbCommand cmd = _commands.Dequeue();
                    cmd.ExecuteNonQuery();
                }

                tr.Commit();
            }
            catch (Exception e)
            {
                // todo: log
                tr.Rollback();
                Console.WriteLine(e);
                throw;
            }

        }
        
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
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
        
        private async Task OpenConnectionAsync()
        {
            if (_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync();
            }
        }

        private void EnsureNotDisposed()
        {
            if (_disposed) throw new ObjectDisposedException("Session disposed");
        }
    }
}
