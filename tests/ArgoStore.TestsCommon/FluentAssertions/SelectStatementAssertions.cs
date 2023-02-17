//using FluentAssertions.Execution;
//using FluentAssertions.Primitives;
//using System.Linq;
//using ArgoStore.Statements;

//namespace FluentAssertions
//{
//    internal static class SelectStatementAssertionsExtensions
//    {
//        public static SelectStatementAssertions Should(this SelectStatement statement)
//        {
//            return new SelectStatementAssertions(statement);
//        }
//    }

//    internal class SelectStatementAssertions : ReferenceTypeAssertions<SelectStatement, SelectStatementAssertions>
//    {
//        public SelectStatementAssertions(SelectStatement statement)
//            : base(statement)
//        {
//        }

//        protected override string Identifier => typeof(SelectStatementAssertions).FullName;

//        [CustomAssertion]
//        public AndConstraint<SelectStatementAssertions> ContainElement(string property, string because = "", params string[] becauseArgs)
//        {
//            return ContainElement(inputProperty: property, outputProperty: property, because: because, becauseArgs: becauseArgs);
//        }

//        [CustomAssertion]
//        public AndConstraint<SelectStatementAssertions> ContainElement(string inputProperty, string outputProperty, string because = "", params string[] becauseArgs)
//        {
//            Execute.Assertion
//                .ForCondition(Subject.SelectElements.Any(x => x.InputProperty == inputProperty && x.OutputProperty == outputProperty))
//                .BecauseOf(because, becauseArgs)
//                .FailWith($"Expected {{context:element}} to be found with inputProperty \"{inputProperty}\" and outputProperty \"{outputProperty}\"");

//            return new AndConstraint<SelectStatementAssertions>(this);
//        }

//    }
//}
