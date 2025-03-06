using MongoDB.Driver;

namespace PartyGame.Entities
{
    public class GameDbContext
    {
        public IMongoDatabase Database { get; }

        public GameDbContext(IMongoClient mongoClient)
        {
            Database = mongoClient.GetDatabase("PartyGame");

            var collection = Database.GetCollection<GameSession>("GameSessions");

            var indexOptions = new CreateIndexOptions
            {
                ExpireAfter = TimeSpan.Zero
            };

            var indexKeys = Builders<GameSession>.IndexKeys.Ascending(x => x.ExpirationDate);
            var indexModel = new CreateIndexModel<GameSession>(indexKeys, indexOptions);

            collection.Indexes.CreateOne(indexModel);

        }

        public IMongoCollection<Place> Places => Database.GetCollection<Place>("Places");
        public IMongoCollection<PlaceToCheck> PlacesToCheck => Database.GetCollection<PlaceToCheck>("PlacesToCheck");
        public IMongoCollection<GameSession> GameSessions => Database.GetCollection<GameSession>("GameSessions");
        public IMongoCollection<FinishedGame> GameResults => Database.GetCollection<FinishedGame>("GameResults");
        public IMongoCollection<User> Users => Database.GetCollection<User>("Users");
    }
}