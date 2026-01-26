
using Labb2_DungeonCrawler;
using Labb2_DungeonCrawler.Elements;

StartGame();

static void StartGame()
{
    Console.CursorVisible = false;
    LevelData.Load("Levels/Level1.txt");
    GameLoop.NewTurn += GameLoop.AddTurn;
    foreach (LevelElement element in LevelData.Elements)
    {
        GameLoop.NewTurn += element.Draw;
        element.Draw();
    }

    GameLoop.Play();
}
