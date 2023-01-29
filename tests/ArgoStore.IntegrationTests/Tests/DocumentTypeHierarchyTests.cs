using System.Text.Json;
using ArgoStore.TestsCommon.Entities.User;

namespace ArgoStore.IntegrationTests.Tests;

public class DocumentTypeHierarchyTests : IntegrationTestBase
{
    public DocumentTypeHierarchyTests()
    {
        Store.RegisterDocument<User>();
    }

    [Fact]
    public void SerializationTest()
    {
        User user1 = new User
        {
            Id = "1",
            Name = "User1",
            Contact = new UserEmailContact
            {
                Weight = 1,
                EmailAddress = "address@example.com"
            }
        };
        
        string json1 = JsonSerializer.Serialize(user1);

        User user1D = JsonSerializer.Deserialize<User>(json1);

        user1D.Should().BeEquivalentTo(user1);
    }

    [Fact]
    public void InsertDocumentsWithAbstractProperty_ToList_GivesExpectedResult()
    {
        User user1 = new User
        {
            Id = "a",
            Name = "User1",
            Contact = new UserEmailContact
            {
                Weight = 1,
                EmailAddress = "address@example.com"
            }
        };

        User user2 = new User
        {
            Id = "b",
            Name = "User2",
            Contact = new UserSnailMailContact
            {
                Weight = 2,
                Address = "Address Line",
                City = "City 2",
                PostCode = "21000"
            }
        };

        using IArgoDocumentSession s = Store.OpenSession();

        s.Insert(user1, user2);
        s.SaveChanges();

        List<User> r = s.Query<User>().ToList();

        List<User> expected = new ()
        {
            user1,
            user2
        };

        r.Should().BeEquivalentTo(expected);
    }
}