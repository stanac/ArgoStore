using Microsoft.Data.Sqlite;

namespace ArgoStore
{
    internal interface IStatementToSqlTranslator
    {
        void SetSqlCommand(TopStatement statement, SqliteCommand cmd);
    }
}