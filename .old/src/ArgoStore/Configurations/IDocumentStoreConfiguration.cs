namespace ArgoStore.Configurations;

public interface IDocumentStoreConfiguration
{
    void ConnectionString(string connectionString);
    void CreateNotConfiguredEntities(bool createNonConfiguredEntities);
    IEntityConfiguration<TEntity> Entity<TEntity>() where TEntity : class, new();
}