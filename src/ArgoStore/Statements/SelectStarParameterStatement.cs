namespace ArgoStore.Statements;

internal class SelectStarParameterStatement : Statement
{
    public override Statement Negate()
    {
        throw new NotSupportedException();
    }

    public override Statement ReduceIfPossible() => this;

    public override string ToDebugString() => "*";
}