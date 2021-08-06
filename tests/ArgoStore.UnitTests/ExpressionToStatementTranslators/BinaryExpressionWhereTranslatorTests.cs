using FluentAssertions;
using ArgoStore.ExpressionToStatementTranslators;
using System;
using System.Linq.Expressions;
using Xunit;
// ReSharper disable PossibleNullReferenceException

namespace ArgoStore.UnitTests.ExpressionToStatementTranslators
{
    public class BinaryExpressionWhereTranslatorTests
    {
        [Fact]
        public void Translate_PropertyEqualsConstant_TranslatesToExpectedExpression()
        {
            Expression<Func<TestEntityPerson, bool>> ex = x => x.Name == "Kovalski";

            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(BinaryComparisonStatement));

            BinaryComparisonStatement c = where as BinaryComparisonStatement;

            c.Left.Should().BeOfType(typeof(PropertyAccessStatement));
            (c.Left as PropertyAccessStatement).Name.Should().Be(nameof(TestEntityPerson.Name));

            c.Right.Should().BeOfType(typeof(ConstantStatement));
            (c.Right as ConstantStatement).IsString.Should().BeTrue();
            (c.Right as ConstantStatement).Value.Should().Be("Kovalski");
        }

        [Fact]
        public void Translate_BoolPropertyAndComparison_TranslatesToExpectedExpression()
        {
            Expression<Func<TestEntityPerson, bool>> ex = x => x.Active || x.Name == "Marcus";

            Statement where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(BinaryLogicalStatement));

            BinaryLogicalStatement b = where as BinaryLogicalStatement;

            b.Left.Should().BeOfType(typeof(PropertyAccessStatement));
            (b.Left as PropertyAccessStatement).Name.Should().Be(nameof(TestEntityPerson.Active));

            b.Right.Should().BeOfType(typeof(BinaryComparisonStatement));

            (b.Right as BinaryComparisonStatement).Left.Should().BeOfType(typeof(PropertyAccessStatement));
            ((b.Right as BinaryComparisonStatement).Left as PropertyAccessStatement).Name.Should().Be(nameof(TestEntityPerson.Name));

            (b.Right as BinaryComparisonStatement).Right.Should().BeOfType(typeof(ConstantStatement));
            ((b.Right as BinaryComparisonStatement).Right as ConstantStatement).Value.Should().Be("Marcus");
        }

        [Fact]
        public void Translate_CompareEqualsNull_TranslatesToExpectedExpression()
        {
            Expression<Func<TestEntityPerson, bool>> ex = x => x.Key != null;

            Statement where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(BinaryComparisonStatement));

            var c = where as BinaryComparisonStatement;

            c.Left.Should().BeOfType<PropertyAccessStatement>();
            c.Left.As<PropertyAccessStatement>().Name.Should().Be(nameof(TestEntityPerson.Key));

            c.Operator.Should().Be(BinaryComparisonStatement.Operators.NotEqual);

            c.Right.Should().BeOfType<ConstantStatement>();
            c.Right.As<ConstantStatement>().IsNull.Should().BeTrue();
        }

        [Fact]
        public void WhereWithEqualFromEntityType_Translate_CreatesValidStatement()
        {
            TestEntityPerson p = new TestEntityPerson
            {
                Name = "Kovalski"
            };

            Expression<Func<TestEntityPerson, bool>> e = x => x.Name == p.Name;

            Statement where = ExpressionToStatementTranslatorStrategy.Translate(e);

            where.Should().BeOfType<BinaryComparisonStatement>();
            where.As<BinaryComparisonStatement>().Left.Should().BeOfType<PropertyAccessStatement>();
            where.As<BinaryComparisonStatement>().Right.Should().BeOfType<ConstantStatement>();
        }
    }
}
