using ArgoStore.Caching;
using ArgoStore.TestsCommon.Entities.Person;

namespace ArgoStore.UnitTests.Caching;

public class ParameterExtractingVisitorTests
{
    [Fact]
    public void ExtractConstant_ExtractsAsParameter()
    {
        ExpressionCachingVisitor v = new();

        Expression<Func<Person, bool>> e = p => p.PrimaryContact.ContactType == 23;

        Expression ex = v.Visit(e);

        v.Params.Should().HaveCount(1);
        v.Params.First().Value.Should().Be(23);

        
    }
}