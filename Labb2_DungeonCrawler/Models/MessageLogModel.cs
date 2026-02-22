using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Labb2_DungeonCrawler.Models
{
    internal class MessageLogModel
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public ObjectId SaveId { get; set; }
        public List<string> Messages { get; set; }
    }
}
