using System.Diagnostics;
using System.Text;
using ArgoStore.Helpers;

namespace ArgoStore;

internal class ArgoActivity
{
    private static readonly double TicksPerMillisecond = Stopwatch.Frequency / 1000.0;

    private readonly long _startTicks;
    private long _endTicks;
    private readonly List<ArgoActivity> _children = new();
    private readonly ArgoActivity? _parent;

    public string Name { get; }
    public int Level { get; }
    public bool IsStopped => _endTicks > 0;
    public double ElapsedMilliseconds { get; private set; }

    public ArgoActivity(string name)
        : this (name, 0, null)
    {
    }

    private ArgoActivity(string name, int level, ArgoActivity? parent)
    {
        Name = name;
        Level = level;
        _startTicks = Stopwatch.GetTimestamp();
        _parent = parent;
    }

    public ArgoActivity CreateChild(string name)
    {
        ArgoActivity child = new ArgoActivity($"{Name}::{name}", Level + 1, this);
        _children.Add(child);
        return child;
    }

    public ArgoActivity StopAndCreateSibling(string name)
    {
        Stop();
        return _parent?.CreateChild(name) ?? throw new InvalidOperationException("Parent not set");
    }

    public void Stop()
    {
        foreach (ArgoActivity child in _children)
        {
            child.Stop();
        }

        if (IsStopped) return;

        _endTicks = Stopwatch.GetTimestamp();
        ElapsedMilliseconds = (_endTicks - _startTicks) / TicksPerMillisecond;
    }

    public string Dump()
    {
        if (!IsStopped) Stop();

        StringBuilder sb = StringBuilderBag.Default.Get();
        Dump(sb);
        string s = sb.ToString();
        StringBuilderBag.Default.Return(sb);
        return s;
    }

    private void Dump(StringBuilder sb)
    {
        sb.Append(new string(' ', Level * 4))
            .Append(Name).Append(" ")
            .Append(ElapsedMilliseconds.ToString("000.000"))
            .AppendLine(" ms");

        foreach (ArgoActivity c in _children)
        {
            c.Dump(sb);
        }
    }
}