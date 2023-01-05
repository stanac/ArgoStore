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

    public object Execute(ArgoCommand command)
    {
        switch (command.CommandType)
        {
            case ArgoCommandTypes.NonQuery:
                throw new NotImplementedException();
            
            case ArgoCommandTypes.Count:
            case ArgoCommandTypes.LongCount:
                return ExecuteCount(command);
                
            case ArgoCommandTypes.ToList:
                return ExecuteToList(command);
                
            case ArgoCommandTypes.Single:
            case ArgoCommandTypes.SingleOrDefault:
            case ArgoCommandTypes.First:
            case ArgoCommandTypes.FirstOrDefault:
                throw new NotImplementedException();
                
            default:
                throw new ArgumentOutOfRangeException();
        }
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

    public object ExecuteFirstOrDefault(ArgoCommand command)
    {
        using SqliteConnection con = CreateAndOpenConnection();
        using SqliteCommand cmd = command.ToSqliteCommand();
        cmd.Connection = con;

        string json = cmd.ExecuteScalar() as string;

        if (json == null) return null;

        return JsonSerializer.Deserialize(json, command.ResultingType, _serializerOptions);
    }

    public object ExecuteCount(ArgoCommand command)
    {
        using SqliteConnection con = CreateAndOpenConnection();
        using SqliteCommand cmd = command.ToSqliteCommand();
        cmd.Connection = con;

        object result = cmd.ExecuteScalar() 
                        ?? throw new InvalidOperationException("Unexpected null result on ExecuteCount(ArgoCommand command)");

        long ret;

        if (result is int i) ret = i;
        else ret = (long)result;

        if (command.CommandType == ArgoCommandTypes.Count)
        {
            return (int) ret;
        }

        return ret;
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