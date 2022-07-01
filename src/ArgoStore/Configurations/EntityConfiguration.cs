using System.Linq.Expressions;
using System.Reflection;
using ArgoStore.Helpers;

namespace ArgoStore.Configurations;

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
        List<EntityIndexMetadata> indexes = GetIndexes().ToList();
        return new EntityMetadata(typeof(TEntity), pkProperty, indexes);
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

        if (ex.Body is MemberExpression ma)
        {
            if (ma.Member is PropertyInfo pi)
            {
                if (!pi.HasPublicGetAndSet())
                {
                    throw new InvalidOperationException(
                        $"Property `{pi.Name}` on `{typeof(TEntity).FullName}` " +
                        "cannot be used as primary key. Property must have public getter and setter.");
                }

                return pi.Name;
            }
        }

        throw new InvalidOperationException(
            $"Expected primary key selector for `{typeof(TEntity).FullName}` to be property selector lambda expression. " +
            "Composite primary keys and anonymous objects are not supported. Expression selecting fields are not supported."
        );
    }

    private IEnumerable<EntityIndexMetadata> GetIndexes()
    {
        foreach (LambdaExpression lambdaExpression in _uniqueIndexes)
        {
            List<string> props = GetIndexProperties(lambdaExpression).ToList();

            yield return new EntityIndexMetadata(true, props, typeof(TEntity));
        }

        foreach (LambdaExpression lambdaExpression in _nonUniqueIndexes)
        {
            List<string> props = GetIndexProperties(lambdaExpression).ToList();

            yield return new EntityIndexMetadata(false, props, typeof(TEntity));
        }
    }

    private static IEnumerable<string> GetIndexProperties(LambdaExpression expression)
    {
        Expression body = expression.Body;

        if (body is MemberExpression me)
        {
            if (me.Member is PropertyInfo pi)
            {
                if (!pi.HasPublicGetAndSet())
                {
                    yield return pi.Name;
                }
            }
            else
            {
                throw new InvalidOperationException(
                    $"Cannot use fields for index selector expression. On entity `{typeof(TEntity).FullName}`"
                );
            }
        }
    }
}