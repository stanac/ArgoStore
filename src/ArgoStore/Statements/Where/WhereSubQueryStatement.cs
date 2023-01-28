using ArgoStore.Command;

namespace ArgoStore.Statements.Where;

internal class WhereSubQueryStatement : WhereStatementBase
{
    public ArgoCommandBuilder CommandBuilder { get; }

    public WhereSubQueryStatement(ArgoCommandBuilder commandBuilder)
    {
        CommandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        commandBuilder.IsSubQuery = true;
    }
}