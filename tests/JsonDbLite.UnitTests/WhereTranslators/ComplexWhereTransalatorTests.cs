using FluentAssertions;
using JsonDbLite.Expressions;
using JsonDbLite.WhereTranslators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace JsonDbLite.UnitTests.WhereTranslators
{
    public class ComplexWhereTransalatorTests
    {

        [Fact]
        public void Transalte_ExpressionWithBrackets_TranslatesToExpectedExpression()
        {
            Expression<Func<TestEntityPerson, bool>> ex = x => x.Active || (x.BirthYear > 1980 && x.BirthYear < 1990);

            WhereClauseExpressionData where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereBinaryLogicalExpressionData));

            var w = where as WhereBinaryLogicalExpressionData;
            w.IsOr.Should().BeTrue();

            w.Left.Should().BeOfType(typeof(WherePropertyExpressionData));
            
            w.Right.Should().BeOfType(typeof(WhereBinaryLogicalExpressionData));
            (w.Right as WhereBinaryLogicalExpressionData).IsAnd.Should().BeTrue();
        }

        [Fact]
        public void Transalte_ExpressionWithStringArrayContains_TranslatesToExpectedExpression()
        {
            string[] allowedNames = new[] { "Marcus", "Kovalski" };

            Expression<Func<TestEntityPerson, bool>> ex = x => allowedNames.Contains(x.Name);

            WhereClauseExpressionData where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            var m = where as WhereMethodCallExpressionData;
            m.MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.EnumerableContains);
            m.Arguments.Should().HaveCount(2);
            m.Arguments[0].Should().BeOfType(typeof(WherePropertyExpressionData));
            m.Arguments[0].As<WherePropertyExpressionData>().Name.Should().Be(nameof(TestEntityPerson.Name));
            m.Arguments[1].Should().BeOfType(typeof(WhereConstantExpressionData));
            m.Arguments[1].As<WhereConstantExpressionData>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().NotBeNull();
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().HaveCount(allowedNames.Length);

            foreach (var n in allowedNames)
            {
                m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain(n);
            }
        }

        [Fact]
        public void Transalte_ExpressionWithStringListContains_TranslatesToExpectedExpression()
        {
            IReadOnlyList<string> allowedNames = new List<string> { "Marcus", "Kovalski" };

            Expression<Func<TestEntityPerson, bool>> ex = x => allowedNames.Contains(x.Name);

            WhereClauseExpressionData where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            var m = where as WhereMethodCallExpressionData;
            m.MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.EnumerableContains);
            m.Arguments.Should().HaveCount(2);
            m.Arguments[0].Should().BeOfType(typeof(WherePropertyExpressionData));
            m.Arguments[0].As<WherePropertyExpressionData>().Name.Should().Be(nameof(TestEntityPerson.Name));
            m.Arguments[1].Should().BeOfType(typeof(WhereConstantExpressionData));
            m.Arguments[1].As<WhereConstantExpressionData>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().NotBeNull();
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().HaveCount(allowedNames.Count);

            foreach (var n in allowedNames)
            {
                m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain(n);
            }
        }

        [Fact]
        public void Transalte_ExpressionWithIntListContains_TranslatesToExpectedExpression()
        {
            IReadOnlyList<int> years = new List<int> { 1, 2, 3, 4 };

            Expression<Func<TestEntityPerson, bool>> ex = x => years.Contains(x.BirthYear);

            WhereClauseExpressionData where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            var m = where as WhereMethodCallExpressionData;
            m.MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.EnumerableContains);
            m.Arguments.Should().HaveCount(2);
            m.Arguments[0].Should().BeOfType(typeof(WherePropertyExpressionData));
            m.Arguments[0].As<WherePropertyExpressionData>().Name.Should().Be(nameof(TestEntityPerson.BirthYear));
            m.Arguments[1].Should().BeOfType(typeof(WhereConstantExpressionData));
            m.Arguments[1].As<WhereConstantExpressionData>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().NotBeNull();
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().HaveCount(years.Count);

            foreach (var n in years)
            {
                m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain(n.ToString());
            }
        }
    }
}
