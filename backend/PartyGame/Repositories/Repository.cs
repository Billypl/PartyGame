using MongoDB.Bson;
using MongoDB.Driver;
using PartyGame.Entities;

namespace PartyGame.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> CreateAsync(T entity);
        Task<T> GetAsync(string id);
        Task<T> GetAsync(ObjectId id);
        Task<IEnumerable<T>> GetAllAsync();
        Task UpdateAsync(T entity);
        Task DeleteAsync(string id);
    }

    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly IMongoCollection<T> Collection;

        public Repository(IMongoDatabase dbContext, string collectionName)
        {
            Collection = dbContext.GetCollection<T>(collectionName);
        }

        public async Task<T> CreateAsync(T entity)
        {
            await Collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<T> GetAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
            return await Collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<T> GetAsync(ObjectId id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            return await Collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Collection.Find(FilterDefinition<T>.Empty).ToListAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            var idValue = entity.GetType().GetProperty("Id")?.GetValue(entity)?.ToString();
            var filter = Builders<T>.Filter.Eq("_id", new ObjectId(idValue));
            await Collection.ReplaceOneAsync(filter, entity);
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
            await Collection.DeleteOneAsync(filter);
        }
    }

}
