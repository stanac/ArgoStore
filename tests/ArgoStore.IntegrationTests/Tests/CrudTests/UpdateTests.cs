using ArgoStore.TestsCommon.Entities.Person;

namespace ArgoStore.IntegrationTests.Tests.CrudTests;

public class UpdateTests : IntegrationTestBase
{
    public UpdateTests()
    {
        InsertTestPersons();
    }

    [Fact]
    public void Update_UpdatesPersons()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        List<Person> persons = s.Query<Person>().ToList().Take(2).ToList();
        Person p1 = persons[0];
        Person p2 = persons[1];

        p1.BirthYear = 9999;
        p1.Points = 123;
        p1.NickName = Guid.NewGuid().ToString();

        p2.BirthYear = 2999;
        p2.Points = 125;
        p2.NickName = Guid.NewGuid().ToString();

        s.Update(p1, p2);
        s.SaveChanges();

        using IArgoDocumentSession s2 = Store.OpenSession();

        Person fromDbP1 = s2.GetById<Person>(p1.Id);
        Person fromDbP2 = s2.GetById<Person>(p2.Id);

        fromDbP1.Should().BeEquivalentTo(p1);
        fromDbP2.Should().BeEquivalentTo(p2);
    }
}