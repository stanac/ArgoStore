using ArgoStore.Configurations;

namespace ArgoStore.UnitTests.Configurations;

public class EntityConfigurationTests
{
    private readonly EntityConfiguration<TestEntityPerson> _sut = new EntityConfiguration<TestEntityPerson>();

    [Fact]
    public void CreateMetadata_SetPrimaryKeyByLambda_SetsPrimaryKey()
    {
        _sut.PrimaryKey(x => x.Name);

        EntityMetadata meta = _sut.CreateMetadata();

        meta.PrimaryKeyProperty.Name.Should().Be("Name");
    }

    [Fact]
    public void CreateMetadata_SetPrimaryKeyByLambdaReadonlyProperty_ThrowsException()
    {
        _sut.PrimaryKey(x => x.NameLower);

        Action a = () => _sut.CreateMetadata();

        a.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void CreateMetadata_SetPrimaryKeyByLambdaPropertyWithPrivateSetter_ThrowsException()
    {
        _sut.PrimaryKey(x => x.EmailProvider);

        Action a = () => _sut.CreateMetadata();

        a.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void CreateMetadata_SetPrimaryKeyByAnonymousObjectLambda_ThrowsException()
    {
        _sut.PrimaryKey(x => new { x.Name });

        Action a = () => _sut.CreateMetadata();

        a.Should().Throw<InvalidOperationException>();
    }
    
    [Fact]
    public void CreateMetadata_SetPrimaryKeyTwice_ThrowsException()
    {
        _sut.PrimaryKey(x => x.Name);
        _sut.PrimaryKey(x => x.Name);

        Action a = () => _sut.CreateMetadata();

        a.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void CreateMetadata_SetPrimaryKeyFromConstant_ThrowsException()
    {
        _sut.PrimaryKey(x => "123");

        Action a = () => _sut.CreateMetadata();
        
        a.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void CreateMetadata_SetPrimaryKeyFromEmptyAnonymousObject_ThrowsException()
    {
        _sut.PrimaryKey(x => new {});

        Action a = () => _sut.CreateMetadata();
        
        a.Should().Throw<InvalidOperationException>();
    }
    
    [Fact]
    public void CreateMetadata_SetIndexFromConstant_ThrowsException()
    {
        _sut.UniqueIndex(x => "123");

        Action a = () => _sut.CreateMetadata();

        a.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void CreateMetadata_SetIndexFromEmptyAnonymousObject_ThrowsException()
    {
        _sut.NonUniqueIndex(x => new { });

        Action a = () => _sut.CreateMetadata();
        
        a.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void CreateMetadata_SetPrimaryKeyWithField_ThrowsException()
    {
        _sut.PrimaryKey(x => x.FieldStr);

        Action a = () => _sut.CreateMetadata();
        
        a.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void CreateMetadata_SetAnonymousObjectIndexWithConstant_ThrowsException()
    {
        _sut.UniqueIndex(x => new { a = 23 });

        Action a = () => _sut.CreateMetadata();
        
        a.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void CreateMetadata_AllValid_SetsPkAndIndexes()
    {
        _sut.PrimaryKey(x => x.Name)
            .UniqueIndex(x => x.EmailAddress)
            .UniqueIndex(x => new {Name2 = x.Name, EmailAddress2 = x.EmailAddress})
            .UniqueIndex(x => new {x.Name, x.ActiveDuration, x.BirthYear})
            .NonUniqueIndex(x => x.Active)
            .NonUniqueIndex(x => new { x.ActiveDuration, x.BirthYear });

        EntityMetadata meta = _sut.CreateMetadata();
        meta.Indexes.Should().HaveCount(5);

        meta.PrimaryKeyProperty.Name.Should().Be("Name");

        EntityIndexMetadata unique1 = meta.Indexes.Single(x => x.Unique && x.PropertyNames.Count == 1);
        unique1.PropertyNames.Should().BeEquivalentTo("EmailAddress");

        EntityIndexMetadata unique2 = meta.Indexes.Single(x => x.Unique && x.PropertyNames.Count == 2);
        unique2.PropertyNames.Should().BeEquivalentTo("Name", "EmailAddress");

        EntityIndexMetadata unique3 = meta.Indexes.Single(x => x.Unique && x.PropertyNames.Count == 3);
        unique3.PropertyNames.Should().BeEquivalentTo("Name", "ActiveDuration", "BirthYear");

        EntityIndexMetadata nonUnique1 = meta.Indexes.Single(x => !x.Unique && x.PropertyNames.Count == 1);
        nonUnique1.PropertyNames.Should().BeEquivalentTo("Active");

        EntityIndexMetadata nonUnique2 = meta.Indexes.Single(x => !x.Unique && x.PropertyNames.Count == 2);
        nonUnique2.PropertyNames.Should().BeEquivalentTo("ActiveDuration", "BirthYear");
    }
}