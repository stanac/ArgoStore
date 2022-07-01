namespace ArgoStore.Statements;

internal class SelectStatementElement
{
    public SelectStatementElement(Statement statement, Type returnType, bool selectsJson, string inputProperty, string outputProperty)
    {
        Statement = statement ?? throw new ArgumentNullException(nameof(statement));
        ReturnType = returnType ?? throw new ArgumentNullException(nameof(returnType));
        SelectsJson = selectsJson;

        if (!selectsJson)
        {
            if (string.IsNullOrWhiteSpace(inputProperty)) throw new ArgumentException($"Parameter {nameof(inputProperty)} needs to be set when {nameof(selectsJson)} is false", nameof(inputProperty));
            if (string.IsNullOrWhiteSpace(outputProperty)) throw new ArgumentException($"Parameter {nameof(outputProperty)} needs to be set when {nameof(selectsJson)} is false", nameof(outputProperty));
        }

        InputProperty = inputProperty;
        OutputProperty = outputProperty;
    }

    public static SelectStatementElement CreateWithStar(Type returnType) => new SelectStatementElement(new SelectStarParameterStatement(), returnType, true, null, null);

    public Statement Statement { get; }
    public Type ReturnType { get; }
    public string Alias { get; set; }
    public bool FromSubQuery { get; set; }
    public string BindsToSubQueryAlias { get; set; }
    public bool SelectsJson { get; }
    public string InputProperty { get; }
    public string OutputProperty { get; }

    public string ToDebugString() => $"{Statement?.ToDebugString()}";
}