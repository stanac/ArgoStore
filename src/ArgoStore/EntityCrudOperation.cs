using System;
using System.Data;
using System.Linq.Expressions;
using ArgoStore.Configurations;

namespace ArgoStore
{
    internal class EntityCrudOperation
    {
        public object Entity { get; }
        public EntityMetadata EntityMeta { get; }
        public PrimaryKeyValue PkValue { get; }
        public CrudOperations CrudOperation { get; }
        public Expression DeleteWhere { get; }
        public IDbCommand Command { get; set; }
        
        public EntityCrudOperation(Expression deleteWhere)
        {
            DeleteWhere = deleteWhere ?? throw new ArgumentNullException(nameof(deleteWhere));
            CrudOperation = CrudOperations.DeleteWhere;
        }

        public EntityCrudOperation(object entity, CrudOperations crudOperation, EntityMetadata entityMeta, PrimaryKeyValue pkValue)
        {
            Entity = entity ?? throw new ArgumentNullException(nameof(entity));
            CrudOperation = crudOperation;
            EntityMeta = entityMeta;
            PkValue = pkValue ?? throw new ArgumentNullException(nameof(pkValue));
        }
    }
}
