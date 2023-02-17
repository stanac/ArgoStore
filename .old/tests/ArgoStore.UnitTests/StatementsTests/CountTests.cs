namespace ArgoStore.UnitTests.StatementsTests;

public class CountTests
{
    [Fact]
    public void CountOnQueryable_CreatesCorrectStatementAndSetsMethod()
    {
        Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Count();

        Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

        st.Should().BeOfType(typeof(SelectCountStatement));
        st.As<SelectCountStatement>().FromType.Should().Be(typeof(TestEntityPerson));
    }

    [Fact]
    public void CountOnQueryableWithCondition_CreatesCorrectStatementAndSetsMethod()
    {
        Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Count(x => x.Active);

        Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

        st.Should().BeOfType(typeof(SelectCountStatement));
        st.As<SelectCountStatement>().FromType.Should().Be(typeof(TestEntityPerson));
        st.As<SelectCountStatement>().Where.Should().NotBeNull();
        st.As<SelectCountStatement>().Where.Statement.Should().BeStatement<PropertyAccessStatement>();
    }

    [Fact]
    public void LongCountOnQueryable_CreatesCorrectStatementAndSetsMethod()
    {
        Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.LongCount();

        Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

        st.Should().BeOfType(typeof(SelectCountStatement));
        st.As<SelectCountStatement>().FromType.Should().Be(typeof(TestEntityPerson));
        st.As<SelectCountStatement>().LongCount.Should().BeTrue();
    }

    [Fact]
    public void LongCountOnQueryableWithCondition_CreatesCorrectStatementAndSetsMethod()
    {
        Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.LongCount(x => x.Active);

        Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

        st.Should().BeOfType(typeof(SelectCountStatement));
        st.As<SelectCountStatement>().FromType.Should().Be(typeof(TestEntityPerson));
        st.As<SelectCountStatement>().LongCount.Should().BeTrue();
        st.As<SelectCountStatement>().Where.Should().NotBeNull();
        st.As<SelectCountStatement>().Where.Statement.Should().BeStatement<PropertyAccessStatement>();
    }
}