using System.Data;

namespace ArgoStore.EntityCrudOperationConverters
{
    public class InsertConverter : IEntityCrudOperationConverter
    {
        public bool CanConvert(EntityCrudOperation op) => op != null && op.CrudOperation == CrudOperations.Insert;

        public IDbCommand ConvertToCommand(EntityCrudOperation op, IDbConnection connection)
        {
            throw new System.NotImplementedException();
        }
    }
}
