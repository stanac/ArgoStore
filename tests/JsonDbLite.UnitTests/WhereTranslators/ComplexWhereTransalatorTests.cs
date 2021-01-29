using FluentAssertions;
using JsonDbLite.Expressions;
using JsonDbLite.WhereTranslators;
using System;
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

        //[Fact]
        //public void Transalte_ExpressionWithIEnumerableContains_TranslatesToExpectedExpression()
        //{
        //    string[] allowedNames = new[] { "Marcus", "Kovalski" };

        //    Expression<Func<TestEntityPerson, bool>> ex = x => allowedNames.Contains(x.Name);

        //    WhereClauseExpressionData where = WhereTranslatorStrategy.Translate(ex);


        //}

    }
}
