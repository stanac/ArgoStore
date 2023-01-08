namespace ArgoStore.Command;

public enum ArgoCommandTypes
{
    NonQuery,
    Count,
    LongCount,
    ToList,
    Single,
    SingleOrDefault,
    First,
    FirstOrDefault
}

public static class ArgoCommandTypesExtensions
{
    public static bool IsSingleOrSingleOrDefault(this ArgoCommandTypes t)
    {
        return t is ArgoCommandTypes.Single or ArgoCommandTypes.SingleOrDefault;
    }
}