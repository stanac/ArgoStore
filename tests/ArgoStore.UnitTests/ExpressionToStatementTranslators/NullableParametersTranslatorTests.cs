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
        s.Should().BeStatement<BinaryLogicalStatement>();
        BinaryLogicalStatement c = (BinaryLogicalStatement)s;
        c.IsOr.Should().BeTrue();

        c.Left.Should().BeOfType<BinaryComparisonStatement>();
        c.Right.Should().BeOfType<BinaryComparisonStatement>();

        BinaryComparisonStatement left = (BinaryComparisonStatement)c.Left;

        // !paramName.HasValue is translated as !(hasValue == True) which is !(false == True)
        // since we work with integers in Sqlite it is translated as !(0 == 1)
        // !(0 == 1) will be reduced to 0 <> 1
        left.Operator.Should().Be(BinaryComparisonStatement.Operators.NotEqual);
        left.Left.Should().BeStatement<ConstantStatement>(x => x.Value == "0");
        left.Right.Should().BeStatement<ConstantStatement>(x => x.Value == "1");

        BinaryComparisonStatement right = (BinaryComparisonStatement)c.Right;
        // since there is not value x.BirthYear >= paramName
        // is translated as x.BirthYear >= NULL
        right.Left.Should().BeStatement<PropertyAccessStatement>(x => x.Name == "BirthYear");
        right.Right.Should().BeStatement<ConstantStatement>(x => x.IsNull);
        right.Operator.Should().Be(BinaryComparisonStatement.Operators.GreaterThanOrEqual);
    }

    [Fact]
    public void NullIntParameterCheckWithoutValueAccess_Translate_GivesExpectedStatement()
    {
        int? paramName = null;

        // we are not using paramName.Value that's the difference between this test and previous one
        Expression<Func<TestEntityPerson, bool>> ex = x => !paramName.HasValue || x.BirthYear >= paramName;

        Statement s = ExpressionToStatementTranslatorStrategy.Translate(ex);
        s.Should().BeStatement<BinaryLogicalStatement>();
        BinaryLogicalStatement c = (BinaryLogicalStatement)s;
        c.IsOr.Should().BeTrue();

        c.Left.Should().BeOfType<BinaryComparisonStatement>();
        c.Right.Should().BeOfType<BinaryComparisonStatement>();

        BinaryComparisonStatement left = (BinaryComparisonStatement)c.Left;

        // !paramName.HasValue is translated as !(hasValue == True) which is !(false == True)
        // since we work with integers in Sqlite it is translated as !(0 == 1)
        // !(0 == 1) will be reduced to 0 <> 1
        left.Operator.Should().Be(BinaryComparisonStatement.Operators.NotEqual);
        left.Left.Should().BeStatement<ConstantStatement>(x => x.Value == "0");
        left.Right.Should().BeStatement<ConstantStatement>(x => x.Value == "1");

        BinaryComparisonStatement right = (BinaryComparisonStatement)c.Right;
        // since there is not value x.BirthYear >= paramName
        // is translated as x.BirthYear >= NULL
        right.Left.Should().BeStatement<PropertyAccessStatement>(x => x.Name == "BirthYear");
        right.Right.Should().BeStatement<ConstantStatement>(x => x.IsNull);
        right.Operator.Should().Be(BinaryComparisonStatement.Operators.GreaterThanOrEqual);
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