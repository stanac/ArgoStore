using ArgoStore.TestsCommon.Entities;

namespace ArgoStore.IntegrationTests;

public class GetByIdTests : IntegrationTestBase
{
    private readonly PersonPkGuid _pGuid = PersonPkGuid.Create();
    private readonly PersonPkString _pString = PersonPkString.Create();
    private readonly PersonPkInt32 _pInt32 = PersonPkInt32.Create();
    private readonly PersonPkInt64 _pInt64 = PersonPkInt64.Create();
    private readonly PersonPkUInt32 _pUInt32 = PersonPkUInt32.Create();
    private readonly PersonPkUInt64 _pUInt64 = PersonPkUInt64.Create();

    public GetByIdTests()
    {
        Store.RegisterDocument<PersonPkGuid>();
        Store.RegisterDocument<PersonPkString>();
        Store.RegisterDocument<PersonPkInt32>();
        Store.RegisterDocument<PersonPkUInt32>();
        Store.RegisterDocument<PersonPkInt64>();
        Store.RegisterDocument<PersonPkUInt64>();

        using IArgoDocumentSession s = Store.OpenSession();

        s.Insert(_pGuid);
        s.Insert(_pString);
        s.Insert(_pInt32);
        s.Insert(_pInt64);
        s.Insert(_pUInt32);
        s.Insert(_pUInt64);

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

    [Fact]
    public void PkInt32_GetById_ReturnsExpectedObject()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        PersonPkInt32 p = s.GetById<PersonPkInt32>(_pInt32.Id);
        p.Should().BeEquivalentTo(_pInt32);
    }

    [Fact]
    public void PkInt64_GetById_ReturnsExpectedObject()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        PersonPkInt64 p = s.GetById<PersonPkInt64>(_pInt64.Id);
        p.Should().BeEquivalentTo(_pInt64);
    }

    [Fact]
    public void PkUInt32_GetById_ReturnsExpectedObject()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        PersonPkUInt32 p = s.GetById<PersonPkUInt32>(_pUInt32.Id);
        p.Should().BeEquivalentTo(_pUInt32);
    }

    [Fact]
    public void PkUInt64_GetById_ReturnsExpectedObject()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        PersonPkUInt64 p = s.GetById<PersonPkUInt64>(_pUInt64.Id);
        p.Should().BeEquivalentTo(_pUInt64);
    }
}