namespace ArgoStore.Helpers;

internal interface IClock
{
    DateTimeOffset GetCurrentUtcDateTime();
    long GetCurrentUtcMilliseconds();
}