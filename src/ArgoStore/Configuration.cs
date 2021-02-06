using System;

namespace ArgoStore
{
    public class Configuration
    {
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
    }
}
