namespace ArgoStore.Helpers;

internal static class DotNet6Types
{
    public static Type DateOnlyType { get; } = GetDateOnlyType();
    public static Type TimeOnlyType { get; } = GetDateOnlyType();

    private static Type GetDateOnlyType()
    {
        return typeof(DateTime).Assembly.GetTypes()
            .FirstOrDefault(x => x.FullName == "System.DateOnly");
    }

    private static Type GetTimeOnlyType()
    {
        return typeof(DateTime).Assembly.GetTypes()
            .FirstOrDefault(x => x.FullName == "System.TimeOnly");
    }
}