using System.Collections;
using System.Text.Json;
using ArgoStore.CrudOperations;
using Microsoft.Data.Sqlite;

namespace ArgoStore;

internal class ArgoCommandExecutor
{
    private readonly string _connectionString;
    private readonly JsonSerializerOptions _serializerOptions;

    public ArgoCommandExecutor(string connectionString, JsonSerializerOptions serializerOptions)
    {
        _connectionString = connectionString;
        _serializerOptions = serializerOptions;
    }

    public object ExecuteToList(ArgoCommand command)
    {
        using SqliteConnection con = CreateAndOpenConnection();
        using SqliteCommand cmd = command.ToSqliteCommand();
        cmd.Connection = con;

        SqliteDataReader reader = cmd.ExecuteReader();

        IList result = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(command.ResultingType));

        while (reader.Read())
        {
            string json = reader[0] as string;
            result.Add(JsonSerializer.Deserialize(json!, command.ResultingType, _serializerOptions));
        }

        return result;
    }

    public void ExecuteInTransaction(IReadOnlyList<CrudOperation> ops, JsonSerializerOptions serializerOptions)
    {
        if (!ops.Any())
        {
            return;
        }

        using SqliteConnection con = CreateAndOpenConnection();
        using SqliteTransaction tr = con.BeginTransaction();

        foreach (CrudOperation op in ops)
        {
            ExecuteOperation(op, tr, serializerOptions);
        }

        tr.Commit();
        con.Close();
    }

    private void ExecuteOperation(CrudOperation op, SqliteTransaction tr, JsonSerializerOptions serializerOptions)
    {
        SqliteCommand cmd = op.CreateCommand(serializerOptions);
        cmd.Connection = tr.Connection;
        cmd.Transaction = tr;
        cmd.ExecuteNonQuery();
    }
    
    private SqliteConnection CreateAndOpenConnection()
    {
        SqliteConnection c = new SqliteConnection(_connectionString);
        c.Open();

        return c;
    }
}