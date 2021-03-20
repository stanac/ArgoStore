using ArgoStore.ExpressionToStatementTranslators;
using FluentAssertions;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace ArgoStore.UnitTests.StatementsTests
{
    public class OrderByTests
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
                    - ✅ then by on queryable
                    - then by on select
                    - then by on where
                    - then by on subquery
                    - ✅ then by 2 or 3 on queryable
                    - then by 2 or 3 on select
                    - then by 2 or 3 on where
                    - then by 2 or 3 on subquery
                    - then by 2 or 3 desc on queryable
                    - then by 2 or 3 desc on select
                    - then by 2 or 3 desc on where
                    - then by 2 or 3 desc on subquery
         */

        [Fact]
        public void OrderBy_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.OrderBy(x => x.Name);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(OrderByStatement));
            st.As<OrderByStatement>().Elements.Should().ContainSingle();
            st.As<OrderByStatement>().Should().ContainOrderByElement(nameof(TestEntityPerson.Name), true);
        }

        [Fact]
        public void OrderByThenBy_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.OrderBy(x => x.Name).ThenBy(x => x.EmailAddress);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(OrderByStatement));
            st.As<OrderByStatement>().Elements.Should().HaveCount(2);
            st.As<OrderByStatement>().Should().ContainOrderByElement(nameof(TestEntityPerson.Name), true);
            st.As<OrderByStatement>().Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), true);
        }


        [Fact]
        public void OrderByMultipleThenBy_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q
                .OrderBy(x => x.Name)
                .ThenBy(x => x.EmailAddress)
                .ThenBy(x => x.BirthYear)
                .ThenBy(x => x.Active);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(OrderByStatement));

            var obst = st as OrderByStatement;

            obst.Elements.Should().HaveCount(4);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), true, 0);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), true, 1);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.BirthYear), true, 2);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.Active), true, 3);
        }


    }
}
