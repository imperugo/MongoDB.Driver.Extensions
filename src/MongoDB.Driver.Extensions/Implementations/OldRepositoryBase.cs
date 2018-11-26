/*

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver.Extensions.Abstractions;
using MongoDB.Driver.Extensions.Configurations;
using MongoDB.Driver.Extensions.Documents;
using MongoDB.Driver.Extensions.Extensions;
using MongoDB.Driver.Extensions.Paging.Requests;
using MongoDB.Driver.Extensions.Paging.Responses;

namespace MongoDB.Driver.Extensions.Implementations
{
    public abstract class RepositoryBase<T,TK> : IRepository<T,TK> where T : DocumentBase<TK>
	{
		protected MongoDbDatabaseConfiguration Configuration { get; }
		protected IMongoClient MongoClient { get; }
		
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
				collectionName = typeof(T).Name;
			}
			
			Database = mongoClient.GetDatabase(namingHelper.GetDatabaseName(configuration,dbName));
			Collection = MongoDatabaseExtensions.GetCollection<T>(Database, namingHelper.GetCollectionName(collectionName));
		}

		public IMongoCollection<T> Collection { get; }
		public IMongoDatabase Database { get; }

		public bool Exist(Expression<Func<T, bool>> predicate)
		{
			return Collection.Find(predicate).CountDocuments() > 0;
		}

		public virtual async Task<bool> ExistAsync(Expression<Func<T, bool>> predicate)
		{
			return await Collection
							.Find(predicate)
							.Limit(1)
							.CountDocumentsAsync()
							.ConfigureAwait(false) > 0;
		}

		public virtual bool Exist(TK id)
		{
			return Exist(Builders<T>.Filter.Eq(x => x.Id, id));
		}

		public virtual Task<bool> ExistAsync(TK id)
		{
			return ExistAsync(Builders<T>.Filter.Eq(x => x.Id, id));
		}

		public virtual bool Exist(FilterDefinition<T> filters)
		{
			return Collection.Find(filters).Limit(1).CountDocuments() > 0;
		}

		public virtual async Task<bool> ExistAsync(FilterDefinition<T> filters)
		{
			return (await Collection.Find(filters).Limit(1).CountDocumentsAsync().ConfigureAwait(false)) > 0;
		}

		public virtual void InsertMany(IEnumerable<T> documents)
		{
			Collection.InsertManyAsync(documents);
		}

		public virtual Task InsertManyAsync(IEnumerable<T> documents)
		{
			return Collection.InsertManyAsync(documents);
		}

		public virtual ReplaceOneResult SaveOrUpdate(Expression<Func<T, bool>> predicate, T entity)
		{
			return Collection.ReplaceOne(predicate, entity, new UpdateOptions {IsUpsert = true});
		}

		public virtual Task<ReplaceOneResult> SaveOrUpdateAsync(Expression<Func<T, bool>> predicate, T entity)
		{
			return Collection.ReplaceOneAsync(predicate, entity, new UpdateOptions {IsUpsert = true});
		}

		public virtual Task<ReplaceOneResult> SaveOrUpdateAsync(T entity)
		{
			var f = Builders<T>.Filter.Eq(x => x.Id,  entity.Id);
			return Collection.ReplaceOneAsync(f, entity, new UpdateOptions {IsUpsert = true});
		}

		public virtual ReplaceOneResult SaveOrUpdate(T entity)
		{
			var f = Builders<T>.Filter.Eq(x => x.Id,  entity.Id);
			return Collection.ReplaceOne(f, entity, new UpdateOptions {IsUpsert = true});
		}

		public virtual T GetById(TK id)
		{
			var f1 = Builders<T>.Filter.Eq(x => x.Id, id);

			return Collection.Find(f1).Limit(1).SingleOrDefault();
		}

		public virtual Task<T> GetByIdAsync(TK id)
		{
			var f1 = Builders<T>.Filter.Eq(x => x.Id, id);

			return Collection.Find(f1).Limit(1).SingleOrDefaultAsync();
		}
		
		public virtual IPagedResult<T> GetPagedList(SimplePagedRequest request, FilterDefinition<T> filters = null,SortDefinition<T> sort = null)
		{
			return Collection.ToPagedResult<T,TK>(filters, request, sort);
		}

		public virtual Task<IPagedResult<T>> GetPagedListAsync(SimplePagedRequest request, FilterDefinition<T> filters = null,SortDefinition<T> sort = null)
		{
			return Collection.ToPagedResultAsync<T,TK>(filters, request, sort);
		}

		public virtual async Task<T> AddAsync(T entity)
		{
			await Collection.InsertOneAsync(entity).ConfigureAwait(false);

			return entity;
		}

		public virtual Task AddAsync(IEnumerable<T> entities)
		{
			return Collection.InsertManyAsync(entities);
		}

		public virtual async Task<T> UpdateAsync(T entity)
		{
			var f = Builders<T>.Filter.Eq(x => x.Id,  entity.Id);
			await Collection.ReplaceOneAsync(f, entity).ConfigureAwait(false);

			return entity;
		}

		public virtual Task UpdateAsync(IEnumerable<T> entities)
		{
			var bulkOps = new List<WriteModel<T>>();
			
			foreach (var entity in entities)
			{
				var filter = Builders<T>.Filter.Eq(x => x.Id, entity.Id);
				var updateOne = new ReplaceOneModel<T>(filter, entity);
				
				bulkOps.Add(updateOne);
			}
			return Collection.BulkWriteAsync(bulkOps);
		}

		public virtual DeleteResult Delete(TK id)
		{
			var f = Builders<T>.Filter.Eq(x => x.Id,  id);
			return Collection.DeleteOne(f);
		}

		public virtual Task<DeleteResult> DeleteAsync(TK id)
		{
			var f = Builders<T>.Filter.Eq(x => x.Id,  id);
			return Collection.DeleteOneAsync(f);
		}
		
		public virtual Task DeleteAsync(Expression<Func<T, bool>> predicate)
		{
			return Collection.DeleteOneAsync(predicate);
		}

		public virtual Task<DeleteResult> DeleteAsync(T entity)
		{
			return DeleteAsync(entity.Id);
		}

		public virtual Task DeleteAllAsync()
		{
			return Database.DropCollectionAsync<T>();
		}

		public virtual Task<long> CountAsync()
		{
			return Collection.Find(x => x.Id != null).Limit(1).CountDocumentsAsync();
		}
	}
}
*/