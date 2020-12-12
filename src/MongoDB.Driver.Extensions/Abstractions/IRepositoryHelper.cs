namespace MongoDB.Driver.Extensions.Abstractions
{
    public interface IRepositoryHelper
    {
        IMongoDatabase GetDatabase(string dbName);

        IMongoCollection<T> GetIMongoCollection<T>(string dbName);

        IMongoCollection<T> GetIMongoCollection<T>(string dbName, string collectionName);
    }
}