using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver.Extensions.Abstractions;
using MongoDB.Driver.Extensions.Configurations;

namespace MongoDB.Driver.Extensions.Implementations
{
    public class AuditRepository : IAuditRepository
    {
        private readonly IMongoClient mongoClient;
        private readonly string databaseName;

        public AuditRepository(MongoDbDatabaseConfiguration configuration, IMongoClient mongoClient, IMongoDbNamingHelper namingHelper = null)
        {
            this.mongoClient = mongoClient;
            
            databaseName = namingHelper.GetDatabaseName(configuration, "admin");
            mongoClient.GetDatabase(databaseName);
        }

        public async Task<DbStatus> CheckAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var command = new BsonDocument("ping", 1);
                await mongoClient
                    .GetDatabase(databaseName)
                    .RunCommandAsync<BsonDocument>(command, null, cancellationToken)
                    .ConfigureAwait(false);
                	
                return new DbStatus(databaseName, true);
            }
            catch
            {
                return new DbStatus(databaseName, false);
            }
        }

        public DbStatus Check()
        {
            try
            {
                var command = new BsonDocument("ping", 1);
                mongoClient
                    .GetDatabase(databaseName)
                    .RunCommand<BsonDocument>(command);
                	
                return new DbStatus(databaseName, true);
            }
            catch
            {
                return new DbStatus(databaseName, false);
            }
        }
    }
}