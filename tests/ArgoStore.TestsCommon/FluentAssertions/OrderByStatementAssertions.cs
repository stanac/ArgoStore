using ArgoStore;
using FluentAssertions.Execution;
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

    internal class OrderByStatementAssertions
    {
        private OrderByStatement _statement;

        public OrderByStatementAssertions(OrderByStatement statement)
        {
            _statement = statement;
        }

        public AndConstraint<OrderByStatementAssertions> ContainOrderByElement(string propertyName, bool ascending, string because = "", params string[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(_statement.Elements.Any(x => x.PropertyName == propertyName && x.Ascending == ascending))
                .BecauseOf(because, becauseArgs)
                .FailWith($"Expected {{context:element}} to be found with \"{propertyName}\" {(ascending ? "ASC" : "DESC")}");

            return new AndConstraint<OrderByStatementAssertions>(this);
        }

        public AndConstraint<OrderByStatementAssertions> ContainOrderByElement(string propertyName, bool ascending, int index, string because = "", params string[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(_statement.Elements.Count > index && _statement.Elements[index].PropertyName == propertyName && _statement.Elements[index].Ascending == ascending)
                .BecauseOf(because, becauseArgs)
                .FailWith($"Expected {{context:element}} to be found with \"{propertyName}\" {(ascending ? "ASC" : "DESC")}");

            return new AndConstraint<OrderByStatementAssertions>(this);
        }
    }
}
