using FluentAssertions;
using JsonDbLite.Expressions;
using JsonDbLite.WhereTranslators;
using System;
using System.Linq.Expressions;
using Xunit;

namespace JsonDbLite.UnitTests.WhereTranslators
{
    public class MemberExpressionTranslatorTests
    {
        [Fact]
        public void Translate_BoolProperty_TranslatesToExpectedExpression()
        {
            Expression<Func<TestEntityPerson, bool>> ex = x => x.Active;

            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WherePropertyExpressionData));

            var p = where as WherePropertyExpressionData;
            p.Name.Should().Be("Active");
            p.IsBoolean.Should().BeTrue();
        }

    }
}
