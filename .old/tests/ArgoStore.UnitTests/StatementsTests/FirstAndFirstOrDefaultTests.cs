namespace ArgoStore.UnitTests.StatementsTests;

public class FirstAndFirstOrDefaultTests
{
    [Fact]
    public void FirstOrDefaultOnQueryable_CreatesCorrectStatementAndSetsTop1()
    {
        Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.FirstOrDefault();

        Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

        st.Should().BeOfType(typeof(SelectStatement));

        SelectStatement s = st as SelectStatement;

        s.SelectElements.Should().ContainSingle();
        s.SelectElements[0].SelectsJson.Should().BeTrue();

        s.Top.Should().HaveValue();
        s.Top.Value.Should().Be(1);

        s.WhereStatement.Should().BeNull();
    }

    [Fact]
    public void FirstOrDefaultOnQueryableWithFilter_SetsWhere()
    {
        Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.FirstOrDefault(x => x.Key == "a");

        Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

        st.Should().BeOfType(typeof(SelectStatement));

        SelectStatement s = st as SelectStatement;

        s.SelectElements.Should().ContainSingle();
        s.SelectElements[0].SelectsJson.Should().BeTrue();

        s.Top.Should().HaveValue();
        s.Top.Value.Should().Be(1);

        s.WhereStatement.Should().NotBeNull();
        s.WhereStatement.Statement.Should().BeOfType<BinaryComparisonStatement>();
        s.WhereStatement.Statement.As<BinaryComparisonStatement>().Left.Should().BeOfType<PropertyAccessStatement>();
    }

    [Fact]
    public void FirstOrDefaultOnWhereWithFilter_SetsWhereStatementsInConjuction()
    {
        Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Where(x => x.Key == "b").FirstOrDefault(x => x.Key == "a");

        Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

        st.Should().BeOfType(typeof(SelectStatement));

        SelectStatement s = st as SelectStatement;

        s.SelectElements.Should().ContainSingle();
        s.SelectElements[0].SelectsJson.Should().BeTrue();

        s.Top.Should().HaveValue();
        s.Top.Value.Should().Be(1);

        s.WhereStatement.Should().NotBeNull();
        s.WhereStatement.Statement.Should().BeOfType<BinaryLogicalStatement>();
        s.WhereStatement.Statement.As<BinaryLogicalStatement>().IsAnd.Should().BeTrue();
    }

    [Fact]
    public void FirstOnQueryable_CreatesCorrectStatementAndSetsTop1()
    {
        Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.First();

        Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

        st.Should().BeOfType(typeof(SelectStatement));

        SelectStatement s = st as SelectStatement;

        s.SelectElements.Should().ContainSingle();
        s.SelectElements[0].SelectsJson.Should().BeTrue();

        s.Top.Should().HaveValue();
        s.Top.Value.Should().Be(1);

        s.WhereStatement.Should().BeNull();
        s.CalledByMethod.Should().Be(CalledByMethods.First);
    }

    [Fact]
    public void FirstOnQueryableWithFilter_SetsWhere()
    {
        Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.First(x => x.Key == "a");

        Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

        st.Should().BeOfType(typeof(SelectStatement));

        SelectStatement s = st as SelectStatement;

        s.SelectElements.Should().ContainSingle();
        s.SelectElements[0].SelectsJson.Should().BeTrue();

        s.Top.Should().HaveValue();
        s.Top.Value.Should().Be(1);

        s.WhereStatement.Should().NotBeNull();
        s.WhereStatement.Statement.Should().BeOfType<BinaryComparisonStatement>();
        s.WhereStatement.Statement.As<BinaryComparisonStatement>().Left.Should().BeOfType<PropertyAccessStatement>();
    }

    [Fact]
    public void FirstOnWhereWithFilter_SetsWhereStatementsInConjuction()
    {
        Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Where(x => x.Key == "b").First(x => x.Key == "a");

        Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

        st.Should().BeOfType(typeof(SelectStatement));

        SelectStatement s = st as SelectStatement;

        s.SelectElements.Should().ContainSingle();
        s.SelectElements[0].SelectsJson.Should().BeTrue();

        s.Top.Should().HaveValue();
        s.Top.Value.Should().Be(1);

        s.WhereStatement.Should().NotBeNull();
        s.WhereStatement.Statement.Should().BeOfType<BinaryLogicalStatement>();
        s.WhereStatement.Statement.As<BinaryLogicalStatement>().IsAnd.Should().BeTrue();
    }
}