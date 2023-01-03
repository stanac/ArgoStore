//using FluentAssertions.Execution;
//using FluentAssertions.Primitives;
//using System;
//using ArgoStore.Statements;

//namespace FluentAssertions
//{
//    internal static class StatementAssertionsExtensions
//    {
//        public static StatementAssertions Should(this Statement statement)
//        {
//            return new StatementAssertions(statement);
//        }
//    }

//    internal class StatementAssertions : ReferenceTypeAssertions<Statement, StatementAssertions>
//    {
//        public StatementAssertions(Statement statement)
//            : base(statement)
//        {
//        }

//        protected override string Identifier => typeof(Statement).FullName;

//        [CustomAssertion]
//        public AndConstraint<StatementAssertions> BeStatement<T>(string because = "", params string[] becauseArgs)
//            where T: Statement
//        {
//            Execute.Assertion
//                .ForCondition(Subject is T)
//                .BecauseOf(because, becauseArgs)
//                .FailWith($"Expected {{context:element}} to be of type {typeof(T).Name}");

//            return new AndConstraint<StatementAssertions>(this);
//        }

//        [CustomAssertion]
//        public AndConstraint<StatementAssertions> BeStatement<T>(Func<T, bool> assert, string because = "", params string[] becauseArgs)
//            where T : Statement
//        {
//            Execute.Assertion
//                .ForCondition(Subject is T && assert((T)Subject))
//                .BecauseOf(because, becauseArgs)
//                .FailWith($"Expected {{context:element}} to be of type {typeof(T).Name} and to fulfill condition {assert}");

//            return new AndConstraint<StatementAssertions>(this);
//        }
//    }
//}
