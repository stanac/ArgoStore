namespace ArgoStore.UnitTests.ExpressionToStatementTranslators
{
    public class MethodStringInstanceTranslatorTests
    {
        [Fact]
        public void Translate_StringTrim_TranslatesToExpectedExpression()
        {
            Expression<Func<TestEntityPerson, bool>> ex = x => x.Name.Trim() == "Marcus";

            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            var methodCall = (where as BinaryComparisonStatement)?.Left as MethodCallStatement;

            methodCall.Should().NotBeNull();
            methodCall.Arguments[0].Should().BeOfType(typeof(PropertyAccessStatement));
            (methodCall.Arguments[0] as PropertyAccessStatement).Name.Should().Be(nameof(TestEntityPerson.Name));

            methodCall.MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringTrim);
        }

        [Fact]
        public void Translate_StringToUpper_TranslatesToExpectedExpression()
        {
            Expression<Action> ex = () => "".ToUpper();

            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            (where as MethodCallStatement).MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringToUpper);
        }

        [Fact]
        public void Translate_StringToLower_TranslatesToExpectedExpression()
        {
            Expression<Action> ex = () => "".ToLower();

            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            (where as MethodCallStatement).MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringToLower);
        }

        [Fact]
        public void Translate_StringTrim_TranslatedToExpectedExpression()
        {
            Expression<Action> ex = () => "".Trim();
            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            var m = where as MethodCallStatement;

            m.MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringTrim);
            m.Arguments.Length.Should().Be(1);
        }

        [Fact]
        public void Translate_StringTrimWithParams_TranslatedToExpectedExpression()
        {
            Expression<Action> ex = () => "".Trim('a', 'b', 'c');
            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            var m = where as MethodCallStatement;

            m.MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringTrim);
            m.Arguments.Length.Should().Be(2);
            m.Arguments[1].As<ConstantStatement>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<ConstantStatement>().Values.Count.Should().Be(3);
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("a");
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("b");
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("c");
        }

        [Fact]
        public void Translate_StringTrimWithStringToCharArray_TranslatedToExpectedExpression()
        {
            Expression<Action> ex = () => "".Trim("abc".ToCharArray());
            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            var m = where as MethodCallStatement;

            m.MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringTrim);
            m.Arguments.Length.Should().Be(2);
            m.Arguments[1].As<ConstantStatement>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<ConstantStatement>().Values.Count.Should().Be(3);
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("a");
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("b");
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("c");
        }

        [Fact]
        public void Translate_StringTrimWithStringVariableToCharArray_TranslatedToExpectedExpression()
        {
            string variable = "abc";
            Expression<Action> ex = () => "".Trim(variable.ToCharArray());
            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            var m = where as MethodCallStatement;

            m.MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringTrim);
            m.Arguments.Length.Should().Be(2);
            m.Arguments[1].As<ConstantStatement>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<ConstantStatement>().Values.Count.Should().Be(3);
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("a");
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("b");
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("c");
        }

        [Fact]
        public void Translate_StringTrimWithVariableToCharArray_TranslatedToExpectedExpression()
        {
            char[] variable = "abc".ToCharArray();
            Expression<Action> ex = () => "".Trim(variable);
            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            var m = where as MethodCallStatement;

            m.MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringTrim);
            m.Arguments.Length.Should().Be(2);
            m.Arguments[1].As<ConstantStatement>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<ConstantStatement>().Values.Count.Should().Be(3);
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("a");
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("b");
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("c");
        }

        [Fact]
        public void Translate_StringTrimStart_TranslatedToExpectedExpression()
        {
            Expression<Action> ex = () => "".TrimStart();
            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            var m = where as MethodCallStatement;

            m.MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringTrimStart);
            m.Arguments.Length.Should().Be(1);
        }

        [Fact]
        public void Translate_StringTrimStartWithParams_TranslatedToExpectedExpression()
        {
            Expression<Action> ex = () => "".TrimStart('a', 'b', 'c');
            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            var m = where as MethodCallStatement;

            m.MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringTrimStart);
            m.Arguments.Length.Should().Be(2);
            m.Arguments[1].As<ConstantStatement>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<ConstantStatement>().Values.Count.Should().Be(3);
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("a");
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("b");
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("c");
        }

        [Fact]
        public void Translate_StringTrimStartWithStringToCharArray_TranslatedToExpectedExpression()
        {
            Expression<Action> ex = () => "".TrimStart("abc".ToCharArray());
            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            var m = where as MethodCallStatement;

            m.MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringTrimStart);
            m.Arguments.Length.Should().Be(2);
            m.Arguments[1].As<ConstantStatement>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<ConstantStatement>().Values.Count.Should().Be(3);
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("a");
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("b");
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("c");
        }

        [Fact]
        public void Translate_StringTrimStartWithStringVariableToCharArray_TranslatedToExpectedExpression()
        {
            string variable = "abc";
            Expression<Action> ex = () => "".TrimStart(variable.ToCharArray());
            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            var m = where as MethodCallStatement;

            m.MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringTrimStart);
            m.Arguments.Length.Should().Be(2);
            m.Arguments[1].As<ConstantStatement>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<ConstantStatement>().Values.Count.Should().Be(3);
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("a");
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("b");
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("c");
        }

        [Fact]
        public void Translate_StringTrimStartWithVariableToCharArray_TranslatedToExpectedExpression()
        {
            char[] variable = "abc".ToCharArray();
            Expression<Action> ex = () => "".TrimStart(variable);
            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            var m = where as MethodCallStatement;

            m.MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringTrimStart);
            m.Arguments.Length.Should().Be(2);
            m.Arguments[1].As<ConstantStatement>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<ConstantStatement>().Values.Count.Should().Be(3);
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("a");
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("b");
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("c");
        }

        [Fact]
        public void Translate_StringTrimEnd_TranslatedToExpectedExpression()
        {
            Expression<Action> ex = () => "".TrimEnd();
            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            var m = where as MethodCallStatement;

            m.MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringTrimEnd);
            m.Arguments.Length.Should().Be(1);
        }

        [Fact]
        public void Translate_StringTrimEndWithParams_TranslatedToExpectedExpression()
        {
            Expression<Action> ex = () => "".TrimEnd('a', 'b', 'c');
            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            var m = where as MethodCallStatement;

            m.MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringTrimEnd);
            m.Arguments.Length.Should().Be(2);
            m.Arguments[1].As<ConstantStatement>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<ConstantStatement>().Values.Count.Should().Be(3);
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("a");
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("b");
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("c");
        }

        [Fact]
        public void Translate_StringTrimEndWithStringToCharArray_TranslatedToExpectedExpression()
        {
            Expression<Action> ex = () => "".TrimEnd("abc".ToCharArray());
            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            var m = where as MethodCallStatement;

            m.MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringTrimEnd);
            m.Arguments.Length.Should().Be(2);
            m.Arguments[1].As<ConstantStatement>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<ConstantStatement>().Values.Count.Should().Be(3);
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("a");
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("b");
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("c");
        }

        [Fact]
        public void Translate_StringTrimEndWithStringVariableToCharArray_TranslatedToExpectedExpression()
        {
            string variable = "abc";
            Expression<Action> ex = () => "".TrimEnd(variable.ToCharArray());
            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            var m = where as MethodCallStatement;

            m.MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringTrimEnd);
            m.Arguments.Length.Should().Be(2);
            m.Arguments[1].As<ConstantStatement>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<ConstantStatement>().Values.Count.Should().Be(3);
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("a");
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("b");
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("c");
        }

        [Fact]
        public void Translate_StringTrimEndWithVariableToCharArray_TranslatedToExpectedExpression()
        {
            char[] variable = "abc".ToCharArray();
            Expression<Action> ex = () => "".TrimEnd(variable);
            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            var m = where as MethodCallStatement;

            m.MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringTrimEnd);
            m.Arguments.Length.Should().Be(2);
            m.Arguments[1].As<ConstantStatement>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<ConstantStatement>().Values.Count.Should().Be(3);
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("a");
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("b");
            m.Arguments[1].As<ConstantStatement>().Values.Should().Contain("c");
        }

        [Fact]
        public void Translate_Contains_TranslatesToExpectedExpression()
        {
            Expression<Action> ex = () => "12345".Contains("123");

            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            (where as MethodCallStatement).MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringContains);
        }

        [Fact]
        public void Translate_ContainsIgnoreCase_TranslatesToExpectedExpression()
        {
            Expression<Action> ex = () => "12345".Contains("123", StringComparison.OrdinalIgnoreCase);

            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            (where as MethodCallStatement).MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringContainsIgnoreCase);
        }

        [Fact]
        public void Translate_StartsWith_TranslatesToExpectedExpression()
        {
            Expression<Action> ex = () => "12345".StartsWith("123");

            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            (where as MethodCallStatement).MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringStartsWith);
        }

        [Fact]
        public void Translate_StartsWithIgnoreCase_TranslatesToExpectedExpression()
        {
            Expression<Action> ex = () => "12345".StartsWith("123", StringComparison.OrdinalIgnoreCase);

            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            (where as MethodCallStatement).MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringStartsWithIgnoreCase);
        }

        [Fact]
        public void Translate_EndsWith_TranslatesToExpectedExpression()
        {
            Expression<Action> ex = () => "12345".EndsWith("123");

            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            (where as MethodCallStatement).MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringEndsWith);
        }

        [Fact]
        public void Translate_EndsWithIgnoreCase_TranslatesToExpectedExpression()
        {
            Expression<Action> ex = () => "12345".EndsWith("123", StringComparison.OrdinalIgnoreCase);

            var where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            (where as MethodCallStatement).MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.StringEndsWithIgnoreCase);
        }
    }
}
