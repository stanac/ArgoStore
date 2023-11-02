using System.Diagnostics;
using System.Text;
using ArgoStore.Caching;
using ArgoStore.Command;
using ArgoStore.TestsCommon.Entities.Person;
using ArgoStore.TestsCommon.TestHelpers;

namespace ArgoStore.UnitTests.Caching;

public class ExpressionCachingVisitorTests
{
    private readonly StringBuilder _sb = new();

    public ExpressionCachingVisitorTests()
    {
        Trace.AutoFlush = true;
        Trace.Listeners.Add(new TextWriterTraceListener(new StringWriter(_sb)));
    }

    [Fact]
    public void ExpressionWithConstantInt_ReplacesConstantWithParameter()
    {
        ExpressionCachingVisitor v = new();

        Expression<Func<Person, bool>> e = p => p.PrimaryContact.ContactType == 23;

        Expression transformed = v.Visit(e);

        Expression paramEx = FindExpressionVisitor.Find(transformed, e =>
        {
            if (e is ParameterExpression pe)
            {
                return pe.Name != null && pe.Name.Contains(ArgoCommandParameter.TransformPrefix);
            }

            return false;
        });

        paramEx.Should().NotBeNull();

        string paramName = (paramEx as ParameterExpression)!.Name;
        v.Params.Should().HaveCount(1);
        v.Params.First().Name.Should().Be(paramName);
    }

    [Fact]
    public void ExpressionWithDateTimeNow_ReplacesConstantWithParameter()
    {
        ExpressionCachingVisitor v = new();

        Expression<Func<Person, bool>> e = p => p.CakeDay == DateTime.Now;

        Expression transformed = v.Visit(e);

        Expression paramEx = FindExpressionVisitor.Find(transformed, e =>
        {
            if (e is ParameterExpression pe)
            {
                return pe.Name != null && pe.Name.Contains(ArgoCommandParameter.TransformPrefix);
            }

            return false;
        });

        paramEx.Should().NotBeNull();

        string paramName = (paramEx as ParameterExpression)!.Name;
        v.Params.Should().HaveCount(1);
        v.Params.First().Name.Should().Be(paramName);
    }
}