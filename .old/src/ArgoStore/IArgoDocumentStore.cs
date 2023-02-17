using System.Diagnostics.CodeAnalysis;
// ReSharper disable RedundantNullableFlowAttribute

namespace ArgoStore;

/// <summary>
/// Entry point to ArgoStore
/// </summary>
public interface IArgoDocumentStore
{
    /// <summary>
    /// Opens CRUD session
    /// </summary>
    /// <returns>Instance of <see cref="IArgoDocumentSession"/></returns>
    IArgoDocumentSession OpenSession();

    /// <summary>
    /// Opens CRUD session for specified tenant
    /// </summary>
    /// <param name="tenantId">Tenant id</param>
    /// <returns>Instance of <see cref="IArgoDocumentSession"/></returns>
    IArgoDocumentSession OpenSession([DisallowNull] string tenantId);

    /// <summary>
    /// Opens query (read only) session
    /// </summary>
    /// <returns>Instance of <see cref="IArgoQueryDocumentSession"/></returns>
    IArgoQueryDocumentSession OpenQuerySession();


    /// <summary>
    /// Opens query (read only) session
    /// </summary>
    /// <param name="tenantId">Tenant id</param>
    /// <returns>Instance of <see cref="IArgoQueryDocumentSession"/></returns>
    IArgoQueryDocumentSession OpenQuerySession([DisallowNull] string tenantId);

    /// <summary>
    /// Registers document type for CRUD operations
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    void RegisterDocument<T>() where T : class, new();
    
    /// <summary>
    /// Registers document type for CRUD operations
    /// </summary>
    /// <param name="configure">Configuration action for document type</param>
    /// <typeparam name="T">Document type</typeparam>
    void RegisterDocument<T>([DisallowNull] Action<IDocumentConfiguration<T>> configure) where T : class, new();

    /// <summary>
    /// Returns distinct list of tenants used for provided document type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>Distinct list of tenants</returns>
    IReadOnlyList<string> ListTenants<T>();
}