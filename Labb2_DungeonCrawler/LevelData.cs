using Labb2_DungeonCrawler.Elements;
using MongoDB.Bson;

namespace Labb2_DungeonCrawler
{
    static class LevelData
    {
        public static List<LevelElement> Elements { get { return _elements; } }
        private static List<LevelElement> _elements = new();

        public static List<string> MessageLog = new();
        public static int CurrentSaveLogIndex = 0;

        private static string map;
        public static int mapHeight;

        public static Player player;
        public static ObjectId currentSaveId;
        public static void LoadMap(string fileName)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                map = reader.ReadToEnd();

                int yPosition = 3;
                int xPosition = 0;

                for (int i = 0; i < map.Length; i++) 
                {   
                    if (map[i] == '#')
                    {
                        Wall wall = new();
                        wall.Position = new(xPosition, yPosition);
                        _elements.Add(wall);
                    }
                    else if (map[i] == 'r')
                    {
                        Rat rat = new Rat();
                        rat.Position = new(xPosition, yPosition);
                        _elements.Add(rat);
                        
                    }
                    else if (map[i] == 's')
                    {
                        Snake snake = new Snake();
                        snake.Position = new(xPosition, yPosition);
                        _elements.Add(snake);
                    }
                    else if (map[i] == '@')
                    {
                        Player p = new Player(xPosition, yPosition);
                        _elements.Add(p);
                        player = p;
                    }
                    xPosition++;
                    if (map[i] == '\n')
                    {
                        yPosition++;
                        xPosition = 0;
                    }
                }
                mapHeight = yPosition;
            }
        }
    }
}
