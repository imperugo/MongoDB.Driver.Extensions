using MongoDB.Bson;
using MongoDB.Driver.Extensions.Abstractions;

namespace MongoDB.Driver.Extensions.Implementations;

/// <summary>
/// The repository that contains the methods needed to check if the connection to mongo id fine.
/// </summary>
public class AuditRepository : IAuditRepository
{
    private const string ADMIN_DATABASE = "admin";
    private readonly IMongoClient mongoClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditRepository"/> class.
    /// </summary>
    /// <param name="mongoClient">The instance of <see cref="IMongoClient"/>.</param>
    public AuditRepository(IMongoClient mongoClient)
    {
        this.mongoClient = mongoClient;
    }

    /// <inheritdoc/>
    public async Task<DbStatus> CheckAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new BsonDocument("ping", 1);
            await mongoClient
                .GetDatabase(ADMIN_DATABASE)
                .RunCommandAsync<BsonDocument>(command, null, cancellationToken)
                .ConfigureAwait(false);

            return new DbStatus(ADMIN_DATABASE, true);
        }
        catch
        {
            return new DbStatus(ADMIN_DATABASE, false);
        }
    }
}
