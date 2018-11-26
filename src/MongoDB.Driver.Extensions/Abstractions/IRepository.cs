using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver.Extensions.Documents;
using MongoDB.Driver.Extensions.Paging.Requests;
using MongoDB.Driver.Extensions.Paging.Responses;

namespace MongoDB.Driver.Extensions.Abstractions
{
	/// <summary>
	///     IRepository definition.
	/// </summary>
	/// <typeparam name="TDocument">The type contained in the repository.</typeparam>
	public interface IRepository<TDocument,TKey> where TDocument : DocumentBase<TKey>
	{
		/// <summary>
		/// An instance of <see cref="IMongoCollection{TDocument}"/>.
		/// </summary>
		IMongoCollection<TDocument> Collection { get; }
		
		/// <summary>
		/// And instance of <see cref="IMongoDatabase"/>.
		/// </summary>
		IMongoDatabase Database { get; }
		
		/// <summary>
		///     Saves the or update.
		/// </summary>
		/// <param name="predicate">The query condition.</param>
		/// <param name="document">The document.</param>
		/// <returns>An instance of <see cref="ReplaceOneResult"/>.</returns>
		ReplaceOneResult SaveOrUpdate(Expression<Func<TDocument, bool>> predicate, TDocument document);

		/// <summary>
		///     Saves the or update the specified document asynchronously.
		/// </summary>
		/// <param name="predicate">The query condition.</param>
		/// <param name="document">The document.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An instance of <see cref="ReplaceOneResult"/>.</returns>
		Task<ReplaceOneResult> SaveOrUpdateAsync(Expression<Func<TDocument, bool>> predicate, TDocument document, CancellationToken cancellationToken = default (CancellationToken));
		
		/// <summary>
		///     Saves the or update the specified document.
		/// </summary>
		/// <param name="document">The document.</param>
		/// <returns>An instance of <see cref="ReplaceOneResult"/>.</returns>
		ReplaceOneResult SaveOrUpdate(TDocument document);
		
		/// <summary>
		///     Saves the or update the specified document asynchronously.
		/// </summary>
		/// <param name="document">The document.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An instance of <see cref="ReplaceOneResult"/>.</returns>
		Task<ReplaceOneResult> SaveOrUpdateAsync(TDocument document, CancellationToken cancellationToken = default (CancellationToken));
		
		/// <summary>
		///     Updates the specified document.
		/// </summary>
		/// <param name="document">The document.</param>
		/// <returns>An instance of <see cref="ReplaceOneResult"/>.</returns>
		ReplaceOneResult Update(TDocument document);
		
		/// <summary>
		///     Updates the specified document asynchronously.
		/// </summary>
		/// <param name="document">The document.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An instance of <see cref="ReplaceOneResult"/>.</returns>
		Task<ReplaceOneResult> UpdateAsync(TDocument document, CancellationToken cancellationToken = default (CancellationToken));
		
		/// <summary>
		///     Check if a document with the specified condition exists into the store.
		/// </summary>
		/// <param name="predicate">The query condition.</param>
		/// <returns><c>True</c> if the document exists. Otherwise <c>False</c></returns>
		bool Exist(Expression<Func<TDocument, bool>> predicate);
		
