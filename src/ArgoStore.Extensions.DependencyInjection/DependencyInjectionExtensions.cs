using ArgoStore;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddArgoStore(this IServiceCollection services, string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

        return services.AddArgoStore(c =>
        {
            c.ConnectionString(connectionString);
        });
    }

    public static IServiceCollection AddArgoStore(this IServiceCollection services, Action<IArgoStoreConfiguration> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        ArgoDocumentStore store = new ArgoDocumentStore(configure);

        services.AddSingleton(store);
        services.AddSingleton<IArgoDocumentStore>(s => s.GetRequiredService<ArgoDocumentStore>());
        services.AddScoped(_ => store.OpenSession());
        services.AddTransient(_ => store.OpenQuerySession());

        return services;
    }
}