using MongoDb.Driver.Extensions.Sample.AspNetCore.Data.Documents;
using MongoDB.Driver;
using MongoDB.Driver.Extensions.Abstractions;
using MongoDB.Driver.Extensions.Configurations;
using MongoDB.Driver.Extensions.Implementations;

namespace MongoDb.Driver.Extensions.Sample.AspNetCore.Data
{
    internal class UserRepository : RepositoryBase<User, string>
    {
        public UserRepository(MongoDbDatabaseConfiguration configuration,
                                IMongoClient mongoClient,
                                IMongoDbNamingHelper namingHelper)
                                        : base(configuration,
                                                mongoClient,
                                                "MyDatabase",
                                                namingHelper,
                                                 "MyCollectionName")
        {
        }
    }
}