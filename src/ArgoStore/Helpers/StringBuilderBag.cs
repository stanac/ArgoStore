using System.Text;

namespace ArgoStore.Helpers;

internal class StringBuilderBag
{
    private static readonly object Sync = new();
    private readonly Queue<StringBuilder> _sbs = new();

    public static StringBuilderBag Default { get; } = new();

    public StringBuilder Get()
    {
        lock (Sync)
        {
            if (_sbs.Count > 0)
            {
                return _sbs.Dequeue();
            }

            return new StringBuilder();
        }
    }

    public void Return(StringBuilder sb)
    {
        lock (Sync)
        {
            sb.Clear();
            if (_sbs.Count < 20)
            {
                _sbs.Enqueue(sb);
            }
        }
    }
}