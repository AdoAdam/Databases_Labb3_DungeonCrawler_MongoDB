
namespace Labb2_DungeonCrawler;

class GameLoop
{
    public static Action? NewTurn;
    public static int turns = 0;
    private static MongoDBService _db;
    public static void Play()
    {
        _db = new MongoDBService();

        while (LevelData.player.isAlive)
        {
            var key = Console.ReadKey();

            if (key.Key == ConsoleKey.F5)
            {
                _db.SaveGame(LevelData.player, turns);
            }

            LevelData.player.Move(key);
            Console.SetCursorPosition(0, LevelData.mapHeight);
            Console.ResetColor();
            Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
            Console.WriteLine($"Name: {LevelData.player.Name} the {LevelData.player.Class} - Health: {LevelData.player.Health}/100 - Turn: {turns}");
        }

        GameOver();
    }

    public static void AddTurn()
    {
        turns++;
    }

    public static void GameOver()
    {
        Console.Clear();
        Console.WriteLine("You Lost");
    }
}
