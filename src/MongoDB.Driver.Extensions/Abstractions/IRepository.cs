using System.Linq.Expressions;
using MongoDB.Driver.Extensions.Documents;
using MongoDB.Driver.Extensions.Paging.Requests;
using MongoDB.Driver.Extensions.Paging.Responses;

namespace MongoDB.Driver.Extensions.Abstractions;

/// <summary>
///     IRepository definition.
/// </summary>
/// <typeparam name="TDocument">The type contained in the repository.</typeparam>
/// <typeparam name="TKey">The type of the document key.</typeparam>
public interface IRepository<TDocument, TKey>
    where TDocument : DocumentBase<TKey>
    where TKey : notnull
{
    /// <summary>
    /// The MongoDB collection.
    /// </summary>
    IMongoCollection<TDocument> Collection { get; }

    /// <summary>
    /// The instance of <see cref="IMongoDatabase"/>.
    /// </summary>
    IMongoDatabase Database { get; }

    /// <summary>
    ///     Check if a document with the specified predicate exists into the store
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>True</c> if the document exists. Otherwise <c>False</c></returns>
    Task<bool> ExistAsync(Expression<Func<TDocument, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Check if a document with the specified predicate exists into the store
    /// </summary>
    /// <param name="filters">The query filters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>True</c> if the document exists. Otherwise <c>False</c></returns>
    Task<bool> ExistAsync(FilterDefinition<TDocument> filters, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Check if a document with the specified id exists into the store
    /// </summary>
    /// <param name="id">The value representing the ObjectId of the entity to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>True</c> if the document exists. Otherwise <c>False</c></returns>
    Task<bool> ExistAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Insert a set of documents in a single roundtrip.
    /// </summary>
    /// <param name="documents">The documents to persist.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task InsertManyAsync(IEnumerable<TDocument> documents, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Saves the or update.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the replacement.</returns>
    Task<ReplaceOneResult> SaveOrUpdateAsync(Expression<Func<TDocument, bool>> predicate, TDocument entity, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Saves the or update.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the replacement.</returns>
    Task<ReplaceOneResult> SaveOrUpdateAsync(TDocument entity, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Returns the T by its given id.
    /// </summary>
    /// <param name="id">The value representing the ObjectId of the entity to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The Entity T.</returns>
    Task<TDocument> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Returns a paged list for the specified filters
    /// </summary>
    /// <param name="request">The pagination information.</param>
    /// <param name="filters">The query filters.</param>
    /// <param name="sort">The query sort rules.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The Entity T.</returns>
    Task<IPagedResult<TDocument>> GetPagedListAsync(SimplePagedRequest request, FilterDefinition<TDocument>? filters = null, SortDefinition<TDocument>? sort = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Adds the new entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The added entity including its new ObjectId.</returns>
    Task<TDocument> AddAsync(TDocument entity, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Update an document.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated entity.</returns>
    Task<TDocument> UpdateAsync(TDocument entity, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Update the specified documents asynchronous.
    /// </summary>
    /// <param name="documents">The documents to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<BulkWriteResult<TDocument>> ReplaceManyAsync(IEnumerable<TDocument> documents, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes an entity from the repository by its id.
    /// </summary>
    /// <param name="id">The entity's id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<DeleteResult> DeleteAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes the given entity.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<DeleteResult> DeleteAsync(TDocument entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a document from Mongo
    /// </summary>
    /// <param name="predicate">The query filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The mongo db operation result.</returns>
    Task<DeleteResult> DeleteAsync(Expression<Func<TDocument, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes all entities in the repository.
    /// </summary>
    Task DeleteAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Counts the total entities in the repository.
    /// </summary>
    /// <returns>Count of entities in the repository.</returns>
    Task<long> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Delete the collection.
    /// </summary>
    Task DropAsync(CancellationToken cancellationToken = default);
}
