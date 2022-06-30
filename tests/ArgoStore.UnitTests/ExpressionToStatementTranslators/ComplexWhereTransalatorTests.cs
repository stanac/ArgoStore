namespace ArgoStore.UnitTests.ExpressionToStatementTranslators
{
    public class ComplexWhereTranslatorTests
    {

        [Fact]
        public void Translate_ExpressionWithBrackets_TranslatesToExpectedExpression()
        {
            Expression<Func<TestEntityPerson, bool>> ex = x => x.Active || (x.BirthYear > 1980 && x.BirthYear < 1990);

            Statement where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(BinaryLogicalStatement));

            var w = where as BinaryLogicalStatement;
            w.IsOr.Should().BeTrue();

            w.Left.Should().BeOfType(typeof(PropertyAccessStatement));

            w.Right.Should().BeOfType(typeof(BinaryLogicalStatement));
            (w.Right as BinaryLogicalStatement).IsAnd.Should().BeTrue();
        }

        [Fact]
        public void Translate_ExpressionWithStringArrayContains_TranslatesToExpectedExpression()
        {
            string[] allowedNames = new[] { "Marcus", "Kovalski" };

            Expression<Func<TestEntityPerson, bool>> ex = x => allowedNames.Contains(x.Name);

            Statement where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            var m = where as MethodCallStatement;
            m.MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.EnumerableContains);
            m.Arguments.Should().HaveCount(2);
            m.Arguments[0].Should().BeOfType(typeof(ConstantStatement));
            m.Arguments[0].As<ConstantStatement>().IsCollection.Should().BeTrue();
            m.Arguments[0].As<ConstantStatement>().Values.Should().NotBeNull();
            m.Arguments[0].As<ConstantStatement>().Values.Should().HaveCount(allowedNames.Length);
            m.Arguments[1].Should().BeOfType(typeof(PropertyAccessStatement));
            m.Arguments[1].As<PropertyAccessStatement>().Name.Should().Be(nameof(TestEntityPerson.Name));

            foreach (var n in allowedNames)
            {
                m.Arguments[0].As<ConstantStatement>().Values.Should().Contain(n);
            }
        }

        [Fact]
        public void Translate_ExpressionWithStringListContains_TranslatesToExpectedExpression()
        {
            IReadOnlyList<string> allowedNames = new List<string> { "Marcus", "Kovalski" };

            Expression<Func<TestEntityPerson, bool>> ex = x => allowedNames.Contains(x.Name);

            Statement where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            var m = where as MethodCallStatement;
            m.MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.EnumerableContains);
            m.Arguments.Should().HaveCount(2);
            m.Arguments[0].Should().BeOfType(typeof(ConstantStatement));
            m.Arguments[0].As<ConstantStatement>().IsCollection.Should().BeTrue();
            m.Arguments[0].As<ConstantStatement>().Values.Should().NotBeNull();
            m.Arguments[0].As<ConstantStatement>().Values.Should().HaveCount(allowedNames.Count);
            m.Arguments[1].Should().BeOfType(typeof(PropertyAccessStatement));
            m.Arguments[1].As<PropertyAccessStatement>().Name.Should().Be(nameof(TestEntityPerson.Name));

            foreach (var n in allowedNames)
            {
                m.Arguments[0].As<ConstantStatement>().Values.Should().Contain(n);
            }
        }

        [Fact]
        public void Translate_ExpressionWithIntListContains_TranslatesToExpectedExpression()
        {
            IReadOnlyList<int> years = new List<int> { 1, 2, 3, 4 };

            Expression<Func<TestEntityPerson, bool>> ex = x => years.Contains(x.BirthYear);

            Statement where = ExpressionToStatementTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(MethodCallStatement));
            var m = where as MethodCallStatement;
            m.MethodName.Should().Be(MethodCallStatement.SupportedMethodNames.EnumerableContains);
            m.Arguments.Should().HaveCount(2);
            m.Arguments[0].Should().BeOfType(typeof(ConstantStatement));
            m.Arguments[0].As<ConstantStatement>().IsCollection.Should().BeTrue();
            m.Arguments[0].As<ConstantStatement>().Values.Should().NotBeNull();
            m.Arguments[0].As<ConstantStatement>().Values.Should().HaveCount(years.Count);
            m.Arguments[1].Should().BeOfType(typeof(PropertyAccessStatement));
            m.Arguments[1].As<PropertyAccessStatement>().Name.Should().Be(nameof(TestEntityPerson.BirthYear));

            foreach (var n in years)
            {
                m.Arguments[0].As<ConstantStatement>().Values.Should().Contain(n.ToString());
            }
        }
    }
}
