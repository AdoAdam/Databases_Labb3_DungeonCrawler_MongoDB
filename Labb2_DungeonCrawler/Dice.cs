
namespace Labb2_DungeonCrawler
{
    internal class Dice
    {
        public int NumberOfDice { get; set; }
        public int SidesPerDice { get; set; }
        public int Modifier { get; set; }
        public Dice(int numberOfDice, int sidesPerDice, int modifier)
        {
            NumberOfDice = numberOfDice;
            SidesPerDice = sidesPerDice;
            Modifier = modifier;
        }

        public int Throw()
        {
            int total = 0;
            Random random = new();
            for (int i = 0; i < NumberOfDice; i++) 
            {
                total += random.Next(1, SidesPerDice + 1);
            }
            return total + Modifier;
        }

        public override string ToString()
        {
            return $"{NumberOfDice}d{SidesPerDice}+{Modifier}";
        }
    }
}
