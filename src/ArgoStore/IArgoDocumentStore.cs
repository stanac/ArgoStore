using System.Diagnostics.CodeAnalysis;
// ReSharper disable RedundantNullableFlowAttribute

namespace ArgoStore;

public interface IArgoDocumentStore
{
    IArgoDocumentSession OpenSession();
    IArgoDocumentSession OpenSession([DisallowNull] string tenantId);
    IArgoQueryDocumentSession OpenQuerySession();
    IArgoQueryDocumentSession OpenQuerySession([DisallowNull] string tenantId);
    void RegisterDocument<T>() where T : class, new();
    void RegisterDocument<T>([DisallowNull] Action<IDocumentConfiguration<T>> configure) where T : class, new();
}