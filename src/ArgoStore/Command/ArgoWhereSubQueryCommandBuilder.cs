using System.Reflection;
using ArgoStore.Statements.Where;

namespace ArgoStore.Command;

internal class ArgoWhereSubQueryCommandBuilder
{
    public PropertyInfo? FromProperty { get; set; }
    public bool IsAny { get; set; }
    public bool IsCount { get; set; }

    public WhereSubQueryStatement Build()
    {
#if DEBUG
        if (FromProperty is null)
        {
            throw new InvalidOperationException($"{nameof(FromProperty)} not set in subquery.");
        }
#endif

        return new WhereSubQueryStatement
        {
            FromProperty = FromProperty,
            IsAny = IsAny,
            IsCount = IsCount
        };
    }
}