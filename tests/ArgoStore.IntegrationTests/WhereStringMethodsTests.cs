using System.Collections.Generic;
using System.Linq;
using ArgoStore.IntegrationTests.Entities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace ArgoStore.IntegrationTests
{
    public class WhereStringMethodsTests : IntegrationTestsBase
    {
        private readonly TestData _td;
        private const string TestNameImogenCampbell = "Imogen Campbell";

        public WhereStringMethodsTests(ITestOutputHelper output) : base(output)
        {
            _td = new TestData(TestDbConnectionString);
            _td.InsertTestPersons();
        }

        [SkippableFact]
        public void StringEqualsOperator_GivesExpectedResult()
        {
            using IQueryDocumentSession s = GetNewDocumentSession();

            List<Person> persons = s.Query<Person>().Where(x => x.Name == TestNameImogenCampbell).ToList();
            persons.Should().ContainSingle();

            persons = s.Query<Person>().Where(x => x.Name == TestNameImogenCampbell + " non existing").ToList();
            persons.Should().BeEmpty();
        }

        [SkippableFact]
        public void StringNotEqualsOperator_GivesExpectedResult()
        {
            using IQueryDocumentSession s = GetNewDocumentSession();

            List<Person> persons = s.Query<Person>().Where(x => x.Name != TestNameImogenCampbell).ToList();
            persons.Should().HaveCountGreaterThan(1);
        }

        [SkippableFact]
        public void StringEqualsMethod_GivesExpectedResult()
        {
            using IQueryDocumentSession s = GetNewDocumentSession();

            List<Person> persons = s.Query<Person>().Where(x => x.Name.Equals(TestNameImogenCampbell)).ToList();
            persons.Should().ContainSingle();
        }


        [SkippableFact]
        public void NegatedStringEqualsMethod_GivesExpectedResult()
        {
            using IQueryDocumentSession s = GetNewDocumentSession();

            List<Person> persons = s.Query<Person>().Where(x => !(x.Name.Equals(TestNameImogenCampbell))).ToList();
            persons.Should().HaveCount(_td.Persons.Count - 1);
        }


        [SkippableFact]
        public void StringEqualsStaticMethod_GivesExpectedResult()
        {
            using IQueryDocumentSession s = GetNewDocumentSession();

            List<Person> persons = s.Query<Person>().Where(x => string.Equals(x.Name, TestNameImogenCampbell)).ToList();
            persons.Should().ContainSingle();
        }
        
        [SkippableFact]
        public void StringEqualsOnParameterMethod_GivesExpectedResult()
        {
            using IQueryDocumentSession s = GetNewDocumentSession();

            List<Person> persons = s.Query<Person>().Where(x => TestNameImogenCampbell.Equals(x.Name)).ToList();
            persons.Should().ContainSingle(x => x.Name == TestNameImogenCampbell);
        }

        [SkippableFact]
        public void NegatedStringEqualsOnParameterMethod_GivesExpectedResult()
        {
            using IQueryDocumentSession s = GetNewDocumentSession();

            List<Person> persons = s.Query<Person>().Where(x => !TestNameImogenCampbell.Equals(x.Name)).ToList();
            persons.Should().HaveCount(_td.Persons.Count - 1);
        }
    }
}
