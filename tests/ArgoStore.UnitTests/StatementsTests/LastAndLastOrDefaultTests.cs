using ArgoStore.ExpressionToStatementTranslators;
using FluentAssertions;
using System;
using System.Linq;
using System.Linq.Expressions;
using ArgoStore.Statements;
using Xunit;

namespace ArgoStore.UnitTests.StatementsTests
{
    public class LastAndLastOrDefaultTests
    {
        [Fact]
        public void LastOrDefaultOnQueryable_CreatesCorrectStatementAndSetsTop1()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.LastOrDefault();

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            SelectStatement s = st as SelectStatement;

            s.SelectElements.Should().ContainSingle();
            s.SelectElements[0].SelectsJson.Should().BeTrue();

            s.Top.Should().HaveValue();
            s.Top.Value.Should().Be(1);

            s.WhereStatement.Should().BeNull();

            s.CalledByMethod.Should().Be(CalledByMethods.LastOrDefault);
        }

        [Fact]
        public void LastOrDefaultOnQueryableWithFilter_SetsWhere()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.LastOrDefault(x => x.Key == "a");

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            SelectStatement s = st as SelectStatement;

            s.SelectElements.Should().ContainSingle();
            s.SelectElements[0].SelectsJson.Should().BeTrue();

            s.Top.Should().HaveValue();
            s.Top.Value.Should().Be(1);

            s.WhereStatement.Should().NotBeNull();
            s.WhereStatement.Statement.Should().BeOfType<BinaryComparisonStatement>();
            s.WhereStatement.Statement.As<BinaryComparisonStatement>().Left.Should().BeOfType<PropertyAccessStatement>();
        }

        [Fact]
        public void LastOrDefaultOnWhereWithFilter_SetsWhereStatementsInConjuction()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Where(x => x.Key == "b").LastOrDefault(x => x.Key == "a");

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            SelectStatement s = st as SelectStatement;

            s.SelectElements.Should().ContainSingle();
            s.SelectElements[0].SelectsJson.Should().BeTrue();

            s.Top.Should().HaveValue();
            s.Top.Value.Should().Be(1);

            s.WhereStatement.Should().NotBeNull();
            s.WhereStatement.Statement.Should().BeOfType<BinaryLogicalStatement>();
            s.WhereStatement.Statement.As<BinaryLogicalStatement>().IsAnd.Should().BeTrue();
        }

        [Fact]
        public void LastOnQueryable_CreatesCorrectStatementAndSetsTop1()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Last();

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            SelectStatement s = st as SelectStatement;

            s.SelectElements.Should().ContainSingle();
            s.SelectElements[0].SelectsJson.Should().BeTrue();

            s.Top.Should().HaveValue();
            s.Top.Value.Should().Be(1);

            s.WhereStatement.Should().BeNull();
        }

        [Fact]
        public void LastOnQueryableWithFilter_SetsWhere()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Last(x => x.Key == "a");

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            SelectStatement s = st as SelectStatement;

            s.SelectElements.Should().ContainSingle();
            s.SelectElements[0].SelectsJson.Should().BeTrue();

            s.Top.Should().HaveValue();
            s.Top.Value.Should().Be(1);

            s.WhereStatement.Should().NotBeNull();
            s.WhereStatement.Statement.Should().BeOfType<BinaryComparisonStatement>();
            s.WhereStatement.Statement.As<BinaryComparisonStatement>().Left.Should().BeOfType<PropertyAccessStatement>();
        }

        [Fact]
        public void LastOnWhereWithFilter_SetsWhereStatementsInConjuction()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Where(x => x.Key == "b").Last(x => x.Key == "a");

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            SelectStatement s = st as SelectStatement;

            s.SelectElements.Should().ContainSingle();
            s.SelectElements[0].SelectsJson.Should().BeTrue();

            s.Top.Should().HaveValue();
            s.Top.Value.Should().Be(1);

            s.WhereStatement.Should().NotBeNull();
            s.WhereStatement.Statement.Should().BeOfType<BinaryLogicalStatement>();
            s.WhereStatement.Statement.As<BinaryLogicalStatement>().IsAnd.Should().BeTrue();
        }
    }
}
