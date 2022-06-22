using System;

namespace ArgoStore.Statements
{
    internal abstract class BinaryStatement : Statement
    {
        protected BinaryStatement(Statement left, Statement right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public Statement Left { get; }
        public Statement Right { get; }

        public abstract string OperatorString { get; }
    }
}