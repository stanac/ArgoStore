namespace ArgoStore;

public interface IArgoStoreConfiguration
{
    void ConnectionString(string connectionString);
    IDocumentConfiguration<TDocument> RegisterDocument<TDocument>() where TDocument : class, new();
    IDocumentConfiguration<TDocument> RegisterDocument<TDocument>(Action<IDocumentConfiguration<TDocument>> configure) where TDocument : class, new();
}