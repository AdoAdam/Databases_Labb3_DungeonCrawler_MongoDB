using Labb2_DungeonCrawler.Elements;
using Labb2_DungeonCrawler.Helpers;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace Labb2_DungeonCrawler;

class Player : LevelElement
{
    public string Name { get; set; }
    public string Class {  get; set; }
    public int Health { get; set; }
    public Dice AttackDice { get; set; }
    public Dice DefenceDice { get; set; }
    public int VisionRange { get; private set; }
    public bool isAlive { get; set; } = true;

    public Player(int x, int y)
    {
        Name = "Player";
        Health = 100;
        AttackDice = new(2, 6, 2);
        DefenceDice = new(2, 6, 0);
        Graphic = '@';
        Color = ConsoleColor.Yellow;
        Position = new(x, y);
        VisionRange = 5;
    }

    public void Move(ConsoleKeyInfo keyInfo)
    {
        Position nextPos = GetNextPosition(keyInfo.Key);
        if (nextPos == null) return;
        
        var levelElement = LevelData.Elements.FirstOrDefault(e => e.Position.X == nextPos.X && e.Position.Y == nextPos.Y);

        if (levelElement != null)
        {
            if (levelElement is Wall)
            {
                return;
            }
            else if (levelElement is Enemy enemy)
            {
                StartNewAttackSequence(enemy);
                return;
            }
        }
        else
        {
            ClearCombatText(1);
            ClearCombatText(2);
        }

        ClearLastPosition();
        Position = new(nextPos.X, nextPos.Y);
        GameLoop.NewTurn.Invoke();
        
    }

    private void StartNewAttackSequence(Enemy enemy)
    {
        Attack(enemy, 1);
        if (EnemyStillAlive(enemy))
        {
            GameLoop.NewTurn.Invoke();
            enemy.AttackOrFlee(2);
            return;
        }
        else
        {
            GameLoop.NewTurn.Invoke();
            return;
        }
    }

    private bool EnemyStillAlive(Enemy enemy)
    {
        return LevelData.Elements.Contains(enemy);
    }

    private void ClearLastPosition()
    {
        Console.SetCursorPosition(Position.X, Position.Y);
        Console.Write(" ");
    }

    private Position GetNextPosition(ConsoleKey keyPressed)
    {
        Position nextPos = new();
        if (keyPressed == ConsoleKey.LeftArrow)
        {
            nextPos = new(Position.X - 1, Position.Y);
        }
        else if (keyPressed == ConsoleKey.RightArrow)
        {
            nextPos = new(Position.X + 1, Position.Y);
        }
        else if (keyPressed == ConsoleKey.UpArrow)
        {
            nextPos = new(Position.X, Position.Y - 1);
        }
        else if (keyPressed == ConsoleKey.DownArrow)
        {
            nextPos = new(Position.X, Position.Y + 1);
        }
        
        if (nextPos.X == 0 && nextPos.Y == 0)
        {
            return null;
        }
        else
        {
            return nextPos;
        }
    }
    public void Attack(Enemy enemy, int attackOrder)
    {
        int enemyAttackOrder;
        if (attackOrder == 1)
        {
            enemy.InCombat = true;
            enemyAttackOrder = 2;
        }
        else
        {
            enemyAttackOrder = 1;
        }

        int attack = AttackDice.Throw();
        int defence = enemy.DefenceDice.Throw();

        ClearCombatText(attackOrder);
        SetCombatText(enemy, enemyAttackOrder, attack, defence);
    }

    private void ClearCombatText(int attackOrder)
    {
        Console.SetCursorPosition(0, attackOrder);
        Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
    }

    public void SetCombatText(Enemy enemy, int enemyAttackOrder, int attack, int defence)
    {
        string combatMessage = $"You attacked {enemy.Name} (ATK {attack}, DEF {defence})";
        
        int damage = attack - defence;

        if (enemy.Health - damage <= 0) 
        {
            combatMessage += ", killing it"; enemy.Kill(enemyAttackOrder);
        } 
        else if (damage > 0) 
        {
            combatMessage += $", wounding the {enemy.Name}";
        } 
        else 
        {
            combatMessage += ", but did not manage to make any damage";
        }

        Log.Add(combatMessage);

        Console.ForegroundColor = Color;
        Console.Write($"You (ATK: {AttackDice.ToString()} => {attack})");
        Console.ResetColor();
        Console.Write(" attacked ");
        Console.ForegroundColor = enemy.Color;
        Console.Write($"{enemy.Name} (DEF: {enemy.DefenceDice.ToString()} => {defence})");
        Console.ResetColor();

        if (enemy.Health - damage <= 0)
        {
            Console.Write(", Killing it");
            enemy.Kill(enemyAttackOrder);
        }
        else if (damage > 0)
        {
            Console.Write($", wounding the ");
            Console.ForegroundColor = enemy.Color;
            Console.Write(enemy.Name);
            Console.ResetColor();
            enemy.Health -= damage;
        }
        else if (damage <= 0)
        {
            Console.Write(", but did not manage to make any damage");
        }
    }

    public void Kill()
    {
        isAlive = false;
        Console.Clear();
    }
}
