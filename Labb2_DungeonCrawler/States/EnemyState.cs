using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler.States
{
    internal class EnemyState
    {
        public string Type { get; set; }
        public Position Position { get; set; }
        public int Health { get; set; }
    }
}
