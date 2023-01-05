using ArgoStore.TestsCommon.Entities;
using ArgoStore.TestsCommon.TestData;

namespace ArgoStore.IntegrationTests;

public class InsertTests : IntegrationTestBase
{
    private readonly Person _testPersonSetId = PersonTestData.GetPersonTestData().First();
    private readonly Person _testPersonNotSetId = PersonTestData.GetPersonTestData().Skip(1).First();

    public InsertTests()
    {
        _testPersonNotSetId.Id = default;
    }

    [Fact]
    public void InsertWithGuidPkSet_Inserts()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        s.Insert(_testPersonSetId);
        s.SaveChanges();

        Person person = s.GetById<Person>(_testPersonSetId.Id);

        person.Should().BeEquivalentTo(_testPersonSetId);
    }
    
}