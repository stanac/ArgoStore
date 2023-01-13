using System.Collections;
using System.Text.Json;
using ArgoStore.Config;
using ArgoStore.CrudOperations;
using ArgoStore.Helpers;
using Microsoft.Data.Sqlite;

namespace ArgoStore.Command;

internal class ArgoCommandExecutor
{
    private readonly string _connectionString;
    private readonly JsonSerializerOptions _serializerOptions;

    public ArgoCommandExecutor(string connectionString, JsonSerializerOptions serializerOptions)
    {
        _connectionString = connectionString;
        _serializerOptions = serializerOptions;
    }

    public object? Execute(ArgoCommand command)
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

            case ArgoCommandTypes.First:
            case ArgoCommandTypes.FirstOrDefault:
                return ExecuteFirstOrDefault(command);

            case ArgoCommandTypes.Single:
            case ArgoCommandTypes.SingleOrDefault:
                return ExecuteSingleOrDefault(command);

            case ArgoCommandTypes.Any:
                return ExecuteAny(command);

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public object ExecuteToList(ArgoCommand command)
    {
        using SqliteConnection con = CreateAndOpenConnection();
        using SqliteCommandCollection cmds = command.ToSqliteCommands();
        cmds.Connection = con;

        SqliteCommand cmd = ExecutePreCommandsAndGetCommand(cmds);

        SqliteDataReader reader = cmd.ExecuteReader();

        IList result = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(command.ResultingType))!;

        if (command.IsResultingTypeJson)
        {
            while (reader.Read())
            {
                string json = (string) reader[0];
                result.Add(JsonSerializer.Deserialize(json, command.ResultingType, _serializerOptions));
            }
        }
        else
        {
            while (reader.Read())
            {
                string json = (string)reader[0];
                SelectValueHolder valueHolder = SelectValueHolder.ParseFromJson(json, command.ResultingType, _serializerOptions);
                result.Add(valueHolder.GetValue());
            }
        }

