using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using MongoDB.Driver.Extensions.Abstractions;
using MongoDB.Driver.Extensions.Configurations;
using MongoDB.Driver.Extensions.Implementations;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// A set of extensions method that help you to use MondoDB with dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register all needed services to the dependency injection
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="configuration">The MongoDb configuration.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddMongoDb(this IServiceCollection services, MongoDbDatabaseConfiguration configuration)
    {
        var configurationAlreadyRegisterd = services.Any(x => x.ServiceType == typeof(MongoDbDatabaseConfiguration));

        if (configurationAlreadyRegisterd)
        {
            var descriptor =
                new ServiceDescriptor(
                    configuration.GetType(),
                    configuration);

            services.Replace(descriptor);
        }

        IMongoClient client = new MongoClient(configuration.ConnectionString);
        services.AddSingleton<IAuditRepository, AuditRepository>();
        services.AddSingleton(client);
        services.AddSingleton(configuration);

        return services;
    }

    /// <summary>
    /// Register all needed services to the dependency injection
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="opt">The MongoDb configuration.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddMongoDb(this IServiceCollection services, Action<MongoDbDatabaseConfiguration> opt)
    {
        var configuration = new MongoDbDatabaseConfiguration(string.Empty);

        opt.Invoke(configuration);

        services.AddMongoDb(configuration);

        var descriptor =
            new ServiceDescriptor(
                configuration.GetType(),
                configuration);

        services.Replace(descriptor);

        return services;
    }
}
