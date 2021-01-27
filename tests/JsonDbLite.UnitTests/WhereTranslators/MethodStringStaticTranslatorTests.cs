using FluentAssertions;
using JsonDbLite.Expressions;
using JsonDbLite.WhereTranslators;
using System;
using System.Linq.Expressions;
using Xunit;

namespace JsonDbLite.UnitTests.WhereTranslators
{
    public class MethodStringStaticTranslatorTests
    {
        [Fact]
        public void Transalte_StringIsNullOrEmpty_TranslatesToExpectedExpression()
        {
            Expression<Action> ex = () => string.IsNullOrEmpty("");

            WhereClauseExpressionData where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));

            (where as WhereMethodCallExpressionData).MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringIsNullOrEmpty);
        }

        [Fact]
        public void Transalte_StringIsNullOrWhiteSpace_TranslatesToExpectedExpression()
        {
            Expression<Action> ex = () => string.IsNullOrWhiteSpace("");

            WhereClauseExpressionData where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));

            (where as WhereMethodCallExpressionData).MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringIsWhiteSpace);
        }

        [Fact]
        public void Transalte_StringComparisonNoTypeSet_TranslatesToExpectedExpression()
        {
            Expression<Action> ex = () => string.Equals("Marcus", "MARCUS");

            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));

            (where as WhereMethodCallExpressionData).MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringEquals);
        }

        [Theory]
        [InlineData(StringComparison.OrdinalIgnoreCase, true)]
        [InlineData(StringComparison.Ordinal, false)]
        [InlineData(StringComparison.CurrentCultureIgnoreCase, true)]
        [InlineData(StringComparison.CurrentCulture, false)]
        [InlineData(StringComparison.InvariantCultureIgnoreCase, true)]
        [InlineData(StringComparison.InvariantCulture, false)]
        public void Transalte_StringComparison_TranslatesToExpectedExpression(StringComparison c, bool shouldIgnoreCase)
        {
            Expression<Action> ex = () => string.Equals("Marcus", "MARCUS", c);

            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));

            WhereMethodCallExpressionData.SupportedMethodNames expected = shouldIgnoreCase 
                ? WhereMethodCallExpressionData.SupportedMethodNames.StringEqualsIgnoreCase
                : WhereMethodCallExpressionData.SupportedMethodNames.StringEquals;

            (where as WhereMethodCallExpressionData).MethodName.Should().Be(expected);
        }
    }
}
