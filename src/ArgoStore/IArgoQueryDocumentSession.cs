namespace ArgoStore;

/// <summary>
/// Query (read-only) session
/// </summary>
public interface IArgoQueryDocumentSession : IDisposable
{
    /// <summary>
    /// Query entry on which LINQ can be used
    /// </summary>
    /// <typeparam name="T">Type of document</typeparam>
    /// <returns>Queryable object</returns>
    IArgoStoreQueryable<T> Query<T>() where T : class, new();

    /// <summary>
    /// Gets document by id
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    /// <param name="id">Id of document, must match in type</param>
    /// <returns>Document object if found or null</returns>
    T GetById<T>(object id) where T : class, new();
}