using System;
using System.Collections.Generic;
using System.Linq;
using ArgoStore.Helpers;

namespace ArgoStore
{
    internal class DocumentSession : IDocumentSession
    {
        private readonly Configuration _config;
        private readonly EntityTableHelper _entityTableHelper;

        public DocumentSession(Configuration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _config.EnsureValid();
            _entityTableHelper = new EntityTableHelper(config);
        }

        public void Dispose()
        {
            // do nothing for now
        }

        public IQueryable<T> Query<T>() where T : class, new()
        {
            return new ArgoStoreQueryable<T>(new ArgoStoreQueryProvider(_config));
        }

        public void Insert<T>(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            Type entityType = typeof(T);

            _entityTableHelper.EnsureEntityTableExists(entityType);

            SetKey(entityType, entity);

            throw new NotImplementedException();
        }

        private void SetKey(Type entityType, object entity)
        {

        }
    }
}
