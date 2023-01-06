﻿using ArgoStore.TestsCommon.Entities;
using ArgoStore.TestsCommon.TestData;

namespace ArgoStore.IntegrationTests;

public class IntegrationTestBase : IDisposable
{
    protected TestDb TestDb { get; private set; } = TestDb.CreateNew();
    protected ArgoDocumentStore Store { get; private set; }

    public IntegrationTestBase()
    {
        Initialize();
    }
    
    protected void AddTestPerson()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        s.Insert(PersonTestData.GetPersonTestData().First());
        s.SaveChanges();
    }

    protected void AddTestPersons()
    {
        using IArgoDocumentSession s = Store.OpenSession();
        
        s.Insert(PersonTestData.GetPersonTestData());
        s.SaveChanges();
    }

    protected void UseFileDb()
    {
        Dispose();

        TestDb = new OnDiskTestDb();
        Initialize();
    }
    
    public void Dispose()
    {
        TestDb.Dispose();
    }

    private void Initialize()
    {
        Store = new ArgoDocumentStore(TestDb.ConnectionString);
        Store.RegisterDocumentType<Person>();
    }
}