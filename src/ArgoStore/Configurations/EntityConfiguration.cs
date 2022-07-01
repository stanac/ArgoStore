using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ArgoStore.Configurations
{
    internal class EntityConfiguration<TEntity> : IEntityConfiguration<TEntity> where TEntity : class, new()
    {
        private readonly List<LambdaExpression> _primaryKeys = new();
        private readonly List<LambdaExpression> _uniqueIndexes = new();
        private readonly List<LambdaExpression> _nonUniqueIndexes = new();

        public IEntityConfiguration<TEntity> PrimaryKey<TProperty>(Expression<Func<TEntity, TProperty>> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            _primaryKeys.Add(selector);

            return this;
        }

        public IEntityConfiguration<TEntity> UniqueIndex<TProperty>(Expression<Func<TEntity, TProperty>> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            _uniqueIndexes.Add(selector);

            return this;
        }

        public IEntityConfiguration<TEntity> NonUniqueIndex<TProperty>(Expression<Func<TEntity, TProperty>> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            _nonUniqueIndexes.Add(selector);

            return this;
        }

        internal EntityMetadata CreateMetadata()
        {
            string pkProperty = GetPrimaryKey();
            throw new NotImplementedException();
        }

        private string GetPrimaryKey()
        {
            if (_primaryKeys.Count == 0)
            {
                return EntityMetadata.GetKeyProperty(typeof(TEntity)).Name;
            }

            if (_primaryKeys.Count == 1)
            {
                return GetPrimaryKeyFromExpression();
            }

            throw new InvalidOperationException($"Primary key set more than once for entity `{typeof(TEntity).FullName}`");
        }

        private string GetPrimaryKeyFromExpression()
        {
            LambdaExpression ex = _primaryKeys.Single();

            throw new NotImplementedException();
        }
        
    }
}
