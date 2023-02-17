using Microsoft.Data.Sqlite;

namespace ArgoStore.EntityCrudOperationConverters;

internal static class EntityCrudOperationConverterStrategies
{
    private static readonly IReadOnlyList<IEntityCrudOperationConverter> _converters =
        typeof(IEntityCrudOperationConverter)
            .Assembly
            .GetTypes()
            .Where(x => x.IsClass && !x.IsAbstract && typeof(IEntityCrudOperationConverter).IsAssignableFrom(x))
            .Select(Activator.CreateInstance)
            .Cast<IEntityCrudOperationConverter>()
            .ToList();

    public static SqliteCommand Convert(EntityCrudOperation op, SqliteConnection connection, IArgoStoreSerializer serializer, string tenantId)
    {
        if (op == null) throw new ArgumentNullException(nameof(op));
        if (string.IsNullOrWhiteSpace(tenantId)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(tenantId));

        IEntityCrudOperationConverter converter = _converters.FirstOrDefault(x => x.CanConvert(op));

        if (converter == null)
        {
            throw new NotSupportedException($"Cannot find converter for CRUD operation {op.CrudOperation}");
        }

        return converter.ConvertToCommand(op, connection, serializer, tenantId);
    }
}