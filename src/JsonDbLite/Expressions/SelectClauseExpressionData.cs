using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonDbLite.Expressions
{
    internal class SelectClauseExpressionData
    {
        private SelectClauseExpressionData() { }

        public static SelectClauseExpressionData CreateWithStar() =>
            new SelectClauseExpressionData
            {
                IsStar = true
            };

        public static SelectClauseExpressionData CreateWithProperties() =>
            new SelectClauseExpressionData
            {
                IsStar = true
            };

        public static SelectClauseExpressionData CreateWithPropertyNames(IEnumerable<string> properties)
        {
            if (properties is null) throw new ArgumentNullException(nameof(properties));

            SelectClauseExpressionData ret = new SelectClauseExpressionData
            {
                Properties = properties.AsEnumerable().ToList()
            };

            ret.EnsurePropertiesAreValid();

            return ret;
        }

        public bool IsStar { get; private set; }
        public List<string> Properties { get; private set;  }

        private void EnsurePropertiesAreValid()
        {
            if (Properties.Any(x => string.IsNullOrWhiteSpace(x)))
            {
                throw new ArgumentException("properties cannot contain null or empty values", "properties");
            }
        }
    }
}
