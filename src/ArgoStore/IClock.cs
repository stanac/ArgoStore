namespace ArgoStore;

internal interface IClock
{
    DateTimeOffset GetCurrentUtcDateTime();
    long GetCurrentUtcMilliseconds();
}