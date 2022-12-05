using System.Linq.Expressions;
using MongoDB.Driver.Extensions.Abstractions;
using MongoDB.Driver.Extensions.Configurations;
using MongoDB.Driver.Extensions.Documents;
using MongoDB.Driver.Extensions.Paging.Requests;
using MongoDB.Driver.Extensions.Paging.Responses;

namespace MongoDB.Driver.Extensions.Implementations;

/// <summary>
/// The base class for all the repository implementations.
/// </summary>
/// <typeparam name="TDocument"></typeparam>
/// <typeparam name="TKey"></typeparam>
public abstract class RepositoryBase<TDocument, TKey> : IRepository<TDocument, TKey>
    where TDocument : DocumentBase<TKey>
    where TKey : notnull
{
    /// <summary>
    /// The instance of <see cref="IMongoClient"/>.
    /// </summary>
    protected IMongoClient MongoClient { get; }

    /// <summary>
    /// The name of the collection.
    /// </summary>
    protected string CollectionName { get; }

    /// <summary>
    /// The database name.
    /// </summary>
    protected string DatabaseName { get; }

    /// <summary>
    /// Initializes a new instance of the RepositoryBase class.
    /// </summary>
    /// <param name="mongoClient">The instance of <see cref="IMongoClient"/>.</param>
    /// <param name="dbName">The name of the database.</param>
    /// <param name="collectionName">The name of the collection.</param>
    protected RepositoryBase(
        IMongoClient mongoClient,
        string dbName,
        string collectionName)
    {
        MongoClient = mongoClient;
        CollectionName = collectionName;
        DatabaseName = dbName;
        Database = mongoClient.GetDatabase(DatabaseName);
        Collection = Database.GetCollection<TDocument>(CollectionName);
    }

    /// <inheritdoc/>
    public IMongoCollection<TDocument> Collection { get; }

    /// <inheritdoc/>
    public IMongoDatabase Database { get; }

    /// <inheritdoc/>
    public virtual async Task<bool> ExistAsync(Expression<Func<TDocument, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var result = await Collection.Find(predicate)
            .Limit(1)
            .Project(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        return result != null;
    }

    /// <inheritdoc/>
    public virtual Task<bool> ExistAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return ExistAsync(Builders<TDocument>.Filter.Eq(x => x.Id, id), cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<bool> ExistAsync(FilterDefinition<TDocument> filters, CancellationToken cancellationToken = default)
    {
        if (filters == null)
            filters = Builders<TDocument>.Filter.Empty;

        var result = await Collection
            .Find(filters)
            .Limit(1)
            .Project(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        return result != null;
    }

    /// <inheritdoc/>
    public virtual Task InsertManyAsync(IEnumerable<TDocument> documents, CancellationToken cancellationToken = default)
    {
        return Collection.InsertManyAsync(documents, cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public virtual Task<ReplaceOneResult> SaveOrUpdateAsync(Expression<Func<TDocument, bool>> predicate, TDocument entity, CancellationToken cancellationToken = default)
    {
        return Collection.ReplaceOneAsync(predicate, entity, new ReplaceOptions { IsUpsert = true }, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual Task<ReplaceOneResult> SaveOrUpdateAsync(TDocument entity, CancellationToken cancellationToken = default)
    {
        var f = Builders<TDocument>.Filter.Eq(x => x.Id, entity.Id);
        return Collection.ReplaceOneAsync(f, entity, new ReplaceOptions { IsUpsert = true }, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual Task<TDocument> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var f1 = Builders<TDocument>.Filter.Eq(x => x.Id, id);

        return Collection.Find(f1)
            .Limit(1)
            .SingleOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public virtual Task<IPagedResult<TDocument>> GetPagedListAsync(SimplePagedRequest request, FilterDefinition<TDocument>? filters = null, SortDefinition<TDocument>? sort = null, CancellationToken cancellationToken = default)
    {
        if (filters == null)
            filters = Builders<TDocument>.Filter.Empty;

        return Collection.ToPagedResultAsync<TDocument, TKey>(filters, request, sort, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<TDocument> AddAsync(TDocument entity, CancellationToken cancellationToken = default)
    {
        await Collection.InsertOneAsync(entity, cancellationToken: cancellationToken).ConfigureAwait(false);

        return entity;
    }

    /// <inheritdoc/>
    public virtual async Task<TDocument> UpdateAsync(TDocument entity, CancellationToken cancellationToken = default)
    {
        var f = Builders<TDocument>.Filter.Eq(x => x.Id, entity.Id);
        await Collection.ReplaceOneAsync(f, entity, cancellationToken: cancellationToken).ConfigureAwait(false);

        return entity;
    }

    /// <inheritdoc/>
    public virtual Task<BulkWriteResult<TDocument>> ReplaceManyAsync(IEnumerable<TDocument> documents, CancellationToken cancellationToken = default)
    {
        var bulkOps = new List<WriteModel<TDocument>>();

        foreach (var document in documents)
        {
            var filter = Builders<TDocument>.Filter.Eq(x => x.Id, document.Id);
            var updateOne = new ReplaceOneModel<TDocument>(filter, document);

            bulkOps.Add(updateOne);
        }

        return Collection.BulkWriteAsync(bulkOps, cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public virtual Task<DeleteResult> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var f = Builders<TDocument>.Filter.Eq(x => x.Id, id);
        return Collection.DeleteOneAsync(f, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual Task<DeleteResult> DeleteAsync(Expression<Func<TDocument, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Collection.DeleteOneAsync(predicate, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual Task<DeleteResult> DeleteAsync(TDocument entity, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(entity.Id, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual Task DeleteAllAsync(CancellationToken cancellationToken = default)
    {
        return Database.DropCollectionAsync(CollectionName, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        return Collection.Find(x => x.Id != null)
            .Limit(1)
            .CountDocumentsAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public virtual Task DropAsync(CancellationToken cancellationToken = default)
    {
        return Database.DropCollectionAsync(CollectionName, cancellationToken);
    }
}
