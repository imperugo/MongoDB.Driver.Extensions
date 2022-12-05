using MongoDB.Driver.Extensions.Abstractions;

namespace MongoDB.Driver.Extensions.Implementations;

/// <summary>
/// The implementation of <see cref="IRepositoryHelper"/>.
/// </summary>
public class RepositoryHelper : IRepositoryHelper
{
    private readonly IMongoClient mongoClient;

    /// <summary>
    /// Initializes a new instance of the RepositoryHelper class.
    /// </summary>
    /// <param name="mongoClient">The instance of <see cref="IMongoClient"/>.</param>
    public RepositoryHelper(IMongoClient mongoClient)
    {
        this.mongoClient = mongoClient;
    }

    /// <inheritdoc/>
    public virtual IMongoDatabase GetDatabase(string dbName)
    {
        return mongoClient.GetDatabase(dbName);
    }

    /// <inheritdoc/>
    public virtual IMongoCollection<T> GetIMongoCollection<T>(string dbName, string collectionName)
    {
        return GetDatabase(dbName).GetCollection<T>(collectionName);
    }
}
