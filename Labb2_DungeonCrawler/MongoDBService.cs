using Labb2_DungeonCrawler.Models;
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
    }
}
