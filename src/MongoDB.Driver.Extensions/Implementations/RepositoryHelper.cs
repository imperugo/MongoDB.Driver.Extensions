using MongoDB.Driver.Extensions.Abstractions;
using MongoDB.Driver.Extensions.Configurations;

namespace MongoDB.Driver.Extensions.Implementations
{
    public class RepositoryHelper : IRepositoryHelper
    {
        private readonly MongoDbDatabaseConfiguration configuration;
        private readonly IMongoClient mongoClient;
        private readonly IMongoDbNamingHelper namingHelper;

        public RepositoryHelper(IMongoClient mongoClient, MongoDbDatabaseConfiguration configuration, IMongoDbNamingHelper namingHelper = null)
        {
            this.mongoClient = mongoClient;
            this.configuration = configuration;
            this.namingHelper = namingHelper;
        }

        public IMongoDatabase GetDatabase(string dbName)
        {
            return mongoClient.GetDatabase(namingHelper.GetDatabaseName(configuration, dbName));
        }

        public IMongoCollection<T> GetIMongoCollection<T>(string dbName)
        {
            return GetIMongoCollection<T>(dbName, null);
        }

        public IMongoCollection<T> GetIMongoCollection<T>(string dbName, string collectionName)
        {
            if (collectionName == null)
                collectionName = typeof(T).Name;

            var calculatedName = namingHelper.GetCollectionName(collectionName);
            return GetDatabase(dbName).GetCollection<T>(calculatedName);
        }
    }
}