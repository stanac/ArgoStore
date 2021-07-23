using System;

namespace ArgoStore.Helpers
{
    internal static class DateTimeFormatter
    {
        public static string ToUtcFormat(DateTime dt) => dt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
    }
}
