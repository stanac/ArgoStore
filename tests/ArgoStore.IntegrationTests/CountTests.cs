using ArgoStore.TestsCommon.Entities;
using ArgoStore.TestsCommon.TestData;

namespace ArgoStore.IntegrationTests;

public class CountTests : IntegrationTestBase
{
    [Fact]
    public void Insert1_Count_Returns1()
    {
        IArgoDocumentSession s = Store.OpenSession();

        int count = s.Query<Person>().Count();
        count.Should().Be(0);

        s.Insert(PersonTestData.GetPersonTestData().First());
        s.SaveChanges();

        count = s.Query<Person>().Count();
        count.Should().Be(1);
    }

    [Fact]
    public void Insert10_Count_Returns10()
    {
        IArgoDocumentSession s = Store.OpenSession();

        int count = s.Query<Person>().Count();
        count.Should().Be(0);

        foreach (Person p in PersonTestData.GetPersonTestData().Take(10))
        {
            s.Insert(p);
        }
        s.SaveChanges();

        count = s.Query<Person>().Count();
        count.Should().Be(10);
    }

    [Fact]
    public void Insert1_LongCount_Returns1()
    {
        IArgoDocumentSession s = Store.OpenSession();

        long count = s.Query<Person>().LongCount();
        count.Should().Be(0);

        s.Insert(PersonTestData.GetPersonTestData().First());
        s.SaveChanges();

        count = s.Query<Person>().LongCount();
        count.Should().Be(1);
    }

    [Fact]
    public void Insert10_CountLong_Returns10()
    {
        IArgoDocumentSession s = Store.OpenSession();

        long count = s.Query<Person>().LongCount();
        count.Should().Be(0);

        foreach (Person p in PersonTestData.GetPersonTestData().Take(10))
        {
            s.Insert(p);
        }
        s.SaveChanges();

        count = s.Query<Person>().LongCount();
        count.Should().Be(10);
    }
}