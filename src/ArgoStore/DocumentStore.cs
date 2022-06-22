using System;

namespace ArgoStore
{
    public class DocumentStore
    {
        private readonly bool _createDocumentTablesOnTheFly;

        public DocumentStore(string dbFilePath)
            : this (dbFilePath, true)
        {
        }

        public DocumentStore(string dbFilePath, bool createDocumentTablesOnTheFly)
        {
            if (string.IsNullOrWhiteSpace(dbFilePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(dbFilePath));

            _createDocumentTablesOnTheFly = createDocumentTablesOnTheFly;
            ConnectionString = $"Data Source={dbFilePath};";
        }

        public string ConnectionString { get; }

        public void CreateTableForEntityIfNotExists<T>()
            => CreateTableForEntityIfNotExists(typeof(T));

        public void CreateTableForEntityIfNotExists(Type entityType)
        {
            if (entityType == null) throw new ArgumentNullException(nameof(entityType));

            Configuration config = GetConfiguration();
            config.CreateEntitiesOnTheFly = true;

            using DocumentSession session = new DocumentSession(config);
            
            session.CreateTableForEntityIfNotExists(entityType);
        }

        public IDocumentSession CreateSession() => CreateSession(TenantIdDefault.DefaultValue);
        
        public IDocumentSession CreateSession(string tenantId)
        {
            return new DocumentSession(GetConfiguration());
        }

        public IQueryDocumentSession CreateReadOnlySession() => CreateReadOnlySession(TenantIdDefault.DefaultValue);

        public IQueryDocumentSession CreateReadOnlySession(string tenantId)
        {
            return new DocumentSession(GetConfiguration());
        }

        private Configuration GetConfiguration()
        {
            return new Configuration
            {
                Serializer = new ArgoStoreSerializer(),
                ConnectionString = ConnectionString,
                CreateEntitiesOnTheFly = _createDocumentTablesOnTheFly
            };
        }
    }
}
