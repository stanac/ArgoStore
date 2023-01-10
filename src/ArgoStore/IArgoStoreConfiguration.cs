using System.Linq.Expressions;

namespace ArgoStore;

public interface IArgoStoreConfiguration
{
    void ConnectionString(string connectionString);
    IDocumentConfiguration<TDocument> Document<TDocument>() where TDocument : class, new();
}