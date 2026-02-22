using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Labb2_DungeonCrawler.Models
{
    internal class PlayerClassModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }
    }
}
