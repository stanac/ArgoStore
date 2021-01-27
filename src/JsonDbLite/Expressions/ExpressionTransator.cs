using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonDbLite.Expressions
{
    internal static class ExpressionTransator
    {
        public static string Translate(ExpressionData data)
        {
            if (data is null) throw new ArgumentNullException(nameof(data));

            return "not implemented";
        }
    }
}
