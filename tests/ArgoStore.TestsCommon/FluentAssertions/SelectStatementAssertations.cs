using ArgoStore;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using System.Linq;

namespace FluentAssertions
{
    internal static class SelectStatementAssertationsExtensions
    {
        public static SelectStatementAssertations Should(this SelectStatement statement)
        {
            return new SelectStatementAssertations(statement);
        }
    }

    internal class SelectStatementAssertations : ReferenceTypeAssertions<SelectStatement, SelectStatementAssertations>
    {
        public SelectStatementAssertations(SelectStatement statement)
            : base(statement)
        {
        }

        protected override string Identifier => typeof(SelectStatementAssertations).FullName;

        [CustomAssertion]
        public AndConstraint<SelectStatementAssertations> ContainElement(string property, string because = "", params string[] becauseArgs)
        {
            return ContainElement(inputProperty: property, outputProperty: property, because: because, becauseArgs: becauseArgs);
        }

        [CustomAssertion]
        public AndConstraint<SelectStatementAssertations> ContainElement(string inputProperty, string outputProperty, string because = "", params string[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(Subject.SelectElements.Any(x => x.InputProperty == inputProperty && x.OutputProperty == outputProperty))
                .BecauseOf(because, becauseArgs)
                .FailWith($"Expected {{context:element}} to be found with inputProperty \"{inputProperty}\" and outputProperty \"{outputProperty}\"");

            return new AndConstraint<SelectStatementAssertations>(this);
        }

    }
}
