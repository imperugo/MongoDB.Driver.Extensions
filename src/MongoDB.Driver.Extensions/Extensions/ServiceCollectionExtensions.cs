using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using MongoDB.Driver.Extensions.Abstractions;
using MongoDB.Driver.Extensions.Configurations;
using MongoDB.Driver.Extensions.Implementations;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDbRepository(this IServiceCollection services, MongoDbDatabaseConfiguration configuration)
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

        public static IServiceCollection AddMongoDbRepository(this IServiceCollection services, Action<MongoDbDatabaseConfiguration> opt = null)
        {
            var configuration = services
                .BuildServiceProvider()
                .GetRequiredService<MongoDbDatabaseConfiguration>();

            opt?.Invoke(configuration);

            services.AddMongoDbRepository(configuration);

            var descriptor =
                new ServiceDescriptor(
                    configuration.GetType(),
                    configuration);

            services.Replace(descriptor);

            return services;
        }
    }
}