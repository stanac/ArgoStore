using ArgoStore.ExpressionToStatementTranslators;
using FluentAssertions;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace ArgoStore.UnitTests.StatementsTests
{

    public class AnyTests
    {
        [Fact]
        public void AnyOnQueryable_CreatesCorrectStatementAndSetsMethod()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Any();

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));
        }
    }
}
