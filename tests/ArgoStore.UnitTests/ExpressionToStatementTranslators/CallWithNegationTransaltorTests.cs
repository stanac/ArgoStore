using FluentAssertions;
using ArgoStore.ExpressionToStatementTranslators;
using System;
using System.Linq.Expressions;
using Xunit;

namespace ArgoStore.UnitTests.ExpressionToStatementTranslators
{
    public class CallWithNegationTransaltorTests
    {
        [Fact]
        public void Translate_NotEqual_TranslatesToNotEqual()
        {
            Expression<Func<TestEntityPerson, bool>> ex = x => !(x.Name == "Kovalski");

            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType<BinaryComparisonStatement>();
            where.As<BinaryComparisonStatement>().Operator.Should().Be(BinaryComparisonStatement.Operators.NotEqual);
        }

        [Fact]
        public void Translate_BinaryLogicalOperator_AppliesDeMorgansLaw()
        {
            Expression<Func<TestEntityPerson, bool>> ex = x => !(x.Name == "Kovalski" || x.Active);

            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);
            where = where.ReduceIfPossible();

            where.Should().BeOfType<BinaryLogicalStatement>();
            where.As<BinaryLogicalStatement>().IsAnd.Should().BeTrue();
        }
    }
}
