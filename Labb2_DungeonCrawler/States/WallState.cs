using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler.States
{
    internal class WallState
    {
        public Position Position {  get; set; }
        public bool Discovered { get; set; }
    }
}
