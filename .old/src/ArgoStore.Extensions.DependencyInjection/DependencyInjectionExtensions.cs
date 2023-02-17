using ArgoStore;
using ArgoStore.Configurations;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds <see cref="DocumentStore"/>, <see cref="IDocumentSession"/> and <see cref="IQueryDocumentSession"/> to DI container
    /// </summary>
    /// <param name="connectionString">Connection string, usually: "Data Source=/path/to/file.sqlite"</param>
    /// <returns>Same instance of IServiceCollection</returns>
    public static IServiceCollection AddArgoStore(this IServiceCollection services, string connectionString)
    { 
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
        
        return services.AddArgoStore(c =>
        {
            c.ConnectionString(connectionString);
            c.CreateNotConfiguredEntities(true);
        });
    }

    public static IServiceCollection AddArgoStore(this IServiceCollection services, Action<IDocumentStoreConfiguration> configAction)
    {
        if (configAction == null) throw new ArgumentNullException(nameof(configAction));

        DocumentStore store = new DocumentStore(configAction);

        services.AddSingleton(store);
        services.AddTransient(_ => store.CreateSession());
        services.AddTransient(_ => store.CreateReadOnlySession());

        return services;
    }
}