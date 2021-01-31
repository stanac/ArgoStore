using System;

namespace JsonDbLite
{
    public class Configuration
    {
        public string ConnectionString { get; set; }
        public bool CreateEntitiesOnTheFly { get; set; } = true;
        public IJsonDbLiteSerializer Serializer { get; set; } = new JsonDbLiteSerializer();

        public void EnsureValid()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                throw new InvalidOperationException($"{nameof(ConnectionString)} not set");
            }
        }
    }
}
