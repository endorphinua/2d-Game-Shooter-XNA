using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DGame.Logic.Agents
{
    public class Environment
    {
        public Graph.Graph GRAPH
        {
            get;
            set;
        }
        /// <summary>
        /// Список агентов "обитающих" в данной среде
        /// </summary>
        public List<Agent> AGENTS
        { get; set; }

        /// <summary>
        /// Игровой уровень
        /// </summary>
        public Level LEVEL
        { get; set; }

        public Environment(List<Agent> AGENTS, Level LEVEL)
        {
            this.LEVEL = LEVEL;
            this.AGENTS = AGENTS;
            this.GRAPH = new Graph.Graph();
        }

        public Environment()
        { }
    }
}
