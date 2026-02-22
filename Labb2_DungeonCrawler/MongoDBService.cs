using Labb2_DungeonCrawler.Elements;
using Labb2_DungeonCrawler.Models;
using Labb2_DungeonCrawler.States;
using MongoDB.Bson;
using MongoDB.Driver;

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

            _db = client.GetDatabase("DungeonCrawler");

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
                    new() { Name = "Thief"},
                    new() { Name = "Priest"}
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
                PlayerClass = player.Class,
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
                save.Id = existing.Id;
                SavedGames.ReplaceOne(filter, save);
            }
        }

        public List<SavedGameModel> GetAllSavedGames()
        {
            return SavedGames.Find(_ => true).ToList();
        }

        public SavedGameModel LoadGame(string playerName)
        {
            var filter = Builders<SavedGameModel>.Filter.Eq(s => s.PlayerName, playerName);
            return SavedGames.Find(filter).FirstOrDefault();
        }

        public void LoadSavedGame(SavedGameModel save)
        {

            var player = LevelData.Elements.OfType<Player>().FirstOrDefault();

            if (player == null)
            {
                player = new Player(save.Position.X, save.Position.Y);
                LevelData.Elements.Add(player);
            }

            player.Name = save.PlayerName;
            player.Health = save.Health;
            player.Class = save.PlayerClass;

            LevelData.player = player;
            LevelData.currentSaveId = save.Id;
            LevelData.MessageLog = LoadMessageLog(save.Id);

            LoadEnemies(save);
            LoadWalls(save);

            GameLoop.turns = save.Turns;
        }

        private void LoadEnemies(SavedGameModel save)
        {
            foreach (var e in save.Enemies)
            {
                Enemy enemy;

                if (e.Type == "Rat")
                {
                    enemy = new Rat();
                }
                else if (e.Type == "Snake")
                {
                    enemy = new Snake();
                }
                else
                {
                    enemy = null;
                }

                if (enemy != null)
                {
                    enemy.Position = new Position(e.Position.X, e.Position.Y);
                    enemy.Health = e.Health;
                    LevelData.Elements.Add(enemy);
                }
            }
        }

        private void LoadWalls(SavedGameModel save)
        {
            foreach (var w in save.Walls)
            {
                Wall wall = new Wall();
                wall.Position = new Position(w.Position.X, w.Position.Y);
                wall.isDiscovered = w.Discovered;
                LevelData.Elements.Add(wall);
            }
        }

        public List<PlayerClassModel> GetAllPlayerClasses()
        {
            return PlayerClasses.Find(_ => true).ToList();
        }

        public void CreateNewGame(Player player, string playerName, string playerClass)
        {
            player.Name = playerName;
            player.Class = playerClass;

            var save = new SavedGameModel
            {
                PlayerName = playerName,
                PlayerClass = playerClass,
                Health = 100,
                Position = new Position(player.Position.X, player.Position.Y),
                Turns = 0,
                IsAlive = true,
                Enemies = new List<EnemyState>(),
                Walls = new List<WallState>()
            };

            SavedGames.InsertOne(save);
            LevelData.currentSaveId = save.Id;
            LevelData.MessageLog = new List<string>();   
        }

        public void UpdatePlayerName(ObjectId id, string newName)
        {
            var filter = Builders<SavedGameModel>.Filter.Eq(s => s.Id, id);
            var update = Builders<SavedGameModel>.Update.Set(s => s.PlayerName, newName);

            SavedGames.UpdateOne(filter, update);
        }

        public void DeleteSave(ObjectId id)
        {
            var filter = Builders<SavedGameModel>.Filter.Eq(s => s.Id, id);
            SavedGames.DeleteOne(filter);
        }

        public void AddMessage(ObjectId saveId, string message)
        {
            var filter = Builders<MessageLogModel>.Filter.Eq(m => m.SaveId, saveId);
            var log = Messages.Find(filter).FirstOrDefault();

            if (log == null)
            {
                log = new MessageLogModel
                {
                    SaveId = saveId,
                    Messages = new()
                };
                Messages.InsertOne(log);
            }

            var update = Builders<MessageLogModel>.Update.Push(m => m.Messages, message);
            Messages.UpdateOne(filter, update);
        }

        public List<string> LoadMessageLog(ObjectId saveId)
        {
            var filter = Builders<MessageLogModel>.Filter.Eq(m => m.SaveId, saveId);
            var log = Messages.Find(filter).FirstOrDefault();

            if (log == null || log.Messages == null)
            {
                return new List<string>();
            }

            return log.Messages;
        }
    }
}
