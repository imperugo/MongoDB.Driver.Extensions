using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver.Extensions.Abstractions;
using MongoDB.Driver.Extensions.Configurations;
using MongoDB.Driver.Extensions.Implementations;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDbRepository(this IServiceCollection services, MongoDbDatabaseConfiguration configuration)
        {
            bool configurationAlreadyRegisterd = services.Any(x => x.ServiceType == typeof(MongoDbDatabaseConfiguration));

            if (!configurationAlreadyRegisterd)
            {
                services.AddSingleton(configuration);
            }
                
            services.AddSingleton<IAuditRepository, AuditRepository>();

            return services;
        }

        public static IServiceCollection AddMongoDbRepository(this IServiceCollection services, Action<MongoDbDatabaseConfiguration> opt)
        {
            if (opt == null)
            {
                throw new ArgumentNullException();
            }

            var conf = new MongoDbDatabaseConfiguration();
            opt.Invoke(conf);

            services.AddMongoDbRepository(conf);

            return services;
        }
    }
}