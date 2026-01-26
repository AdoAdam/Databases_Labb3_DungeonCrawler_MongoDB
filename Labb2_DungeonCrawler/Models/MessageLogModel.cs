using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler.Models
{
    internal class MessageLogModel
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string PlayerName { get; set; }
        public List<string> Messages { get; set; }
    }
}
