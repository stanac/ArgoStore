using System.Linq.Expressions;

namespace ArgoStore;

/// <summary>
/// Document session that supports all CRUD operations
/// </summary>
public interface IDocumentSession : IQueryDocumentSession
{
    /// <summary>
    /// Inserts entities, changes applied on <see cref="SaveChanges"/>
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="entities">Entities to insert</param>
    void Insert<T>(params T[] entities) where T: class, new();

    /// <summary>
    /// Updates entities, changes applied on <see cref="SaveChanges"/>
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="entities">Entities to update</param>
    void Update<T>(params T[] entities) where T : class, new();

    /// <summary>
    /// Deletes entities, changes applied on <see cref="SaveChanges"/>
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="entities">Entities to delete</param>
    void Delete<T>(params T[] entities) where T : class, new();

    /// <summary>
    /// Upserts entities (insert or update), changes applied on <see cref="SaveChanges"/>
    /// If entity exists it will be updated, otherwise inserted
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="entities">Entities to save</param>
    void InsertOrUpdate<T>(params T[] entities) where T : class, new();

    /// <summary>
    /// Deletes entities that match predicate, changes applied on <see cref="SaveChanges"/>
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="predicate">Predicate, condition for deleting</param>
    void DeleteWhere<T>(Expression<Func<T, bool>> predicate) where T : class, new();
        
    /// <summary>
    /// Executes all pending commands (insert, update, delete, upsert) but doesn't commit changes (transaction).
    /// All changes applied after <see cref="SaveChanges"/>.
    /// </summary>
    void Execute();

    /// <summary>
    /// Saves all changes in this session
    /// </summary>
    void SaveChanges();
        
    /// <summary>
    /// Discards all changes performed since last <see cref="SaveChanges"/> or since <see cref="IDocumentSession"/> is created.
    /// </summary>
    void DiscardChanges();
}