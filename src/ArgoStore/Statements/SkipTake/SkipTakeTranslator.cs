using System.Linq.Expressions;
using ArgoStore.Helpers;

namespace ArgoStore.Statements.SkipTake;

internal static class SkipTakeTranslator
{
    public static int GetSkipOrTakeValue(Expression e, bool isSkip)
    {
        if (e is ConstantExpression ce)
        {
            if (ce.Value is int i)
            {
                return i;
            }
            else if (ce.Value is long l)
            {
                return (int) l;
            }
        }

        string error = $"Expression: {e.Describe()} not supported for "
                       + (isSkip ? "Skip" : "Take")
                       + " operator";

        throw new NotSupportedException(error);
    }
}