using System.Linq.Expressions;
using System.Reflection;
using ArgoStore.Helpers;

namespace ArgoStore.Configurations;

internal abstract class EntityConfiguration
{
    protected readonly List<LambdaExpression> PrimaryKeyExpressions = new();
    protected readonly List<LambdaExpression> UniqueIndexesExpressions = new();
    protected readonly List<LambdaExpression> NonUniqueIndexesExpressions = new();
    protected abstract Type EntityType { get; }
    
    internal EntityMetadata CreateMetadata()
    {
        string pkProperty = GetPrimaryKey();
        List<EntityIndexMetadata> indexes = GetIndexes().ToList();
        return new EntityMetadata(EntityType, pkProperty, indexes);
    }

    private string GetPrimaryKey()
    {
        if (PrimaryKeyExpressions.Count == 0)
        {
            return EntityMetadata.GetKeyProperty(EntityType).Name;
        }

        if (PrimaryKeyExpressions.Count == 1)
        {
            return GetPrimaryKeyFromExpression();
        }

        throw new InvalidOperationException($"Primary key set more than once for entity `{EntityType.FullName}`");
    }

    private string GetPrimaryKeyFromExpression()
    {
        LambdaExpression ex = PrimaryKeyExpressions.Single();

        if (ex.Body is MemberExpression ma)
        {
            if (ma.Member is PropertyInfo pi)
            {
                if (!pi.HasPublicGetAndSet())
                {
                    throw new InvalidOperationException(
                        $"Property `{pi.Name}` on `{EntityType.FullName}` " +
                        "cannot be used as primary key. Property must have public getter and setter.");
                }

                return pi.Name;
            }
        }

        throw new InvalidOperationException(
            $"Expected primary key selector for `{EntityType.FullName}` to be property selector lambda expression. " +
            "Composite primary keys and anonymous objects are not supported. Expression selecting fields are not supported."
        );
    }

    private IEnumerable<EntityIndexMetadata> GetIndexes()
    {
        foreach (LambdaExpression lambdaExpression in UniqueIndexesExpressions)
        {
            List<string> props = GetIndexProperties(lambdaExpression).ToList();

            yield return new EntityIndexMetadata(true, props, EntityType);
        }

        foreach (LambdaExpression lambdaExpression in NonUniqueIndexesExpressions)
        {
            List<string> props = GetIndexProperties(lambdaExpression).ToList();

            yield return new EntityIndexMetadata(false, props, EntityType);
        }
    }

    private IEnumerable<string> GetIndexProperties(LambdaExpression expression)
    {
        Expression body = expression.Body;

        if (body is MemberExpression me)
        {
            yield return GetIndexMemberName(me.Member);
        }
        else if (body is NewExpression ne)
        {
            if (ne.Arguments.Count == 0)
            {
                throw new InvalidOperationException("Cannot create index from empty anonymous object");
            }

            foreach (Expression arg in ne.Arguments)
            {
                if (arg is MemberExpression argMember)
                {
                    yield return GetIndexMemberName(argMember.Member);
                }
                else
                {
                    throw new InvalidOperationException($"Cannot create index from anonymous object composed of: {arg.GetType().Name}");
                }
            }
        }
        else
        {
            throw new InvalidOperationException($"Cannot create index from expression of type `{expression.Body.GetType().Name}`");
        }
    }

    private string GetIndexMemberName(MemberInfo mi)
    {
        if (mi is PropertyInfo pi)
        {
            if (pi.HasPublicGetAndSet())
            {
                return pi.Name;
            }

            throw new InvalidOperationException(
                $"Property `{pi.Name}` on `{EntityType.FullName}` cannot be used for index, it doesn't have public getter and setter."
                );
        }

        throw new InvalidOperationException(
            $"On entity `{EntityType.FullName}` cannot use `{mi.Name}` for index, expected property selector."
        );
    }
}

internal class EntityConfiguration<TEntity> : EntityConfiguration, IEntityConfiguration<TEntity> where TEntity : class, new()
{
    public IEntityConfiguration<TEntity> PrimaryKey<TProperty>(Expression<Func<TEntity, TProperty>> selector)
    {
        if (selector == null) throw new ArgumentNullException(nameof(selector));

        PrimaryKeyExpressions.Add(selector);

        return this;
    }

    public IEntityConfiguration<TEntity> UniqueIndex<TProperty>(Expression<Func<TEntity, TProperty>> selector)
    {
        if (selector == null) throw new ArgumentNullException(nameof(selector));

        UniqueIndexesExpressions.Add(selector);

        return this;
    }

    public IEntityConfiguration<TEntity> NonUniqueIndex<TProperty>(Expression<Func<TEntity, TProperty>> selector)
    {
        if (selector == null) throw new ArgumentNullException(nameof(selector));

        NonUniqueIndexesExpressions.Add(selector);

        return this;
    }

    protected override Type EntityType => typeof(TEntity);
}