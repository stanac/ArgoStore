namespace ArgoStore.IntegrationTests.Bugs;

public class EqualsWithNullableIntegerBug : IntegrationTestBase
{
    public EqualsWithNullableIntegerBug()
    {
        Store.RegisterDocument<BugObject>();

        using IArgoDocumentSession s = Store.OpenSession();

        s.Insert(BugObject.TestData().ToArray());
        s.SaveChanges();
    }

    [InlineData(null)]
    [InlineData(2)]
    [Theory]
    public void QueryWithNullableIntegerComparedToInteger_ReturnsExpectedResult(int? value)
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        IQueryable<BugObject> query = s.Query<BugObject>();

        List<BugObject> expected = BugObject.TestData().ToList();

        if (value.HasValue)
        {
            query = query.Where(x => x.ValueA == value);
            expected = expected.Where(x => x.ValueA == value).ToList();
        }

        List<BugObject> result = query.ToList();
        
    }

    private class BugObject
    {
        public string Id { get; set; } = "";
        public int ValueA { get; set; }
        public int? ValueB { get; set; }

        public static IEnumerable<BugObject> TestData()
        {
            yield return new BugObject
            {
                Id = "a",
                ValueA = 1,
                ValueB = null,
            };

            yield return new BugObject
            {
                Id = "b",
                ValueA = 2,
                ValueB = null,
            };
            yield return new BugObject
            {
                Id = "c",
                ValueA = 2,
                ValueB = 2,
            };

            yield return new BugObject
            {
                Id = "d",
                ValueA = 2,
                ValueB = null,
            };
        }
    }
}