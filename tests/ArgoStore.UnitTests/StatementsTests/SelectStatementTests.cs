using ArgoStore.ExpressionToStatementTranslators;
using ArgoStore.Helpers;
using FluentAssertions;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace ArgoStore.UnitTests.StatementsTests
{
    public class SelectStatementTests
    {
        [Fact]
        public void SelectAnonymousObject_Translate_ReturnsCorrectStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Select(x => new { x.Active, x.BirthYear });

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            SelectStatement s = st as SelectStatement;
            s.TypeFrom.Should().Be(typeof(TestEntityPerson));
            TypeHelpers.IsAnonymousType(s.TypeTo).Should().BeTrue();

            s.SelectElements.Should().HaveCount(2);

            s.SelectElements[0].InputProperty.Should().Be("Active");
            s.SelectElements[0].OutputProperty.Should().Be("Active");

            s.SelectElements[1].InputProperty.Should().Be("BirthYear");
            s.SelectElements[1].OutputProperty.Should().Be("BirthYear");
        }

        [Fact]
        public void SelectAnonymousObjectRenameProps_Translate_ReturnsCorrectStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Select(x => new { Active1 = x.Active, BirthYear1 = x.BirthYear });

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            SelectStatement s = st as SelectStatement;
            s.TypeFrom.Should().Be(typeof(TestEntityPerson));
            TypeHelpers.IsAnonymousType(s.TypeTo).Should().BeTrue();

            s.SelectElements.Should().HaveCount(2);

            s.SelectElements[0].InputProperty.Should().Be("Active");
            s.SelectElements[0].OutputProperty.Should().Be("Active1");

            s.SelectElements[1].InputProperty.Should().Be("BirthYear");
            s.SelectElements[1].OutputProperty.Should().Be("BirthYear1");
        }


        [Fact]
        public void SelectRenameProps_Translate_ReturnsCorrectStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q =>
                q.Select(x => new TestEntityPerson { EmailAddress = x.Name, Key = x.EmailAddress });

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            SelectStatement s = st as SelectStatement;
            s.TypeFrom.Should().Be(typeof(TestEntityPerson));
            TypeHelpers.IsAnonymousType(s.TypeTo).Should().BeFalse();

            s.SelectElements.Should().HaveCount(2);

            s.SelectElements[0].InputProperty.Should().Be("Name");
            s.SelectElements[0].OutputProperty.Should().Be("EmailAddress");

            s.SelectElements[1].InputProperty.Should().Be("EmailAddress");
            s.SelectElements[1].OutputProperty.Should().Be("Key");
        }
    }
}
