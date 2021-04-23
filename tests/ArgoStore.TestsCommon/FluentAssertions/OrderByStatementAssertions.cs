using ArgoStore;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using System.Linq;

namespace FluentAssertions
{
    internal static class OrderByStatementAssertionsExtensions
    {
        public static OrderByStatementAssertions Should(this OrderByStatement statement)
        {
            return new OrderByStatementAssertions(statement);
        }
    }

    internal class OrderByStatementAssertions : ReferenceTypeAssertions<OrderByStatement, OrderByStatementAssertions>
    {
        public OrderByStatementAssertions(OrderByStatement statement)
            : base (statement)
        {
        }

        protected override string Identifier => typeof(OrderByStatementAssertions).FullName;

        [CustomAssertion]
        public AndConstraint<OrderByStatementAssertions> ContainOrderByElement(string propertyName, bool ascending, string because = "", params string[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(Subject.Elements.Any(x => x.PropertyName == propertyName && x.Ascending == ascending))
                .BecauseOf(because, becauseArgs)
                .FailWith($"Expected {{context:element}} to be found with \"{propertyName}\" {(ascending ? "ASC" : "DESC")}");

            return new AndConstraint<OrderByStatementAssertions>(this);
        }

        [CustomAssertion]
        public AndConstraint<OrderByStatementAssertions> ContainOrderByElement(string propertyName, bool ascending, int index, string because = "", params string[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(Subject.Elements.Count > index && Subject.Elements[index].PropertyName == propertyName && Subject.Elements[index].Ascending == ascending)
                .BecauseOf(because, becauseArgs)
                .FailWith($"Expected {{context:element}} to be found with \"{propertyName}\" {(ascending ? "ASC" : "DESC")}");

            return new AndConstraint<OrderByStatementAssertions>(this);
        }
    }
}
