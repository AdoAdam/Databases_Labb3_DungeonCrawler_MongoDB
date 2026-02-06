using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler.Helpers;

public static class Log
{
    public static void Add(string message)
    {
        LevelData.MessageLog.Add(message);

        MongoDBService db = new MongoDBService();
        db.AddMessage(LevelData.currentSaveId, message);
    }
}
