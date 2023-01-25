using System.Reflection;

namespace ArgoStore.Statements.Where;

internal class WhereSubQueryStatement : WhereStatementBase
{
    public PropertyInfo? FromProperty { get; set; }
    public bool IsAny { get; set; }
    public bool IsCount { get; set; }
}