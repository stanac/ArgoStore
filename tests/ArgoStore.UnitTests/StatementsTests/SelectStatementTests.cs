using ArgoStore.ExpressionToStatementTranslators;
using ArgoStore.Helpers;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
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
    }
}
