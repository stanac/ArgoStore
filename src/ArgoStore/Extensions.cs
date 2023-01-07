namespace ArgoStore;

internal static class Extensions
{
    public static bool IsCaseSensitive(this StringComparison sc)
    {
        return !(
            sc is StringComparison.OrdinalIgnoreCase
            or StringComparison.CurrentCultureIgnoreCase
            or StringComparison.InvariantCultureIgnoreCase
            );
    }
}