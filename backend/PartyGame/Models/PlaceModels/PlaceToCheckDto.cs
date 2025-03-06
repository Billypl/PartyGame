using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PartyGame.Models.PlaceModels
{
    public class PlaceToCheckDto
    {
        public string Id { get; set; }
        public NewPlaceDto NewPlace { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string AuthorId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
