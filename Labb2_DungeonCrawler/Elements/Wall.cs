
namespace Labb2_DungeonCrawler.Elements
{
    class Wall : LevelElement
    {
        public bool isDiscovered = false;
        public ConsoleColor fogColor = ConsoleColor.DarkGray;
        public Wall()
        {
            Graphic = '#';
            Color = ConsoleColor.Gray;
        }

        public override void Draw()
        {
            if (LevelData.player.Position.Distance(Position) < LevelData.player.VisionRange)
                isDiscovered = true;
            if (isDiscovered && LevelData.player.Position.Distance(Position) > LevelData.player.VisionRange)
            {
                Console.SetCursorPosition(Position.X, Position.Y);
                Console.ForegroundColor = fogColor;
                Console.Write(Graphic);
            }
            else if (isDiscovered)
                base.Draw();
        }

    }
}
