using System.Collections;
using System.Text.Json;
using ArgoStore.CrudOperations;
using ArgoStore.Helpers;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace ArgoStore.Command;

internal class ArgoCommandExecutor
{
    private readonly string _connectionString;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly ILogger<ArgoCommandExecutor> _logger;
    private readonly SessionId _sessionId;

    public ArgoCommandExecutor(string connectionString, JsonSerializerOptions serializerOptions, ILogger<ArgoCommandExecutor> logger, SessionId sessionId)
    {
        _connectionString = connectionString;
        _serializerOptions = serializerOptions;
        _logger = logger;
        _sessionId = sessionId;
    }

    public object? Execute(ArgoCommand command, ArgoActivity? argoActivity)
    {
        ArgoActivity? ca = argoActivity?.CreateChild("Execute");

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Execute command type: {CommandType} in session: {SessionId} with command text: " +
                             Environment.NewLine + "{CommandText}",
                command.CommandType, _sessionId.Id, command.Sql);
        }

        object? result;

        switch (command.CommandType)
        {
            case ArgoCommandTypes.NonQuery:
                throw new NotImplementedException();

            case ArgoCommandTypes.Count:
            case ArgoCommandTypes.LongCount:
                result = ExecuteCount(command, ca);
                break;

            case ArgoCommandTypes.ToList:
                result = ExecuteToList(command, ca);
                break;

            case ArgoCommandTypes.First:
            case ArgoCommandTypes.FirstOrDefault:
                result = ExecuteFirstOrDefault(command, ca);
                break;

            case ArgoCommandTypes.Single:
            case ArgoCommandTypes.SingleOrDefault:
                result = ExecuteSingleOrDefault(command, ca);
                break;

            case ArgoCommandTypes.Any:
                result = ExecuteAny(command, ca);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        ca?.Stop();

        return result;
    }

    public object ExecuteToList(ArgoCommand command, ArgoActivity? activity)
    {
        ArgoActivity? ca = activity?.CreateChild("ExecuteToList");

        using SqliteConnection con = CreateAndOpenConnection(ca);
        using SqliteCommandCollection cmds = command.ToSqliteCommands();
        cmds.Connection = con;

        ArgoActivity? executePreCommands = ca?.CreateChild("ExecutePreCommands");

        SqliteCommand cmd = ExecutePreCommandsAndGetCommand(cmds);

        ArgoActivity? executeReaderActivity = executePreCommands?.StopAndCreateSibling("ExecuteReader");

        SqliteDataReader reader = cmd.ExecuteReader();

        ArgoActivity? createListInstanceActivity = executeReaderActivity?.StopAndCreateSibling("CreateListInstance");

        IList result = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(command.ResultingType))!;

        createListInstanceActivity?.Stop();

        if (command.IsResultingTypeJson)
        {
            while (reader.Read())
            {
                string json = (string) reader[0];

                ArgoActivity? deserializeActivity = ca?.CreateChild("Deserialize");

                result.Add(JsonSerializer.Deserialize(json, command.ResultingType, _serializerOptions));

                deserializeActivity?.Stop();
            }
        }
        else
        {
            while (reader.Read())
            {
                string json = (string)reader[0];

                ArgoActivity? deserializeActivity = ca?.CreateChild("Deserialize");

                SelectValueHolder valueHolder = SelectValueHolder.ParseFromJson(json, command.ResultingType, _serializerOptions);
                result.Add(valueHolder.GetValue());

                deserializeActivity?.Stop();
            }
        }

        ca?.Stop();

        return result;
    }

    public object? ExecuteFirstOrDefault(ArgoCommand command, ArgoActivity? activity)
    {
        ArgoActivity? ca = activity?.CreateChild("ExecuteFirstOrDefault");

        using SqliteConnection con = CreateAndOpenConnection(ca);
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

        object? result = JsonSerializer.Deserialize(json, command.ResultingType, _serializerOptions);

        ca?.Stop();

        return result;
    }

    public object? ExecuteSingleOrDefault(ArgoCommand command, ArgoActivity? activity)
    {
        ArgoActivity? ca = activity?.CreateChild("ExecuteSingleOrDefault");

        object? result = ExecuteFirstOrDefault(command, activity);

        if (result is null && command.CommandType == ArgoCommandTypes.SingleOrDefault)
        {
            return null;
        }

        ArgoCommand countCommand = command.ConvertToLongCount(2);
        long count = (long)ExecuteCount(countCommand, ca);

        if (count > 1)
        {
            throw new InvalidOperationException("More than one item found in collection.");
        }

        if (count == 0 && command.CommandType == ArgoCommandTypes.Single)
        {
            throw new InvalidOperationException("No item found in collection.");
        }

        ca?.Stop();

        return result;
    }

    public object ExecuteCount(ArgoCommand command, ArgoActivity? activity)
    {
        ArgoActivity? ca = activity?.CreateChild("ExecuteCount");
        
        using SqliteConnection con = CreateAndOpenConnection(ca);
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
        
        ca?.Stop();

        return ret;
    }

    public object ExecuteAny(ArgoCommand command, ArgoActivity? activity)
    {
        ArgoActivity? ca = activity?.CreateChild("ExecuteAny");

        object result = ExecuteCount(command, ca);

        int value;
        if (result is int i) value = i;
        else value = (int)(long) result;

        ca?.Stop();
        
        return value > 0;
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

        using SqliteConnection con = CreateAndOpenConnection(null);
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

        cmd.EnsureNoGuidParams();
        cmd.Connection = tr.Connection;
        cmd.Transaction = tr;

        cmd.ExecuteNonQuery();
    }
    
    private SqliteConnection CreateAndOpenConnection(ArgoActivity? activity)
    {
        ArgoActivity? ca = activity?.CreateChild("CreateAndOpenConnection");

        SqliteConnection c = new SqliteConnection(_connectionString);
        c.Open();

        ca?.Stop();

        return c;
    }
}