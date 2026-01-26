
namespace Labb2_DungeonCrawler.Elements;

class Rat : Enemy
{
    int movesTested = 0;

    public Rat()
    {
        Name = "Rat";
        Health = 10;
        AttackDice = new(1, 6, 3);
        DefenceDice = new(1, 6, 1);
        Graphic = 'r';
        Color = ConsoleColor.Red;
        GameLoop.NewTurn += Update;
    }

    public override void Update()
    {
        if (InCombat) return;

        Position nextPos = GetNextPosition();

        var levelElement = LevelData.Elements.FirstOrDefault(e => e.Position.X == nextPos.X && e.Position.Y == nextPos.Y);
        if (levelElement != null)
        {
            if (levelElement is Enemy || levelElement is Wall) 
            {
                movesTested++;
                if (movesTested < 4)
                    Update();
                return;
            }
            else if (levelElement is Player p)
            {
                AttackOrFlee(1);
                if (LevelData.Elements.Contains(p))
                    p.Attack(this, 2);
                return;
            }
        }

        Console.SetCursorPosition(Position.X, Position.Y);
        Console.Write(" ");
        Position = new(nextPos.X, nextPos.Y);
        movesTested = 0;
    }

    public override Position GetNextPosition()
    {
        Position nextPos = new(Position.X, Position.Y);
        Random randomDirection = new Random();
        int direction = randomDirection.Next(0, 4);
        if (direction == 0)
        {
            nextPos = new(Position.X - 1, Position.Y);
        }
        else if (direction == 1)
        {
            nextPos = new(Position.X + 1, Position.Y);
        }
        else if (direction == 2)
        {
            nextPos = new(Position.X, Position.Y - 1);
        }
        else if (direction == 3)
        {
            nextPos = new(Position.X, Position.Y + 1);
        }
        return nextPos;
    }

    public override void Flee(int attackOrder)
    {
        base.Flee(attackOrder);
        Update();
        Draw();
    }
}
