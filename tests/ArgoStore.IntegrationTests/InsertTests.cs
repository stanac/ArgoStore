using System;
using System.Collections.Generic;
using System.Linq;
using ArgoStore.IntegrationTests.Entities;
using FluentAssertions;
using Xunit;

namespace ArgoStore.IntegrationTests
{
    public class InsertTests : IntegrationTestsBase
    {
        [SkippableFact]
        public void InsertEntity_SaveChanges_GetEntity_ReturnsInsertedEntity()
        {
            IDocumentSession s = GetNewDocumentSession();

            Person toInsert = new Person
            {
                Id = Guid.NewGuid(),
                Name = "Person Name",
                BirthYear = 1990,
                CackeDay = DateTime.UtcNow,
                EmailAddress = "person@example.com",
                EmailConfirmed = true,
                RegistrationTime = DateTimeOffset.UtcNow,
                Roles = new List<string>
                {
                    "1", "2", "Role 3"
                }
            };

            s.Insert(toInsert);
            s.SaveChanges();

            Person gotPerson = s.Query<Person>().First(x => x.Id == toInsert.Id);

            gotPerson.Should().BeEquivalentTo(toInsert);
        }
    }
}
