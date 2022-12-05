using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MongoDB.Driver.Extensions.Documents;
using MongoDB.Driver.Extensions.Paging.Requests;
using MongoDB.Driver.Extensions.Paging.Responses;

namespace MongoDB.Driver;

/// <summary>
/// A set of extensions methods for IMongoCollection{T}
/// </summary>
public static class ICollectionExtensions
{
    /// <summary>
    ///     Check if a document with the specified predicate exists into the store
    /// </summary>
    /// <param name="instance">The MongoDb collection.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellation">The cancellation token.</param>
    /// <returns><c>True</c> if the document exists. Otherwise <c>False</c></returns>
    public static async Task<bool> ExistAsync<TDocument, TKey>(this IMongoCollection<TDocument> instance,
        Expression<Func<TDocument, bool>> predicate, CancellationToken cancellation = default)
        where TDocument : DocumentBase<TKey>
        where TKey : notnull
    {
        return await instance.Find(predicate).CountDocumentsAsync(cancellation).ConfigureAwait(false) > 0;
    }

    /// <summary>
    ///     Check if a document with the specified id exists into the store
    /// </summary>
    /// <param name="instance">The MongoDb collection.</param>
    /// <param name="id">The value representing the ObjectId of the entity to retrieve.</param>
    /// <param name="cancellation">The cancellation token.</param>
    /// <returns><c>True</c> if the document exists. Otherwise <c>False</c></returns>
    public static Task<bool> ExistAsync<TDocument, TKey>(this IMongoCollection<TDocument> instance, TKey id,
        CancellationToken cancellation = default)
        where TDocument : DocumentBase<TKey>
        where TKey : notnull
    {
        return ExistAsync<TDocument, TKey>(instance, Builders<TDocument>.Filter.Eq(x => x.Id, id), cancellation);
    }

    /// <summary>
    ///     Check if a document with the specified predicate exists into the store
    /// </summary>
    /// <param name="instance">The MongoDb collection.</param>
    /// <param name="filters">The query filters.</param>
    /// <param name="cancellation">The cancellation token.</param>
    /// <returns><c>True</c> if the document exists. Otherwise <c>False</c></returns>
    public static async Task<bool> ExistAsync<TDocument, TKey>(this IMongoCollection<TDocument> instance,
        FilterDefinition<TDocument> filters, CancellationToken cancellation = default)
        where TDocument : DocumentBase<TKey>
        where TKey : notnull
    {
        return await instance.Find(filters)
            .CountDocumentsAsync(cancellation)
            .ConfigureAwait(false) > 0;
    }

    /// <summary>
    ///     Insert a set of documents in a single roundtrip.
    /// </summary>
    /// <param name="instance">The MongoDb collection.</param>
    /// <param name="documents">The documents to persist.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellation">The cancellation token.</param>
    public static Task InsertManyAsync<TDocument, TKey>(this IMongoCollection<TDocument> instance,
        IEnumerable<TDocument> documents, InsertManyOptions? options = null, CancellationToken cancellation = default)
        where TDocument : DocumentBase<TKey>
        where TKey : notnull
    {
        return instance.InsertManyAsync(documents, options, cancellation);
    }

    /// <summary>
    ///     Saves the or update.
    /// </summary>
    /// <param name="instance">The MongoDb collection.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="cancellation">The cancellation token.</param>
    /// <returns>The result of the replacement.</returns>
    public static Task<ReplaceOneResult> SaveOrUpdateAsync<TDocument, TKey>(this IMongoCollection<TDocument> instance,
        Expression<Func<TDocument, bool>> predicate, TDocument entity, CancellationToken cancellation = default)
        where TDocument : DocumentBase<TKey>
        where TKey : notnull
    {
        return instance.ReplaceOneAsync(predicate, entity, new ReplaceOptions { IsUpsert = true }, cancellation);
    }

    /// <summary>
    ///     Saves the or update.
    /// </summary>
    /// <param name="instance">The MongoDb collection.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="cancellation">The cancellation token.</param>
    /// <returns>The result of the replacement.</returns>
    public static Task<ReplaceOneResult> SaveOrUpdateAsync<TDocument, TKey>(this IMongoCollection<TDocument> instance,
        TDocument entity, CancellationToken cancellation = default)
        where TDocument : DocumentBase<TKey>
        where TKey : notnull
    {
        var f = Builders<TDocument>.Filter.Eq(x => x.Id, entity.Id);
        return instance.ReplaceOneAsync(f, entity, new ReplaceOptions { IsUpsert = true }, cancellation);
    }

    /// <summary>
    ///     Returns the T by its given id.
    /// </summary>
    /// <param name="instance">The MongoDb collection.</param>
    /// <param name="id">The value representing the ObjectId of the entity to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The Entity T.</returns>
    public static Task<TDocument> GetByIdAsync<TDocument, TKey>(this IMongoCollection<TDocument> instance, TKey id,
        CancellationToken cancellationToken = default)
        where TDocument : DocumentBase<TKey>
        where TKey : notnull
    {
        return instance.GetByIdAsync(id, (TDocument item) => item, cancellationToken);
    }

