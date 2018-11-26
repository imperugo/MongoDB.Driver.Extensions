using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver.Extensions.Abstractions;
using MongoDB.Driver.Extensions.Configurations;
using MongoDB.Driver.Extensions.Documents;
using MongoDB.Driver.Extensions.Extensions;
using MongoDB.Driver.Extensions.Paging.Requests;
using MongoDB.Driver.Extensions.Paging.Responses;

namespace MongoDB.Driver.Extensions.Implementations
{
    public abstract class RepositoryBase<TDocument, TKey> : IRepository<TDocument, TKey>
        where TDocument : DocumentBase<TKey>
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
            {
                namingHelper = new DefaultMongoDbNamingHelper();
            }

            if (collectionName == null)
            {
                collectionName = typeof(TDocument).Name;
            }

            CollectionName = namingHelper.GetCollectionName(collectionName);
            DatabaseName = namingHelper.GetDatabaseName(configuration, dbName);
            Database = mongoClient.GetDatabase(DatabaseName);
            Collection = Database.GetCollection<TDocument>(CollectionName);
        }

        public IMongoCollection<TDocument> Collection { get; }
        public IMongoDatabase Database { get; }
        
        public ReplaceOneResult SaveOrUpdate(Expression<Func<TDocument, bool>> predicate, TDocument document)
        {
            return Collection.ReplaceOne(predicate, document, new UpdateOptions {IsUpsert = true});
        }

        public Task<ReplaceOneResult> SaveOrUpdateAsync(Expression<Func<TDocument, bool>> predicate, TDocument document, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Collection.ReplaceOneAsync(predicate, document, new UpdateOptions {IsUpsert = true},cancellationToken);
        }

        public ReplaceOneResult SaveOrUpdate(TDocument document)
        {
            var f = Builders<TDocument>.Filter.Eq(x => x.Id,  document.Id);
            return Collection.ReplaceOne(f, document, new UpdateOptions {IsUpsert = true});
        }

        public Task<ReplaceOneResult> SaveOrUpdateAsync(TDocument document, CancellationToken cancellationToken = default(CancellationToken))
        {
            var f = Builders<TDocument>.Filter.Eq(x => x.Id,  document.Id);
            return Collection.ReplaceOneAsync(f, document, new UpdateOptions {IsUpsert = true},cancellationToken);
        }

        public ReplaceOneResult Update(TDocument document)
        {
            var f = Builders<TDocument>.Filter.Eq(x => x.Id,  document.Id);
            return Collection.ReplaceOne(f, document);
        }

        public Task<ReplaceOneResult> UpdateAsync(TDocument document, CancellationToken cancellationToken = default(CancellationToken))
        {
            var f = Builders<TDocument>.Filter.Eq(x => x.Id,  document.Id);
            return Collection.ReplaceOneAsync(f, document, null, cancellationToken);
        }

        public bool Exist(Expression<Func<TDocument, bool>> predicate)
        {
            return Collection
                       .Find(predicate)
                       .Limit(1)
                       .CountDocuments() > 0;
        }

        public async Task<bool> ExistAsync(Expression<Func<TDocument, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Collection
                .Find(predicate)
                .Limit(1)
                .CountDocumentsAsync(cancellationToken)
                .ConfigureAwait(false) > 0;
        }

        public bool Exist(FilterDefinition<TDocument> filters)
        {
            return Collection
                       .Find(filters)
                       .Limit(1)
                       .CountDocuments() > 0;
        }

        public async Task<bool> ExistAsync(FilterDefinition<TDocument> filters, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Collection
                       .Find(filters)
                       .Limit(1)
                       .CountDocumentsAsync(cancellationToken)
                       .ConfigureAwait(false) > 0;
        }

        public bool Exist(TKey id)
        {
            var filter = Builders<TDocument>.Filter.Eq(x => x.Id, id);
            return Exist(filter);
        }

        public Task<bool> ExistAsync(TKey id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var filter = Builders<TDocument>.Filter.Eq(x => x.Id, id);
            return ExistAsync(filter, cancellationToken);
        }

        public void InsertMany(IEnumerable<TDocument> documents)
        {
            Collection.InsertManyAsync(documents);
        }

        public Task InsertManyAsync(IEnumerable<TDocument> documents, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Collection.InsertManyAsync(documents,null,cancellationToken);
        }

        public BulkWriteResult<TDocument> UpdateMany(IEnumerable<TDocument> entities)
        {
            var bulkOps = new List<WriteModel<TDocument>>();
			
            foreach (var entity in entities)
            {
                var filter = Builders<TDocument>.Filter.Eq(x => x.Id, entity.Id);
                var updateOne = new ReplaceOneModel<TDocument>(filter, entity);
				
                bulkOps.Add(updateOne);
            }

            return Collection.BulkWrite(bulkOps);
        }

        public Task<BulkWriteResult<TDocument>> UpdateManyAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken = default(CancellationToken))
        {
            var bulkOps = new List<WriteModel<TDocument>>();
			
            foreach (var entity in entities)
            {
                var filter = Builders<TDocument>.Filter.Eq(x => x.Id, entity.Id);
                var updateOne = new ReplaceOneModel<TDocument>(filter, entity);
				
                bulkOps.Add(updateOne);
            }

            return Collection.BulkWriteAsync(bulkOps, null, cancellationToken);
        }

        public TDocument GetById(TKey id)
        {
            var f1 = Builders<TDocument>.Filter.Eq(x => x.Id, id);

            return Collection.Find(f1).Limit(1).SingleOrDefault();
        }

        public Task<TDocument> GetByIdAsync(TKey id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var f1 = Builders<TDocument>.Filter.Eq(x => x.Id, id);

            return Collection.Find(f1).Limit(1).SingleOrDefaultAsync(cancellationToken);
        }

        public IPagedResult<TDocument> GetPagedList(SimplePagedRequest request, FilterDefinition<TDocument> filters = null, SortDefinition<TDocument> sort = null)
        {
            return Collection.ToPagedResult<TDocument,TKey>(filters, request, sort);
        }

        public Task<IPagedResult<TDocument>> GetPagedListAsync(SimplePagedRequest request, FilterDefinition<TDocument> filters = null, SortDefinition<TDocument> sort = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Collection.ToPagedResultAsync<TDocument,TKey>(filters, request, sort,cancellationToken);
        }

        public void Insert(TDocument document)
        {
            Collection.InsertOne(document);
        }

        public Task InsertAsync(TDocument document, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Collection.InsertOneAsync(document,null,cancellationToken);
        }

        public DeleteResult Delete(TKey id)
        {
            var f = Builders<TDocument>.Filter.Eq(x => x.Id,  id);
            return Collection.DeleteOne(f);
        }

        public Task<DeleteResult> DeleteAsync(TKey id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var f = Builders<TDocument>.Filter.Eq(x => x.Id,  id);
            return Collection.DeleteOneAsync(f, cancellationToken);
        }

        public DeleteResult Delete(Expression<Func<TDocument, bool>> predicate)
        {
            return Collection.DeleteOne(predicate);
        }

        public Task<DeleteResult> DeleteAsync(Expression<Func<TDocument, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Collection.DeleteOneAsync(predicate, cancellationToken);
        }

        public DeleteResult Delete(TDocument document)
        {
            return Delete(document.Id);
        }

        public Task<DeleteResult> DeleteAsync(TDocument document, CancellationToken cancellationToken = default(CancellationToken))
        {
            return DeleteAsync(document.Id,cancellationToken);
        }

        public void DeleteAll()
        {
            Database.DropCollection(CollectionName);
        }

        public Task DeleteAllAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Database.DropCollectionAsync(CollectionName,cancellationToken);
        }

        public long Count()
        {
            return Collection.Find(x => true).Limit(1).CountDocuments();
        }

        public Task<long> CountAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Collection.Find(x => true).Limit(1).CountDocumentsAsync(cancellationToken);
        }

        public long Count(FilterDefinition<TDocument> filters)
        {
            return Collection.Find(filters).Limit(1).CountDocuments();
        }

        public Task<long> CountAsync(FilterDefinition<TDocument> filters, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Collection.Find(filters).Limit(1).CountDocumentsAsync(cancellationToken);
        }

        public long Count(Expression<Func<TDocument, bool>> predicate)
        {
            return Collection.Find(predicate).Limit(1).CountDocuments();
        }

        public Task<long> CountAsync(Expression<Func<TDocument, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Collection.Find(predicate).Limit(1).CountDocumentsAsync(cancellationToken);
        }
    }
}