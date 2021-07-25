using System.Collections.Generic;

namespace ArgoStore.Helpers
{
    internal class CacheHashSet<T>
    {
        private readonly object _sync = new object();
        private readonly HashSet<T> _cache = new HashSet<T>();

        public void Add(T value)
        {
            lock (_sync)
            {
                _cache.Add(value);
            }
        }

        public bool Contains(T value)
        {
            lock (_sync)
            {
                return _cache.Contains(value);
            }
        }
    }
}
