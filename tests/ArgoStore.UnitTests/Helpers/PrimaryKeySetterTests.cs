using System;
using ArgoStore.Helpers;
using FluentAssertions;
using Xunit;
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace ArgoStore.UnitTests.Helpers
{
    public class PrimaryKeySetterTests
    {
        [Fact]
        public void PrimaryKeyGuidNotSet_SetPrimaryKey_SetsPk()
        {
            EntityMetadata meta = new EntityMetadata(typeof(TestEntityWithGuidPk));

            TestEntityWithGuidPk entity = new TestEntityWithGuidPk();

            PrimaryKeySetter.SetPrimaryKey(meta, entity, out string id);

            entity.Id.Should().NotBeEmpty();
            entity.Id.ToString().Should().Be(id);
        }

        [Fact]
        public void PrimaryKeyStringNotSet_SetPrimaryKey_SetsPk()
        {
            EntityMetadata meta = new EntityMetadata(typeof(TestEntityWithStringPk));

            TestEntityWithStringPk entity = new TestEntityWithStringPk();

            PrimaryKeySetter.SetPrimaryKey(meta, entity, out string id);

            entity.Id.Should().NotBeEmpty();
            entity.Id.Should().Be(id);
        }

        [Fact]
        public void PrimaryKeyGuidSet_SetPrimaryKey_DoesNotChangePk()
        {
            EntityMetadata meta = new EntityMetadata(typeof(TestEntityWithGuidPk));

            TestEntityWithGuidPk entity = new TestEntityWithGuidPk()
            {
                Id = Guid.NewGuid()
            };

            Guid originalGuid = entity.Id;

            PrimaryKeySetter.SetPrimaryKey(meta, entity, out string id);

            entity.Id.Should().Be(originalGuid);
            entity.Id.ToString().Should().Be(id);
        }

        [Fact]
        public void PrimaryKeyStringSet_SetPrimaryKey_DoesNotChangePk()
        {
            EntityMetadata meta = new EntityMetadata(typeof(TestEntityWithStringPk));

            TestEntityWithStringPk entity = new TestEntityWithStringPk()
            {
                Id = "something"
            };

            string originalId = entity.Id;

            PrimaryKeySetter.SetPrimaryKey(meta, entity, out string id);

            entity.Id.Should().Be(originalId);
            id.Should().Be(originalId);
        }

        [Fact]
        public void PrimaryKeyIntNotSet_SetPrimaryKey_DoesNotSetsPk()
        {
            EntityMetadata meta = new EntityMetadata(typeof(TestEntityWithIntPk));

            TestEntityWithIntPk entity = new TestEntityWithIntPk();

            PrimaryKeySetter.SetPrimaryKey(meta, entity, out string id);

            entity.Id.Should().Be(0);
            entity.Id.ToString().Should().Be(id);
        }

        [Fact]
        public void PrimaryKeyLongNotSet_SetPrimaryKey_DoesNotSetsPk()
        {
            EntityMetadata meta = new EntityMetadata(typeof(TestEntityWithLongPk));

            TestEntityWithLongPk entity = new TestEntityWithLongPk();

            PrimaryKeySetter.SetPrimaryKey(meta, entity, out string id);

            entity.Id.Should().Be(0);
            entity.Id.ToString().Should().Be(id);
        }
        
        private class TestEntityWithGuidPk
        {
            public Guid Id { get; set; }
        }

        private class TestEntityWithStringPk
        {
            public string Id { get; set; }
        }

        private class TestEntityWithIntPk
        {
            public int Id { get; set; }
        }

        private class TestEntityWithLongPk
        {
            public long Id { get; set; }
        }
    }
}
