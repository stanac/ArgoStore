using System.Reflection;

namespace ArgoStore.StatementTranslators.From;

internal class FromProperty : FromStatementBase
{
    public PropertyInfo Property { get; }

    public FromProperty(PropertyInfo property)
    {
        Property = property ?? throw new ArgumentNullException(nameof(property));
    }
}