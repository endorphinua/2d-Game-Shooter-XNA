using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DGame.Logic
{
    public class Weapon
    {
        /// <summary>
        /// "Имя" оружия
        /// </summary>
        public String Name
        { get; set; }

        /// <summary>
        /// Количество миллисекунд между выстрелом
        /// </summary>
        public int MillisecondsBetweenShot
        { get; set; }

        /// <summary>
        /// Количество наносимого урона
        /// </summary>
        public int Damage
        { get; set; }

        public Weapon(String Name, int MillisecondsBetweenShot, int Damage)
        {
            this.Name = Name;
            this.MillisecondsBetweenShot = MillisecondsBetweenShot;
            this.Damage = Damage;
        }
    }
}
