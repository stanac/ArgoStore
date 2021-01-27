using FluentAssertions;
using JsonDbLite.Expressions;
using JsonDbLite.WhereTranslators;
using System;
using System.Linq.Expressions;
using Xunit;

namespace JsonDbLite.UnitTests.WhereTranslators
{
    public class MethodStringInstanceTranslatorTests
    {
        [Fact]
        public void Transalte_StringTrim_TranslatesToExpectedExpression()
        {
            Expression<Func<TestEntityPerson, bool>> ex = x => x.Name.Trim() == "Marcus";

            var where = WhereTranslatorStrategy.Translate(ex);

            var methodCall = (where as WhereBinaryComparisonExpressionData)?.Left as WhereMethodCallExpressionData;

            methodCall.Should().NotBeNull();
            methodCall.Arguments[0].Should().BeOfType(typeof(WherePropertyExpressionData));
            (methodCall.Arguments[0] as WherePropertyExpressionData).Name.Should().Be(nameof(TestEntityPerson.Name));

            methodCall.MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringTrim);
        }

        [Fact]
        public void Transalte_StringToUpper_TranslatesToExpectedExpression()
        {
            Expression<Action> ex = () => "".ToUpper();

            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            (where as WhereMethodCallExpressionData).MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringToUpper);
        }

        [Fact]
        public void Transalte_StringToLower_TranslatesToExpectedExpression()
        {
            Expression<Action> ex = () => "".ToLower();

            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            (where as WhereMethodCallExpressionData).MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringToLower);
        }
    }
}
