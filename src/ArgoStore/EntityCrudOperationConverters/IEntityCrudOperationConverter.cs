using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace ArgoStore.EntityCrudOperationConverters
{
    internal interface IEntityCrudOperationConverter
    {
        bool CanConvert(EntityCrudOperation op);
        IDbCommand ConvertToCommand(EntityCrudOperation op, IDbConnection connection);
    }
}
