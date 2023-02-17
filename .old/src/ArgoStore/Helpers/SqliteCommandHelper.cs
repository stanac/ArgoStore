using Microsoft.Data.Sqlite;

namespace ArgoStore.Helpers;

internal static class SqliteCommandHelper
{
    public static void EnsureNoGuidParams(this SqliteCommand cmd)
    {
#if DEBUG
        foreach (SqliteParameter p in cmd.Parameters)
        {
            if (p.Value is Guid)
            {
                throw new InvalidOperationException("Guid in parameter");
            }
        }
#else
        // noop
#endif
    }
}