using ArgoStore.ExpressionToStatementTranslators;
using FluentAssertions;
using System;
using System.Linq;
using System.Linq.Expressions;
using ArgoStore.Statements;
using Xunit;

namespace ArgoStore.UnitTests.StatementsTests
{
    public class SingleAndSingleOrDefaultTests
    {
        [Fact]
        public void SingleOrDefaultOnQueryable_CreatesCorrectStatementAndSetsTop1()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.SingleOrDefault();

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
        public void SingleOrDefaultOnQueryableWithFilter_SetsWhere()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.SingleOrDefault(x => x.Key == "a");

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
        public void SingleOrDefaultOnWhereWithFilter_SetsWhereStatementsInConjuction()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Where(x => x.Key == "b").SingleOrDefault(x => x.Key == "a");

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
        public void SingleOnQueryable_CreatesCorrectStatementAndSetsTop1()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Single();

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
        public void SingleOnQueryableWithFilter_SetsWhere()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Single(x => x.Key == "a");

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
        public void SingleOnWhereWithFilter_SetsWhereStatementsInConjuction()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Where(x => x.Key == "b").Single(x => x.Key == "a");

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
