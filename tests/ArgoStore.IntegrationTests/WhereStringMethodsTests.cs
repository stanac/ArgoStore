using System;
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
        private const string TestNameNeilHughesBell = "Neil HughesBell";
        private const string TestNameNeil = "Neil";
        private const string TestNameHughesBell = "HughesBell";

        public WhereStringMethodsTests(ITestOutputHelper output) : base(output)
        {
            _td = new TestData(TestDbConnectionString);
            _td.InsertTestPersons();
        }

        [SkippableFact]
        public void StringEqualsOperator_GivesExpectedResult()
        {
            using IQueryDocumentSession s = GetNewDocumentSession();

            List<Person> persons = s.Query<Person>().Where(x => x.Name == TestNameNeilHughesBell).ToList();
            persons.Should().ContainSingle();

            persons = s.Query<Person>().Where(x => x.Name == TestNameNeilHughesBell + " non existing").ToList();
            persons.Should().BeEmpty();
        }

        [SkippableFact]
        public void StringNotEqualsOperator_GivesExpectedResult()
        {
            using IQueryDocumentSession s = GetNewDocumentSession();

            List<Person> persons = s.Query<Person>().Where(x => x.Name != TestNameNeilHughesBell).ToList();
            persons.Should().HaveCountGreaterThan(1);
        }

        [SkippableFact]
        public void StringEqualsMethod_GivesExpectedResult()
        {
            using IQueryDocumentSession s = GetNewDocumentSession();

            List<Person> persons = s.Query<Person>().Where(x => x.Name.Equals(TestNameNeilHughesBell)).ToList();
            persons.Should().ContainSingle();
        }


        [SkippableFact]
        public void NegatedStringEqualsMethod_GivesExpectedResult()
        {
            using IQueryDocumentSession s = GetNewDocumentSession();

            List<Person> persons = s.Query<Person>().Where(x => !(x.Name.Equals(TestNameNeilHughesBell))).ToList();
            persons.Should().HaveCount(_td.Persons.Count - 1);
        }


        [SkippableFact]
        public void StringEqualsStaticMethod_GivesExpectedResult()
        {
            using IQueryDocumentSession s = GetNewDocumentSession();

            List<Person> persons = s.Query<Person>().Where(x => string.Equals(x.Name, TestNameNeilHughesBell)).ToList();
            persons.Should().ContainSingle();
        }
        
        [SkippableFact]
        public void StringEqualsOnParameterMethod_GivesExpectedResult()
        {
            using IQueryDocumentSession s = GetNewDocumentSession();

            List<Person> persons = s.Query<Person>().Where(x => TestNameNeilHughesBell.Equals(x.Name)).ToList();
            persons.Should().ContainSingle(x => x.Name == TestNameNeilHughesBell);
        }

        [SkippableFact]
        public void NegatedStringEqualsOnParameterMethod_GivesExpectedResult()
        {
            using IQueryDocumentSession s = GetNewDocumentSession();

            List<Person> persons = s.Query<Person>().Where(x => !TestNameNeilHughesBell.Equals(x.Name)).ToList();
            persons.Should().HaveCount(_td.Persons.Count - 1);
        }

        [SkippableFact]
        public void StringContainsMethod_GivesExpectedResult()
        {
            using IQueryDocumentSession s = GetNewDocumentSession();

            string nameLowercase = TestNameNeil.ToLower();

            List<Person> p = s.Query<Person>().Where(x => x.Name.Contains(nameLowercase)).ToList();
            p.Should().BeEmpty();

            p = s.Query<Person>().Where(x => x.Name.Contains(TestNameNeil)).ToList();
            p.Should().ContainSingle();
        }
        
        [SkippableFact]
        public void StringContainsCaseInsensitiveMethod_GivesExpectedResult()
        {
            using IQueryDocumentSession s = GetNewDocumentSession();

            string nameLowercase = TestNameNeil.ToLower();

            List<Person> p = s.Query<Person>().Where(x => x.Name.Contains(nameLowercase)).ToList();
            p.Should().BeEmpty();

            p = s.Query<Person>().Where(x => x.Name.Contains(nameLowercase, StringComparison.Ordinal)).ToList();
            p.Should().BeEmpty();

            p = s.Query<Person>().Where(x => x.Name.Contains(nameLowercase, StringComparison.InvariantCulture)).ToList();
            p.Should().BeEmpty();

            p = s.Query<Person>().Where(x => x.Name.Contains(nameLowercase, StringComparison.CurrentCulture)).ToList();
            p.Should().BeEmpty();

            p = s.Query<Person>().Where(x => x.Name.Contains(nameLowercase, StringComparison.OrdinalIgnoreCase)).ToList();
            p.Should().ContainSingle();

            p = s.Query<Person>().Where(x => x.Name.Contains(nameLowercase, StringComparison.InvariantCultureIgnoreCase)).ToList();
            p.Should().ContainSingle();

            p = s.Query<Person>().Where(x => x.Name.Contains(nameLowercase, StringComparison.CurrentCultureIgnoreCase)).ToList();
            p.Should().ContainSingle();
        }

        [SkippableFact]
        public void StringStartsWithMethod_GivesExpectedResult()
        {
            string nameLowercase = TestNameNeil.ToLower();

            using IDocumentSession s = GetNewDocumentSession();
            
            List<Person> p = s.Query<Person>().Where(x => x.Name.StartsWith(nameLowercase)).ToList();
            p.Should().BeEmpty();

            p = s.Query<Person>().Where(x => x.Name.StartsWith(TestNameNeil)).ToList();
            p.Should().ContainSingle();
        }

        [SkippableFact]
        public void StringStartsWithCaseInsensitiveMethod_GivesExpectedResult()
        {
            string nameLowercase = TestNameNeil.ToLower();

            using IDocumentSession s = GetNewDocumentSession();

            List<Person> p = s.Query<Person>().Where(x => x.Name.StartsWith(nameLowercase)).ToList();
            p.Should().BeEmpty();

            p = s.Query<Person>().Where(x => x.Name.StartsWith(nameLowercase, StringComparison.Ordinal)).ToList();
            p.Should().BeEmpty();

            p = s.Query<Person>().Where(x => x.Name.StartsWith(nameLowercase, StringComparison.InvariantCulture)).ToList();
            p.Should().BeEmpty();

            p = s.Query<Person>().Where(x => x.Name.StartsWith(nameLowercase, StringComparison.CurrentCulture)).ToList();
            p.Should().BeEmpty();

            p = s.Query<Person>().Where(x => x.Name.StartsWith(nameLowercase, StringComparison.OrdinalIgnoreCase)).ToList();
            p.Should().ContainSingle();
        }

        [SkippableFact]
        public void StringEndsWithMethod_GivesExpectedResult()
        {
            string nameLowercase = TestNameHughesBell.ToLower();

            using IDocumentSession s = GetNewDocumentSession();

            List<Person> p = s.Query<Person>().Where(x => x.Name.EndsWith(nameLowercase)).ToList();
            p.Should().BeEmpty();
            
            p = s.Query<Person>().Where(x => x.Name.EndsWith(TestNameHughesBell)).ToList();
            p.Should().ContainSingle();

            p = s.Query<Person>().Where(x => x.Name.EndsWith(nameLowercase, StringComparison.OrdinalIgnoreCase)).ToList();
            p.Should().ContainSingle();
        }

        [SkippableFact]
        public void StringEndsWithCaseInsensitiveMethod_GivesExpectedResult()
        {
            string nameLowercase = TestNameHughesBell.ToLower();

            using IDocumentSession s = GetNewDocumentSession();

            List<Person> p = s.Query<Person>().Where(x => x.Name.EndsWith(nameLowercase)).ToList();
            p.Should().BeEmpty();

            p = s.Query<Person>().Where(x => x.Name.EndsWith(nameLowercase, StringComparison.Ordinal)).ToList();
            p.Should().BeEmpty();

            p = s.Query<Person>().Where(x => x.Name.EndsWith(nameLowercase, StringComparison.InvariantCulture)).ToList();
            p.Should().BeEmpty();

            p = s.Query<Person>().Where(x => x.Name.EndsWith(nameLowercase, StringComparison.CurrentCulture)).ToList();
            p.Should().BeEmpty();

            p = s.Query<Person>().Where(x => x.Name.EndsWith(nameLowercase, StringComparison.OrdinalIgnoreCase)).ToList();
            p.Should().ContainSingle();
        }

        [SkippableFact]
        public void NegateStringContainsMethod_GivesExpectedResult()
        {
            using IQueryDocumentSession s = GetNewDocumentSession();

            string nameLowercase = TestNameNeil.ToLower();

            List<Person> p = s.Query<Person>().Where(x => !x.Name.Contains(nameLowercase)).ToList();
            p.Should().HaveCount(_td.Persons.Count);

            p = s.Query<Person>().Where(x => !x.Name.Contains(TestNameNeil)).ToList();
            p.Should().HaveCount(_td.Persons.Count - 1);
        }

        [SkippableFact]
        public void NegateStringContainsCaseInsensitiveMethod_GivesExpectedResult()
        {
            using IQueryDocumentSession s = GetNewDocumentSession();

            string nameLowercase = TestNameNeil.ToLower();

            List<Person> p = s.Query<Person>().Where(x => !x.Name.Contains(nameLowercase)).ToList();
            p.Should().HaveCount(_td.Persons.Count);

            p = s.Query<Person>().Where(x => !x.Name.Contains(nameLowercase, StringComparison.Ordinal)).ToList();
            p.Should().HaveCount(_td.Persons.Count);

            p = s.Query<Person>().Where(x => !x.Name.Contains(nameLowercase, StringComparison.InvariantCulture)).ToList();
            p.Should().HaveCount(_td.Persons.Count);

            p = s.Query<Person>().Where(x => !x.Name.Contains(nameLowercase, StringComparison.CurrentCulture)).ToList();
            p.Should().HaveCount(_td.Persons.Count);

            p = s.Query<Person>().Where(x => !x.Name.Contains(nameLowercase, StringComparison.OrdinalIgnoreCase)).ToList();
            p.Should().HaveCount(_td.Persons.Count - 1);

            p = s.Query<Person>().Where(x => !x.Name.Contains(nameLowercase, StringComparison.InvariantCultureIgnoreCase)).ToList();
            p.Should().HaveCount(_td.Persons.Count - 1);

            p = s.Query<Person>().Where(x => !x.Name.Contains(nameLowercase, StringComparison.CurrentCultureIgnoreCase)).ToList();
            p.Should().HaveCount(_td.Persons.Count - 1);
        }

        [SkippableFact]
        public void NegateStringStartsWithMethod_GivesExpectedResult()
        {
            string nameLowercase = TestNameNeil.ToLower();

            using IDocumentSession s = GetNewDocumentSession();

            List<Person> p = s.Query<Person>().Where(x => !x.Name.StartsWith(nameLowercase)).ToList();
            p.Should().HaveCount(_td.Persons.Count);

            p = s.Query<Person>().Where(x => !x.Name.StartsWith(TestNameNeil)).ToList();
            p.Should().HaveCount(_td.Persons.Count - 1);
        }

        [SkippableFact]
        public void NegateStringStartsWithCaseInsensitiveMethod_GivesExpectedResult()
        {
            string nameLowercase = TestNameNeil.ToLower();

            using IDocumentSession s = GetNewDocumentSession();

            List<Person> p = s.Query<Person>().Where(x => !x.Name.StartsWith(nameLowercase)).ToList();
            p.Should().HaveCount(_td.Persons.Count);

            p = s.Query<Person>().Where(x => !x.Name.StartsWith(nameLowercase, StringComparison.Ordinal)).ToList();
            p.Should().HaveCount(_td.Persons.Count);

            p = s.Query<Person>().Where(x => !x.Name.StartsWith(nameLowercase, StringComparison.InvariantCulture)).ToList();
            p.Should().HaveCount(_td.Persons.Count);

            p = s.Query<Person>().Where(x => !x.Name.StartsWith(nameLowercase, StringComparison.CurrentCulture)).ToList();
            p.Should().HaveCount(_td.Persons.Count);

            p = s.Query<Person>().Where(x => !x.Name.StartsWith(nameLowercase, StringComparison.OrdinalIgnoreCase)).ToList();
            p.Should().HaveCount(_td.Persons.Count - 1);
        }

        [SkippableFact]
        public void NegateStringEndsWithMethod_GivesExpectedResult()
        {

            string nameLowercase = TestNameHughesBell.ToLower();

            using IDocumentSession s = GetNewDocumentSession();

            List<Person> p = s.Query<Person>().Where(x => !x.Name.EndsWith(nameLowercase)).ToList();
            p.Should().HaveCount(_td.Persons.Count);

            p = s.Query<Person>().Where(x => !x.Name.EndsWith(TestNameHughesBell)).ToList();
            p.Should().HaveCount(_td.Persons.Count - 1);

            p = s.Query<Person>().Where(x => !x.Name.EndsWith(nameLowercase, StringComparison.OrdinalIgnoreCase)).ToList();
            p.Should().HaveCount(_td.Persons.Count - 1);
        }

        [SkippableFact]
        public void NegateStringEndsWithCaseInsensitiveMethod_GivesExpectedResult()
        {
            string nameLowercase = TestNameHughesBell.ToLower();

            using IDocumentSession s = GetNewDocumentSession();

            List<Person> p = s.Query<Person>().Where(x => !x.Name.EndsWith(nameLowercase)).ToList();
            p.Should().HaveCount(_td.Persons.Count);

            p = s.Query<Person>().Where(x => !x.Name.EndsWith(nameLowercase, StringComparison.Ordinal)).ToList();
            p.Should().HaveCount(_td.Persons.Count);

            p = s.Query<Person>().Where(x => !x.Name.EndsWith(nameLowercase, StringComparison.InvariantCulture)).ToList();
            p.Should().HaveCount(_td.Persons.Count);

            p = s.Query<Person>().Where(x => !x.Name.EndsWith(nameLowercase, StringComparison.CurrentCulture)).ToList();
            p.Should().HaveCount(_td.Persons.Count);

            p = s.Query<Person>().Where(x => !x.Name.EndsWith(nameLowercase, StringComparison.OrdinalIgnoreCase)).ToList();
            p.Should().HaveCount(_td.Persons.Count - 1);
        }

        [SkippableFact]
        public void StringContainsUnderscoreMethod_GivesExpectedResult()
        {
            InsertWildcardPersons();

            using IDocumentSession s = GetNewDocumentSession();
            
            List<Person> persons = s.Query<Person>().Where(x => x.Name.Contains("_")).ToList();
            persons.Should().HaveCount(5);
        }

        [SkippableFact]
        public void StringContainsPercentageMethod_GivesExpectedResult()
        {
            InsertWildcardPersons();

            using IDocumentSession s = GetNewDocumentSession();

            List<Person> persons = s.Query<Person>().Where(x => x.Name.Contains("%")).ToList();
            persons.Should().HaveCount(3);
        }

        [SkippableFact]
        public void StringContainsUnderscoreCaseInsensitiveMethod_GivesExpectedResult()
        {
            InsertWildcardPersons();

            using IDocumentSession s = GetNewDocumentSession();
            
            List<Person> persons = s.Query<Person>().Where(x => x.Name.Contains("_")).ToList();
            persons.Should().HaveCount(5);
            
            persons = s.Query<Person>().Where(x => x.Name.Contains("_", StringComparison.OrdinalIgnoreCase)).ToList();
            persons.Should().HaveCount(5);
        }

        [SkippableFact]
        public void StringContainsPercentageCaseInsensitiveMethod_GivesExpectedResult()
        {
            InsertWildcardPersons();

            using IDocumentSession s = GetNewDocumentSession();
            
            List<Person> persons = s.Query<Person>().Where(x => x.Name.Contains("%")).ToList();
            persons.Should().HaveCount(3);
            
            persons = s.Query<Person>().Where(x => x.Name.Contains("%", StringComparison.OrdinalIgnoreCase)).ToList();
            persons.Should().HaveCount(3);
        }

        [SkippableFact]
        public void StringStartsWithUnderscore_GivesExpectedResult()
        {
            InsertWildcardPersons();

            using IDocumentSession s = GetNewDocumentSession();

            List<Person> persons = s.Query<Person>().Where(x => x.Name.StartsWith("_S")).ToList();
            persons.Should().ContainSingle();
        }

        [SkippableFact]
        public void StringStartsWithPercentage_GivesExpectedResult()
        {
            InsertWildcardPersons();

            using IDocumentSession s = GetNewDocumentSession();

            List<Person> persons = s.Query<Person>().Where(x => x.Name.StartsWith("%S")).ToList();
            persons.Should().ContainSingle();
        }

        [SkippableFact]
        public void StringStartsWithUnderscoreCaseInsensitive_GivesExpectedResult()
        {
            InsertWildcardPersons();

            using IDocumentSession s = GetNewDocumentSession();

            List<Person> persons = s.Query<Person>().Where(x => x.Name.StartsWith("_s")).ToList();
            persons.Should().BeEmpty();

            persons = s.Query<Person>().Where(x => x.Name.StartsWith("_s", StringComparison.OrdinalIgnoreCase)).ToList();
            persons.Should().ContainSingle();
        }

        [SkippableFact]
        public void StringStartsWithPercentageCaseInsensitive_GivesExpectedResult()
        {
            InsertWildcardPersons();

            using IDocumentSession s = GetNewDocumentSession();

            List<Person> persons = s.Query<Person>().Where(x => x.Name.StartsWith("%s")).ToList();
            persons.Should().BeEmpty();

            persons = s.Query<Person>().Where(x => x.Name.StartsWith("%s", StringComparison.OrdinalIgnoreCase)).ToList();
            persons.Should().ContainSingle();
        }

        [SkippableFact]
        public void StringEndsWithWithUnderscore_GivesExpectedResult()
        {
            InsertWildcardPersons();

            using IDocumentSession s = GetNewDocumentSession();

            List<Person> persons = s.Query<Person>().Where(x => x.Name.EndsWith("e_")).ToList();
            persons.Should().ContainSingle();
        }

        [SkippableFact]
        public void StringEndsWithWithPercentage_GivesExpectedResult()
        {
            InsertWildcardPersons();

            using IDocumentSession s = GetNewDocumentSession();

            List<Person> persons = s.Query<Person>().Where(x => x.Name.EndsWith("e%")).ToList();
            persons.Should().ContainSingle();
        }

        [SkippableFact]
        public void StringEndsWithUnderscoreCaseInsensitive_GivesExpectedResult()
        {
            InsertWildcardPersons();

            using IDocumentSession s = GetNewDocumentSession();

            List<Person> persons = s.Query<Person>().Where(x => x.Name.EndsWith("E_")).ToList();
            persons.Should().BeEmpty();

            persons = s.Query<Person>().Where(x => x.Name.EndsWith("E_", StringComparison.OrdinalIgnoreCase)).ToList();
            persons.Should().ContainSingle();
        }

        [SkippableFact]
        public void StringEndsWithPercentageCaseInsensitive_GivesExpectedResult()
        {
            InsertWildcardPersons();

            using IDocumentSession s = GetNewDocumentSession();

            List<Person> persons = s.Query<Person>().Where(x => x.Name.EndsWith("E%")).ToList();
            persons.Should().BeEmpty();

            persons = s.Query<Person>().Where(x => x.Name.EndsWith("E%", StringComparison.OrdinalIgnoreCase)).ToList();
            persons.Should().ContainSingle();
        }

        [SkippableFact]
        public void StringNull_GivesExpectedResult()
        {
            using IDocumentSession s = GetNewDocumentSession();
            List<Person> p = s.Query<Person>().Where(x => x.NickName == null).ToList();
            p.Should().ContainSingle();
        }

        [SkippableFact]
        public void StringNullOrEmpty_GivesExpectedResult()
        {
            using IDocumentSession s = GetNewDocumentSession();
            List<Person> p = s.Query<Person>().Where(x => string.IsNullOrEmpty(x.NickName)).ToList();
            p.Should().HaveCount(2);
        }

        [SkippableFact]
        public void StringNullOrWhiteSpace_GivesExpectedResult()
        {
            using IDocumentSession s = GetNewDocumentSession();
            List<Person> p = s.Query<Person>().Where(x => string.IsNullOrWhiteSpace(x.NickName)).ToList();
            p.Should().HaveCount(3);
        }

        [SkippableFact]
        public void NegatedStringNull_GivesExpectedResult()
        {
            using IDocumentSession s = GetNewDocumentSession();
            List<Person> p = s.Query<Person>().Where(x => x.NickName != null).ToList();
            p.Should().HaveCount(_td.Persons.Count - 1);
        }

        [SkippableFact]
        public void NegatedStringNullOrEmpty_GivesExpectedResult()
        {
            using IDocumentSession s = GetNewDocumentSession();
            List<Person> p = s.Query<Person>().Where(x => !string.IsNullOrEmpty(x.NickName)).ToList();
            p.Should().HaveCount(_td.Persons.Count - 2);
        }

        [SkippableFact]
        public void NegatedStringNullOrWhiteSpace_GivesExpectedResult()
        {
            using IDocumentSession s = GetNewDocumentSession();
            List<Person> p = s.Query<Person>().Where(x => !string.IsNullOrWhiteSpace(x.NickName)).ToList();
            p.Should().HaveCount(_td.Persons.Count - 3);
        }

        private void InsertWildcardPersons()
        {
            using IDocumentSession s = GetNewDocumentSession();

            s.Insert(new Person
            {
                Name = "Some_One",
                EmailAddress = "someone_1@example.com"
            });

            s.Insert(new Person
            {
                Name = "_Some_One",
                EmailAddress = "someone_2@example.com"
            });
            
            s.Insert(new Person
            {
                Name = "Some_One_",
                EmailAddress = "someone_3@example.com"
            });

            s.Insert(new Person
            {
                Name = "Some%One",
                EmailAddress = "someone_4@example.com"
            });

            s.Insert(new Person
            {
                Name = "%Some_One",
                EmailAddress = "someone_5@example.com"
            });

            s.Insert(new Person
            {
                Name = "Some_One%",
                EmailAddress = "someone_6@example.com"
            });

            s.SaveChanges();
        }
    }
}
