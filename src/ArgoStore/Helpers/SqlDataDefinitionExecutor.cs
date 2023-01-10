﻿using ArgoStore.Config;
using Microsoft.Data.Sqlite;

namespace ArgoStore.Helpers;

internal class SqlDataDefinitionExecutor
{
    private readonly object Sync = new();
    private readonly HashSet<string> CreatedObjects = new();

    private readonly string _connectionString;

    public SqlDataDefinitionExecutor(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))  throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
        _connectionString = connectionString;
    }

    public void CreateDocumentObjects(DocumentMetadata meta)
    {
        CreateTable(meta.DocumentName);

        // TODO: create indexes
    }

    private void CreateTable(string documentName)
    {
        // ReSharper disable once InconsistentlySynchronizedField
        if (CreatedObjects.Contains(documentName))
        {
            return;
        }

        lock (Sync)
        {
            if (CreatedObjects.Contains(documentName))
            {
                return;
            }

            string[] sql =
            {
                $"""
                CREATE TABLE IF NOT EXISTS {documentName}    (
                    serialId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    stringId TEXT NOT NULL,
                    jsonData JSON NOT NULL,
                    tenantId TEXT NOT NULL,
                    createdAt BIGINT NOT NULL,
                    updatedAt BIGINT NULL
                )
            """ ,
                $"""
                CREATE UNIQUE INDEX IF NOT EXISTS ux_{documentName}_tenant_stringId
                ON {documentName}  (tenantId, stringId)
            """ ,
                $"""
                CREATE INDEX IF NOT EXISTS ix_{documentName}_tenant 
                ON {documentName} (tenantId)
            """ /*,
            $"""
            CREATE INDEX IF NOT EXISTS ix_{documentName}_createdAt
            ON {documentName} (createdAt)
            """,
            $"""
            CREATE INDEX IF NOT EXISTS ix_{documentName}_updatedAt
            ON {documentName} (updatedAt)
            """*/
            };

            using SqliteConnection c = GetAndOpenConnection();

            foreach (string s in sql)
            {
                SqliteCommand cmd = c.CreateCommand();
                cmd.CommandText = s;
                cmd.ExecuteNonQuery();
            }

            CreatedObjects.Add(documentName);
        }
    }

    private SqliteConnection GetAndOpenConnection()
    {
        SqliteConnection c = new SqliteConnection(_connectionString);
        c.Open();
        return c;
    }

}