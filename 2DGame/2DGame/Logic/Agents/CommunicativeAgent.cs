using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DGame.Logic.Agents
{
    public abstract class CommunicativeAgent : Agent
    {
        /// <summary>
        /// Отправка сообщения
        /// </summary>
        public abstract void SendMessage();

        /// <summary>
        /// Получение сообщения
        /// </summary>
        public abstract void RecieveMessage();

        /// <summary>
        /// Отдельный поток для отправки сообщений
        /// </summary>
        public abstract void Sender();

        /// <summary>
        /// "Зашифровка" сообщения
        /// </summary>
        public abstract string EncodeMessage();
        
        /// <summary>
        /// "Расшифровка" сообщения
        /// </summary>
        public abstract void DecodeMessage();

        public CommunicativeAgent(int ID,string AID)
            :base(ID,AID)
        { }
    }
}
