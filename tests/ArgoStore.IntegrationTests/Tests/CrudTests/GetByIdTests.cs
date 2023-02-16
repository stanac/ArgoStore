using ArgoStore.TestsCommon.Entities.Person;

namespace ArgoStore.IntegrationTests.Tests.CrudTests;

public class GetByIdTests : IntegrationTestBase
{
    private readonly PersonPkGuid _pGuid = PersonPkGuid.Create();
    private readonly PersonPkString _pString = PersonPkString.Create();
    
    public GetByIdTests()
    {
        Store.RegisterDocument<PersonPkGuid>();
        Store.RegisterDocument<PersonPkString>();

        using IArgoDocumentSession s = Store.OpenSession();

        s.Insert(_pGuid);
        s.Insert(_pString);

        s.SaveChanges();
    }

    [Fact]
    public void PkGuid_GetById_ReturnsExpectedObject()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        PersonPkGuid p = s.GetById<PersonPkGuid>(_pGuid.Id);
        p.Should().BeEquivalentTo(_pGuid);
    }

    [Fact]
    public void PkString_GetById_ReturnsExpectedObject()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        PersonPkString p = s.GetById<PersonPkString>(_pString.Id);
        p.Should().BeEquivalentTo(_pString);
    }
}