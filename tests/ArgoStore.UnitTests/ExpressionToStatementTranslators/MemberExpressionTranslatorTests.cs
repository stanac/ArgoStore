namespace ArgoStore.UnitTests.ExpressionToStatementTranslators;

public class MemberExpressionTranslatorTests
{
    [Fact]
    public void Translate_BoolProperty_TranslatesToExpectedExpression()
    {
        Expression<Func<TestEntityPerson, bool>> ex = x => x.Active;

        var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

        where.Should().BeOfType(typeof(PropertyAccessStatement));

        var p = where as PropertyAccessStatement;
        p.Name.Should().Be("Active");
        p.IsBoolean.Should().BeTrue();
    }

}