﻿using System.Linq;
using ArgoStore.IntegrationTests.Entities;
using FluentAssertions;
using Xunit;

namespace ArgoStore.IntegrationTests
{
    public class AnyTests : IntegrationTestsBase
    {
        private const string TestNameImogenCampbell = "Imogen Campbell";
        private const string TestNameNonExisting = "Someone who is not in test DB";

        private readonly TestData _td;

        public AnyTests()
        {
            _td = new TestData(TestDbConnectionString);

            using IDocumentSession session = GetNewDocumentSession();
            _td.InsertTestPersons();
        }

        [Fact]
        public void Any_ReturnsExpectedResult()
        {
            using IDocumentSession session = GetNewDocumentSession();

            bool exists = session.Query<Person>().Any();
            exists.Should().BeTrue();
        }
        
        [Fact]
        public void AnyWithCondition_ReturnsExpectedResult()
        {
            using IDocumentSession session = GetNewDocumentSession();

            bool exists = session.Query<Person>().Any(x => x.Name == TestNameImogenCampbell);
            exists.Should().BeTrue();
        }

        [Fact]
        public void AnyWithCondition_NonExistingPerson_ReturnsExpectedResult()
        {
            using IDocumentSession session = GetNewDocumentSession();

            bool exists = session.Query<Person>().Any(x => x.Name == TestNameNonExisting);
            exists.Should().BeFalse();
        }
    }
}