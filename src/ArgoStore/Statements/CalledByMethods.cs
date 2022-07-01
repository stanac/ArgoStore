namespace ArgoStore.Statements;

internal enum CalledByMethods
{
    Select, First, FirstOrDefault, Last, LastOrDefault, Single, SingleOrDefault, Count, Any
}

internal static class CalledByMethodsExtensions
{
    private static readonly CalledByMethods[] _returningOnlyOneMethods = 
    {
        CalledByMethods.First,
        CalledByMethods.FirstOrDefault,
        CalledByMethods.Last,
        CalledByMethods.LastOrDefault,
        CalledByMethods.Single,
        CalledByMethods.SingleOrDefault
    };

    private static readonly CalledByMethods[] _shouldThrowOnNotFound = 
    {
        CalledByMethods.First,
        CalledByMethods.Last,
        CalledByMethods.Single
    };

    public static bool ItSelectsOnlyOne(this CalledByMethods m) => _returningOnlyOneMethods.Contains(m);

    public static bool ShouldThrowOnNotFound(this CalledByMethods m) => _shouldThrowOnNotFound.Contains(m);
}