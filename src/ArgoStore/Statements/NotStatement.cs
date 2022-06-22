using System;

namespace ArgoStore.Statements
{
    internal class NotStatement : Statement
    {
        public Statement InnerStatement { get; }

        public NotStatement(Statement innerStatement)
        {
            InnerStatement = innerStatement ?? throw new ArgumentNullException(nameof(innerStatement));
        }

        public override Statement Negate()
        {
            return InnerStatement;
        }

        public override string ToDebugString() => $"NOT ({InnerStatement})";

        public override Statement ReduceIfPossible() => InnerStatement.Negate();
    }
}