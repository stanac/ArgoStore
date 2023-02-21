namespace ArgoStore.Command;

public class ArgoCommandParameter
{
    private readonly Func<object> _valueFactory;

    public const string TransformPrefix = "ArgoTransformPx";
    public string Name { get; }
    public object Value => _valueFactory();
    public bool IsFromTransform => Name.Contains(TransformPrefix);

    public ArgoCommandParameter(string name, object value)
    {
        Name = name;
        _valueFactory = () => value;
    }

    public ArgoCommandParameter(string name, Func<object> valueFact)
    {
        Name = name;
        _valueFactory = valueFact;
    }
}