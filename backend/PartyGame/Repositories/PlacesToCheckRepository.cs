using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using PartyGame.Entities;

namespace PartyGame.Repositories
{
    public interface IPlacesToCheckRepository: IRepository<PlaceToCheck>
    {
    }

    public class PlacesToCheckRepository :  Repository<PlaceToCheck>,IPlacesToCheckRepository
    {

        public PlacesToCheckRepository(GameDbContext gameDbContext):base(gameDbContext.Database,"PlacesToCheck")
        {
        }

        public async Task<List<PlaceToCheck>> GetAllPlaces()
        {
            return await Collection.Find(FilterDefinition<PlaceToCheck>.Empty).ToListAsync();
        }

        public async void AddNewPlace(PlaceToCheck newPlace)
        {
            await Collection.InsertOneAsync(newPlace);
        }

        public async Task<PlaceToCheck> GetPlaceToCheckById(string id)
        {
            return await Collection
                .Find(p => p.Id == ObjectId.Parse(id))
                .FirstOrDefaultAsync();
        }

        public async void RemovePlaceToCheckById(string id)
        { 
            var filter = Builders<PlaceToCheck>.Filter.Eq(s => s.Id, ObjectId.Parse(id));
            await Collection.DeleteOneAsync(filter);
        }
    }
}
