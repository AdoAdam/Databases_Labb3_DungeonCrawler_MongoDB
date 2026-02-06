
using Labb2_DungeonCrawler;
using Labb2_DungeonCrawler.Elements;
using Labb2_DungeonCrawler.Models;
using System.Transactions;
using System.Xml.Linq;

bool isNewGame = false;

while (true)
{
    Menu(ref isNewGame);

    AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

    StartGame(isNewGame);
}

static void Menu(ref bool IsNewGame)
{
    bool inMenu = true;
    MongoDBService _db = new();

    while (inMenu)
    {
        ShowMainMenu();

        var key = Console.ReadKey();

        if (key.Key == ConsoleKey.D2)
        {
            HandleLoadGame(_db, ref IsNewGame, ref inMenu);
        }
        else if (key.Key == ConsoleKey.D1)
        {
            HandleNewGame(_db, ref IsNewGame, ref inMenu);
        }
    }
}

static void ShowMainMenu()
{
    Console.Clear();
    Console.WriteLine("1. New Game");
    Console.WriteLine("2. Load Game");
}

static void HandleLoadGame(MongoDBService db, ref bool IsNewGame, ref bool inMenu)
{
    var savedGames = db.GetAllSavedGames();

    if (savedGames.Count == 0)
    {
        Console.Clear();
        Console.WriteLine("No saved games found, press any button to return");
        Console.ReadKey();
        return;
    }

    while (true)
    {
        int? choice = ShowLoadGameMenu(savedGames);
        if (choice == null) return;

        var selectedSave = savedGames[choice.Value];

        while (true)
        {
            int option = ShowSaveOptionsMenu();

            if (option == 0) return;
            if (option == -1) continue;

            if (option == 1)
            {
                db.LoadSavedGame(selectedSave);

                IsNewGame = false;
                inMenu = false;
                return;
            }
            else if (option == 2)
            {
                string oldName = selectedSave.PlayerName;

                Console.Clear();
                Console.WriteLine("Enter new player name (or go back with ESC)");

                string? newName = GetPlayerName();
                if (newName == null) continue;

                if (!Confirm($"Are you sure you want to rename '{selectedSave.PlayerName}' to '{newName}'"))
                    continue;

                db.UpdatePlayerName(selectedSave.Id, newName);
                selectedSave.PlayerName = newName;

                Console.WriteLine("Name updated (Press any key to go back)");
                Console.ReadKey(true);
            }
            else if (option == 3)
            {
                if (!Confirm($"Are you sure you want to delete save '{selectedSave.PlayerName} - {selectedSave.PlayerClass} (HP: {selectedSave.Health}, Turn: {selectedSave.Turns}'"))
                    continue;

                db.DeleteSave(selectedSave.Id);
                savedGames.RemoveAt(choice.Value);

                Console.WriteLine("Save deleted (Press any key to go back)");
                Console.ReadKey(true);
                return;
            }
        }

    }
}

static bool Confirm(string message)
{
    Console.WriteLine();
    Console.WriteLine(message + "(y/n)");

    while (true)
    {
        ConsoleKeyInfo key = Console.ReadKey(true);

        if (key.Key == ConsoleKey.Y) return true;
        if (key.Key == ConsoleKey.N) return false;

        Console.WriteLine("Invalid input");
    }
}

static int ShowSaveOptionsMenu()
{
    Console.Clear();
    Console.WriteLine("1. Play");
    Console.WriteLine("2. Edit player name");
    Console.WriteLine("3. Delete save");

    ConsoleKeyInfo key = Console.ReadKey();

    if (key.Key == ConsoleKey.Escape) return 0;
    if (!char.IsDigit(key.KeyChar)) return -1;

    return int.Parse(key.KeyChar.ToString());
}

static int? ShowLoadGameMenu(List<SavedGameModel> savedGames)
{
    Console.Clear();
    Console.WriteLine("Select a saved game:");

    for (int i = 0; i < savedGames.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {savedGames[i].PlayerName} - {savedGames[i].PlayerClass} (HP: {savedGames[i].Health}, Turn: {savedGames[i].Turns})");
    }

    Console.WriteLine("Enter number to play: (or go back with ESC) ");

    ConsoleKeyInfo choiceKey = Console.ReadKey(true);

    if (choiceKey.Key == ConsoleKey.Escape) { return null; }

    if (!char.IsDigit(choiceKey.KeyChar)) { return InvalidChoice(); }

    int choice = int.Parse(choiceKey.KeyChar.ToString()) - 1;

    if (choice < 0 || choice >= savedGames.Count) { InvalidChoice(); }

    return choice;
}

static int? InvalidChoice()
{
    Console.WriteLine("Invalid input. Press any key to try again");
    Console.ReadKey(true);
    return null;
}

static void HandleNewGame(MongoDBService _db, ref bool IsNewGame, ref bool inMenu)
{
    var classes = _db.GetAllPlayerClasses();

    while (true)
    {
        int? choice = ShowClassSelectionMenu(classes);
        if (choice == null) { return; }

        var chosenClass = classes[choice.Value];
        Console.WriteLine("Chosen class: " + chosenClass.Name);

        string? playerName = GetPlayerName();

        if (playerName == null) { return; }

        LevelData.Elements.Clear();
        LevelData.LoadMap("Levels/Level1.txt");

        _db.CreateNewGame(LevelData.player, playerName, chosenClass.Name);

        IsNewGame = true;
        inMenu = false;
        return;
    }
}

static string? GetPlayerName()
{
    Console.Write("Enter character name: ");

    string playerName = "";
    ConsoleKeyInfo key;

    while (true)
    {
        key = Console.ReadKey(true);

        if (key.Key == ConsoleKey.Escape)
        {
            return null;
        }
        else if (key.Key == ConsoleKey.Enter)
        {
            if (playerName.Length == 0)
            {
                playerName = "Player";
            }
            return playerName;
        }
        if (key.Key == ConsoleKey.Backspace && playerName.Length > 0)
        { 
            playerName = playerName[..^1];
            Console.Write("\b \b");
        } 
        else if (!char.IsControl(key.KeyChar))
        {
            playerName += key.KeyChar; Console.Write(key.KeyChar);
        }
    }
}

static int? ShowClassSelectionMenu(List<PlayerClassModel> classes)
{
    Console.Clear();
    Console.WriteLine("Choose your class: (or go back with ESC)");

    for (int i = 0; i < classes.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {classes[i].Name}");
    }

    Console.WriteLine("Enter number");

    ConsoleKeyInfo choiceKey = Console.ReadKey(true);

    if (choiceKey.Key == ConsoleKey.Escape) { return null; }

    if (!char.IsDigit(choiceKey.KeyChar)) { return InvalidChoice(); }

    int choice = int.Parse(choiceKey.KeyChar.ToString()) - 1;

    if (choice < 0 || choice >= classes.Count) { return InvalidChoice(); }

    return choice;
}

static void StartGame(bool IsNewGame)
{
    Console.Clear();
    Console.CursorVisible = false;

    GameLoop.Play();
}

static void OnProcessExit(object? sender, EventArgs e)
{
    MongoDBService _db = new();
    _db.SaveGame(LevelData.player, GameLoop.turns);
}
