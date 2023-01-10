using System.Linq.Expressions;
using System.Reflection;
using ArgoStore.Helpers;

namespace ArgoStore.Config;

internal abstract class DocumentConfiguration
{
    protected readonly List<LambdaExpression> PrimaryKeyExpressions = new();
    protected readonly List<LambdaExpression> UniqueIndexesExpressions = new();
    protected readonly List<LambdaExpression> NonUniqueIndexesExpressions = new();
    protected abstract Type DocumentType { get; }
    protected abstract string DocumentName { get; }

    internal DocumentMetadata CreateMetadata()
    {
        string pkProperty = GetPrimaryKey();
        List<DocumentIndexMetadata> indexes = GetIndexes().ToList();
        return new DocumentMetadata(DocumentType, pkProperty, DocumentName, indexes);
    }

    private string GetPrimaryKey()
    {
        if (PrimaryKeyExpressions.Count == 0)
        {
            return DocumentMetadata.GetKeyProperty(DocumentType).Name;
        }

        if (PrimaryKeyExpressions.Count == 1)
        {
            return GetPrimaryKeyFromExpression();
        }

        throw new InvalidOperationException($"Primary key set more than once for Document `{DocumentType.FullName}`");
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
                        $"Property `{pi.Name}` on `{DocumentType.FullName}` " +
                        "cannot be used as primary key. Property must have public getter and setter.");
                }

                return pi.Name;
            }
        }

        throw new InvalidOperationException(
            $"Expected primary key selector for `{DocumentType.FullName}` to be property selector lambda expression. " +
            "Composite primary keys and anonymous objects are not supported. Expression selecting fields are not supported."
        );
    }

    private IEnumerable<DocumentIndexMetadata> GetIndexes()
    {
        foreach (LambdaExpression lambdaExpression in UniqueIndexesExpressions)
        {
            List<string> props = GetIndexProperties(lambdaExpression).ToList();

            yield return new DocumentIndexMetadata(true, props, DocumentType);
        }

        foreach (LambdaExpression lambdaExpression in NonUniqueIndexesExpressions)
        {
            List<string> props = GetIndexProperties(lambdaExpression).ToList();

            yield return new DocumentIndexMetadata(false, props, DocumentType);
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
                $"Property `{pi.Name}` on `{DocumentType.FullName}` cannot be used for index, it doesn't have public getter and setter."
            );
        }

        throw new InvalidOperationException(
            $"On Document `{DocumentType.FullName}` cannot use `{mi.Name}` for index, expected property selector."
        );
    }
}

internal class DocumentConfiguration<TDocument> : DocumentConfiguration, IDocumentConfiguration<TDocument> where TDocument : class, new()
{
    private string _documentName;
    protected override Type DocumentType => typeof(TDocument);
    protected override string DocumentName => _documentName;

    public DocumentConfiguration()
    {
        _documentName = typeof(TDocument).Name;
    }

    public IDocumentConfiguration<TDocument> PrimaryKey<TProperty>(Expression<Func<TDocument, TProperty>> selector)
    {
        if (selector == null) throw new ArgumentNullException(nameof(selector));

        PrimaryKeyExpressions.Add(selector);

        return this;
    }

    public IDocumentConfiguration<TDocument> UniqueIndex<TProperty>(Expression<Func<TDocument, TProperty>> selector)
    {
        if (selector == null) throw new ArgumentNullException(nameof(selector));

        UniqueIndexesExpressions.Add(selector);

        return this;
    }

    public IDocumentConfiguration<TDocument> NonUniqueIndex<TProperty>(Expression<Func<TDocument, TProperty>> selector)
    {
        if (selector == null) throw new ArgumentNullException(nameof(selector));

        NonUniqueIndexesExpressions.Add(selector);

        return this;
    }

    public IDocumentConfiguration<TDocument> TableName(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(tableName));

        _documentName = tableName;

        return this;
    }
}