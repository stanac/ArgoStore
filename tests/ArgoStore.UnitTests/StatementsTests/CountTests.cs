using ArgoStore.ExpressionToStatementTranslators;
using FluentAssertions;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace ArgoStore.UnitTests.StatementsTests
{
    public class CountTests
    {
        [Fact]
        public void CountOnQueryable_CreatesCorrectStatementAndSetsMethod()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Count();

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectCountStatement));
            st.As<SelectCountStatement>().FromType.Should().Be(typeof(TestEntityPerson));
        }

        [Fact]
        public void LongCountOnQueryable_CreatesCorrectStatementAndSetsMethod()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.LongCount();

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectCountStatement));
            st.As<SelectCountStatement>().FromType.Should().Be(typeof(TestEntityPerson));
            st.As<SelectCountStatement>().LongCount.Should().BeTrue();
        }
    }
}
