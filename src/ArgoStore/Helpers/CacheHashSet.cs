namespace ArgoStore.Helpers;

internal class CacheHashSet<T>
{
    private readonly object _sync = new();
    private readonly HashSet<T> _cache = new();

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