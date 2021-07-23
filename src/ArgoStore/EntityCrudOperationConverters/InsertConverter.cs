using System.Data;

namespace ArgoStore.EntityCrudOperationConverters
{
    public class InsertConverter : IEntityCrudOperationConverter
    {
        public bool CanConvert(EntityCrudOperation op) => op != null && op.CrudOperation == CrudOperations.Insert;

        public IDbCommand ConvertToCommand(EntityCrudOperation op, IDbConnection connection)
        {
            var c = connection.CreateCommand();
            c.CommandText = "SELECT 1 WHERE 1 = 1";
            return c;
        }
    }
}
