using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver.Extensions.Documents;
using MongoDB.Driver.Extensions.Paging.Requests;
using MongoDB.Driver.Extensions.Paging.Responses;

namespace MongoDB.Driver
{
    public static class ICollectionExtensions
    {
        public static async Task<bool> ExistAsync<T, TK>(this IMongoCollection<T> instance, Expression<Func<T, bool>> predicate, CancellationToken cancellation = default)
                    where T : DocumentBase<TK>
        {
            return await instance.Find(predicate).CountDocumentsAsync(cancellation).ConfigureAwait(false) > 0;
        }

        public static Task<bool> ExistAsync<T, TK>(this IMongoCollection<T> instance, TK id, CancellationToken cancellation = default)
             where T : DocumentBase<TK>
        {
            return ExistAsync<T, TK>(instance, Builders<T>.Filter.Eq(x => x.Id, id), cancellation);
        }

        public static async Task<bool> ExistAsync<T, TK>(this IMongoCollection<T> instance, FilterDefinition<T> filters, CancellationToken cancellation = default)
             where T : DocumentBase<TK>
        {
            return await instance.Find(filters)
                .CountDocumentsAsync(cancellation)
                .ConfigureAwait(false) > 0;
        }

        public static Task InsertManyAsync<T, TK>(this IMongoCollection<T> instance, IEnumerable<T> documents, InsertManyOptions options = null, CancellationToken cancellation = default)
            where T : DocumentBase<TK>
        {
            return instance.InsertManyAsync(documents, options, cancellation);
        }

        public static Task<ReplaceOneResult> SaveOrUpdateAsync<T, TK>(this IMongoCollection<T> instance, Expression<Func<T, bool>> predicate, T entity, CancellationToken cancellation = default)
            where T : DocumentBase<TK>
        {
            return instance.ReplaceOneAsync(predicate, entity, new ReplaceOptions { IsUpsert = true }, cancellation);
        }

        public static Task<ReplaceOneResult> SaveOrUpdateAsync<T, TK>(this IMongoCollection<T> instance, T entity, CancellationToken cancellation = default)
            where T : DocumentBase<TK>
        {
            var f = Builders<T>.Filter.Eq(x => x.Id, entity.Id);
            return instance.ReplaceOneAsync(f, entity, new ReplaceOptions { IsUpsert = true }, cancellation);
        }

        public static Task<T> GetByIdAsync<T, TK>(this IMongoCollection<T> instance, TK id, CancellationToken cancellationToken = default)
            where T : DocumentBase<TK>
        {
            return instance.GetByIdAsync(id, (T item) => item, cancellationToken);
        }

        public static async Task<TR> GetByIdAsync<T, TK, TR>(this IMongoCollection<T> instance, TK id, Func<T, TR> transform, CancellationToken cancellationToken = default)
            where T : DocumentBase<TK>
        {
            var f1 = Builders<T>.Filter.Eq(x => x.Id, id);

            var dbInstance = await instance
                .Find(f1)
                .Limit(1)
                .FirstOrDefaultAsync(cancellationToken);

            return transform(dbInstance);
        }

        public static Task<Dictionary<TK, T>> GetByIdsAsync<T, TK>(this IMongoCollection<T> instance, IEnumerable<TK> ids, CancellationToken cancellationToken = default)
            where T : DocumentBase<TK>
        {
            return instance.GetByIdsAsync(ids, (T item) => item, cancellationToken);
        }

        public static async Task<Dictionary<TK, TR>> GetByIdsAsync<T, TK, TR>(this IMongoCollection<T> instance, IEnumerable<TK> ids, Func<T, TR> transform, CancellationToken cancellation = default)
            where T : DocumentBase<TK>
        {
            var f1 = Builders<T>.Filter.In(x => x.Id, ids);

            var dbResponse = await instance
                .Find(f1)
                .ToListAsync(cancellationToken: cancellation);

            var result = new Dictionary<TK, TR>(dbResponse.Count);

            for (var i = 0; i < dbResponse.Count; i++)
            {
                if (transform != null)
                    result.Add(dbResponse[i].Id, transform(dbResponse[i]));
            }

            return result;
        }

