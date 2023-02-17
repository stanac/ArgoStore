namespace ArgoStore.Statements.Where;

internal class WhereStringContainsMethodCallStatement : WhereStatementBase
{
    public WhereStatementBase ObjectStatement { get; }
    public StringMethods Method { get; }
    public bool IgnoreCase { get; }
    public WhereStatementBase SubjectStatement { get; }

    public WhereStringContainsMethodCallStatement(WhereStatementBase objectStatement, 
        WhereStatementBase subjectStatement, StringMethods method, bool ignoreCase)
    {
        ObjectStatement = objectStatement;
        Method = method;
        IgnoreCase = ignoreCase;
        SubjectStatement = subjectStatement;
    }
}