using FluentAssertions;
using System;
using Xunit;

namespace ArgoStore.UnitTests
{
    public class TenantIdTests
    {
        [Fact]
        public void CreateDefault_CreatesDefaultTenantId()
        {
            TenantId tenantId = TenantId.CreateDefault();
            tenantId.ToString().Should().NotBeNullOrEmpty();
            tenantId.IsDefault.Should().BeTrue();
            IsGuid(tenantId.ToString()).Should().BeFalse();
        }

        [Fact]
        public void CtorWithNonEmptyGuid_CreatesDefaultTenantId()
        {
            TenantId tenantId = new TenantId(Guid.NewGuid());
            tenantId.ToString().Should().NotBeNullOrEmpty();
            tenantId.IsDefault.Should().BeFalse();
            IsGuid(tenantId.ToString()).Should().BeTrue();
        }

        [Fact]
        public void CtorWithEmptyGuid_ThrowsException()
        {
            Action a = () => new TenantId(Guid.Empty);
            a.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Parse_DefaultString_ReturnsDefaultTenantId()
        {
            TenantId id = TenantId.Parse(TenantId.DefaultValue);
            id.IsDefault.Should().BeTrue();
        }

        [Fact]
        public void Parse_GuidString_ReturnsDefaultTenantId()
        {
            TenantId id = TenantId.Parse(Guid.NewGuid().ToString());
            id.IsDefault.Should().BeFalse();
        }

        [Fact]
        public void Parse_GuidNString_ReturnsDefaultTenantId()
        {
            TenantId id = TenantId.Parse(Guid.NewGuid().ToString("N"));
            id.IsDefault.Should().BeFalse();
        }

        [Theory]
        [InlineData(TenantId.DefaultValue)]
        [InlineData("{9B39708B-7A6B-4175-ADEF-F224E5CF7B5E}")]
        [InlineData("9B39708B-7A6B-4175-ADEF-F224E5CF7B5E")]
        [InlineData("9B39708B7A6B4175ADEFF224E5CF7B5E")]
        public void TryParse_ValuedValue_ReturnsFalse(string s)
        {
            bool success = TenantId.TryParse(s, out _);
            success.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData("123")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public void TryParse_NullOrEmptyValueOrInvalidValue_ReturnsFalse(string s)
        {
            bool success = TenantId.TryParse(s, out _);
            success.Should().BeFalse();
        }

        private bool IsGuid(TenantId id) => Guid.TryParse(id?.ToString(), out _);

        private bool IsGuid(string s) => Guid.TryParse(s, out _);
    }
}
