using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DGame.Logic
{
    public class AgentLevel
    {
        public List<GameBlock> Blocks
        { get; set; }

        public List<GameBonus> Bonuses
        { get; set; }

        public AgentLevel()
        {
            Blocks = new List<GameBlock>() { };
            Bonuses = new List<GameBonus>() { };
        }
    }
}
