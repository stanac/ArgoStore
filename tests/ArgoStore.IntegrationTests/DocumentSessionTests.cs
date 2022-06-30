using ArgoStore.IntegrationTests.Entities;

namespace ArgoStore.IntegrationTests
{
    public class DocumentSessionTests : IntegrationTestsBase
    {
        private readonly TestData _td;

        public DocumentSessionTests(ITestOutputHelper output) : base(output)
        {
            _td = new TestData(TestDbConnectionString);
        }

        [SkippableFact]
        public void Insert_Execute_IdSet()
        {
            using IDocumentSession s = GetNewDocumentSession();

            PersonIntPk p0 = _td.PersonIntPkValues[0].Copy();
            PersonIntPk p1 = _td.PersonIntPkValues[1].Copy();

            s.Insert(p0);
            s.Insert(p1);

            p0.Id.Should().Be(p1.Id);

            s.Execute();

            p0.Id.Should().NotBe(p1.Id);
        }

        [SkippableFact]
        public void Insert_Execute_SaveChanges_EntityIsInDb()
        {
            using IDocumentSession s = GetNewDocumentSession();

            PersonIntPk p0 = _td.PersonIntPkValues[0].Copy();

            s.Insert(p0);
            
            s.Execute();

            s.SaveChanges();

            bool exists = s.Query<PersonIntPk>().Any(x => x.EmailAddress == p0.EmailAddress);
            
            exists.Should().BeTrue();
        }

        [SkippableFact]
        public void Insert_Execute_EntityNotIsInDb()
        {
            using IDocumentSession s = GetNewDocumentSession();

            PersonIntPk p0 = _td.PersonIntPkValues[0].Copy();

            s.Insert(p0);
            
            s.Execute();
            
            bool exists = s.Query<PersonIntPk>().Any(x => x.EmailAddress == p0.EmailAddress);

            exists.Should().BeFalse();
        }
        
        [SkippableFact]
        public void Insert_Execute_DiscardChanges_SaveChanges_EntityNotIsInDb()
        {
            using IDocumentSession s = GetNewDocumentSession();

            PersonIntPk p0 = _td.PersonIntPkValues[0].Copy();

            s.Insert(p0);

            s.Execute();

            s.DiscardChanges();
            
            s.SaveChanges();

            bool exists = s.Query<PersonIntPk>().Any(x => x.EmailAddress == p0.EmailAddress);

            exists.Should().BeFalse();
        }
    }
}
