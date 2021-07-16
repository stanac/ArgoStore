using System;
using System.Collections.Generic;

namespace ArgoStore
{
    public class Configuration
    {
        private readonly Dictionary<Type, EntityMetadata> _entityMeta = new();

        public string ConnectionString { get; set; }
        public bool CreateEntitiesOnTheFly { get; set; } = true;
        public IArgoStoreSerializer Serializer { get; set; } = new ArgoStoreSerializer();
        
        public void EnsureValid()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                throw new InvalidOperationException($"{nameof(ConnectionString)} not set");
            }
        }

        internal EntityMetadata GetOrCreateEntityMetadata(Type entityType)
        {
            if (_entityMeta.TryGetValue(entityType, out EntityMetadata m))
            {
                return m;
            }

            m = new EntityMetadata(entityType);

            _entityMeta[entityType] = m;

            return m;
        }
    }
}
