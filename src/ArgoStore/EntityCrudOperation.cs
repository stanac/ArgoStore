﻿using System;
using System.Linq.Expressions;

namespace ArgoStore
{
    internal class EntityCrudOperation
    {
        public object Entity { get; }
        public EntityMetadata EntityMeta { get; }
        public string StringId { get; }
        public CrudOperations CrudOperation { get; }
        public Expression DeleteWhere { get; }
        
        public EntityCrudOperation(Expression deleteWhere)
        {
            DeleteWhere = deleteWhere ?? throw new ArgumentNullException(nameof(deleteWhere));
            CrudOperation = CrudOperations.DeleteWhere;
        }

        public EntityCrudOperation(object entity, CrudOperations crudOperation, EntityMetadata entityMeta, string stringId)
        {
            Entity = entity ?? throw new ArgumentNullException(nameof(entity));
            CrudOperation = crudOperation;
            EntityMeta = entityMeta;
            StringId = stringId;
        }
    }
}
