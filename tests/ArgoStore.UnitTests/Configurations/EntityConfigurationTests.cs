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
    public void CreateMetadata_SetPrimaryKeyWithField_ThrowsException()
    {
        _sut.PrimaryKey(x => x.FieldStr);

        Action a = () => _sut.CreateMetadata();

        a.Should().Throw<InvalidOperationException>();
    }
}