    /// <summary>
    ///     Returns the TResponse by its given id.
    /// </summary>
    /// <param name="instance">The MongoDb collection.</param>
    /// <param name="id">The value representing the ObjectId of the entity to retrieve.</param>
    /// <param name="transform"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <returns>The found documents.</returns>
    public static async Task<TResponse> GetByIdAsync<TDocument, TKey, TResponse>(this IMongoCollection<TDocument> instance,
        TKey id, Func<TDocument, TResponse> transform, CancellationToken cancellationToken = default)
        where TDocument : DocumentBase<TKey>
        where TKey : notnull
    {
        var f1 = Builders<TDocument>.Filter.Eq(x => x.Id, id);

        var dbInstance = await instance
            .Find(f1)
            .Limit(1)
            .FirstOrDefaultAsync(cancellationToken);

        return transform(dbInstance);
    }

    /// <summary>
    /// Return multiple document in a single round trip.
    /// </summary>
    /// <param name="instance">The MongoDb collection.</param>
    /// <param name="ids">A list of ids to retrieve from mongo.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <returns></returns>
    public static Task<Dictionary<TKey, TDocument>> GetByIdsAsync<TDocument, TKey>(this IMongoCollection<TDocument> instance,
        IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
        where TDocument : DocumentBase<TKey>
        where TKey : notnull
    {
        return instance.GetByIdsAsync(ids, (TDocument item) => item, cancellationToken);
    }

    /// <summary>
    /// Return multiple document in a single round trip.
    /// </summary>
    /// <param name="instance">The MongoDb collection.</param>
    /// <param name="ids">A list of ids to retrieve from mongo.</param>
    /// <param name="transform">The transformation function from the <typeparam name="TDocument"></typeparam> to <typeparam name="TResponse"></typeparam>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <returns>The found documents.</returns>
    public static async Task<Dictionary<TKey, TResponse>> GetByIdsAsync<TDocument, TKey, TResponse>(
        this IMongoCollection<TDocument> instance, IEnumerable<TKey> ids, Func<TDocument, TResponse> transform,
        CancellationToken cancellationToken = default)
        where TDocument : DocumentBase<TKey>
        where TKey : notnull
    {
        var f1 = Builders<TDocument>.Filter.In(x => x.Id, ids);

        var dbResponse = await instance
            .Find(f1)
            .ToListAsync(cancellationToken: cancellationToken);

        var result = new Dictionary<TKey, TResponse>(dbResponse.Count);

        for (var i = 0; i < dbResponse.Count; i++)
        {
            if (transform != null)
                result.Add(dbResponse[i].Id, transform(dbResponse[i]));
        }

        return result;
    }

    /// <summary>
    ///     Returns a paged list for the specified filters
    /// </summary>
    /// <param name="instance">The MongoDb collection.</param>
    /// <param name="request">The pagination information.</param>
    /// <param name="filters">The query filters</param>
    /// <param name="sort">The query sort.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <returns>The paged result.</returns>
    public static Task<IPagedResult<TDocument>> GetPagedListAsync<TDocument, TKey>(this IMongoCollection<TDocument> instance,
        SimplePagedRequest request, FilterDefinition<TDocument> filters, SortDefinition<TDocument>? sort = null,
        CancellationToken cancellationToken = default)
        where TDocument : DocumentBase<TKey>
        where TKey : notnull
    {
        return instance.ToPagedResultAsync<TDocument, TKey>(filters, request, sort, cancellationToken: cancellationToken);
    }

    /// <summary>
    ///     Returns a paged list for the specified filters
    /// </summary>
    /// <param name="instance">The MongoDb collection.</param>
    /// <param name="request">The pagination information.</param>
    /// <param name="filters">The query filters</param>
    /// <param name="transform">The transformation function from the <typeparam name="TDocument"></typeparam> to <typeparam name="TResponse"></typeparam>.</param>
    /// <param name="sort">The query sort.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <returns>The paged result.</returns>
    public static Task<IPagedResult<TResponse>> GetPagedListAsync<TDocument, TKey, TResponse>(
        this IMongoCollection<TDocument> instance, SimplePagedRequest request, FilterDefinition<TDocument> filters,
        Func<TDocument, TResponse> transform, SortDefinition<TDocument>? sort = null,
        CancellationToken cancellationToken = default)
        where TDocument : DocumentBase<TKey>
        where TKey : notnull
    {
        return instance.ToPagedResultAsync<TDocument, TKey, TResponse>(filters, request, transform, sort,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Adds the new entity in the repository.
    /// </summary>
    /// <param name="instance">The MongoDb collection.</param>
    /// <param name="document">The document to insert into MongoDb.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <returns>The added document.</returns>
    public static async Task<TDocument> AddAsync<TDocument, TKey>(this IMongoCollection<TDocument> instance, TDocument document,
        CancellationToken cancellationToken = default)
        where TDocument : DocumentBase<TKey>
        where TKey : notnull
    {
        await instance.InsertOneAsync(document, cancellationToken: cancellationToken);

        return document;
    }

    /// <summary>
    /// Update an document.
    /// </summary>
    /// <param name="instance">The MongoDb collection.</param>
    /// <param name="document">The document to insert into MongoDb.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <returns>The updated document.</returns>
    public static async Task<TDocument> UpdateAsync<TDocument, TKey>(this IMongoCollection<TDocument> instance,
        TDocument document, CancellationToken cancellationToken = default)
        where TDocument : DocumentBase<TKey>
        where TKey : notnull
    {
        var f = Builders<TDocument>.Filter.Eq(x => x.Id, document.Id);

        await instance.ReplaceOneAsync(f, document, cancellationToken: cancellationToken);

        return document;
    }

    /// <summary>
    /// Replase a set of documents in a singe roundtrip.
    /// </summary>
    /// <param name="instance">The MongoDb collection.</param>
    /// <param name="documents">The documents to repace</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <returns>The result of the bulk operation</returns>
    public static Task<BulkWriteResult<TDocument>> ReplaceManyAsync<TDocument, TKey>(
        this IMongoCollection<TDocument> instance, List<TDocument> documents, CancellationToken cancellationToken = default)
        where TDocument : DocumentBase<TKey>
        where TKey : notnull
    {
        var bulkOps = new WriteModel<TDocument>[documents.Count];

        var collectionAsSpan = CollectionsMarshal.AsSpan(documents);
        ref var searchSpace = ref MemoryMarshal.GetReference(collectionAsSpan);

        for (var i = 0; i < collectionAsSpan.Length; i++)
        {
            var document = Unsafe.Add(ref searchSpace, i);
            var filter = Builders<TDocument>.Filter.Eq(x => x.Id, document.Id);
            bulkOps[i] = new ReplaceOneModel<TDocument>(filter, document);
        }

        return instance.BulkWriteAsync(bulkOps, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Replase a set of documents in a singe roundtrip.
    /// </summary>
    /// <param name="instance">The MongoDb collection.</param>
    /// <param name="documents">The documents to repace</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <returns>The result of the bulk operation</returns>
    public static Task<BulkWriteResult<TDocument>> ReplaceManyAsync<TDocument, TKey>(
        this IMongoCollection<TDocument> instance, TDocument[] documents, CancellationToken cancellationToken = default)
        where TDocument : DocumentBase<TKey>
        where TKey : notnull
    {
        var bulkOps = new WriteModel<TDocument>[documents.Length];

        ref var searchSpace = ref MemoryMarshal.GetArrayDataReference(documents);

        for (var i = 0; i < documents.Length; i++)
        {
            var document = Unsafe.Add(ref searchSpace, i);
            var filter = Builders<TDocument>.Filter.Eq(x => x.Id, document.Id);
            bulkOps[i] = new ReplaceOneModel<TDocument>(filter, document);
        }

        return instance.BulkWriteAsync(bulkOps, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Delete a document from Mongo
    /// </summary>
    /// <param name="instance">The MongoDb collection.</param>
    /// <param name="id">The document id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <returns>The mongo db operation result.</returns>
    public static Task<DeleteResult> DeleteAsync<TDocument, TKey>(this IMongoCollection<TDocument> instance, TKey id,
        CancellationToken cancellationToken = default)
        where TDocument : DocumentBase<TKey>
        where TKey : notnull
    {
        var f = Builders<TDocument>.Filter.Eq(x => x.Id, id);
        return instance.DeleteOneAsync(f, cancellationToken);
    }

    /// <summary>
    /// Delete a document from Mongo
    /// </summary>
    /// <param name="instance">The MongoDb collection.</param>
    /// <param name="predicate">The query filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <returns>The mongo db operation result.</returns>
    public static Task<DeleteResult> DeleteAsync<TDocument, TKey>(this IMongoCollection<TDocument> instance,
        Expression<Func<TDocument, bool>> predicate, CancellationToken cancellationToken = default)
        where TDocument : DocumentBase<TKey>
        where TKey : notnull
    {
        return instance.DeleteOneAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Delete a document from Mongo
    /// </summary>
    /// <param name="instance">The MongoDb collection.</param>
    /// <param name="entity">The document to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <returns>The mongo db operation result.</returns>
    public static Task<DeleteResult> DeleteAsync<TDocument, TKey>(this IMongoCollection<TDocument> instance, TDocument entity,
        CancellationToken cancellationToken = default)
        where TDocument : DocumentBase<TKey>
        where TKey : notnull
    {
        return DeleteAsync<TDocument, TKey>(instance, entity.Id, cancellationToken);
    }

    /// <summary>
    /// Count the number of documents into the collection filters with the specified parameters
    /// </summary>
    /// <param name="instance">The MongoDb collection.</param>
    /// <param name="filters">The query filters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <returns>The number of document present into the collection with the specified filter</returns>
    public static Task<long> CountAsync<TDocument, TKey>(this IMongoCollection<TDocument> instance,
        FilterDefinition<TDocument>? filters = null, CancellationToken cancellationToken = default)
        where TDocument : DocumentBase<TKey>
        where TKey : notnull
    {
        if (filters == null)
            filters = Builders<TDocument>.Filter.Empty;

        return instance
            .Find(filters)
            .CountDocumentsAsync(cancellationToken);
    }
}
