using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ArgoStore.ExpressionToStatementTranslators
{
    public class SelectOnOrderByExpressionToStatementTranslator : IExpressionToStatementTranslator
    {
        public bool CanTranslate(Expression expression)
        {
            if (expression is MethodCallExpression m)
            {
                return m.Method.Name == "Select"
            }

            return false;
        }

        public Statement Translate(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}
