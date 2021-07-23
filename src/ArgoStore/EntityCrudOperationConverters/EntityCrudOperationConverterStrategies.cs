using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ArgoStore.EntityCrudOperationConverters
{
    public static class EntityCrudOperationConverterStrategies
    {
        private static readonly IReadOnlyList<IEntityCrudOperationConverter> _converters =
            typeof(IEntityCrudOperationConverter)
                .Assembly
                .GetTypes()
                .Where(x => x.IsClass && !x.IsAbstract && typeof(IEntityCrudOperationConverter).IsAssignableFrom(x))
                .Select(Activator.CreateInstance)
                .Cast<IEntityCrudOperationConverter>()
                .ToList();

        public static IDbCommand Convert(EntityCrudOperation op, IDbConnection connection)
        {
            if (op == null) throw new ArgumentNullException(nameof(op));

            IEntityCrudOperationConverter converter = _converters.FirstOrDefault(x => x.CanConvert(op));

            if (converter == null)
            {
                throw new NotSupportedException($"Cannot find converter for CRUD operation {op.CrudOperation}");
            }

            return converter.ConvertToCommand(op, connection);
        }
    }
}
