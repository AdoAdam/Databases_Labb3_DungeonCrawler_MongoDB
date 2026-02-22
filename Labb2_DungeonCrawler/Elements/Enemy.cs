
using Labb2_DungeonCrawler.Helpers;
using static System.Net.Mime.MediaTypeNames;

namespace Labb2_DungeonCrawler.Elements
{
    abstract class Enemy : LevelElement
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public Dice AttackDice { get; set; }
        public Dice DefenceDice { get; set; }
        public bool InCombat { get; set; }

        public abstract void Update();
        public abstract Position GetNextPosition();

        public override void Draw()
        {
            if (LevelData.player.Position.Distance(Position) < LevelData.player.VisionRange)
            {
                base.Draw();
            }
            else
            {
                Console.SetCursorPosition(Position.X, Position.Y);
                Console.Write(' ');
            }
        }

        public void AttackOrFlee(int attackOrder)
        {
            Console.SetCursorPosition(0, attackOrder);
            Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");

            bool isFleeSuccessful = IsFleeSuccessful();

            if (isFleeSuccessful && attackOrder == 2)
            {
                InCombat = false;
                Flee(attackOrder);
            }
            else if (!isFleeSuccessful || attackOrder == 1)
            {
                int attack = AttackDice.Throw();
                int defence = LevelData.player.DefenceDice.Throw();

                CombatText(attack, defence);
            }

            InCombat = false;
        }

        public bool IsFleeSuccessful()
        {
            Random random = new Random();
            int rand = random.Next(0, 2);
            bool flee = rand == 1 ? true : false;
            return flee;
        }

        public virtual void Flee(int attackOrder)
        {
            Console.SetCursorPosition(0, attackOrder);
            Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
            Console.ForegroundColor = Color;
            Console.Write($"The {Name} Fled");
        }

        public void CombatText(int attack, int defence)
        {
            string combatMessage = $"The {Name} attacked you (ATK {attack}, DEF {defence})";

            int damage = attack - defence;

            if (damage > 0)
            {
                combatMessage += ", wounding you";
            }
            else if (damage <= 0)
            {
                combatMessage += ", but did not manage to make any damage";
            }

            Log.Add(combatMessage);

            Console.ForegroundColor = Color;
            Console.Write($"The {Name} (ATK: {AttackDice.ToString()} => {attack})");
            Console.ResetColor();
            Console.Write(" attacked ");
            Console.ForegroundColor = LevelData.player.Color;
            Console.Write($"you (DEF: {LevelData.player.DefenceDice.ToString()} => {defence})");
            Console.ResetColor();

            if (damage > 0)
            {
                Console.Write(", wounding you");
                LevelData.player.Health -= damage;
                if (LevelData.player.Health <= 0)
                    LevelData.player.Kill();
            }
            else if (damage <= 0)
            {
                Console.Write(", but did not manage to make any damage");
            }
        }

        public void Kill(int attackOrder)
        {
            
            Console.SetCursorPosition(Position.X, Position.Y);
            Console.Write(' ');
            
            if (attackOrder == 2)
            {
                Console.SetCursorPosition(0, LevelData.mapHeight + attackOrder);
                Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
            }

            GameLoop.DrawAll -= Draw;
            GameLoop.NewTurn -= Draw;
            LevelData.Elements.Remove(this);
            //GameLoop.NewTurn.Invoke();
        }
    }
}
