namespace ArgoStore.UnitTests.StatementsTests;

public class AnyTests
{
    [Fact]
    public void AnyOnQueryable_CreatesCorrectStatementAndSetsMethod()
    {
        Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Any();

        Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

        st.Should().BeOfType(typeof(SelectExistsStatement));
        st.As<SelectExistsStatement>().FromType.Should().Be(typeof(TestEntityPerson));
    }

    [Fact]
    public void AnyOnQueryableWithCondition_CreatesCorrectStatementAndSetsMethod()
    {
        Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Any(x => x.Active);

        Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

        st.Should().BeOfType(typeof(SelectExistsStatement));
        st.As<SelectExistsStatement>().FromType.Should().Be(typeof(TestEntityPerson));
        st.As<SelectExistsStatement>().Where.Should().NotBeNull();
        st.As<SelectExistsStatement>().Where.Statement.Should().BeStatement<PropertyAccessStatement>();
    }
}