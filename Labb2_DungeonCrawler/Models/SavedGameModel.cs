using Labb2_DungeonCrawler.States;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Labb2_DungeonCrawler.Models
{
    internal class SavedGameModel
    {
        [BsonId]
        public ObjectId Id;

        public string PlayerName {  get; set; }
        public string PlayerClass { get; set; }
        public int Health { get; set; }
        public Position Position { get; set; }
        public int Turns { get; set; }
        public bool IsAlive { get; set; }

        public List<EnemyState> Enemies { get; set; }
        public List <WallState> Walls { get; set; }
    }
}
