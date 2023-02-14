namespace ArgoStore.IntegrationTests.Bugs;

public class RolesContainsBug : IntegrationTestBase
{
    public RolesContainsBug()
    {
        UseFileDb();
        RegisterAndInsertTestData();
    }

    [Fact]
    public void ContainsOnStringArray_ReturnsExpectedResult()
    {
        List<BugObject> expected = GetTestData().Where(x => x.Items.Contains("value2")).ToList();

        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<BugObject> result = s.Query<BugObject>()
            .Where(x => x.Items.Contains("value2"))
            .ToList();

        result.Should().BeEquivalentTo(expected);
    }

    private void RegisterAndInsertTestData()
    {
        Store.RegisterDocument<BugObject>();

        using IArgoDocumentSession s = Store.OpenSession();

        s.Insert(GetTestData().ToArray());
        s.SaveChanges();
    }

    private IEnumerable<BugObject> GetTestData()
    {
        yield return new BugObject
        {
            Id = Guid.Parse("12bba9c5-fb91-440e-972d-a4385862d86f"),
            Items = new[] {"value1", "value2"}
        };

        yield return new BugObject
        {
            Id = Guid.Parse("7edf3e02-f7a7-4745-a9c2-9373e06f0e47"),
            Items = new[] { "value2" }
        };

        yield return new BugObject
        {
            Id = Guid.Parse("bc39f44d-78f7-47ad-963b-5ef7033ef999"),
            Items = new[] { "value3" }
        };
    }

    private class BugObject
    {
        public Guid Id { get; set; }
        public string[] Items { get; set; } = Array.Empty<string>();
    }
}