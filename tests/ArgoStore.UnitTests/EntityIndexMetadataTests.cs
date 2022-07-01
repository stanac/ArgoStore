// ReSharper disable ObjectCreationAsStatement

using ArgoStore.Configurations;

namespace ArgoStore.UnitTests;

public class EntityIndexMetadataTests
{
    [Fact]
    public void Ctor_PropertyNamesNotSet_ThrowsException()
    {
        Action a = () => new EntityIndexMetadata(true, null, typeof(TestEntityPerson));

        a.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_EntityTypeNotSet_ThrowsException()
    {
        Action a = () => new EntityIndexMetadata(true, new List<string> { "Key" }, null);

        a.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_PropertyNamesEmpty_ThrowsException()
    {
        Action a = () => new EntityIndexMetadata(true, new List<string>(), typeof(TestEntityPerson));

        a.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Ctor_PropertiesValid_DoesNotThrowException()
    {
        Action a = () => new EntityIndexMetadata(true, new List<string> {"Key"}, typeof(TestEntityPerson));

        a.Should().NotThrow();
    }

    [Fact]
    public void Ctor_PropertiesNotUnique_ThrowsException()
    {
        Action a = () => new EntityIndexMetadata(true, new List<string> { "Key", "Key" }, typeof(TestEntityPerson));

        a.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Ctor_PropertiesNotExist_ThrowsException()
    {
        Action a = () => new EntityIndexMetadata(true, new List<string> { "Key1", "Key2" }, typeof(TestEntityPerson));

        a.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void HasSameProperties_TwoIndexesWithSameProperties_ReturnsTrue()
    {
        EntityIndexMetadata i1 = new EntityIndexMetadata(true, new List<string> { "Key", "Name" }, typeof(TestEntityPerson));
        EntityIndexMetadata i2 = new EntityIndexMetadata(false, new List<string> { "Name", "Key" }, typeof(TestEntityPerson));

        i1.HasSameProperties(i2).Should().BeTrue();
    }

    [Fact]
    public void HasSameProperties_TwoIndexesWithDifferentProperties_ReturnsFalse()
    {

        EntityIndexMetadata i1 = new EntityIndexMetadata(true, new List<string> { "Key", "Name" }, typeof(TestEntityPerson));
        EntityIndexMetadata i2 = new EntityIndexMetadata(true, new List<string> { "Key", "Active" }, typeof(TestEntityPerson));

        i1.HasSameProperties(i2).Should().BeFalse();
    }
}