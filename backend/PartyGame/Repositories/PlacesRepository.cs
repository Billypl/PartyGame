using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using PartyGame.Entities;

namespace PartyGame.Repositories
{
    public interface IPlacesRepository:IRepository<Place>
    {
        Task<long> GetPlacesCount();
        Task<List<Place>> GetPlacesByDifficulty(DifficultyLevel difficultyLevel);
    }

    public class PlacesRepository : Repository<Place>,IPlacesRepository
    {
        
        public PlacesRepository(GameDbContext gameDbContext) :base(gameDbContext.Database,"Places")
        {
           
        }
        public async Task<long> GetPlacesCount()
        {
            return await Collection.CountDocumentsAsync(FilterDefinition<Place>.Empty);
        }

        public async Task<List<Place>> GetPlacesByDifficulty(DifficultyLevel difficultyLevel)
        {
            return await Collection
                .Find(p => p.DifficultyLevel == difficultyLevel)
                .ToListAsync();
        }
    }
}
