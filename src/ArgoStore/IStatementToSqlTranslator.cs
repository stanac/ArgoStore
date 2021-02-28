namespace ArgoStore
{
    internal interface IStatementToSqlTranslator
    {
        string ToSql(TopStatement statement);
    }
}