using System;
using MongoDB.Driver;

namespace ArticleWebScraping.Helpers
{
	public class MongoHelper<T>
	{
        private readonly IMongoCollection<T> _collection;

        public MongoHelper(string connString, string databaseName, string collectionName)
        {
            var client = new MongoClient(connString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<T>(collectionName);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var documents = await _collection.FindAsync(FilterDefinition<T>.Empty);
            return await documents.ToListAsync();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            var document = await _collection.FindAsync(filter);
            return await document.FirstOrDefaultAsync();
        }

        public async Task<T> GetByUrlAsync(string url)
        {
            var filter = Builders<T>.Filter.Eq("ArticleUrl", url);
            var document = await _collection.FindAsync(filter);
            return await document.FirstOrDefaultAsync();
        }
        

        public async Task<IEnumerable<T>> GetAllFilterAsync(string fieldName, string filterValue)
        {
            var filter = Builders<T>.Filter.Eq(fieldName, filterValue);
            var documents = await _collection.FindAsync(filter);
            return await documents.ToListAsync();
        }

        public async Task<IEnumerable<T>> getAllPublishDateUzak(string fieldName, string filterValue)
        {
            var documents = await _collection.FindAsync(FilterDefinition<T>.Empty);
            return await documents.ToListAsync();
        }

        public async Task<IEnumerable<T>> getAllPublishDateYakin(string fieldName, string filterValue)
        {
            var documents = await _collection.FindAsync(FilterDefinition<T>.Empty);
            return await documents.ToListAsync();
        }



        public async Task InsertAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task<bool> UpdateAsync(string id, T entity)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            var result = await _collection.ReplaceOneAsync(filter, entity);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            var result = await _collection.DeleteOneAsync(filter);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

    }
}

