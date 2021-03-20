using ArgoStore.ExpressionToStatementTranslators;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace ArgoStore.UnitTests.StatementsTests
{
    /*
            to cover:
                - ✅ order by on queryable
                - order by on select
                - order by on where
                - order by on subquery
                - order by desc on queryable
                - order by desc on select
                - order by desc on where
                - order by desc on subquery
                - then by on queryable
                - then by on select
                - then by on where
                - then by on subquery
                - then by 2 or 3 desc on queryable
                - then by 2 or 3 desc on select
                - then by 2 or 3 desc on where
                - then by 2 or 3 desc on subquery
                - then by 2 or 3 on queryable
                - then by 2 or 3 on select
                - then by 2 or 3 on where
                - then by 2 or 3 on subquery
                - then by 2 or 3 desc on queryable
                - then by 2 or 3 desc on select
                - then by 2 or 3 desc on where
                - then by 2 or 3 desc on subquery
     */

    public class OrderByTests
    {
        [Fact]
        public void OrderBy_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.OrderBy(x => x.Name);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            // 
        }
    }
}
