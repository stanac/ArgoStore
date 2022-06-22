namespace ArgoStore.Statements
{
    internal abstract class Statement
    {
        public abstract string ToDebugString();

        public override string ToString() => ToDebugString();

        public abstract Statement Negate();

        public abstract Statement ReduceIfPossible();

        public string StatementTypeName => GetType().Name;
    }
}
