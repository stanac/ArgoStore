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
        public void Transalte_StringComparisonOrinalIgnoreCase_TranslatesToExpectedExpression()
        {
            Expression<Action> ex = () => string.Equals("Marcus", "MARCUS", StringComparison.OrdinalIgnoreCase);

            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            (where as WhereMethodCallExpressionData).MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringEqualsIgnoreCase);
        }
    }
}