        public static Task<IPagedResult<T>> GetPagedListAsync<T, TK>(this IMongoCollection<T> instance, SimplePagedRequest request, FilterDefinition<T> filters, SortDefinition<T> sort = null, CancellationToken cancellation = default)
            where T : DocumentBase<TK>
        {
            return instance.ToPagedResultAsync<T, TK>(filters, request, sort, cancellationToken: cancellation);
        }

        public static Task<IPagedResult<TR>> GetPagedListAsync<T, TK, TR>(this IMongoCollection<T> instance, SimplePagedRequest request, FilterDefinition<T> filters, Func<T, TR> transform, SortDefinition<T> sort = null, CancellationToken cancellation = default)
            where T : DocumentBase<TK>
        {
            return instance.ToPagedResultAsync<T, TK, TR>(filters, request, transform, sort, cancellationToken: cancellation);
        }

        public static async Task<T> AddAsync<T, TK>(this IMongoCollection<T> instance, T entity, CancellationToken cancellation = default)
            where T : DocumentBase<TK>
        {
            await instance.InsertOneAsync(entity, cancellationToken: cancellation);

            return entity;
        }

        public static async Task<T> UpdateAsync<T, TK>(this IMongoCollection<T> instance, T entity, CancellationToken cancellation = default)
            where T : DocumentBase<TK>
        {
            var f = Builders<T>.Filter.Eq(x => x.Id, entity.Id);

            await instance.ReplaceOneAsync(f, entity, cancellationToken: cancellation);

            return entity;
        }

        public static Task<BulkWriteResult<T>> ReplaceManyAsync<T, TK>(this IMongoCollection<T> instance, IList<T> documents, CancellationToken cancellation = default)
            where T : DocumentBase<TK>
        {
            var bulkOps = new WriteModel<T>[documents.Count];

            for (var i = 0; i < documents.Count; i++)
            {
                var filter = Builders<T>.Filter.Eq(x => x.Id, documents[i].Id);
                bulkOps[i] = new ReplaceOneModel<T>(filter, documents[i]);
            }

            return instance.BulkWriteAsync(bulkOps, cancellationToken: cancellation);
        }

        public static Task<BulkWriteResult<T>> ReplaceManyAsync<T, TK>(this IMongoCollection<T> instance, T[] documents, CancellationToken cancellation = default)
            where T : DocumentBase<TK>
        {
            var bulkOps = new WriteModel<T>[documents.Length];

            for (var i = 0; i < documents.Length; i++)
            {
                var filter = Builders<T>.Filter.Eq(x => x.Id, documents[i].Id);
                bulkOps[i] = new ReplaceOneModel<T>(filter, documents[i]);
            }

            return instance.BulkWriteAsync(bulkOps, cancellationToken: cancellation);
        }

        public static Task<DeleteResult> DeleteAsync<T, TK>(this IMongoCollection<T> instance, TK id, CancellationToken cancellation = default)
            where T : DocumentBase<TK>
        {
            var f = Builders<T>.Filter.Eq(x => x.Id, id);
            return instance.DeleteOneAsync(f, cancellation);
        }

        public static Task<DeleteResult> DeleteAsync<T, TK>(this IMongoCollection<T> instance, Expression<Func<T, bool>> predicate, CancellationToken cancellation = default)
            where T : DocumentBase<TK>
        {
            return instance.DeleteOneAsync(predicate, cancellation);
        }

        public static Task<DeleteResult> DeleteAsync<T, TK>(this IMongoCollection<T> instance, T entity, CancellationToken cancellation = default)
            where T : DocumentBase<TK>
        {
            return DeleteAsync<T, TK>(instance, entity.Id, cancellation);
        }

        public static Task<long> CountAsync<T, TK>(this IMongoCollection<T> instance, FilterDefinition<T> filters = null, CancellationToken cancellation = default)
            where T : DocumentBase<TK>
        {
            if (filters == null)
                filters = Builders<T>.Filter.Empty;

            return instance
                .Find(filters)
                .CountDocumentsAsync(cancellation);
        }
    }
}