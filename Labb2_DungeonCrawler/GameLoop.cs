
using Labb2_DungeonCrawler.Elements;

namespace Labb2_DungeonCrawler;

class GameLoop
{
    public static Action? NewTurn;
    public static Action? DrawAll;
    public static int turns = 0;
    private static MongoDBService _db;

    public static void Play()
    {

        NewTurn += AddTurn;
        NewTurn += Draw;

        foreach (LevelElement element in LevelData.Elements)
        {
            DrawAll += element.Draw;
            element.Draw();
        }

        _db = new MongoDBService();

        DrawHUD();

        while (LevelData.player.isAlive)
        {
            var key = Console.ReadKey();

            if (key.Key == ConsoleKey.F1)
            {
                ShowMessageLog();
                DrawHUD();
                continue;
            }

            if (key.Key == ConsoleKey.F5)
            {
                _db.SaveGame(LevelData.player, turns);
                continue;
            }

            LevelData.player.Move(key);
            DrawHUD();
        }

        GameOver(_db);
        return;
    }

    private static void DrawHUD()
    {
        Console.ResetColor();
        Console.SetCursorPosition(0, 0);
        Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
        Console.WriteLine($"Name: {LevelData.player.Name} the {LevelData.player.Class} - Health: {LevelData.player.Health}/100 - Turn: {turns} (F1 - logs, F5 - Save)");
        Console.SetCursorPosition(0, LevelData.mapHeight);
    }

    public static void AddTurn()
    {
        turns++;
    }

    public static void Draw()
    {
        DrawAll?.Invoke();
        Console.SetCursorPosition(0, LevelData.mapHeight + 2);
    }

    public static void ShowMessageLog()
    {
        int index = LevelData.MessageLog.Count - 1;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("========== COMBAT LOGS ========== (Press esc to return)");

            int start = Math.Max(0, index - 20);
            for (int i = start; i <= index; i++)
            {
                Console.WriteLine(LevelData.MessageLog[i]);
            }

            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Escape)
            {
                Console.Clear();
                Draw();
                DrawHUD();
                return;
            }

            if (key.Key == ConsoleKey.UpArrow && index > 0)
                index--;

            if (key.Key == ConsoleKey.DownArrow && index < LevelData.MessageLog.Count - 1)
                index++;
        }
    }

    public static void GameOver(MongoDBService db)
    {
        Console.Clear();
        Console.WriteLine("You Lost. Save deleted (Press any key to return to main menu)");

        db.DeleteSave(LevelData.currentSaveId);
        Console.ReadKey(true);

    }
}
