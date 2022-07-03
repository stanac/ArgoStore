namespace ArgoStore.UnitTests.ExpressionToStatementTranslators;

public class NullableParametersTranslatorTests
{
    [Fact]
    public void NullStringParameterCheck_Translate_GivesExpectedStatement()
    {
        string paramName = null;
        Expression<Func<TestEntityPerson, bool>> ex = x => paramName == null;
        
        Statement s = ExpressionToStatementTranslatorStrategy.Translate(ex);
        s.Should().BeStatement<BinaryComparisonStatement>();
        BinaryComparisonStatement c = (BinaryComparisonStatement) s;

        c.Left.Should().BeStatement<ConstantStatement>(x => x.IsNull);
        c.Right.Should().BeStatement<ConstantStatement>(x => x.IsNull);
    }

    [Fact]
    public void NullIntParameterCheck_Translate_GivesExpectedStatement()
    {
        int? paramName = null;
        Expression<Func<TestEntityPerson, bool>> ex = x => !paramName.HasValue || x.BirthYear >= paramName.Value;

        Statement s = ExpressionToStatementTranslatorStrategy.Translate(ex);
        s.Should().BeStatement<BinaryComparisonStatement>();
        BinaryComparisonStatement c = (BinaryComparisonStatement)s;
        
    }

    [Fact]
    public void NullableStringParameterCheckWithOr_Translate_GivesExpectedStatement()
    {
        string paramName = null;

        Expression<Func<TestEntityPerson, bool>> ex = x => paramName == null || x.Name == paramName;
        
        Statement where = ExpressionToStatementTranslatorStrategy.Translate(ex);

        where.Should().BeStatement<BinaryLogicalStatement>(x => x.IsOr &&
            x.Left is BinaryComparisonStatement && x.Right is BinaryComparisonStatement);

        BinaryComparisonStatement leftNullStatement = (BinaryComparisonStatement)((BinaryLogicalStatement) where).Left;
        BinaryComparisonStatement rightParamStatement = (BinaryComparisonStatement)((BinaryLogicalStatement) where).Right;

        leftNullStatement.Left.Should().BeStatement<ConstantStatement>(x => x.IsNull);
        leftNullStatement.Right.Should().BeStatement<ConstantStatement>(x => x.IsNull);
        leftNullStatement.Operator.Should().Be(BinaryComparisonStatement.Operators.Equal);

        rightParamStatement.Left.Should().BeStatement<PropertyAccessStatement>();
        rightParamStatement.Right.Should().BeStatement<ConstantStatement>(x => x.IsNull);
        rightParamStatement.Operator.Should().Be(BinaryComparisonStatement.Operators.Equal);
    }
}