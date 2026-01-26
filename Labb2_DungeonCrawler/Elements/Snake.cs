
namespace Labb2_DungeonCrawler.Elements;

class Snake : Enemy
{
    double lastDistance;

    public Snake()
    {
        Name = "Snake";
        Health = 25;
        AttackDice = new(2, 4, 2);
        DefenceDice = new(1, 6, 5);
        Graphic = 's';
        Color = ConsoleColor.Green;
        GameLoop.NewTurn += Update;
    }

    public override void Update()
    {
        if (InCombat) return;

        Position nextPos = GetNextPosition();

        var levelElement = LevelData.Elements.FirstOrDefault(e => e.Position.X == nextPos.X && e.Position.Y == nextPos.Y);

        if (levelElement != null)
        {
            if (levelElement is Wall || levelElement is Enemy)
            {
                return;
            }
        }

        if (LevelData.player.Position.Distance(Position) <= 2)
        {
            if (LevelData.player.Position.Distance(Position) >= lastDistance) return;

            Console.SetCursorPosition(Position.X, Position.Y);
            Console.Write(' ');
            Position = nextPos;
        }

        lastDistance = LevelData.player.Position.Distance(Position);
    }

    public override Position GetNextPosition()
    {
        Position nextPos = new(Position.X, Position.Y);

        int dx = LevelData.player.Position.X - Position.X;
        int dy = LevelData.player.Position.Y - Position.Y;
        dx = int.Clamp(dx, -1, 1);
        dy = int.Clamp(dy, -1, 1);

        nextPos.X = Position.X - dx;
        nextPos.Y = Position.Y - dy;

        return nextPos;
    }

    public override void Flee(int attackOrder)
    {
        base.Flee(attackOrder);
        SetFleePosition();
        Draw();
    }

    public void SetFleePosition()
    {
        Console.SetCursorPosition(Position.X, Position.Y);
        Console.Write(" ");
        var elementEast = LevelData.Elements.FirstOrDefault(e => e.Position.X == Position.X + 1 && e.Position.Y == Position.Y);
        var elementWest = LevelData.Elements.FirstOrDefault(e => e.Position.X == Position.X - 1 && e.Position.Y == Position.Y);
        var elementNorth = LevelData.Elements.FirstOrDefault(e => e.Position.X == Position.X && e.Position.Y == Position.Y - 1);
        var elementSouth = LevelData.Elements.FirstOrDefault(e => e.Position.X == Position.X && e.Position.Y == Position.Y + 1);

        if (IsPossibleMove(elementEast))
        {
            Position.X += 1;
        }
        else if (IsPossibleMove(elementWest))
        {
            Position.X -= 1;
        }
        else if (IsPossibleMove(elementNorth))
        {
            Position.Y -= 1;
        }
        else if (IsPossibleMove(elementSouth))
        {
            Position.Y += 1;
        }
    }

    private bool IsPossibleMove(LevelElement element)
    {
        if (element is null) return true;

        if (element is Wall || element is Player || element is Enemy)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

}
