using FluentAssertions;
using JsonDbLite.Expressions;
using JsonDbLite.WhereTranslators;
using System;
using System.Linq.Expressions;
using Xunit;

namespace JsonDbLite.UnitTests.WhereTranslators
{
    public class MethodStringInstanceTranslatorTests
    {
        [Fact]
        public void Transalte_StringTrim_TranslatesToExpectedExpression()
        {
            Expression<Func<TestEntityPerson, bool>> ex = x => x.Name.Trim() == "Marcus";

            var where = WhereTranslatorStrategy.Translate(ex);

            var methodCall = (where as WhereBinaryComparisonExpressionData)?.Left as WhereMethodCallExpressionData;

            methodCall.Should().NotBeNull();
            methodCall.Arguments[0].Should().BeOfType(typeof(WherePropertyExpressionData));
            (methodCall.Arguments[0] as WherePropertyExpressionData).Name.Should().Be(nameof(TestEntityPerson.Name));

            methodCall.MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringTrim);
        }

        [Fact]
        public void Transalte_StringToUpper_TranslatesToExpectedExpression()
        {
            Expression<Action> ex = () => "".ToUpper();

            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            (where as WhereMethodCallExpressionData).MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringToUpper);
        }

        [Fact]
        public void Transalte_StringToLower_TranslatesToExpectedExpression()
        {
            Expression<Action> ex = () => "".ToLower();

            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            (where as WhereMethodCallExpressionData).MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringToLower);
        }

        [Fact]
        public void Translate_StringTrim_TransaltedToExpectedExpression()
        {
            Expression<Action> ex = () => "".Trim();
            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            var m = where as WhereMethodCallExpressionData;

            m.MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringTrim);
            m.Arguments.Length.Should().Be(1);
        }

        [Fact]
        public void Translate_StringTrimWithParams_TransaltedToExpectedExpression()
        {
            Expression<Action> ex = () => "".Trim('a', 'b', 'c');
            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            var m = where as WhereMethodCallExpressionData;

            m.MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringTrim);
            m.Arguments.Length.Should().Be(2);
            m.Arguments[1].As<WhereConstantExpressionData>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Count.Should().Be(3);
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("a");
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("b");
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("c");
        }

        [Fact]
        public void Translate_StringTrimWithStringToCharArray_TransaltedToExpectedExpression()
        {
            Expression<Action> ex = () => "".Trim("abc".ToCharArray());
            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            var m = where as WhereMethodCallExpressionData;

            m.MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringTrim);
            m.Arguments.Length.Should().Be(2);
            m.Arguments[1].As<WhereConstantExpressionData>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Count.Should().Be(3);
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("a");
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("b");
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("c");
        }

        [Fact]
        public void Translate_StringTrimWithStringVariableToCharArray_TransaltedToExpectedExpression()
        {
            string variable = "abc";
            Expression<Action> ex = () => "".Trim(variable.ToCharArray());
            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            var m = where as WhereMethodCallExpressionData;

            m.MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringTrim);
            m.Arguments.Length.Should().Be(2);
            m.Arguments[1].As<WhereConstantExpressionData>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Count.Should().Be(3);
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("a");
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("b");
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("c");
        }

        [Fact]
        public void Translate_StringTrimWithVariableToCharArray_TransaltedToExpectedExpression()
        {
            char[] variable = "abc".ToCharArray();
            Expression<Action> ex = () => "".Trim(variable);
            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            var m = where as WhereMethodCallExpressionData;

            m.MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringTrim);
            m.Arguments.Length.Should().Be(2);
            m.Arguments[1].As<WhereConstantExpressionData>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Count.Should().Be(3);
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("a");
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("b");
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("c");
        }

        [Fact]
        public void Translate_StringTrimStart_TransaltedToExpectedExpression()
        {
            Expression<Action> ex = () => "".TrimStart();
            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            var m = where as WhereMethodCallExpressionData;

            m.MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringTrimStart);
            m.Arguments.Length.Should().Be(1);
        }

        [Fact]
        public void Translate_StringTrimStartWithParams_TransaltedToExpectedExpression()
        {
            Expression<Action> ex = () => "".TrimStart('a', 'b', 'c');
            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            var m = where as WhereMethodCallExpressionData;

            m.MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringTrimStart);
            m.Arguments.Length.Should().Be(2);
            m.Arguments[1].As<WhereConstantExpressionData>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Count.Should().Be(3);
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("a");
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("b");
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("c");
        }

        [Fact]
        public void Translate_StringTrimStartWithStringToCharArray_TransaltedToExpectedExpression()
        {
            Expression<Action> ex = () => "".TrimStart("abc".ToCharArray());
            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            var m = where as WhereMethodCallExpressionData;

            m.MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringTrimStart);
            m.Arguments.Length.Should().Be(2);
            m.Arguments[1].As<WhereConstantExpressionData>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Count.Should().Be(3);
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("a");
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("b");
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("c");
        }

        [Fact]
        public void Translate_StringTrimStartWithStringVariableToCharArray_TransaltedToExpectedExpression()
        {
            string variable = "abc";
            Expression<Action> ex = () => "".TrimStart(variable.ToCharArray());
            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            var m = where as WhereMethodCallExpressionData;

            m.MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringTrimStart);
            m.Arguments.Length.Should().Be(2);
            m.Arguments[1].As<WhereConstantExpressionData>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Count.Should().Be(3);
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("a");
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("b");
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("c");
        }

        [Fact]
        public void Translate_StringTrimStartWithVariableToCharArray_TransaltedToExpectedExpression()
        {
            char[] variable = "abc".ToCharArray();
            Expression<Action> ex = () => "".TrimStart(variable);
            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            var m = where as WhereMethodCallExpressionData;

            m.MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringTrimStart);
            m.Arguments.Length.Should().Be(2);
            m.Arguments[1].As<WhereConstantExpressionData>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Count.Should().Be(3);
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("a");
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("b");
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("c");
        }

        [Fact]
        public void Translate_StringTrimEnd_TransaltedToExpectedExpression()
        {
            Expression<Action> ex = () => "".TrimEnd();
            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            var m = where as WhereMethodCallExpressionData;

            m.MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringTrimEnd);
            m.Arguments.Length.Should().Be(1);
        }

        [Fact]
        public void Translate_StringTrimEndWithParams_TransaltedToExpectedExpression()
        {
            Expression<Action> ex = () => "".TrimEnd('a', 'b', 'c');
            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            var m = where as WhereMethodCallExpressionData;

            m.MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringTrimEnd);
            m.Arguments.Length.Should().Be(2);
            m.Arguments[1].As<WhereConstantExpressionData>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Count.Should().Be(3);
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("a");
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("b");
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("c");
        }

        [Fact]
        public void Translate_StringTrimEndWithStringToCharArray_TransaltedToExpectedExpression()
        {
            Expression<Action> ex = () => "".TrimEnd("abc".ToCharArray());
            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            var m = where as WhereMethodCallExpressionData;

            m.MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringTrimEnd);
            m.Arguments.Length.Should().Be(2);
            m.Arguments[1].As<WhereConstantExpressionData>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Count.Should().Be(3);
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("a");
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("b");
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("c");
        }

        [Fact]
        public void Translate_StringTrimEndWithStringVariableToCharArray_TransaltedToExpectedExpression()
        {
            string variable = "abc";
            Expression<Action> ex = () => "".TrimEnd(variable.ToCharArray());
            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            var m = where as WhereMethodCallExpressionData;

            m.MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringTrimEnd);
            m.Arguments.Length.Should().Be(2);
            m.Arguments[1].As<WhereConstantExpressionData>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Count.Should().Be(3);
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("a");
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("b");
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("c");
        }

        [Fact]
        public void Translate_StringTrimEndWithVariableToCharArray_TransaltedToExpectedExpression()
        {
            char[] variable = "abc".ToCharArray();
            Expression<Action> ex = () => "".TrimEnd(variable);
            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            var m = where as WhereMethodCallExpressionData;

            m.MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringTrimEnd);
            m.Arguments.Length.Should().Be(2);
            m.Arguments[1].As<WhereConstantExpressionData>().IsCollection.Should().BeTrue();
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Count.Should().Be(3);
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("a");
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("b");
            m.Arguments[1].As<WhereConstantExpressionData>().Values.Should().Contain("c");
        }

        [Fact]
        public void Translate_Contains_TranslatesToExpectedExpression()
        {
            Expression<Action> ex = () => "12345".Contains("123");

            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            (where as WhereMethodCallExpressionData).MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringContains);
        }

        [Fact]
        public void Translate_ContainsIgnoreCase_TranslatesToExpectedExpression()
        {
            Expression<Action> ex = () => "12345".Contains("123", StringComparison.OrdinalIgnoreCase);

            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            (where as WhereMethodCallExpressionData).MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringContainsIgnoreCase);
        }

        [Fact]
        public void Translate_StartsWith_TranslatesToExpectedExpression()
        {
            Expression<Action> ex = () => "12345".StartsWith("123");

            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            (where as WhereMethodCallExpressionData).MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringStartsWith);
        }

        [Fact]
        public void Translate_StartsWithIgnoreCase_TranslatesToExpectedExpression()
        {
            Expression<Action> ex = () => "12345".StartsWith("123", StringComparison.OrdinalIgnoreCase);

            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            (where as WhereMethodCallExpressionData).MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringStartsWithIgnoreCase);
        }

        [Fact]
        public void Translate_EndsWith_TranslatesToExpectedExpression()
        {
            Expression<Action> ex = () => "12345".EndsWith("123");

            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            (where as WhereMethodCallExpressionData).MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringEndsWith);
        }

        [Fact]
        public void Translate_EndsWithIgnoreCase_TranslatesToExpectedExpression()
        {
            Expression<Action> ex = () => "12345".EndsWith("123", StringComparison.OrdinalIgnoreCase);

            var where = WhereTranslatorStrategy.Translate(ex);

            where.Should().BeOfType(typeof(WhereMethodCallExpressionData));
            (where as WhereMethodCallExpressionData).MethodName.Should().Be(WhereMethodCallExpressionData.SupportedMethodNames.StringEndsWithIgnoreCase);
        }
    }
}
