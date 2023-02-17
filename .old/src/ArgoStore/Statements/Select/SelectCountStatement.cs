namespace ArgoStore.Statements.Select;

internal class SelectCountStatement : SelectStatementBase
{
    public bool CountLong { get; }

    public SelectCountStatement()
        : this (false)
    {
    }

    public SelectCountStatement(bool countLong)
    {
        CountLong = countLong;
    }
}