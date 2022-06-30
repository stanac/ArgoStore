using ArgoStore.TestsCommon.Entities;

namespace ArgoStore.UnitTests.ExpressionToStatementTranslators;

public class SelectExpressionTranslatorTests
{
    [Fact]
    public void Test()
    {
        Expression<Func<IQueryable<Person>, IQueryable<string>>> expression =
            q => q.Select(x => x.Name);

        Statement statement = ExpressionToStatementTranslatorStrategy.Translate(expression);

        statement.Should().NotBeNull();

        statement.Should().BeStatement<SelectStatement>();

        SelectStatement s = (SelectStatement)statement;

        s.TypeFrom.Should().Be<Person>();
        s.TypeTo.Should().Be<string>();
    }
}