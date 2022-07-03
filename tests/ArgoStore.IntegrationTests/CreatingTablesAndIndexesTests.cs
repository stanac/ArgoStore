namespace ArgoStore.IntegrationTests;

public class CreatingTablesAndIndexesTests : IntegrationTestsBase
{
    public CreatingTablesAndIndexesTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void CreateTable_CreatesIndexes()
    {
        // ReSharper disable once ObjectCreationAsStatement
        new DocumentStore(c =>
        {
            c.ConnectionString(TestDbConnectionString);
            c.CreateNonConfiguredEntities(false);
            c.Entity<Person>()
                .PrimaryKey(x => x.Id)
                .UniqueIndex(x => x.Name)
                .UniqueIndex(x => new {x.Id, x.NickName})
                .NonUniqueIndex(x => x.CakeDay)
                .NonUniqueIndex(x => new {x.NickName, x.CakeDay});
        });
        
        List<string> unique = ListUniqueIndexes(typeof(Person));

        unique.Should().HaveCount(2);
        unique.Should().Contain(n => n.Contains("Name"));
        unique.Should().Contain(n => n.Contains("Id") && n.Contains("NickName"));

        string unique1 = unique.First(x => x.Contains("Name") && !x.Contains("NickName"));
        AssertIndexColumns(unique1, "Name");

        string unique2 = unique.First(x => x.Contains("Id") && x.Contains("NickName"));
        AssertIndexColumns(unique2, "Id", "NickName");

        List<string> nonUnique = ListNonUniqueIndexes(typeof(Person));

        string nonUnique1 = nonUnique.First(x => x.Contains("CakeDay") && !x.Contains("NickName"));
        string nonUnique2 = nonUnique.First(x => x.Contains("CakeDay") && x.Contains("NickName"));

        nonUnique1.Should().NotBe(nonUnique2);

        AssertIndexColumns(nonUnique1, "CakeDay");
        AssertIndexColumns(nonUnique2, "CakeDay", "NickName");
    }

    private void AssertIndexColumns(string indexName, params string[] columnNames)
    {
        string def = GetIndexDefinition(indexName);

        def.Should().NotBeNullOrEmpty();

        foreach (string name in columnNames)
        {
            def.Contains($"'$.{name}'", StringComparison.OrdinalIgnoreCase).Should().BeTrue();
        }

    }
}