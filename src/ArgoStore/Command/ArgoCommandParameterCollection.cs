using System.Collections;

namespace ArgoStore.Command;

public class ArgoCommandParameterCollection : IEnumerable<ArgoCommandParameter>
{
    private readonly Dictionary<string, object> _parameters = new();

    public IEnumerator<ArgoCommandParameter> GetEnumerator()
    {
        foreach (KeyValuePair<string, object> pair in _parameters)
        {
            yield return new ArgoCommandParameter(pair.Key, pair.Value);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void AddWithName(string name, object value)
    {
        _parameters[name] = value;
    }

    public string AddNewParameter(object value)
    {
        string name = $"p{_parameters.Count + 1}";

        _parameters[name] = value;
        return name;
    }
}