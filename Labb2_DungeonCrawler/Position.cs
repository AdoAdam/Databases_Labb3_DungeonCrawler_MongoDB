
namespace Labb2_DungeonCrawler;

class Position
{
    public int X { get; set; }
    public int Y { get; set; }
    public Position()
    {
        X = 0;
        Y = 0;
    }

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }

    public double Distance(Position b)
    {
        int dx = b.X - X;
        int dy = b.Y - Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}
