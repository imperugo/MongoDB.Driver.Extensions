using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver.Extensions.Abstractions;
using MongoDB.Driver.Extensions.Configurations;
using MongoDB.Driver.Extensions.Documents;
using MongoDB.Driver.Extensions.Paging.Requests;
using MongoDB.Driver.Extensions.Paging.Responses;

namespace MongoDB.Driver.Extensions.Implementations
{
    public abstract class RepositoryBase<T, TK> : IRepository<T, TK> where T : DocumentBase<TK>
    {
        protected MongoDbDatabaseConfiguration Configuration { get; }
        protected IMongoClient MongoClient { get; }
        protected string CollectionName { get; }
        protected string DatabaseName { get; }

        protected RepositoryBase(MongoDbDatabaseConfiguration configuration,
            IMongoClient mongoClient,
            string dbName,
            IMongoDbNamingHelper namingHelper = null,
            string collectionName = null)
        {
            MongoClient = mongoClient;
            Configuration = configuration;

            if (namingHelper == null)
                namingHelper = new DefaultMongoDbNamingHelper();

            if (collectionName == null)
                collectionName = typeof(T).Name;

            CollectionName = namingHelper.GetCollectionName(collectionName);
            DatabaseName = namingHelper.GetDatabaseName(configuration, dbName);
            Database = mongoClient.GetDatabase(DatabaseName);
            Collection = Database.GetCollection<T>(CollectionName);
        }

        public IMongoCollection<T> Collection { get; }
        public IMongoDatabase Database { get; }

        public virtual async Task<bool> ExistAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellation = default)
        {
            var result = await Collection.Find(predicate)
                .Limit(1)
                .Project(x => x.Id)
                .FirstOrDefaultAsync(cancellation)
                .ConfigureAwait(false);

            return result != null;
        }

        public virtual Task<bool> ExistAsync(TK id, CancellationToken cancellation = default)
        {
            return ExistAsync(Builders<T>.Filter.Eq(x => x.Id, id), cancellation);
        }

        public virtual async Task<bool> ExistAsync(FilterDefinition<T> filters, CancellationToken cancellation = default)
        {
            if (filters == null)
                filters = Builders<T>.Filter.Empty;

            var result = await Collection
                .Find(filters)
                .Limit(1)
                .Project(x => x.Id)
                .FirstOrDefaultAsync(cancellation)
                .ConfigureAwait(false);

            return result != null;
        }

        public virtual Task InsertManyAsync(IEnumerable<T> documents, CancellationToken cancellation = default)
        {
            return Collection.InsertManyAsync(documents, cancellationToken: cancellation);
        }

        public virtual Task<ReplaceOneResult> SaveOrUpdateAsync(Expression<Func<T, bool>> predicate, T entity, CancellationToken cancellation = default)
        {
            return Collection.ReplaceOneAsync(predicate, entity, new ReplaceOptions { IsUpsert = true }, cancellation);
        }

        public virtual Task<ReplaceOneResult> SaveOrUpdateAsync(T entity, CancellationToken cancellation = default)
        {
            var f = Builders<T>.Filter.Eq(x => x.Id, entity.Id);
            return Collection.ReplaceOneAsync(f, entity, new ReplaceOptions { IsUpsert = true }, cancellation);
        }

        public virtual Task<T> GetByIdAsync(TK id, CancellationToken cancellation = default)
        {
            var f1 = Builders<T>.Filter.Eq(x => x.Id, id);

            return Collection.Find(f1)
                .Limit(1)
                .SingleOrDefaultAsync(cancellation);
        }

        public virtual Task<IPagedResult<T>> GetPagedListAsync(SimplePagedRequest request, FilterDefinition<T> filters = null, SortDefinition<T> sort = null, CancellationToken cancellation = default)
        {
            if (filters == null)
                filters = Builders<T>.Filter.Empty;

            return Collection.ToPagedResultAsync<T, TK>(filters, request, sort, cancellation);
        }

        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellation = default)
        {
            await Collection.InsertOneAsync(entity, cancellationToken: cancellation).ConfigureAwait(false);

            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity, CancellationToken cancellation = default)
        {
            var f = Builders<T>.Filter.Eq(x => x.Id, entity.Id);
            await Collection.ReplaceOneAsync(f, entity, cancellationToken: cancellation).ConfigureAwait(false);

            return entity;
        }

        public virtual Task<BulkWriteResult<T>> ReplaceManyAsync(IEnumerable<T> documents, CancellationToken cancellation = default)
        {
            var bulkOps = new List<WriteModel<T>>();

            foreach (var document in documents)
            {
                var filter = Builders<T>.Filter.Eq(x => x.Id, document.Id);
                var updateOne = new ReplaceOneModel<T>(filter, document);

                bulkOps.Add(updateOne);
            }

            return Collection.BulkWriteAsync(bulkOps, cancellationToken: cancellation);
        }

        public virtual Task<DeleteResult> DeleteAsync(TK id, CancellationToken cancellation = default)
        {
            var f = Builders<T>.Filter.Eq(x => x.Id, id);
            return Collection.DeleteOneAsync(f, cancellation);
        }

        public virtual Task<DeleteResult> DeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellation = default)
        {
            return Collection.DeleteOneAsync(predicate, cancellation);
        }

        public virtual Task<DeleteResult> DeleteAsync(T entity, CancellationToken cancellation = default)
        {
            return DeleteAsync(entity.Id, cancellation);
        }

        public virtual Task DeleteAllAsync(CancellationToken cancellation = default)
        {
            return Database.DropCollectionAsync(CollectionName, cancellation);
        }

        public virtual Task<long> CountAsync(CancellationToken cancellation = default)
        {
            return Collection.Find(x => x.Id != null)
                .Limit(1)
                .CountDocumentsAsync(cancellation);
        }

        public virtual Task Drop(CancellationToken cancellation = default)
        {
            return Database.DropCollectionAsync(CollectionName, cancellation);
        }

        protected virtual string NormalizeDbName(string dbName)
        {
            return string.Concat(dbName, Configuration.EnvironmentSuffix);
        }
    }
}