		/// <summary>
		///     Check if a document with the specified condition exists into the store asynchronously.
		/// </summary>
		/// <param name="predicate">The query condition.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns><c>True</c> if the document exists. Otherwise <c>False</c></returns>
		Task<bool> ExistAsync(Expression<Func<TDocument, bool>> predicate, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		///     Check if a document with the specified filter exists into the store.
		/// </summary>
		/// <param name="filters">The query filters.</param>
		/// <returns><c>True</c> if the document exists. Otherwise <c>False</c></returns>
		bool Exist(FilterDefinition<TDocument> filters);

		/// <summary>
		///     Check if a document with the specified filters exists into the store asynchronously.
		/// </summary>
		/// <param name="filters">The query filters.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns><c>True</c> if the document exists. Otherwise <c>False</c></returns>
		Task<bool> ExistAsync(FilterDefinition<TDocument> filters, CancellationToken cancellationToken = default (CancellationToken));
		
		/// <summary>
		///     Check if a document with the specified id exists into the store.
		/// </summary>
		/// <param name="id">The value representing the ObjectId of the entity to retrieve.</param>
		/// <returns><c>True</c> if the document exists. Otherwise <c>False</c></returns>
		bool Exist(TKey id);

		/// <summary>
		///     Check if a document with the specified id exists into the store asynchronously.
		/// </summary>
		/// <param name="id">The value representing the ObjectId of the entity to retrieve.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns><c>True</c> if the document exists. Otherwise <c>False</c></returns>
		Task<bool> ExistAsync(TKey id, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		///    Insert a set of documents in a single roundtrip.
		/// </summary>
		/// <param name="documents">The documents to persist.</param>
		void InsertMany(IEnumerable<TDocument> documents);

		/// <summary>
		///     Insert a set of documents in a single roundtrip asynchronously.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="documents">The documents to persist.</param>
		Task InsertManyAsync(IEnumerable<TDocument> documents, CancellationToken cancellationToken = default (CancellationToken));
		
		/// <summary>
		///   Update the specified documents
		/// </summary>
		/// <param name="entities">The entities to update.</param>
		BulkWriteResult<TDocument> UpdateMany(IEnumerable<TDocument> entities);
		
		/// <summary>
		///   Update the specified documents asynchronously.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="entities">The entities to update.</param>
		Task<BulkWriteResult<TDocument>> UpdateManyAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		///     Returns the T by its given id.
		/// </summary>
		/// <param name="id">The value representing the Id of the document to retrieve.</param>
		/// <returns>The document T.</returns>
		TDocument GetById(TKey id);

		/// <summary>
		///     Returns the T by its given id asynchronously.
		/// </summary>
		/// <param name="id">The value representing the Id of the document to retrieve.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The document T.</returns>
		Task<TDocument> GetByIdAsync(TKey id, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		///     Returns a paged list for the specified filters
		/// </summary>
		/// <param name="request">The pagination information.</param>
		/// <param name="filters">The query filters.</param>
		/// <param name="sort">The query sort rules.</param>
		/// <returns>An instance of <see cref="IPagedResult{T}"/>.</returns>
		IPagedResult<TDocument> GetPagedList(SimplePagedRequest request, FilterDefinition<TDocument> filters = null, SortDefinition<TDocument> sort = null);
		
		/// <summary>
		///     Returns a paged list for the specified filters asynchronously.
		/// </summary>
		/// <param name="request">The pagination information.</param>
		/// <param name="filters">The query filters.</param>
		/// <param name="sort">The query sort rules.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An instance of <see cref="IPagedResult{T}"/>.</returns>
		Task<IPagedResult<TDocument>> GetPagedListAsync(SimplePagedRequest request, FilterDefinition<TDocument> filters = null, SortDefinition<TDocument> sort = null, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		///     Adds the new document in the repository.
		/// </summary>
		/// <param name="document">The document to add.</param>
		/// <returns>The added document including its new ObjectId.</returns>
		void Insert(TDocument document);
		
		/// <summary>
		///     Adds the new document in the repository asynchronously.
		/// </summary>
		/// <param name="document">The document to add.</param>
		
		/// <param name="cancellationToken">The cancellation token.</param>/// <returns>The added document including its new ObjectId.</returns>
		Task InsertAsync(TDocument document, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		///     Deletes an document from the repository by its id.
		/// </summary>
		/// <param name="id">The document's id.</param>
		DeleteResult Delete(TKey id);

		/// <summary>
		///     Deletes an document from the repository by its id asynchronously.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="id">The document's id.</param>
		Task<DeleteResult> DeleteAsync(TKey id, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		///     Deletes an document from the repository by its id.
		/// </summary>
		/// <param name="predicate">The delete condition.</param>
		DeleteResult Delete(Expression<Func<TDocument, bool>> predicate);
		
		/// <summary>
		///     Deletes an document from the repository by its id asynchronously.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="predicate">The delete condition.</param>
		Task<DeleteResult> DeleteAsync(Expression<Func<TDocument, bool>> predicate, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		///     Deletes the given document.
		/// </summary>
		/// <param name="document">The document to delete.</param>
		DeleteResult Delete(TDocument document);
		
		/// <summary>
		///     Deletes the given document asynchronously.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="document">The document to delete.</param>
		Task<DeleteResult> DeleteAsync(TDocument document, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		///     Deletes all document in the repository.
		/// </summary>
		void DeleteAll();
		
		/// <summary>
		///     Deletes all document in the repository asynchronously.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task DeleteAllAsync(CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		///     Counts the total document in the repository.
		/// </summary>
		/// <returns>Count of document in the repository.</returns>
		long Count();
		
		/// <summary>
		///     Counts the total document in the repository asynchronously.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>Count of document in the repository.</returns>
		Task<long> CountAsync(CancellationToken cancellationToken = default (CancellationToken));
		
		/// <summary>
		///     Counts the total document in the repository.
		/// </summary>
		/// <param name="filters">The query filters.</param>
		/// <returns>Count of document in the repository.</returns>
		long Count(FilterDefinition<TDocument> filters);

		/// <summary>
		///     Counts the total document in the repository asynchronously.
		/// </summary>
		/// <param name="filters">The query filters.</param>pr
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>Count of document in the repository.</returns>
		Task<long> CountAsync(FilterDefinition<TDocument> filters, CancellationToken cancellationToken = default (CancellationToken));
		
		/// <summary>
		///     Counts the total document in the repository.
		/// </summary>
		/// <param name="predicate">The query condition.</param>
		/// <returns>Count of document in the repository.</returns>
		long Count(Expression<Func<TDocument, bool>> predicate);

		/// <summary>
		///     Counts the total document in the repository asynchronously.
		/// </summary>
		/// <param name="predicate">The query condition.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>Count of document in the repository.</returns>
		Task<long> CountAsync(Expression<Func<TDocument, bool>> predicate, CancellationToken cancellationToken = default (CancellationToken));
	}
}