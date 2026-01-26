using Labb2_DungeonCrawler.Elements;
using Labb2_DungeonCrawler.Models;
using Labb2_DungeonCrawler.States;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler
{
    internal class MongoDBService
    {
        private readonly IMongoDatabase _db;

        public IMongoCollection<PlayerClassModel> PlayerClasses => _db.GetCollection<PlayerClassModel>("PlayerClasses");
        public IMongoCollection<SavedGameModel> SavedGames => _db.GetCollection<SavedGameModel>("SavedGames");
        public IMongoCollection<MessageLogModel> Messages => _db.GetCollection<MessageLogModel>("Messages");

        public MongoDBService()
        {
            var client = new MongoClient("mongodb://localhost:27017");

            _db = client.GetDatabase("AdamAdolfsson");

            CreateCollectionsIfMissing();
            SeedPlayerClasses();
        }

        private void CreateCollectionsIfMissing()
        {
            var existingCollections = _db.ListCollectionNames().ToList();

            if (!existingCollections.Contains("PlayerClasses"))
            {
                _db.CreateCollection("PlayerClasses");
            }

            if (!existingCollections.Contains("SavedGames"))
            {
                _db.CreateCollection("SavedGames");
            }

            if (!existingCollections.Contains("Messages"))
            {
                _db.CreateCollection("Messages");
            }
        }

        private void SeedPlayerClasses()
        {
            if (PlayerClasses.CountDocuments(_ => true) == 0)
            {
                var defaults = new List<PlayerClassModel>
                {
                    new() { Name = "Warrior"},
                    new() { Name = "Wizard"},
                    new() { Name = "Thief"}
                };

                PlayerClasses.InsertMany(defaults);
            }
        }

        public void SaveGame(Player player, int turns)
        {
            var enemies = LevelData.Elements.OfType<Enemy>().Select(e => new EnemyState 
            { 
                Type = e.GetType().Name,
                Position = new Position(e.Position.X, e.Position.Y ),
                Health = e.Health
            }).ToList();

            var walls = LevelData.Elements.OfType<Wall>().Select(w => new WallState { 
                Position = new Position(w.Position.X, w.Position.Y ),
                Discovered = w.isDiscovered
            }).ToList();

            var save = new SavedGameModel
            {
                PlayerName = player.Name,
                PlayerClass = "Unknown",
                Health = player.Health,
                Position = new Position(player.Position.X, player.Position.Y),
                Turns = turns,
                IsAlive = player.isAlive,
                Enemies = enemies,
                Walls = walls
            };

            var filter = Builders<SavedGameModel>.Filter.Eq(s => s.PlayerName, player.Name);
            var existing =  SavedGames.Find(filter).FirstOrDefault();

            if (existing == null)
            {
                SavedGames.InsertOne(save);
            }
            else
            {
                SavedGames.ReplaceOne(filter, save);
            }
        }
    }
}
