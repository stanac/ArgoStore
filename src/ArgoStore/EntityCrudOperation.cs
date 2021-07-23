using System;
using System.Linq.Expressions;

namespace ArgoStore
{
    internal class EntityCrudOperation
    {
        public object Entity { get; }
        public EntityMetadata EntityMeta { get; }
        public string Id { get; }
        public CrudOperations CrudOperation { get; }
        public Expression DeleteWhere { get; }
        
        public EntityCrudOperation(Expression deleteWhere)
        {
            DeleteWhere = deleteWhere ?? throw new ArgumentNullException(nameof(deleteWhere));
            CrudOperation = CrudOperations.DeleteWhere;
        }

        public EntityCrudOperation(object entity, CrudOperations crudOperation, EntityMetadata entityMeta, string id)
        {
            Entity = entity ?? throw new ArgumentNullException(nameof(entity));
            CrudOperation = crudOperation;
            EntityMeta = entityMeta;
            Id = id;
        }
    }
}
