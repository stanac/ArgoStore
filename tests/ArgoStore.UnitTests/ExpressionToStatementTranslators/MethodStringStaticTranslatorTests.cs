namespace ArgoStore.UnitTests.ExpressionToStatementTranslators;

public class MethodStringStaticTranslatorTests
{
    [Fact]
    public void Translate_StringIsNullOrEmpty_TranslatesToExpectedExpression()
    {
        Expression<Action> ex = () => string.IsNullOrEmpty("");

        Statement where = ExpressionToStatementTranslatorStrategy.Translate(ex);

        where.Should().BeOfType(typeof(MethodCallStatement));

        (where as MethodCallStatement).MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringIsNullOrEmpty);
    }

    [Fact]
    public void Translate_StringIsNullOrWhiteSpace_TranslatesToExpectedExpression()
    {
        Expression<Action> ex = () => string.IsNullOrWhiteSpace("");

        Statement where = ExpressionToStatementTranslatorStrategy.Translate(ex);

        where.Should().BeOfType(typeof(MethodCallStatement));

        (where as MethodCallStatement).MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringIsNullOrWhiteSpace);
    }

    [Fact]
    public void Translate_StringComparisonNoTypeSet_TranslatesToExpectedExpression()
    {
        Expression<Action> ex = () => string.Equals("Marcus", "MARCUS");

        var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

        where.Should().BeOfType(typeof(MethodCallStatement));

        (where as MethodCallStatement).MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringEquals);
    }

    [Theory]
    [InlineData(StringComparison.OrdinalIgnoreCase, true)]
    [InlineData(StringComparison.Ordinal, false)]
    [InlineData(StringComparison.CurrentCultureIgnoreCase, true)]
    [InlineData(StringComparison.CurrentCulture, false)]
    [InlineData(StringComparison.InvariantCultureIgnoreCase, true)]
    [InlineData(StringComparison.InvariantCulture, false)]
    public void Translate_StringComparison_TranslatesToExpectedExpression(StringComparison c, bool shouldIgnoreCase)
    {
        Expression<Action> ex = () => string.Equals("Marcus", "MARCUS", c);

        var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

        where.Should().BeOfType(typeof(MethodCallStatement));

        MethodCallStatement.SupportedMethodNames expected = shouldIgnoreCase
            ? MethodCallStatement.SupportedMethodNames.StringEqualsIgnoreCase
            : MethodCallStatement.SupportedMethodNames.StringEquals;

        (where as MethodCallStatement).MethodName.Should().Be(expected);
    }
}