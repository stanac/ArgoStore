using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ArgoStore;

/// <summary>
/// Document session with CRUD methods
/// </summary>
[SuppressMessage("ReSharper", "RedundantNullableFlowAttribute")]
public interface IArgoDocumentSession : IArgoQueryDocumentSession
{
    /// <summary>
    /// Adds documents to be inserted. Insert is actually executed when calling <see cref="SaveChanges"/>
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    /// <param name="documents">Document(s) to insert</param>
    void Insert<T>([DisallowNull] params T[] documents) where T : class, new();

    /// <summary>
    /// Adds documents to be updated. Update is actually executed when calling <see cref="SaveChanges"/>
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    /// <param name="documents">Document(s) to insert</param>
    void Update<T>([DisallowNull] params T[] documents) where T : class, new();
    
    /// <summary>
    /// Adds documents to be upserted (insert if not exists, otherwise update). Operation is actually executed when calling <see cref="SaveChanges"/>
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    /// <param name="documents">Document(s) to upsert</param>
    void Upsert<T>([DisallowNull] params T[] documents) where T : class, new();

    /// <summary>
    /// Adds documents to be deleted. Delete is actually executed when calling <see cref="SaveChanges"/>
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    /// <param name="documentId">Id(s) of document(s)</param>
    void DeleteById<T>([DisallowNull] params object[] documentId);

    /// <summary>
    /// Adds documents to be deleted. Delete is actually executed when calling <see cref="SaveChanges"/>
    /// </summary>
    /// <param name="document">Document(s) to delete</param>
    void Delete([DisallowNull] params object[] document);

    /// <summary>
    /// Conditionally deletes documents matching condition
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    /// <param name="predicate">Condition for deleting</param>
    void DeleteWhere<T>([DisallowNull] Expression<Func<T, bool>> predicate) where T : class, new();

    /// <summary>
    /// Save changes, executes all pending operations (Insert, Update, Updsert, DeleteById, Delete, DeleteWhere)
    /// </summary>
    void SaveChanges();

    /// <summary>
    /// Discards all pending operations (Insert, Update, Updsert, DeleteById, Delete, DeleteWhere)
    /// </summary>
    void DiscardChanges();
}