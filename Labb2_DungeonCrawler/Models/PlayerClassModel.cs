using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
