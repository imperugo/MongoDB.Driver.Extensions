using Humanizer;
using MongoDB.Driver.Extensions.Abstractions;
using MongoDB.Driver.Extensions.Configurations;

namespace MongoDB.Driver.Extensions.Implementations
{
    public class DefaultMongoDbNamingHelper : IMongoDbNamingHelper
    {
        public string GetDatabaseName(MongoDbDatabaseConfiguration configuration, string dbName)
        {
            return string.Concat(dbName,configuration.EnvironmentSuffix);
        }

        public string GetCollectionName(string requiredCollection)
        {
            return requiredCollection?.Pluralize().ToLowerInvariant();
        }
    }
}