using MongoDB.Driver.Extensions.Configurations;

namespace MongoDB.Driver.Extensions.Abstractions
{
    public interface IMongoDbNamingHelper
    {
        string GetDatabaseName(MongoDbDatabaseConfiguration configuration, string dbName);
        string GetCollectionName(string requiredCollection);
    }
}