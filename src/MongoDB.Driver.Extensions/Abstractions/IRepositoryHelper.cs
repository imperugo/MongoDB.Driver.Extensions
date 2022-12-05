namespace MongoDB.Driver.Extensions.Abstractions;

/// <summary>
/// The contract for the repository base class.
/// </summary>
public interface IRepositoryHelper
{
    /// <summary>
    /// Create an instance of <see cref="IMongoDatabase"/>.
    /// </summary>
    /// <param name="dbName">The name of the database.</param>
    /// <returns>An instance of <see cref="IMongoDatabase"/>.</returns>
    IMongoDatabase GetDatabase(string dbName);

    /// <summary>
    /// Create an instance of <see cref="IMongoCollection{T}"/>.
    /// </summary>
    /// <param name="dbName">The name of the database.</param>
    /// <param name="collectionName">The collection name.</param>
    /// <typeparam name="T">The type of the collection document.</typeparam>
    /// <returns>An instance of <see cref="IMongoCollection{T}"/>.</returns>
    IMongoCollection<T> GetIMongoCollection<T>(string dbName, string collectionName);
}
