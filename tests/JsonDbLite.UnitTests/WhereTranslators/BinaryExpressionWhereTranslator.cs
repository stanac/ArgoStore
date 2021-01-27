using FluentAssertions;
using JsonDbLite.Expressions;
using JsonDbLite.WhereTranslators;
using System;
using System.Linq.Expressions;
using Xunit;

namespace JsonDbLite.UnitTests.WhereTranslators
{
    public class BinaryExpressionWhereTranslator
    {
        [Fact]
        public void Translate_PropertyEqualsConstant_TranslatesToExpectedExpression()
        {
            Expression<Func<TestEntityPerson, bool>> ex = x => x.Name == "Kovalski";

            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereBinaryComparisonExpressionData));

            WhereBinaryComparisonExpressionData c = where as WhereBinaryComparisonExpressionData;
          
            c.Left.Should().BeOfType(typeof(WherePropertyExpressionData));
            (c.Left as WherePropertyExpressionData).Name.Should().Be("Name");

            c.Right.Should().BeOfType(typeof(WhereConstantExpressionData));
            (c.Right as WhereConstantExpressionData).IsString.Should().BeTrue();
            (c.Right as WhereConstantExpressionData).Value.Should().Be("Kovalski");
        }

        [Fact]
        public void Translate_BoolPropertyAndComparison_TranslatesToExpectedExpression()
        {
            Expression<Func<TestEntityPerson, bool>> ex = x => x.Active || x.Name == "Marcus";

            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereBinaryLogicalExpressionData));

            var b = where as WhereBinaryLogicalExpressionData;

            b.Left.Should().BeOfType(typeof(WherePropertyExpressionData));
            (b.Left as WherePropertyExpressionData).Name.Should().Be("Active");

            b.Right.Should().BeOfType(typeof(WhereBinaryComparisonExpressionData));

            (b.Right as WhereBinaryComparisonExpressionData).Left.Should().BeOfType(typeof(WherePropertyExpressionData));
            ((b.Right as WhereBinaryComparisonExpressionData).Left as WherePropertyExpressionData).Name.Should().Be("Name");

            (b.Right as WhereBinaryComparisonExpressionData).Right.Should().BeOfType(typeof(WhereConstantExpressionData));
            ((b.Right as WhereBinaryComparisonExpressionData).Right as WhereConstantExpressionData).Value.Should().Be("Marcus");
        }
    }
}
