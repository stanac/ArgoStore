using System;

namespace ArgoStore.Statements
{
    internal class SelectCountStatement : Statement
    {
        public SelectCountStatement(Type fromType, bool longCount)
        {
            FromType = fromType ?? throw new ArgumentNullException(nameof(fromType));
            LongCount = longCount;
        }

        public SelectCountStatement(Type fromType, WhereStatement where, bool longCount)
        {
            FromType = fromType ?? throw new ArgumentNullException(nameof(fromType));
            Where = where ?? throw new ArgumentNullException(nameof(where));
            LongCount = longCount;
        }

        public SelectCountStatement(Type fromType, SelectStatement subQuery, bool longCount)
        {
            FromType = fromType ?? throw new ArgumentNullException(nameof(fromType));
            SubQuery = subQuery ?? throw new ArgumentNullException(nameof(subQuery));
            LongCount = longCount;
        }

        public Type FromType { get; }
        public WhereStatement Where { get; }
        public SelectStatement SubQuery { get; }
        public bool LongCount { get; }

        public override Statement Negate() => throw new NotSupportedException();

        public override Statement ReduceIfPossible() => this;

        public override string ToDebugString() => "SELECT COUNT()";
    }
}