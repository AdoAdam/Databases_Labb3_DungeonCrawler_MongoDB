
using Labb2_DungeonCrawler;
using Labb2_DungeonCrawler.Elements;
using System.Runtime.CompilerServices;

Menu();
StartGame();

static void Menu()
{
    bool inMenu = true;
    MongoDBService _db = new();

    while (inMenu)
    {
        Console.Clear();
        Console.WriteLine("1. New Game");
        Console.WriteLine("2. Load Game");

        var key = Console.ReadKey();

        if (key.Key == ConsoleKey.D2)
        {
            var savedGames = _db.GetAllSavedGames();

            if (savedGames.Count == 0)
            {
                Console.WriteLine("No saved games found");
                return;
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Select a saved game:");

                for (int i = 0; i < savedGames.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {savedGames[i].PlayerName} (HP: {savedGames[i].Health}, Turn: {savedGames[i].Turns})");
                }

                Console.WriteLine("Enter number to play: (or go back with ESC) ");

                ConsoleKeyInfo choiceKey = Console.ReadKey(true);

                if (choiceKey.Key == ConsoleKey.Escape) { break; }

                if (!char.IsDigit(choiceKey.KeyChar))
                {
                    Console.WriteLine("Invalid input. Press any key to try again");
                    Console.ReadKey(true);
                    continue;
                }

                int choice = int.Parse(choiceKey.KeyChar.ToString());

                if (choice < 1 || choice > savedGames.Count)
                {
                    Console.WriteLine("Invalid choice. Press any key to try again");
                    Console.ReadKey(true);
                    continue;
                }


                Console.Clear();

                var selectedSave = savedGames[choice - 1];
                _db.LoadSavedGame(selectedSave);
                inMenu = false;
                return;
            }
        }
    }
    StartGame();
}

static void StartGame()
{
    Console.CursorVisible = false;
    //LevelData.Load("Levels/Level1.txt");
    GameLoop.NewTurn += GameLoop.AddTurn;
    foreach (LevelElement element in LevelData.Elements)
    {
        GameLoop.NewTurn += element.Draw;
        element.Draw();
    }

    GameLoop.Play();
}
