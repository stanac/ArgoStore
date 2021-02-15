using ArgoStore.Helpers;
using ArgoStore.IntegrationTests.Entities;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ArgoStore.IntegrationTests
{
    public class TestData
    {
        private readonly string _connectionString;

        private static readonly string _personsJson = @"[{
  ""id"": ""784bcc08-ce14-4854-9c04-ab6a57a62100"",
  ""name"": ""Imogen Campbell"",
  ""emailAddress"": ""imogell@a.example.com"",
  ""birthYear"": 1990,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cackeDay"": ""2020-09-20""
},{
  ""id"": ""1fcfe4b7-7d04-4fb7-b0e7-406f5c106811"",
  ""name"": ""Neil HughesBell"",
  ""emailAddress"": ""neilhes@a.example.com"",
  ""birthYear"": 1991,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.111Z"",
  ""cackeDay"": ""2020-09-19""
},{
  ""id"": ""cdb1ae3a-476e-496b-9c57-53136b853f40"",
  ""name"": ""Candice Ellis"",
  ""emailAddress"": ""candicis@b.example.com"",
  ""birthYear"": 1992,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.333Z"",
  ""cackeDay"": ""2020-09-18""
},{
  ""id"": ""ba194361-3496-489e-b9c7-5feb29acc4c8"",
  ""name"": ""Charlie Palmer"",
  ""emailAddress"": ""charler@b.example.com"",
  ""birthYear"": 1993,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T11:23:29.321Z"",
  ""cackeDay"": ""2020-09-17""
},{
  ""id"": ""eb88dadd-ce09-446e-86bf-135bf85f25e4"",
  ""name"": ""Jordan Simpson"",
  ""emailAddress"": ""jordaon@example.com"",
  ""birthYear"": 1994,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T12:23:29.321Z"",
  ""cackeDay"": ""2020-09-16""
},{
  ""id"": ""1209ef1e-3a4c-43b2-b5d3-578a7ff17831"",
  ""name"": ""Justine Graham"",
  ""emailAddress"": ""justham@example.com"",
  ""birthYear"": null,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T13:23:29.321Z"",
  ""cackeDay"": ""2020-09-15""
},{
  ""id"": ""ca1b10b6-8b1d-4c12-b8b7-76653ac3d95e"",
  ""name"": ""Alex Lewis"",
  ""emailAddress"": ""alexlis@example.com"",
  ""birthYear"": null,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T14:23:29.321Z"",
  ""cackeDay"": ""2020-09-14""
},{
  ""id"": ""9c7dc4f7-4cfc-4159-8483-273d43cd034e"",
  ""name"": ""Elliot Thomas"",
  ""emailAddress"": ""elliotmas@example.com"",
  ""birthYear"": 1981,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T15:23:29.321Z"",
  ""cackeDay"": ""2020-09-13""
},{
  ""id"": ""ded6562f-62c7-4395-b197-600caeb3763f"",
  ""name"": ""Amber Khan"",
  ""emailAddress"": ""amberkan@example.com"",
  ""birthYear"": 1982,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cackeDay"": ""2020-09-12""
},{
  ""id"": ""24950d28-6aae-4689-8e67-9bda4064043c"",
  ""name"": ""Dale Murphy"",
  ""emailAddress"": ""dalemuhy@example.com"",
  ""birthYear"": 1983,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cackeDay"": ""2020-09-11""
},{
  ""id"": ""6cdadae1-51ee-4ac1-90a3-28b9ba10f5af"",
  ""name"": ""Tracy Wilson"",
  ""emailAddress"": ""tracyon@example.com"",
  ""birthYear"": 1983,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cackeDay"": ""2020-09-10""
},{
  ""id"": ""c96d2a56-a92e-4e03-93c5-9bcf5aae30b9"",
  ""name"": ""Jonathan Bennett"",
  ""emailAddress"": ""jonathtt@example.com"",
  ""birthYear"": 1983,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cackeDay"": ""2020-09-09""
},{
  ""id"": ""a9a745ed-b326-4c82-aa5d-00324884c1e5"",
  ""name"": ""Isabelle Martin"",
  ""emailAddress"": ""isabelin@example.com"",
  ""birthYear"": 1984,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cackeDay"": ""2020-09-08""
},{
  ""id"": ""ceea12cc-e4eb-4a0b-9f9d-a60ec9b8f3d7"",
  ""name"": ""Karlie Griffiths"",
  ""emailAddress"": ""karlths@example.com"",
  ""birthYear"": 1984,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cackeDay"": ""2020-09-07""
},{
  ""id"": ""dfcfe066-981d-4fea-addb-de578bcd4365"",
  ""name"": ""Sebastian Ellis"",
  ""emailAddress"": ""sebalis@example.com"",
  ""birthYear"": 1984,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cackeDay"": ""2020-09-07""
},{
  ""id"": ""0a6a86f5-5ae0-4357-8a17-2b90b45ac1df"",
  ""name"": ""Carmen Matthews"",
  ""emailAddress"": ""carmews@example.com"",
  ""birthYear"": 1984,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cackeDay"": ""2020-09-07""
},{
  ""id"": ""9f25788a-1da1-4692-a66d-527baedab864"",
  ""name"": ""Pauline Richardson"",
  ""emailAddress"": ""paulson@example.com"",
  ""birthYear"": 1984,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cackeDay"": null
},{
  ""id"": ""aca3588e-e63c-4950-ad20-ed3515bf244f"",
  ""name"": ""Eleanor Cooper"",
  ""emailAddress"": ""eleaner@example.com"",
  ""birthYear"": 1988,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cackeDay"": ""2020-09-07""
},{
  ""id"": ""28982c51-e2c1-4367-9e9d-ee1e4474c03e"",
  ""name"": ""Logan Lloyd"",
  ""emailAddress"": ""logayd@example.com"",
  ""birthYear"": 1988,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cackeDay"": null
},{
  ""id"": ""f523061c-eb38-4567-80c7-57398cec1ef8"",
  ""name"": ""Kyle Mitchell"",
  ""emailAddress"": ""kylemell@example.com"",
  ""birthYear"": 1985,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cackeDay"": ""2020-09-06""
},{
  ""id"": ""d21ec145-99cf-4737-ae7b-4756fe9aecfa"",
  ""name"": ""Ben Mason"",
  ""emailAddress"": ""benmasson@example.com"",
  ""birthYear"": 1999,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cackeDay"": ""2020-09-05""
},{
  ""id"": ""146855dd-ed0c-4de8-8b3c-2af80cc9a164"",
  ""name"": ""Dave Cook"",
  ""emailAddress"": ""davecook@example.com"",
  ""birthYear"": 1972,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cackeDay"": ""2020-09-04""
},{
  ""id"": ""3a7d4861-a789-4ae0-a49b-e77fc7b32247"",
  ""name"": ""Ruth Campbell"",
  ""emailAddress"": ""ruthcll@example.com"",
  ""birthYear"": 1973,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cackeDay"": ""2020-09-03""
},{
  ""id"": ""e8f7f88c-68af-41ca-8e6d-df1dc29d7b2c"",
  ""name"": ""Anthony Hill"",
  ""emailAddress"": ""anthoill@example.com"",
  ""birthYear"": 1974,
  ""emailConfirmed"": true,
  ""roles"": [""user-read""],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cackeDay"": ""2020-09-02""
},{
  ""id"": ""d4edb9af-9b53-44c7-acc4-3ff0c9d2c136"",
  ""name"": ""Kieran Lewis"",
  ""emailAddress"": ""kierwis@example.com"",
  ""birthYear"": 1974,
  ""emailConfirmed"": false,
  ""roles"": [""user-read"", ""user-write""],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cackeDay"": ""2020-08-01""
},{
  ""id"": ""4f9fd8f8-9ac8-4e5e-bee5-bf51b0a2622f"",
  ""name"": ""Lisa Shaw"",
  ""emailAddress"": ""lisashhaw@example.com"",
  ""birthYear"": 1975,
  ""emailConfirmed"": false,
  ""roles"": [""admin""],
  ""registrationTime"": ""2020-09-19T10:23:29.32Z"",
  ""cackeDay"": ""2020-09-01""
}]";

        public TestData(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException($"'{nameof(connectionString)}' cannot be null or whitespace", nameof(connectionString));
            }

            _connectionString = connectionString;

            var th = new EntityTableHelper(new Configuration
            {
                ConnectionString = _connectionString,
                CreateEntitiesOnTheFly = true,
                Serializer = new ArgoStoreSerializer()
            });

            th.EnsureEntityTableExists<Person>();
        }

        internal void InsertTestPersons()
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    InsertTestPersonsInner();
                    return;
                }
                catch (SqliteException ex)
                {
                    if (ex.Message.Contains("no such table"))
                    {
                        Thread.Sleep(100);

                        var th = new EntityTableHelper(new Configuration
                        {
                            ConnectionString = _connectionString,
                            CreateEntitiesOnTheFly = true,
                            Serializer = new ArgoStoreSerializer()
                        });

                        th.EnsureEntityTableExists<Person>();
                        Thread.Sleep(100);
                        InsertTestPersonsInner();
                        return;
                    }
                }
            }
        }

        internal void InsertTestPersonsInner()
        {
            EntityTableHelper dbTableHelper = new EntityTableHelper(new Configuration
            {
                ConnectionString = _connectionString,
                CreateEntitiesOnTheFly = true
            });

            dbTableHelper.EnsureEntityTableExists<Person>();
            dbTableHelper.EnsureEntityTableExists<Person>();

            ArgoStoreSerializer s = new ArgoStoreSerializer();
            var persons = s.Deserialize<List<Person>>(_personsJson);

            using (var c = new SqliteConnection(_connectionString))
            {
                c.Open();

                foreach (var p in persons)
                {
                    string json = s.Serialize(p);

                    string sql = $@"
                        INSERT INTO {EntityTableHelper.GetTableName<Person>()} (string_id, json_data, created_at)
                        VALUES (@id, json(@jsonData), @createdTime)
                    ";

                    var cmd = c.CreateCommand();
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@id", p.Id.ToString("N"));
                    cmd.Parameters.AddWithValue("@jsonData", json);
                    cmd.Parameters.AddWithValue("@createdTime", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
