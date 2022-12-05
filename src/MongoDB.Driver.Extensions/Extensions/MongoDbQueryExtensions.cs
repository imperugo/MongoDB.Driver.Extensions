using System.Runtime.CompilerServices;
using MongoDB.Driver.Extensions.Documents;
using MongoDB.Driver.Extensions.Paging.Requests;
using MongoDB.Driver.Extensions.Paging.Responses;

namespace MongoDB.Driver;

/// <summary>
/// A set of extensions methods for most common queries in MongoDb
/// </summary>
public static class MongoDbQueryExtensions
{
    /// <summary>
    /// Set a paged query on <see cref="IFindFluent{TDocument, TDocument}"/> without execute the query
    /// </summary>
    /// <param name="find">The instance of <see cref="IFindFluent{TDocument, TDocument}"/>.</param>
    /// <param name="request">The paged request.</param>
    /// <typeparam name="TDocument">The type of the document to search.</typeparam>
    /// <returns>The paged instance of <see cref="IFindFluent{TDocument, TDocument}"/> passed.</returns>
    public static IFindFluent<TDocument, TDocument> Paged<TDocument>(this IFindFluent<TDocument, TDocument> find, SimplePagedRequest request)
    {
        return find
            .Limit(request.PageSize)
            .Skip(request.PageIndex * request.PageSize);
    }

    /// <summary>
    /// Set a paged query on <see cref="IFindFluent{TDocument, TDocument}"/> without execute the query
    /// </summary>
    /// <param name="find">The instance of <see cref="IFindFluent{TDocument, TDocument}"/>.</param>
    /// <param name="from">The begin of the pagination.</param>
    /// <param name="to">The end of the pagination.</param>
    /// <typeparam name="TDocument">The type of the document to search.</typeparam>
    /// <returns>The paged instance of <see cref="IFindFluent{TDocument, TDocument}"/> passed.</returns>
    public static IFindFluent<TDocument, TDocument> Paged<TDocument>(this IFindFluent<TDocument, TDocument> find, uint from, uint to)
    {
        var skip = from - 1;
        var take = to - skip;

        return find
            .Limit((int?)take)
            .Skip((int?)skip);
    }

    /// <summary>
    /// Execute a paged query on a specific collection.
    /// </summary>
    /// <param name="collection">The MongoDb collection.</param>
    /// <param name="filter">The query filter.</param>
    /// <param name="request">The paged request.</param>
    /// <param name="sort">The query sort.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TDocument">The type of the document to search.</typeparam>
    /// <typeparam name="TKey">The Type of the document key.</typeparam>
    /// <returns>The paged result.</returns>
    public static Task<IPagedResult<TDocument>> ToPagedResultAsync<TDocument, TKey>(
        this IMongoCollection<TDocument> collection,
        FilterDefinition<TDocument> filter,
        SimplePagedRequest request,
        SortDefinition<TDocument>? sort = null,
        CancellationToken cancellationToken = default)
        where TDocument : DocumentBase<TKey>
        where TKey : notnull
    {
        return collection.ToPagedResultAsync<TDocument, TKey, TDocument>(filter, request, (TDocument item) => item, sort, cancellationToken);
    }

    /// <summary>
    /// Execute a paged query on a specific collection.
    /// </summary>
    /// <param name="collection">The MongoDb collection.</param>
    /// <param name="filter">The query filter.</param>
    /// <param name="request">The paged request.</param>
    /// <param name="transform">The transformation function.</param>
    /// <param name="sort">The query sort.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TDocument">The type of the document to search.</typeparam>
    /// <typeparam name="TKey">The type of the document key.</typeparam>
    /// <typeparam name="TResult">The type of the returned document.</typeparam>
    /// <returns>The paged result.</returns>
    /// <returns></returns>
    public static async Task<IPagedResult<TResult>> ToPagedResultAsync<TDocument, TKey, TResult>(
        this IMongoCollection<TDocument> collection,
        FilterDefinition<TDocument>? filter,
        SimplePagedRequest request,
        Func<TDocument, TResult> transform,
        SortDefinition<TDocument>? sort = null,
        CancellationToken cancellationToken = default)
        where TDocument : DocumentBase<TKey>
        where TKey : notnull
    {
        if (filter == null)
            filter = Builders<TDocument>.Filter.Empty;

        var query = collection.Find(filter);

        if (sort != null)
            query.Sort(sort);

        var result = query
            .Paged(request)
            .ToListAsync(cancellationToken);

        var count = collection
            .Find(filter)
            .CountDocumentsAsync(cancellationToken);

        await Task.WhenAll(result, count);

        var methodResult = new TResult[result.Result.Count];

        for (var i = 0; i < result.Result.Count; i++)
            methodResult[i] = transform(result.Result[i]);

        return new PagedResult<TResult>(request.PageIndex, request.PageSize, methodResult, count.Result);
    }

    /// <summary>
    /// Provides asynchronous iteration over all document returned by <paramref name="collection"/>.
    /// </summary>
    /// <param name="collection">The collection to iterate.</param>
    /// <param name="filter">The query filter.</param>
    /// <param name="options">The find options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TDocument">The type of the collection document.</typeparam>
    /// <returns>Async-enumerable that iterates over all document returned by cursor.</returns>
    public static async IAsyncEnumerable<TDocument> ToAsyncEnumerableAsync<TDocument>(this IMongoCollection<TDocument> collection, FilterDefinition<TDocument> filter, FindOptions<TDocument, TDocument>? options = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var cursor = await collection.FindAsync(filter, options, cancellationToken);

        while (await cursor.MoveNextAsync(cancellationToken))
        {
            foreach (var doc in cursor.Current)
                yield return doc;
        }
    }
}
