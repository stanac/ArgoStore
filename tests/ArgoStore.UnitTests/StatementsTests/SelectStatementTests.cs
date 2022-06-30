using ArgoStore.Helpers;

namespace ArgoStore.UnitTests.StatementsTests;

public class SelectStatementTests
{
    [Fact]
    public void SelectAnonymousObject_Translate_ReturnsCorrectStatement()
    {
        Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Select(x => new { x.Active, x.BirthYear });

        Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

        st.Should().BeOfType(typeof(SelectStatement));

        SelectStatement s = st as SelectStatement;
        s.TypeFrom.Should().Be(typeof(TestEntityPerson));
        TypeHelpers.IsAnonymousType(s.TypeTo).Should().BeTrue();

        s.SelectElements.Should().HaveCount(2);

        s.SelectElements[0].InputProperty.Should().Be("Active");
        s.SelectElements[0].OutputProperty.Should().Be("Active");

        s.SelectElements[1].InputProperty.Should().Be("BirthYear");
        s.SelectElements[1].OutputProperty.Should().Be("BirthYear");
    }

    [Fact]
    public void SelectAnonymousObjectRenameProps_Translate_ReturnsCorrectStatement()
    {
        Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q.Select(x => new { Active1 = x.Active, BirthYear1 = x.BirthYear });

        Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

        st.Should().BeOfType(typeof(SelectStatement));

        SelectStatement s = st as SelectStatement;
        s.TypeFrom.Should().Be(typeof(TestEntityPerson));
        TypeHelpers.IsAnonymousType(s.TypeTo).Should().BeTrue();

        s.SelectElements.Should().HaveCount(2);

        s.SelectElements[0].InputProperty.Should().Be("Active");
        s.SelectElements[0].OutputProperty.Should().Be("Active1");

        s.SelectElements[1].InputProperty.Should().Be("BirthYear");
        s.SelectElements[1].OutputProperty.Should().Be("BirthYear1");
    }

    [Fact]
    public void SelectRenameProps_Translate_ReturnsCorrectStatement()
    {
        Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q =>
            q.Select(x => new TestEntityPerson { EmailAddress = x.Name, Key = x.EmailAddress });

        Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

        st.Should().BeOfType(typeof(SelectStatement));

        SelectStatement s = st as SelectStatement;
        s.TypeFrom.Should().Be(typeof(TestEntityPerson));
        TypeHelpers.IsAnonymousType(s.TypeTo).Should().BeFalse();

        s.SelectElements.Should().HaveCount(2);

        s.SelectElements[0].InputProperty.Should().Be("Name");
        s.SelectElements[0].OutputProperty.Should().Be("EmailAddress");

        s.SelectElements[1].InputProperty.Should().Be("EmailAddress");
        s.SelectElements[1].OutputProperty.Should().Be("Key");
    }

    [Fact]
    public void SelectWhereSelect_Translate_ReturnsCorrectStatement()
    {
        Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q =>
            q.Select(x => new { EmailAddress = x.Name, Key = x.BirthYear, x.Active })
                .Where(x => x.Active)
                .Select(x => new { Name = x.EmailAddress, BirthYear = x.Key });

        Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

        st.Should().BeOfType(typeof(SelectStatement));

        var s1 = st as SelectStatement;

        s1.WhereStatement.Should().BeNull();
        s1.SelectElements.Should().HaveCount(2);
        s1.SelectElements[0].InputProperty.Should().Be(nameof(TestEntityPerson.EmailAddress));
        s1.SelectElements[0].OutputProperty.Should().Be(nameof(TestEntityPerson.Name));
        s1.SelectElements[1].InputProperty.Should().Be(nameof(TestEntityPerson.Key));
        s1.SelectElements[1].OutputProperty.Should().Be(nameof(TestEntityPerson.BirthYear));
        s1.SubQueryStatement.Should().NotBeNull();

        var s2 = s1.SubQueryStatement;

        s2.WhereStatement.Should().NotBeNull();
        s2.WhereStatement.Statement.Should().BeStatement<PropertyAccessStatement>(x => x.IsBoolean && x.Name == nameof(TestEntityPerson.Active));

        s2.SubQueryStatement.Should().BeNull();

        s2.SelectElements.Should().HaveCount(3);
        s2.SelectElements[0].InputProperty.Should().Be(nameof(TestEntityPerson.Name));
        s2.SelectElements[0].OutputProperty.Should().Be(nameof(TestEntityPerson.EmailAddress));
        s2.SelectElements[1].InputProperty.Should().Be(nameof(TestEntityPerson.BirthYear));
        s2.SelectElements[1].OutputProperty.Should().Be(nameof(TestEntityPerson.Key));
        s2.SelectElements[2].InputProperty.Should().Be(nameof(TestEntityPerson.Active));
        s2.SelectElements[2].OutputProperty.Should().Be(nameof(TestEntityPerson.Active));
    }
        
    [Fact]
    public void SelectOnOrderBy_Translate_ReturnsCorrectStatement()
    {
        Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q
                .OrderBy(x => x.EmailAddress)
                .Select(x => new { EmailAddress = x.Name, Key = x.BirthYear, x.Active })
            ;
            
        Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

        st.Should().BeOfType(typeof(SelectStatement));
        SelectStatement s = st as SelectStatement;

        s.OrderByStatement.Should().NotBeNull();
        s.OrderByStatement.Elements.Should().ContainSingle();
        s.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), true);

        s.SelectElements.Should().HaveCount(3);
        s.Should().ContainElement(nameof(TestEntityPerson.Active));
        s.Should().ContainElement(inputProperty: nameof(TestEntityPerson.Name), outputProperty: nameof(TestEntityPerson.EmailAddress));
        s.Should().ContainElement(inputProperty: nameof(TestEntityPerson.BirthYear), outputProperty: nameof(TestEntityPerson.Key));
    }

    [Fact]
    public void SelectOnOrderByDesc_Translate_ReturnsCorrectStatement()
    {
        Expression<Func<IQueryable<TestEntityPerson>, object>> ex = q => q
                .OrderByDescending(x => x.EmailAddress)
                .Select(x => new { EmailAddress = x.Name, Key = x.BirthYear, x.Active })
            ;

        Statement st = ExpressionToStatementTranslatorStrategy.Translate(ex);

        st.Should().BeOfType(typeof(SelectStatement));
        SelectStatement s = st as SelectStatement;

        s.OrderByStatement.Should().NotBeNull();
        s.OrderByStatement.Elements.Should().ContainSingle();
        s.OrderByStatement.Should().ContainOrderByElement(nameof(TestEntityPerson.EmailAddress), false);

        s.SelectElements.Should().HaveCount(3);
        s.Should().ContainElement(nameof(TestEntityPerson.Active));
        s.Should().ContainElement(inputProperty: nameof(TestEntityPerson.Name), outputProperty: nameof(TestEntityPerson.EmailAddress));
        s.Should().ContainElement(inputProperty: nameof(TestEntityPerson.BirthYear), outputProperty: nameof(TestEntityPerson.Key));
    }
}