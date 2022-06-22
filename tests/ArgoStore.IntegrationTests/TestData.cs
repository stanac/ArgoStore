using ArgoStore.Helpers;
using ArgoStore.IntegrationTests.Entities;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

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
  ""cakeDay"": ""2020-09-20"",
  ""NickName"": ""Im""
},{
  ""id"": ""1fcfe4b7-7d04-4fb7-b0e7-406f5c106811"",
  ""name"": ""Neil HughesBell"",
  ""emailAddress"": ""neilhes@a.example.com"",
  ""birthYear"": 1991,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.111Z"",
  ""cakeDay"": ""2020-09-19"",
  ""NickName"": null
},{
  ""id"": ""cdb1ae3a-476e-496b-9c57-53136b853f40"",
  ""name"": ""Candice Ellis"",
  ""emailAddress"": ""candicis@b.example.com"",
  ""birthYear"": 1992,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.333Z"",
  ""cakeDay"": ""2020-09-18"",
  ""NickName"": """"
},{
  ""id"": ""ba194361-3496-489e-b9c7-5feb29acc4c8"",
  ""name"": ""Charlie Palmer"",
  ""emailAddress"": ""charler@b.example.com"",
  ""birthYear"": 1993,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T11:23:29.321Z"",
  ""cakeDay"": ""2020-09-17"",
  ""NickName"": ""  ""
},{
  ""id"": ""eb88dadd-ce09-446e-86bf-135bf85f25e4"",
  ""name"": ""Jordan Simpson"",
  ""emailAddress"": ""jordaon@example.com"",
  ""birthYear"": 1994,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T12:23:29.321Z"",
  ""cakeDay"": ""2020-09-16"",
  ""NickName"": ""Sim""
},{
  ""id"": ""1209ef1e-3a4c-43b2-b5d3-578a7ff17831"",
  ""name"": ""Justine Graham"",
  ""emailAddress"": ""justham@example.com"",
  ""birthYear"": null,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T13:23:29.321Z"",
  ""cakeDay"": ""2020-09-15"",
  ""NickName"": ""Gram""
},{
  ""id"": ""ca1b10b6-8b1d-4c12-b8b7-76653ac3d95e"",
  ""name"": ""Alex Lewis"",
  ""emailAddress"": ""alexlis@example.com"",
  ""birthYear"": null,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T14:23:29.321Z"",
  ""cakeDay"": ""2020-09-14"",
  ""NickName"": ""Ali""
},{
  ""id"": ""9c7dc4f7-4cfc-4159-8483-273d43cd034e"",
  ""name"": ""Elliot Thomas"",
  ""emailAddress"": ""elliotmas@example.com"",
  ""birthYear"": 1981,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T15:23:29.321Z"",
  ""cakeDay"": ""2020-09-13"",
  ""NickName"": ""El""
},{
  ""id"": ""ded6562f-62c7-4395-b197-600caeb3763f"",
  ""name"": ""Amber Khan"",
  ""emailAddress"": ""amberkan@example.com"",
  ""birthYear"": 1982,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cakeDay"": ""2020-09-12"",
  ""NickName"": ""Amy""
},{
  ""id"": ""24950d28-6aae-4689-8e67-9bda4064043c"",
  ""name"": ""Dale Murphy"",
  ""emailAddress"": ""dalemuhy@example.com"",
  ""birthYear"": 1983,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cakeDay"": ""2020-09-11"",
  ""NickName"": ""Dally""
},{
  ""id"": ""6cdadae1-51ee-4ac1-90a3-28b9ba10f5af"",
  ""name"": ""Tracy Wilson"",
  ""emailAddress"": ""tracyon@example.com"",
  ""birthYear"": 1983,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cakeDay"": ""2020-09-10"",
  ""NickName"": ""Tracy Will""
},{
  ""id"": ""c96d2a56-a92e-4e03-93c5-9bcf5aae30b9"",
  ""name"": ""Jonathan Bennett"",
  ""emailAddress"": ""jonathtt@example.com"",
  ""birthYear"": 1983,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cakeDay"": ""2020-09-09"",
  ""NickName"": ""Johny""
},{
  ""id"": ""a9a745ed-b326-4c82-aa5d-00324884c1e5"",
  ""name"": ""Isabelle Martin"",
  ""emailAddress"": ""isabelin@example.com"",
  ""birthYear"": 1984,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cakeDay"": ""2020-09-08"",
  ""NickName"": ""Easy Bell""
},{
  ""id"": ""ceea12cc-e4eb-4a0b-9f9d-a60ec9b8f3d7"",
  ""name"": ""Karlie Griffiths"",
  ""emailAddress"": ""karlths@example.com"",
  ""birthYear"": 1984,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cakeDay"": ""2020-09-07"",
  ""NickName"": ""Karlito""
},{
  ""id"": ""dfcfe066-981d-4fea-addb-de578bcd4365"",
  ""name"": ""Sebastian Ellis"",
  ""emailAddress"": ""sebalis@example.com"",
  ""birthYear"": 1984,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cakeDay"": ""2020-09-07"",
  ""NickName"": ""Seba""
},{
  ""id"": ""0a6a86f5-5ae0-4357-8a17-2b90b45ac1df"",
  ""name"": ""Carmen Matthews"",
  ""emailAddress"": ""carmews@example.com"",
  ""birthYear"": 1984,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cakeDay"": ""2020-09-07"",
  ""NickName"": ""Cat""
},{
  ""id"": ""9f25788a-1da1-4692-a66d-527baedab864"",
  ""name"": ""Pauline Richardson"",
  ""emailAddress"": ""paulson@example.com"",
  ""birthYear"": 1984,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cakeDay"": null,
  ""NickName"": ""Pauly""
},{
  ""id"": ""aca3588e-e63c-4950-ad20-ed3515bf244f"",
  ""name"": ""Eleanor Cooper"",
  ""emailAddress"": ""eleaner@example.com"",
  ""birthYear"": 1988,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cakeDay"": ""2020-09-07"",
  ""NickName"": ""Elly""
},{
  ""id"": ""28982c51-e2c1-4367-9e9d-ee1e4474c03e"",
  ""name"": ""Logan Lloyd"",
  ""emailAddress"": ""logayd@example.com"",
  ""birthYear"": 1988,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cakeDay"": null,
  ""NickName"": ""Log""
},{
  ""id"": ""f523061c-eb38-4567-80c7-57398cec1ef8"",
  ""name"": ""Kyle Mitchell"",
  ""emailAddress"": ""kylemell@example.com"",
  ""birthYear"": 1985,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cakeDay"": ""2020-09-06"",
  ""NickName"": ""K""
},{
  ""id"": ""d21ec145-99cf-4737-ae7b-4756fe9aecfa"",
  ""name"": ""Ben Mason"",
  ""emailAddress"": ""benmasson@example.com"",
  ""birthYear"": 1999,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cakeDay"": ""2020-09-05"",
  ""NickName"": ""Ben""
},{
  ""id"": ""146855dd-ed0c-4de8-8b3c-2af80cc9a164"",
  ""name"": ""Dave Cook"",
  ""emailAddress"": ""davecook@example.com"",
  ""birthYear"": 1972,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cakeDay"": ""2020-09-04"",
  ""NickName"": ""Davo""
},{
  ""id"": ""3a7d4861-a789-4ae0-a49b-e77fc7b32247"",
  ""name"": ""Ruth Campbell"",
  ""emailAddress"": ""ruthcll@example.com"",
  ""birthYear"": 1973,
  ""emailConfirmed"": true,
  ""roles"": [],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cakeDay"": ""2020-09-03"",
  ""NickName"": ""Ruthy""
},{
  ""id"": ""e8f7f88c-68af-41ca-8e6d-df1dc29d7b2c"",
  ""name"": ""Anthony Hill"",
  ""emailAddress"": ""anthoill@example.com"",
  ""birthYear"": 1974,
  ""emailConfirmed"": true,
  ""roles"": [""user-read""],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cakeDay"": ""2020-09-02"",
  ""NickName"": ""Anton""
},{
  ""id"": ""d4edb9af-9b53-44c7-acc4-3ff0c9d2c136"",
  ""name"": ""Kieran Lewis"",
  ""emailAddress"": ""kierwis@example.com"",
  ""birthYear"": 1974,
  ""emailConfirmed"": false,
  ""roles"": [""user-read"", ""user-write""],
  ""registrationTime"": ""2020-09-19T10:23:29.321Z"",
  ""cakeDay"": ""2020-08-01"",
  ""NickName"": ""Key2""
},{
  ""id"": ""4f9fd8f8-9ac8-4e5e-bee5-bf51b0a2622f"",
  ""name"": ""Lisa Shaw"",
  ""emailAddress"": ""lisashhaw@example.com"",
  ""birthYear"": 1975,
  ""emailConfirmed"": false,
  ""roles"": [""admin""],
  ""registrationTime"": ""2020-09-19T10:23:29.32Z"",
  ""cakeDay"": ""2020-09-01"",
  ""NickName"": ""Lissy""
}]";

        private readonly IReadOnlyList<PersonIntPk> _intPersons;

        public IReadOnlyList<Person> Persons { get; }

        public IReadOnlyList<PersonIntPk> PersonIntPkValues => _intPersons.Select(x => x.Copy()).ToList();

        public TestData(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException($"'{nameof(connectionString)}' cannot be null or whitespace", nameof(connectionString));
            }

            _connectionString = connectionString;

            ArgoStoreSerializer s = new ArgoStoreSerializer();
            Persons = s.Deserialize<List<Person>>(_personsJson);

            List<PersonIntPk> intPersons = new List<PersonIntPk>();

            for (int i = 0; i < 30; i++)
            {
                intPersons.Add(new PersonIntPk
                {
                    BirthYear = 1999 - i,
                    EmailAddress = $"testintperson{i}@example.com",
                    Name = $"Integer Person {i}"
                });
            }

            _intPersons = intPersons;

            var th = new EntityTableHelper(new Configuration
            {
                ConnectionString = _connectionString,
                CreateEntitiesOnTheFly = true,
                Serializer = new ArgoStoreSerializer()
            });

            th.CreateDocumentTableIfNotExists<Person>();
        }

        internal void InsertTestPersons()
        {
            EntityTableHelper dbTableHelper = new EntityTableHelper(new Configuration
            {
                ConnectionString = _connectionString,
                CreateEntitiesOnTheFly = true
            });

            dbTableHelper.CreateDocumentTableIfNotExists<Person>();
            dbTableHelper.CreateDocumentTableIfNotExists<Person>();

            using var c = new SqliteConnection(_connectionString);

            c.Open();
            ArgoStoreSerializer s = new ArgoStoreSerializer();

            foreach (var p in Persons)
            {
                string json = s.Serialize(p);

                string sql = $@"
                        INSERT INTO {EntityTableHelper.GetTableName<Person>()} (string_id, json_data, created_at, tenant_id)
                        VALUES (@id, json(@jsonData), @createdTime, @tenantId)
                    ";

                SqliteCommand cmd = c.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@id", p.Id.ToString());
                cmd.Parameters.AddWithValue("@jsonData", json);
                cmd.Parameters.AddWithValue("@createdTime", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@tenantId", TenantIdDefault.DefaultValue);

                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteTestPersons()
        {
            string sql = $"DELETE FROM {EntityTableHelper.GetTableName<Person>()}";

            using var c = new SqliteConnection(_connectionString);

            c.Open();

            SqliteCommand cmd = c.CreateCommand();
            cmd.CommandText = sql;

            cmd.ExecuteNonQuery();
        }
    }
}
