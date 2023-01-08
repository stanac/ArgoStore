namespace ArgoStore.Command;

public class ArgoCommandParameter
{
    public string Name { get; }
    public object Value { get; }

    public ArgoCommandParameter(string name, object value)
    {
        Name = name;
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }
}