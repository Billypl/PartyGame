using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PartyGame.Models.GameModels;

namespace PartyGame.Entities
{
    public class Place
    {
        [BsonId]
        public ObjectId Id { get; set; } 
        public string Name { get; set; } 
        public string Description { get; set; } 
        public Coordinates Coordinates { get; set; }
        public string ImageUrl { get; set; } 
        public string Alt { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? AuthorId  { get; set; }
        
    }
}
