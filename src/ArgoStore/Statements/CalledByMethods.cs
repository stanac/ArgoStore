using System.Linq;

namespace ArgoStore.Statements
{
    internal enum CalledByMethods
    {
        Select, First, FirstOrDefault, Last, LastOrDefault, Single, SingleOrDefault, Count, Any
    }

    internal static class CalledByMethodsExtensions
    {
        private static readonly CalledByMethods[] _returningOnlyOneMethods = new CalledByMethods[]
        {
            CalledByMethods.First,
            CalledByMethods.FirstOrDefault,
            CalledByMethods.Last,
            CalledByMethods.LastOrDefault,
            CalledByMethods.Single,
            CalledByMethods.SingleOrDefault
        };

        public static bool ItSelectsOnlyOne(this CalledByMethods m) => _returningOnlyOneMethods.Contains(m);
    }
}
