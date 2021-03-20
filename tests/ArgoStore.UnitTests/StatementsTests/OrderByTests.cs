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
                    - ✅ order by desc on queryable
                    - ✅ then by on queryable
                    - ✅ then by 3x on queryable
                    - ✅ then by 3x desc on queryable

                    - order by on select
                    - order by desc on select
                    - then by on select
                    - then by 3x on select
                    - then by 3x desc on select

                    - order by on where
                    - order by desc on where
                    - then by on where
                    - then by 3x on where
                    - then by 3x desc on where

                    - order by on subquery
                    - order by desc on subquery
                    - then by on subquery
                    - then by 3x on subquery
                    - then by 3x desc on subquery
         */

        #region on queryable

        [Fact]
        public void OrderByOnQueryable_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.OrderBy(x => x.Name);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(OrderByStatement));
            st.As<OrderByStatement>().Elements.Should().ContainSingle();
            st.As<OrderByStatement>().Should().ContainOrderByElement(nameof(TestEntityPerson.Name), true);
        }

        [Fact]
        public void OrderByDescendingOnQueryable_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.OrderByDescending(x => x.Name);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(OrderByStatement));
            st.As<OrderByStatement>().Elements.Should().ContainSingle();
            st.As<OrderByStatement>().Should().ContainOrderByElement(nameof(TestEntityPerson.Name), false);
        }

        [Fact]
        public void OrderByThenByOnQueryable_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.OrderBy(x => x.Name).ThenBy(x => x.EmailAddress);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(OrderByStatement));
            st.As<OrderByStatement>().Elements.Should().HaveCount(2);
            st.As<OrderByStatement>().Should().ContainOrderByElement(nameof(TestEntityPerson.Name), true);
            st.As<OrderByStatement>().Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), true);
        }


        [Fact]
        public void OrderByMultipleThenByOnQueryable_SetsCorrectOrderInStatement()
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

        [Fact]
        public void OrderByDescMultipleThenByOnQueryable_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q
                .OrderByDescending(x => x.Name)
                .ThenByDescending(x => x.EmailAddress)
                .ThenByDescending(x => x.BirthYear)
                .ThenByDescending(x => x.Active);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(OrderByStatement));

            var obst = st as OrderByStatement;

            obst.Elements.Should().HaveCount(4);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), false, 0);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), false, 1);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.BirthYear), false, 2);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.Active), false, 3);
        }

        [Fact]
        public void OrderByAscDescMultipleThenByOnQueryable_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q
                .OrderByDescending(x => x.Name)
                .ThenBy(x => x.EmailAddress)
                .ThenByDescending(x => x.BirthYear)
                .ThenBy(x => x.Active);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(OrderByStatement));

            var obst = st as OrderByStatement;

            obst.Elements.Should().HaveCount(4);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), false, 0);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), true, 1);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.BirthYear), false, 2);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.Active), true, 3);
        }

        #endregion on queryable

        #region on select

        [Fact]
        public void OrderByOnSelect_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Select(x => x).OrderBy(x => x.Name);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var select = st as SelectStatement;

            select.OrderByStatement.Should().NotBeNull();
        }

        #endregion on select
    }
}
