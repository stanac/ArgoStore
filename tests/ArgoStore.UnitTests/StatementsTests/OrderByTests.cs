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

             - ✅ order by on select
             - ✅ order by desc on select
             - ✅ then by on select
             - ✅ then by 3x on select
             - ✅ then by 3x desc on select

             - ✅ order by on where
             - ✅ order by desc on where
             - ✅ then by on where
             - ✅ then by 3x on where
             - ✅ then by 3x desc on where
        
             - ✅ where, select, order by
             - ✅ where, select, order by desc
             - ✅ where, select, then by
             - ✅ where, select, 3x order by
             - ✅ where, select, 3x then by desc
             - ✅ where, select, 3x then by asc, desc
        
             - ✅ order, where, select, order by
             - ✅ order, where, select, order by desc
             - ✅ order, where, select, then by
             - ✅ order, where, select, then by desc
             - ✅ order, where, select, 3x order by
             - ✅ order, where, select, 3x then by desc
             - ✅ order, where, select, 3x then by asc, desc
        
             - ✅ order by on subquery
             - ✅ order by desc on subquery
             - ✅ then by on subquery
             - ✅ then desc by on subquery
             - ✅ order, select, where, select, order by
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

            select.OrderByStatement.Elements.Should().ContainSingle();
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), true);
        }

        [Fact]
        public void OrderByDescendingOnSelect_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Select(x => x).OrderByDescending(x => x.Name);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var select = st as SelectStatement;

            select.OrderByStatement.Should().NotBeNull();

            select.OrderByStatement.Elements.Should().ContainSingle();
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), false);
        }

        [Fact]
        public void OrderByThenByOnSelect_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Select(x => x).OrderBy(x => x.Name).ThenBy(x => x.EmailAddress);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));
            var s = st as SelectStatement;

            s.OrderByStatement.Elements.Should().HaveCount(2);
            s.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), true);
            s.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), true);
        }

        [Fact]
        public void OrderByMultipleThenByOnSelect_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q
                .Select(x => x)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.EmailAddress)
                .ThenBy(x => x.BirthYear)
                .ThenBy(x => x.Active);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var obst = (st as SelectStatement).OrderByStatement;

            obst.Elements.Should().HaveCount(4);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), true, 0);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), true, 1);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.BirthYear), true, 2);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.Active), true, 3);
        }

        [Fact]
        public void OrderByDescMultipleThenByOnSelect_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q
                .Select(x => x)
                .OrderByDescending(x => x.Name)
                .ThenByDescending(x => x.EmailAddress)
                .ThenByDescending(x => x.BirthYear)
                .ThenByDescending(x => x.Active);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var obst = (st as SelectStatement).OrderByStatement;

            obst.Elements.Should().HaveCount(4);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), false, 0);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), false, 1);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.BirthYear), false, 2);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.Active), false, 3);
        }

        [Fact]
        public void OrderByAscDescMultipleThenByOnSelect_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q
                .Select(x => x)
                .OrderByDescending(x => x.Name)
                .ThenBy(x => x.EmailAddress)
                .ThenByDescending(x => x.BirthYear)
                .ThenBy(x => x.Active);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var obst = (st as SelectStatement).OrderByStatement;

            obst.Elements.Should().HaveCount(4);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), false, 0);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), true, 1);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.BirthYear), false, 2);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.Active), true, 3);
        }

        #endregion on select

        #region on where

        [Fact]
        public void OrderByOnWhere_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Where(x => x.Name != null).OrderBy(x => x.Name);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var select = st as SelectStatement;

            select.OrderByStatement.Should().NotBeNull();

            select.OrderByStatement.Elements.Should().ContainSingle();
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), true);
        }

        [Fact]
        public void OrderByDescendingOnWhere_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Where(x => x.Name != null).OrderByDescending(x => x.Name);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var select = st as SelectStatement;

            select.OrderByStatement.Should().NotBeNull();

            select.OrderByStatement.Elements.Should().ContainSingle();
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), false);
        }

        [Fact]
        public void OrderByThenByOnWhere_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Where(x => x.Name != null).OrderBy(x => x.Name).ThenBy(x => x.EmailAddress);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));
            var s = st as SelectStatement;

            s.OrderByStatement.Elements.Should().HaveCount(2);
            s.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), true);
            s.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), true);
        }

        [Fact]
        public void OrderByMultipleThenByOnWhere_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q
                .Where(x => x.Name != null)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.EmailAddress)
                .ThenBy(x => x.BirthYear)
                .ThenBy(x => x.Active);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var obst = (st as SelectStatement).OrderByStatement;

            obst.Elements.Should().HaveCount(4);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), true, 0);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), true, 1);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.BirthYear), true, 2);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.Active), true, 3);
        }

        [Fact]
        public void OrderByDescMultipleThenByOnWhere_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q
                .Where(x => x.Name != null)
                .OrderByDescending(x => x.Name)
                .ThenByDescending(x => x.EmailAddress)
                .ThenByDescending(x => x.BirthYear)
                .ThenByDescending(x => x.Active);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var obst = (st as SelectStatement).OrderByStatement;

            obst.Elements.Should().HaveCount(4);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), false, 0);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), false, 1);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.BirthYear), false, 2);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.Active), false, 3);
        }

        [Fact]
        public void OrderByAscDescMultipleThenByOnWhere_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q
                .Where(x => x.Name != null)
                .OrderByDescending(x => x.Name)
                .ThenBy(x => x.EmailAddress)
                .ThenByDescending(x => x.BirthYear)
                .ThenBy(x => x.Active);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var obst = (st as SelectStatement).OrderByStatement;

            obst.Elements.Should().HaveCount(4);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), false, 0);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), true, 1);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.BirthYear), false, 2);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.Active), true, 3);
        }

        #endregion on where

        #region order by where select

        [Fact]
        public void OrderByThenWhere_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.OrderBy(x => x.Name).Where(x => x.Active);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var select = st as SelectStatement;

            select.OrderByStatement.Should().NotBeNull();

            select.OrderByStatement.Elements.Should().ContainSingle();
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), true);

            select.WhereStatement.Statement.Should().BeStatement<PropertyAccessStatement>(s => s.Name == nameof(TestEntityPerson.Active) && s.IsBoolean);
        }

        [Fact]
        public void OrderByDescendingThenWhere_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.OrderByDescending(x => x.Name).Where(x => x.Active);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var select = st as SelectStatement;

            select.OrderByStatement.Should().NotBeNull();

            select.OrderByStatement.Elements.Should().ContainSingle();
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), false);

            select.WhereStatement.Statement.Should().BeStatement<PropertyAccessStatement>(s => s.Name == nameof(TestEntityPerson.Active) && s.IsBoolean);
        }

        [Fact]
        public void OrderByThenByThenWhere_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.OrderBy(x => x.Name).ThenBy(x => x.EmailAddress).Where(x => x.Active);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));
            var s = st as SelectStatement;

            s.OrderByStatement.Elements.Should().HaveCount(2);
            s.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), true);
            s.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), true);

            s.WhereStatement.Statement.Should().BeStatement<PropertyAccessStatement>(s => s.Name == nameof(TestEntityPerson.Active) && s.IsBoolean);
        }

        [Fact]
        public void OrderByMultipleThenByThenWhere_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q
                .OrderBy(x => x.Name)
                .ThenBy(x => x.EmailAddress)
                .ThenBy(x => x.BirthYear)
                .ThenBy(x => x.Active)
                .Where(x => x.Active);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var obst = (st as SelectStatement).OrderByStatement;

            obst.Elements.Should().HaveCount(4);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), true, 0);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), true, 1);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.BirthYear), true, 2);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.Active), true, 3);

            (st as SelectStatement).WhereStatement.Statement.Should().BeStatement<PropertyAccessStatement>(s => s.Name == nameof(TestEntityPerson.Active) && s.IsBoolean);
        }

        [Fact]
        public void OrderByDescMultipleThenByThenWhere_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q
                .OrderByDescending(x => x.Name)
                .ThenByDescending(x => x.EmailAddress)
                .ThenByDescending(x => x.BirthYear)
                .ThenByDescending(x => x.Active)
                .Where(x => x.Active);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var obst = (st as SelectStatement).OrderByStatement;

            obst.Elements.Should().HaveCount(4);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), false, 0);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), false, 1);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.BirthYear), false, 2);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.Active), false, 3);

            (st as SelectStatement).WhereStatement.Statement.Should().BeStatement<PropertyAccessStatement>(s => s.Name == nameof(TestEntityPerson.Active) && s.IsBoolean);
        }

        [Fact]
        public void OrderByAscDescMultipleThenByThenWhere_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q
                .OrderByDescending(x => x.Name)
                .ThenBy(x => x.EmailAddress)
                .ThenByDescending(x => x.BirthYear)
                .ThenBy(x => x.Active)
                .Where(x => x.Active);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var obst = (st as SelectStatement).OrderByStatement;

            obst.Elements.Should().HaveCount(4);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), false, 0);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), true, 1);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.BirthYear), false, 2);
            obst.Should().ContainOrderByElement(nameof(TestEntityPerson.Active), true, 3);

            (st as SelectStatement).WhereStatement.Statement.Should().BeStatement<PropertyAccessStatement>(s => s.Name == nameof(TestEntityPerson.Active) && s.IsBoolean);
        }

        #endregion order by where select

        #region order by where select order

        [Fact]
        public void OrderByThenWhereThenOrder_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.OrderBy(x => x.Name).Where(x => x.Active).OrderBy(x => x.Key);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var select = st as SelectStatement;

            select.OrderByStatement.Should().NotBeNull();

            select.OrderByStatement.Elements.Should().HaveCount(2);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), true);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Key), true);

            select.WhereStatement.Statement.Should().BeStatement<PropertyAccessStatement>(s => s.Name == nameof(TestEntityPerson.Active) && s.IsBoolean);
        }

        [Fact]
        public void OrderByDescendingThenWhereThenOrder_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => 
                q.OrderBy(x => x.Name)
                .Where(x => x.Active)
                .OrderByDescending(x => x.Key);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var select = st as SelectStatement;

            select.OrderByStatement.Should().NotBeNull();

            select.OrderByStatement.Elements.Should().HaveCount(2);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), true);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Key), false);

            select.WhereStatement.Statement.Should().BeStatement<PropertyAccessStatement>(s => s.Name == nameof(TestEntityPerson.Active) && s.IsBoolean);
        }

        [Fact]
        public void OrderByThenWhereThenOrderThenBy_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q =>
                q.OrderBy(x => x.Name)
                .Where(x => x.Active)
                .OrderBy(x => x.Key)
                .ThenBy(x => x.EmailAddress);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var select = st as SelectStatement;

            select.OrderByStatement.Should().NotBeNull();

            select.OrderByStatement.Elements.Should().HaveCount(3);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), true);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Key), true);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), true);

            select.WhereStatement.Statement.Should().BeStatement<PropertyAccessStatement>(s => s.Name == nameof(TestEntityPerson.Active) && s.IsBoolean);
        }

        [Fact]
        public void OrderByThenWhereThenOrderThenByDesc_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q =>
                q.OrderBy(x => x.Name)
                .Where(x => x.Active)
                .OrderBy(x => x.Key)
                .ThenByDescending(x => x.EmailAddress);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var select = st as SelectStatement;

            select.OrderByStatement.Should().NotBeNull();

            select.OrderByStatement.Elements.Should().HaveCount(3);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), true);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Key), true);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), false);

            select.WhereStatement.Statement.Should().BeStatement<PropertyAccessStatement>(s => s.Name == nameof(TestEntityPerson.Active) && s.IsBoolean);
        }

        [Fact]
        public void OrderByThenWhereThenOrderThenByThreeTimes_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q =>
                q.OrderBy(x => x.Name)
                .Where(x => x.Active)
                .OrderBy(x => x.Key)
                .ThenBy(x => x.EmailAddress)
                .ThenBy(x => x.BirthYear)
                .ThenBy(x => x.Active);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var select = st as SelectStatement;

            select.OrderByStatement.Should().NotBeNull();

            select.OrderByStatement.Elements.Should().HaveCount(5);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), true);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Key), true);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), true);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.BirthYear), true);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Active), true);

            select.WhereStatement.Statement.Should().BeStatement<PropertyAccessStatement>(s => s.Name == nameof(TestEntityPerson.Active) && s.IsBoolean);
        }

        [Fact]
        public void OrderByThenWhereThenOrderThenByDescThreeTimes_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q =>
                q.OrderBy(x => x.Name)
                .Where(x => x.Active)
                .OrderBy(x => x.Key)
                .ThenByDescending(x => x.EmailAddress)
                .ThenByDescending(x => x.BirthYear)
                .ThenByDescending(x => x.Active);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var select = st as SelectStatement;

            select.OrderByStatement.Should().NotBeNull();

            select.OrderByStatement.Elements.Should().HaveCount(5);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), true);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Key), true);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), false);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.BirthYear), false);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Active), false);

            select.WhereStatement.Statement.Should().BeStatement<PropertyAccessStatement>(s => s.Name == nameof(TestEntityPerson.Active) && s.IsBoolean);
        }

        [Fact]
        public void OrderByThenWhereThenOrderThenByAscDescThreeTimes_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q =>
                q.OrderBy(x => x.Name)
                .Where(x => x.Active)
                .OrderBy(x => x.Key)
                .ThenBy(x => x.EmailAddress)
                .ThenByDescending(x => x.BirthYear)
                .ThenBy(x => x.Active);

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var select = st as SelectStatement;

            select.OrderByStatement.Should().NotBeNull();

            select.OrderByStatement.Elements.Should().HaveCount(5);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Name), true);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Key), true);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), true);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.BirthYear), false);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Active), true);

            select.WhereStatement.Statement.Should().BeStatement<PropertyAccessStatement>(s => s.Name == nameof(TestEntityPerson.Active) && s.IsBoolean);
        }

        #endregion order by where select order

        #region order by on subquery

        [Fact]
        public void OrderByOnSubQuery_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q
                .Select(x => new { x.Active, x.EmailAddress, x.BirthYear })
                .Where(x => x.Active)
                .Select(x => new { x.EmailAddress, x.BirthYear })
                .OrderBy(x => x.BirthYear)
            ;

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var select = st as SelectStatement;

            select.OrderByStatement.Should().NotBeNull();

            select.OrderByStatement.Elements.Should().ContainSingle();
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.BirthYear), true);

            select.SubQueryStatement.Should().NotBeNull();
        }

        [Fact]
        public void OrderByDescOnSubQuery_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q
                .Select(x => new { x.Active, x.EmailAddress, x.BirthYear })
                .Where(x => x.Active)
                .Select(x => new { x.EmailAddress, x.BirthYear })
                .OrderByDescending(x => x.BirthYear)
            ;

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var select = st as SelectStatement;

            select.OrderByStatement.Should().NotBeNull();

            select.OrderByStatement.Elements.Should().ContainSingle();
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.BirthYear), false);

            select.SubQueryStatement.Should().NotBeNull();
        }

        [Fact]
        public void OrderByThenByOnSubQuery_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q
                .Select(x => new { x.Active, x.EmailAddress, x.BirthYear })
                .Where(x => x.Active)
                .Select(x => new { x.EmailAddress, x.BirthYear })
                .OrderBy(x => x.BirthYear)
                .ThenBy(x => x.EmailAddress)
            ;

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var select = st as SelectStatement;

            select.OrderByStatement.Should().NotBeNull();

            select.OrderByStatement.Elements.Should().HaveCount(2);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.BirthYear), true);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), true);

            select.SubQueryStatement.Should().NotBeNull();
        }

        [Fact]
        public void OrderByThenByDescOnSubQuery_SetsCorrectOrderInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q
                .Select(x => new { x.Active, x.EmailAddress, x.BirthYear })
                .Where(x => x.Active)
                .Select(x => new { x.EmailAddress, x.BirthYear })
                .OrderBy(x => x.BirthYear)
                .ThenByDescending(x => x.EmailAddress)
            ;

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var select = st as SelectStatement;

            select.OrderByStatement.Should().NotBeNull();

            select.OrderByStatement.Elements.Should().HaveCount(2);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.BirthYear), true);
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), false);

            select.SubQueryStatement.Should().NotBeNull();
        }

        [Fact]
        public void OrderByWhereOnSubQueryOrderBy_SetsCorrectOrderByInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q
                .OrderBy(x => x.Active)
                .Select(x => new { x.Active, x.EmailAddress, x.BirthYear })
                .Where(x => x.Active)
                .Select(x => new { x.EmailAddress, x.BirthYear })
                .OrderBy(x => x.BirthYear)
            ;

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var select = st as SelectStatement;

            select.OrderByStatement.Should().NotBeNull();

            select.OrderByStatement.Elements.Should().ContainSingle();
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.BirthYear), true);
            
            select.SubQueryStatement.Should().NotBeNull();
            select.SubQueryStatement.OrderByStatement.Should().NotBeNull();
            select.SubQueryStatement.OrderByStatement.Elements.Should().ContainSingle();
            select.SubQueryStatement.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Active), true);
        }
        [Fact]
        public void OrderByDescWhereOnSubQueryOrderByDesc_SetsCorrectOrderByInStatement()
        {
            Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q
                .OrderByDescending(x => x.Active)
                .Select(x => new { x.Active, x.EmailAddress, x.BirthYear })
                .Where(x => x.Active)
                .Select(x => new { x.EmailAddress, x.BirthYear })
                .OrderByDescending(x => x.BirthYear)
            ;

            Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

            st.Should().BeOfType(typeof(SelectStatement));

            var select = st as SelectStatement;

            select.OrderByStatement.Should().NotBeNull();

            select.OrderByStatement.Elements.Should().ContainSingle();
            select.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.BirthYear), false);

            select.SubQueryStatement.Should().NotBeNull();
            select.SubQueryStatement.OrderByStatement.Should().NotBeNull();
            select.SubQueryStatement.OrderByStatement.Elements.Should().ContainSingle();
            select.SubQueryStatement.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.Active), false);
        }


        #endregion order by on subquery
    }
}
