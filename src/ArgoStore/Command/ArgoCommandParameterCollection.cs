using System.Collections;
using ArgoStore.Statements;

namespace ArgoStore.Command;

public class ArgoCommandParameterCollection : IEnumerable<ArgoCommandParameter>
{
    private readonly FromAlias _alias;
    private readonly Dictionary<string, ArgoCommandParameter> _parameters = new();
    
    internal ArgoCommandParameterCollection()
        : this (new FromAlias())
    {
    }

    internal ArgoCommandParameterCollection(FromAlias alias)
    {
        _alias = alias;
    }

    public IEnumerator<ArgoCommandParameter> GetEnumerator()
    {
        foreach (KeyValuePair<string, ArgoCommandParameter> pair in _parameters)
        {
            yield return pair.Value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void AddWithName(string name, object value)
    {
        _parameters[name] = new ArgoCommandParameter(name, value);
    }
    
    public string AddNewParameter(object value, string prefix = "")
    {
        prefix = TransformPrefix(prefix);

        string name = $"p{prefix}_{_alias.CurrentAlias}_{_parameters.Count + 1}";

        _parameters[name] = new ArgoCommandParameter(name, value);
        return name;
    }

    public string AddNewParameter(Func<object> valueFact, string prefix = "")
    {
        prefix = TransformPrefix(prefix);

        string name = $"p{prefix}_{_alias.CurrentAlias}_{_parameters.Count + 1}";

        _parameters[name] = new ArgoCommandParameter(name, valueFact);
        return name;
    }

    private static string TransformPrefix(string prefix)
    {
        prefix = prefix ?? "";
        if (prefix != "" && !prefix.StartsWith("_"))
        {
            prefix = "_" + prefix;
        }

        return prefix;
    }
}