using System;

namespace ArgoStore.Statements
{
    internal class WhereStatement : Statement
    {
        public WhereStatement(Statement statement, Type targetType)
        {
            Statement = statement ?? throw new ArgumentNullException(nameof(statement));
            TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
        }

        public string Alias { get; private set; }

        public Statement Statement { get; private set; }

        public Type TargetType { get; }

        public override Statement Negate()
        {
            throw new NotSupportedException();
        }

        public override Statement ReduceIfPossible() => new WhereStatement(Statement.ReduceIfPossible(), TargetType);

        public void AddConjunctedCondition(Statement statement)
        {
            if (statement is null) throw new ArgumentNullException(nameof(statement));

            Statement = new BinaryLogicalStatement(Statement, statement, false);
        }

        public override string ToDebugString()
        {
            return $"WHERE {Statement?.ToDebugString()}";
        }

        internal void SetAliases(int level)
        {
            Alias = $"\"..t{level}\"";
        }
    }
}