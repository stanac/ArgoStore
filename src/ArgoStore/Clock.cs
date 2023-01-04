namespace ArgoStore;

internal class Clock : IClock
{
    public static Clock Default { get; } = new Clock();

    public DateTimeOffset GetCurrentUtcDateTime()
    {
        return DateTimeOffset.UtcNow;
    }

    public long GetCurrentUtcMilliseconds()
    {
        return GetCurrentUtcDateTime().ToUnixTimeMilliseconds();
    }
}