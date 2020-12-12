using MongoDB.Driver.Extensions.Abstractions;
using MongoDB.Driver.Extensions.Configurations;

namespace MongoDb.Driver.Extensions.Sample.AspNetCore
{
    public class OverrideNamingConvention : IMongoDbNamingHelper
    {
        public string GetDatabaseName(MongoDbDatabaseConfiguration configuration, string dbName)
        {
            return dbName + "TEST";
        }

        public string GetCollectionName(string requiredCollection)
        {
            return requiredCollection + "TEST";
        }
    }
}