        return result;
    }

    public object? ExecuteFirstOrDefault(ArgoCommand command)
    {
        using SqliteConnection con = CreateAndOpenConnection();
        using SqliteCommandCollection cmds = command.ToSqliteCommands();
        cmds.Connection = con;

        SqliteCommand cmd = ExecutePreCommandsAndGetCommand(cmds);

        string? json = cmd.ExecuteScalar() as string;

        if (json == null)
        {
            if (command.CommandType == ArgoCommandTypes.First)
            {
                throw new InvalidOperationException("Collection is empty");
            }

            return null;
        }

        return JsonSerializer.Deserialize(json, command.ResultingType, _serializerOptions);
    }

    public object? ExecuteSingleOrDefault(ArgoCommand command)
    {
        object? result = ExecuteFirstOrDefault(command);

        if (result is null && command.CommandType == ArgoCommandTypes.SingleOrDefault)
        {
            return null;
        }

        ArgoCommand countCommand = command.ConvertToLongCount(2);
        long count = (long)ExecuteCount(countCommand);

        if (count > 1)
        {
            throw new InvalidOperationException("More than one item found in collection.");
        }

        if (count == 0 && command.CommandType == ArgoCommandTypes.Single)
        {
            throw new InvalidOperationException("No item found in collection.");
        }

        return result;
    }

    public object ExecuteCount(ArgoCommand command)
    {
        using SqliteConnection con = CreateAndOpenConnection();
        using SqliteCommandCollection cmds = command.ToSqliteCommands();
        cmds.Connection = con;

        SqliteCommand cmd = ExecutePreCommandsAndGetCommand(cmds);

        object result = cmd.ExecuteScalar()
                        ?? throw new InvalidOperationException("Unexpected null result on ExecuteCount(ArgoCommand command)");

        long ret;

        if (result is int i) ret = i;
        else ret = (long)result;

        if (command.CommandType == ArgoCommandTypes.Count)
        {
            return (int)ret;
        }

        return ret;
    }

    public object ExecuteAny(ArgoCommand command)
    {
        object result = ExecuteCount(command);

        int value;
        if (result is int i) value = i;
        else value = (int)(long) result;

        return value == 1;
    }

    private SqliteCommand ExecutePreCommandsAndGetCommand(SqliteCommandCollection cmds)
    {
        if (cmds.Commands.Length > 1)
        {
            for (int i = 0; i < cmds.Commands.Length - 1; i++)
            {
                cmds.Commands[i].ExecuteNonQuery();
            }
        }

        return cmds.Commands.Last();
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
        if (op == null) throw new ArgumentNullException(nameof(op));
        if (tr == null) throw new ArgumentNullException(nameof(tr));
        if (serializerOptions == null) throw new ArgumentNullException(nameof(serializerOptions));

        SqliteCommand cmd = op.CreateCommand(serializerOptions);
        cmd.Connection = tr.Connection;
        cmd.Transaction = tr;

        if (op is InsertOperation && op.Metadata.IsKeyPropertyInt)
        {
            ExecuteInsert(tr, op, cmd);
        }
        else
        {
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    private void ExecuteInsert(SqliteTransaction tr, CrudOperation op, SqliteCommand cmd)
    {
        object serialId = cmd.ExecuteScalar() ?? throw new InvalidOperationException("Insert failed");

        if (serialId is int i)
        {
            if (op.Metadata.KeyPropertyType == typeof(int))
            {
                op.Metadata.SetKey(op.Document!, i);
            }
            else if (op.Metadata.KeyPropertyType == typeof(uint))
            {
                op.Metadata.SetKey(op.Document!, (uint)i);
            }
            else if (op.Metadata.KeyPropertyType == typeof(long))
            {
                op.Metadata.SetKey(op.Document!, (long)i);
            }
            else if (op.Metadata.KeyPropertyType == typeof(uint))
            {
                op.Metadata.SetKey(op.Document!, (uint)i);
            }
            else if (op.Metadata.KeyPropertyType == typeof(ulong))
            {
                op.Metadata.SetKey(op.Document!, (ulong)i);
            }
        }
        else if (serialId is long l)
        {
            if (op.Metadata.KeyPropertyType == typeof(int))
            {
                op.Metadata.SetKey(op.Document!, (int)l);
            }
            else if (op.Metadata.KeyPropertyType == typeof(uint))
            {
                op.Metadata.SetKey(op.Document!, (uint)l);
            }
            else if (op.Metadata.KeyPropertyType == typeof(long))
            {
                op.Metadata.SetKey(op.Document!, l);
            }
            else if (op.Metadata.KeyPropertyType == typeof(uint))
            {
                op.Metadata.SetKey(op.Document!, (uint)l);
            }
            else if (op.Metadata.KeyPropertyType == typeof(ulong))
            {
                op.Metadata.SetKey(op.Document!, (ulong)l);
            }
        }

        UpdateDocumentSerialIdAfterSerialInsert(tr, op.Metadata, serialId);
    }

    private void UpdateDocumentSerialIdAfterSerialInsert(SqliteTransaction tr, DocumentMetadata meta, object serialId)
    {
        string propName = ConvertPropertyName(meta.KeyPropertyName);

        string sql = $"""
            UPDATE {meta.DocumentName}
            SET jsonData = json_set(jsonData, '$.{propName}', {serialId})
            """;

        SqliteCommand cmd = tr.Connection!.CreateCommand();
        cmd.CommandText = sql;
        cmd.Transaction = tr;
        cmd.ExecuteNonQuery();
    }

    private SqliteConnection CreateAndOpenConnection()
    {
        SqliteConnection c = new SqliteConnection(_connectionString);
        c.Open();

        return c;
    }

    private static string ConvertPropertyName(string propertyName)
    {
        propertyName = JsonNamingPolicy.CamelCase.ConvertName(propertyName);
        return propertyName.Replace("'", "''");
    }
}