using System;
using System.Linq.Expressions;

namespace ArgoStore
{
    public class EntityCrudOperation
    {
        public object Entity { get; }
        public CrudOperations CrudOperation { get; }
        public Expression DeleteWhere { get; }
        
        public EntityCrudOperation(Expression deleteWhere)
        {
            DeleteWhere = deleteWhere ?? throw new ArgumentNullException(nameof(deleteWhere));
        }

        public EntityCrudOperation(object entity, CrudOperations crudOperation)
        {
            Entity = entity ?? throw new ArgumentNullException(nameof(entity));
            CrudOperation = crudOperation;
        }
    }
}
