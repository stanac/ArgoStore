namespace ArgoStore.Statements;

internal class IntegerIncrement
{
    public int Value { get; private set; }

    public int Increment()
    {
        Value++;
        return Value;
    }
}