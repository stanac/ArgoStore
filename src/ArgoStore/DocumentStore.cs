using System;

namespace ArgoStore
{
    public class DocumentStore
    {
        private readonly bool _createEntityTablesOnTheFly;

        public DocumentStore(string dbFilePath, bool createEntityTablesOnTheFly)
        {
            if (string.IsNullOrWhiteSpace(dbFilePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(dbFilePath));

            _createEntityTablesOnTheFly = createEntityTablesOnTheFly;
            ConnectionString = $"Data Source={dbFilePath};";
        }

        public string ConnectionString { get; }

        public IDocumentSession CreateSession()
        {
            return new DocumentSession(new Configuration
            {
                Serializer = new ArgoStoreSerializer(),
                ConnectionString = ConnectionString,
                CreateEntitiesOnTheFly = _createEntityTablesOnTheFly
            });
        }
    }
}
