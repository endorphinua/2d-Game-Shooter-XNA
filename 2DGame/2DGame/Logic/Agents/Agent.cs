using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DGame.Logic.Agents
{
    public class Agent
    {
        /// <summary>
        /// Идентификатор агента
        /// </summary>
        public int ID
        { get; set; }

        /// <summary>
        /// Имя aгента
        /// </summary>
        public string AID
        { get; set; }

        /// <summary>
        /// Среда в которой агент обитает
        /// </summary>
        public Environment ENVIRONMENT
        { get; set; }

        public Agent()
        {
            this.ID = 0;
            this.AID = "NULL";
            this.ENVIRONMENT = new Environment();
        }

        public Agent(int ID, string AID)
        {
            this.ID = ID;
            this.AID = AID;
        }

        public Agent(int ID, string AID, Environment ENVIRONMENT)
            :this(ID,AID)
        {
            this.ENVIRONMENT = ENVIRONMENT;
        }
    }
}
