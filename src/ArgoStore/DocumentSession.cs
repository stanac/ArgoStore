using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using ArgoStore.Configurations;
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
        private readonly Queue<EntityCrudOperation> _commands = new Queue<EntityCrudOperation>();
        
        private SqliteTransaction _transaction;
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
                PrimaryKeyValue pk = PrimaryKeyValue.CreateFromEntity(meta, entity);

                if (pk.IsStringKey)
                {
                    if (pk.HasDefaultValue())
                    {
                        pk.SetRandomStringKey();
                        pk.SetInEntity(entity);
                    }
                }
                else
                {
                    if (!pk.HasDefaultValue())
                    {
                        throw new InvalidOperationException("Cannot insert entity with integer/long PK set.");
                    }
                }

                EntityCrudOperation op = new EntityCrudOperation(entity, CrudOperations.Insert, meta, pk);
                op.Command = EntityCrudOperationConverterStrategies.Convert(op, _connection, _config.Serializer, _config.TenantId);
                _commands.Enqueue(op);
            }
        }

        public void Update<T>(params T[] entities)
        {
            EnsureNotDisposed();
            EntityMetadata meta = ValidateEntityAndGetMeta(entities);

            foreach (T entity in entities)
            {
                PrimaryKeyValue pkValue = PrimaryKeyValue.CreateFromEntity(meta, entity);

                if (pkValue.HasDefaultValue())
                {
                    throw new InvalidOperationException($"Cannot update entity `{typeof(T).Name}` which doesn't have PK set.");
                }

                EntityCrudOperation op = new EntityCrudOperation(entity, CrudOperations.Update, meta, pkValue);
                op.Command = EntityCrudOperationConverterStrategies.Convert(op, _connection, _config.Serializer, _config.TenantId);
                _commands.Enqueue(op);
            }
        }

        public void Delete<T>(params T[] entities)
        {
            EnsureNotDisposed();
            EntityMetadata meta = ValidateEntityAndGetMeta(entities);

            foreach (T entity in entities)
            {
                PrimaryKeyValue pk = PrimaryKeyValue.CreateFromEntity(meta, entity);

                if (pk.HasDefaultValue())
                {
                    throw new InvalidOperationException($"Cannot delete entity `{typeof(T).Name}` which doesn't have PK set.");
                }
                
                EntityCrudOperation op = new EntityCrudOperation(entity, CrudOperations.Delete, meta, pk);
                op.Command = EntityCrudOperationConverterStrategies.Convert(op, _connection, _config.Serializer, _config.TenantId);
                _commands.Enqueue(op);
            }
        }

        public void InsertOrUpdate<T>(params T[] entities)
        {
            EnsureNotDisposed();
            EntityMetadata meta = ValidateEntityAndGetMeta(entities);
            
            foreach (T entity in entities)
            {
                PrimaryKeyValue pk = PrimaryKeyValue.CreateFromEntity(meta, entity);

                EntityCrudOperation op = new EntityCrudOperation(entity, CrudOperations.Upsert, meta, pk);
                op.Command = EntityCrudOperationConverterStrategies.Convert(op, _connection, _config.Serializer, _config.TenantId);
                _commands.Enqueue(op);
            }
        }

        public void DeleteWhere<T>(Expression<Func<T, bool>> predicate)
        {
            EnsureNotDisposed();
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            EntityCrudOperation op = new EntityCrudOperation(predicate);
            op.Command = EntityCrudOperationConverterStrategies.Convert(op, _connection, _config.Serializer, _config.TenantId);
            _commands.Enqueue(op);
        }

        public void SaveChanges()
        {
            EnsureNotDisposed();

            Execute();

            _transaction.Commit();
            _transaction.Dispose();
            _transaction = null;
        }

        public void DiscardChanges()
        {
            if (_commands.Any())
            {
                _commands.Clear();
            }

            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public void Execute()
        {
            OpenConnectionAndCreateTransaction();

            foreach (EntityCrudOperation op in _commands)
            {
                op.Command.Transaction = _transaction;
            }

            try
            {
                while (_commands.Any())
                {
                    EntityCrudOperation operation = _commands.Dequeue();
                    operation.Command.ExecuteNonQuery();

                    if (operation.CrudOperation == CrudOperations.Insert && operation.PkValue.IsLongKey)
                    {
                        long id = GetLastInsertedId();

                        operation.PkValue.SetLongKey(id);
                        operation.PkValue.SetInEntity(operation.Entity);
                        UpdateIntegerIdInDocumentAfterInsert(operation, _transaction);
                    }
                }
            }
            catch (Exception e)
            {
                // todo: log
                _transaction.Rollback();
                Console.WriteLine(e);
                throw;
            }
        }

        private void UpdateIntegerIdInDocumentAfterInsert(EntityCrudOperation insertOp, SqliteTransaction transaction)
        {
            EntityCrudOperation op = new EntityCrudOperation(insertOp.Entity, CrudOperations.Update,
                insertOp.EntityMeta, insertOp.PkValue);

            op.Command = EntityCrudOperationConverterStrategies.Convert(op, _connection, _config.Serializer, _config.TenantId);
            op.Command.Transaction = transaction;
            op.Command.ExecuteNonQuery();
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

            CreateTableForEntityIfNotExists(entityType);
            
            return _config.GetOrCreateEntityMetadata(entityType);
        }

        internal void CreateTableForEntityIfNotExists(Type entityType)
        {
            if (entityType == null) throw new ArgumentNullException(nameof(entityType));

            _entityTableHelper.EnsureEntityTableExists(entityType);
        }

        private void OpenConnectionAndCreateTransaction()
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            if (_transaction == null)
            {
                _transaction = _connection.BeginTransaction();
            }
        }

        private void EnsureNotDisposed()
        {
            if (_disposed) throw new ObjectDisposedException("Session disposed");
        }

        private long GetLastInsertedId()
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT last_insert_rowid()";
            object result = cmd.ExecuteScalar();
            return (long) result;
        }

        public void Dispose()
        {
            if (_disposed) throw new InvalidOperationException("Session already disposed");

            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }

            if (_connection != null && _connection.State != ConnectionState.Closed)
            {
                _connection.Close();
                _connection.Dispose();
            }

            _disposed = true;
        }

    }
}
