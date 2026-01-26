
namespace Labb2_DungeonCrawler;

class GameLoop
{
    public static Action? NewTurn;
    public static int turns = 0;
    public static void Play()
    {
        while (LevelData.player.isAlive)
        {
            var key = Console.ReadKey();

            LevelData.player.Move(key);
            Console.SetCursorPosition(0, LevelData.mapHeight);
            Console.ResetColor();
            Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
            Console.WriteLine($"Name: {LevelData.player.Name} - Health: {LevelData.player.Health}/100 - Turn: {turns}");
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
