using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver.Extensions.Documents;
using MongoDB.Driver.Extensions.Paging.Requests;
using MongoDB.Driver.Extensions.Paging.Responses;

namespace MongoDB.Driver.Extensions.Extensions
{
	public static class MongoDbQueryExtensions
	{
		public static IFindFluent<TDocument, TDocument> Paged<TDocument>(this IFindFluent<TDocument, TDocument> find, SimplePagedRequest request)
		{
			return find
				.Limit(request.PageSize)
				.Skip(request.PageIndex * request.PageSize);
		}

		public static async Task<IPagedResult<TDocument>> ToPagedResultAsync<TDocument, TKey>(
			this IMongoCollection<TDocument> collection,
			FilterDefinition<TDocument> filter,
			SimplePagedRequest request,
			SortDefinition<TDocument> sort = null,
			CancellationToken cancellationToken = default(CancellationToken)) where TDocument : DocumentBase<TKey>
		{
			if (sort == null)
				sort = Builders<TDocument>.Sort.Descending(x => x.Id);

			if (filter == null)
				filter = Builders<TDocument>.Filter.Empty;

			var result = collection
				.Find(filter)
				.Sort(sort)
				.Limit(request.PageSize)
				.Skip(request.PageIndex * request.PageSize)
				.ToListAsync(cancellationToken);

			var count = collection
				.Find(filter)
				.CountDocumentsAsync(cancellationToken);

			await Task.WhenAll(result, count).ConfigureAwait(false);

			return new PagedResult<TDocument>(request.PageIndex, request.PageSize, result.Result, count.Result);
		}
		
		public static IPagedResult<TDocument> ToPagedResult<TDocument, TKey>(
			this IMongoCollection<TDocument> collection,
			FilterDefinition<TDocument> filter,
			SimplePagedRequest request,
			SortDefinition<TDocument> sort = null) where TDocument : DocumentBase<TKey>
		{
			if (sort == null)
				sort = Builders<TDocument>.Sort.Descending(x => x.Id);

			if (filter == null)
				filter = Builders<TDocument>.Filter.Empty;


			var result = collection
				.Find(filter)
				.Sort(sort)
				.Limit(request.PageSize)
				.Skip(request.PageIndex * request.PageSize)
				.ToList();

			var count = collection
				.Find(filter)
				.CountDocuments();

			return new PagedResult<TDocument>(request.PageIndex, request.PageSize, result, count);
		}
	}
}