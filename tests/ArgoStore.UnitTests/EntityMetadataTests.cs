using System;
using FluentAssertions;
using Xunit;
// ReSharper disable UnusedMember.Local
// ReSharper disable ObjectCreationAsStatement

namespace ArgoStore.UnitTests
{
    public class EntityMetadataTests
    {
        [Theory]
        [InlineData(typeof(TestEntityPkIdString))]
        [InlineData(typeof(TestEntityPkIdGuid))]
        [InlineData(typeof(TestEntityPkIdInt))]
        [InlineData(typeof(TestEntityPkIdLong))]
        public void Ctor_PkId_CreateValidEntityMetadata(Type entityType)
        {
            EntityMetadata m = new EntityMetadata(entityType);
            m.PrimaryKeyProperty.Name.Should().Be("StringId");
        }

        [Theory]
        [InlineData(typeof(TestEntityPkIdString))]
        [InlineData(typeof(TestEntityPkIdGuid))]
        [InlineData(typeof(TestEntityPkIdInt))]
        [InlineData(typeof(TestEntityPkIdLong))]
        public void Ctor_PkManuallySet_CreateValidEntityMetadata(Type entityType)
        {
            EntityMetadata m = new EntityMetadata(entityType, "OtherIdProp");
            m.PrimaryKeyProperty.Name.Should().Be("OtherIdProp");
        }

        [Theory]
        [InlineData(typeof(TestEntityPkKeyString))]
        [InlineData(typeof(TestEntityPkKeyGuid))]
        [InlineData(typeof(TestEntityPkKeyInt))]
        [InlineData(typeof(TestEntityPkKeyLong))]
        public void Ctor_PkKey_CreateValidEntityMetadata(Type entityType)
        {
            EntityMetadata m = new EntityMetadata(entityType);
            m.PrimaryKeyProperty.Name.Should().Be("Key");
        }

        [Fact]
        public void Ctor_NoPkProp_ThrowsException()
        {
            Action a = () => new EntityMetadata(typeof(NoPk));
            a.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Ctor_FullEntityNamePkId_SetsAppropriatePropertyAsKey()
        {
            EntityMetadata meta = new EntityMetadata(typeof(MyEntity));
            meta.PrimaryKeyProperty.Name.Should().Be("MyEntityId");
        }

        [Fact]
        public void Ctor_FullEntityNamePkKey_SetsAppropriatePropertyAsKey()
        {
            EntityMetadata meta = new EntityMetadata(typeof(YourEntity));
            meta.PrimaryKeyProperty.Name.Should().Be("YourEntityKey");
        }

        [Fact]
        public void Ctor_InvalidKeyTypeAutoSet_ThrowsException()
        {
            Action a = () => new EntityMetadata(typeof(InvalidEntityKey));
            a.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Ctor_InvalidKeyTypeManualSet_ThrowsException()
        {
            Action a = () => new EntityMetadata(typeof(InvalidEntityKey), "Value");
            a.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Ctor_NonExistingPropertyPkManualSet_ThrowsException()
        {
            Action a = () => new EntityMetadata(typeof(InvalidEntityKey), "NonExistingProp");
            a.Should().Throw<ArgumentException>();
        }

        private class TestEntityPkIdString
        {
            public string Id { get; set; }
            public string OtherIdProp { get; set; }
        }

        private class TestEntityPkIdGuid
        {
            public Guid Id { get; set; }
            public Guid OtherIdProp { get; set; }
        }

        private class TestEntityPkIdInt
        {
            public int Id { get; set; }
            public int OtherIdProp { get; set; }
        }

        private class TestEntityPkIdLong
        {
            public long Id { get; set; }
            public long OtherIdProp { get; set; }
        }

        private class TestEntityPkKeyString
        {
            public string Key { get; set; }
        }

        private class TestEntityPkKeyGuid
        {
            public Guid Key { get; set; }
        }

        private class TestEntityPkKeyInt
        {
            public int Key { get; set; }
        }

        private class TestEntityPkKeyLong
        {
            public long Key { get; set; }
        }

        private class NoPk
        {
            public string Value { get; set; }
        }

        private class MyEntity
        {
            public int MyEntityId { get; set; }
        }

        public class YourEntity
        {
            public Guid YourEntityKey { get; set; }
        }

        public class InvalidEntityKey
        {
            public DateTime Key { get; set; }
            public DateTime Value { get; set; }
        }
    }
}
