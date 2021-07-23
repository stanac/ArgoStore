using System.Data;
using Microsoft.Data.Sqlite;

namespace ArgoStore.EntityCrudOperationConverters
{
    internal interface IEntityCrudOperationConverter
    {
        bool CanConvert(EntityCrudOperation op);
        SqliteCommand ConvertToCommand(EntityCrudOperation op, SqliteConnection connection, IArgoStoreSerializer serializer);
    }
}
