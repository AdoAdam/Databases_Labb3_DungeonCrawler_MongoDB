
using Labb2_DungeonCrawler;
using Labb2_DungeonCrawler.Elements;

bool isNewGame = false;


Menu(ref isNewGame);

AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

StartGame(isNewGame);

static void Menu(ref bool IsNewGame)
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
                Console.Clear();
                Console.WriteLine("No saved games found, press any button to return");
                Console.ReadKey();
                continue;
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

                var selectedSave = savedGames[choice - 1];

                _db.LoadSavedGame(selectedSave);

                IsNewGame = false;
                inMenu = false;
                return;
            }
        }
        else if (key.Key == ConsoleKey.D1)
        {
            var classes = _db.GetAllPlayerClasses();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Choose your class: (or go back with ESC)");

                for (int i = 0; i < classes.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {classes[i].Name}");
                }

                Console.WriteLine("Enter number");

                ConsoleKeyInfo choiceKey = Console.ReadKey(true);

                if (choiceKey.Key == ConsoleKey.Escape) { break; }

                if (!char.IsDigit(choiceKey.KeyChar))
                {
                    Console.WriteLine("Invalid input. Press any key to try again");
                    Console.ReadKey(true);
                    continue;
                }

                int choice = int.Parse(choiceKey.KeyChar.ToString());

                if (choice < 1 || choice > classes.Count)
                {
                    Console.WriteLine("Invalid choice. Press any key to try again");
                    Console.ReadKey(true);
                    continue;
                }

                var chosenClass = classes[choice - 1];

                Console.Write("Enter character name: ");
                string playerName = Console.ReadLine();

                LevelData.Elements.Clear();
                LevelData.LoadMap("Levels/Level1.txt");
                Console.WriteLine();

                _db.CreateNewGame(LevelData.player, playerName, chosenClass.Name);

                IsNewGame = true;
                inMenu = false;
                return;
            }
        }
    }
}

static void StartGame(bool IsNewGame)
{
    Console.Clear();
    Console.CursorVisible = false;

    GameLoop.NewTurn += GameLoop.AddTurn;
    foreach (LevelElement element in LevelData.Elements)
    {
        GameLoop.NewTurn += element.Draw;
        element.Draw();
    }

    GameLoop.Play();
}

static void OnProcessExit(object? sender, EventArgs e)
{
    MongoDBService _db = new();
    _db.SaveGame(LevelData.player, GameLoop.turns);
}
