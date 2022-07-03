namespace ArgoStore.UnitTests.ExpressionToStatementTranslators;

public class NullableParametersTranslatorTests
{
    [Fact]
    public void NullStringParameterCheck_Translate_GivesExpectedStatement()
    {
        string paramName = null;
        Expression<Func<TestEntityPerson, bool>> ex = x => paramName == null;

        Statement where = ExpressionToStatementTranslatorStrategy.Translate(ex);

    }

    [Fact]
    public void NullableStringParameterCheckWithOr_Translate_GivesExpectedStatement()
    {
        string paramName = null;

        Expression<Func<TestEntityPerson, bool>> ex = x => paramName == null || x.Name == paramName;
        
        Statement where = ExpressionToStatementTranslatorStrategy.Translate(ex);

    }
}