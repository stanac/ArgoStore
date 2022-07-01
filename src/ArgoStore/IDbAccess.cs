using Microsoft.Data.Sqlite;

namespace ArgoStore;

internal interface IDbAccess
{
    IEnumerable<string> QueryStringField(SqliteCommand cmd);
    IEnumerable<object[]> QueryFields(SqliteCommand cmd, Type[] expectedResultTypes);
    IEnumerable<object> QueryField(SqliteCommand cmd);
    Task<IEnumerable<string>> QueryJsonFieldAsync(SqliteCommand cmd);
}