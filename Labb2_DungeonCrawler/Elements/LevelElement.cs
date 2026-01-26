
namespace Labb2_DungeonCrawler.Elements
{
    abstract class LevelElement
    {
        public Position Position { get; set; }
        public char Graphic { get; set; }
        public ConsoleColor Color { get; set; }
        
        public virtual void Draw()
        {
            Console.SetCursorPosition(Position.X, Position.Y);
            Console.ForegroundColor = Color;
            Console.Write(Graphic);
        }
    }
}
