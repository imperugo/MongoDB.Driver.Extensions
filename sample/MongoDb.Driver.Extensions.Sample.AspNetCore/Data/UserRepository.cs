using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Extensions.Implementations;
using MongoDb.Driver.Extensions.Sample.AspNetCore.Data.Documents;

namespace MongoDb.Driver.Extensions.Sample.AspNetCore.Data;

internal class UserRepository : RepositoryBase<User, ObjectId>
{
    public UserRepository( IMongoClient mongoClient)
        : base(mongoClient, "MyDatabase", "MyCollectionName")
    {
    }
}
