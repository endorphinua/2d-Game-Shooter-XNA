using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DGame.Logic
{
    /// <summary>
    /// Класс представляет свобой набор оружия для игрока или врага игрока
    /// </summary>
    public class WeaponSet
    {
        /// <summary>
        /// Игрок или враг
        /// </summary>
        public Player Player
        { get; set; }

        /// <summary>
        /// Набор оружия
        /// </summary>
        public List<Weapon> Weapons
        { get; set; }

        /// <summary>
        /// Индекс оружия выбранного в данный момент
        /// </summary>
        private int weaponIndex;

        /// <summary>
        /// Выбранное оружие в данный момент
        /// </summary>
        public Weapon selectedWeapon
        {
            get
            {
                return Weapons[weaponIndex];
            }
        }

        /// <summary>
        /// Количество патронов
        /// </summary>
        private int bullets = 0;

        /// <summary>
        /// Количество патронов
        /// </summary>
        public int Bullets
        { get { return this.bullets; } }

        public WeaponSet()
        {
            this.Weapons = new List<Weapon>() { };
            weaponIndex = 0;
        }

        /// <summary>
        /// Смена оружия
        /// </summary>
        public void changeWeapon()
        {
            weaponIndex += 1;
            if (weaponIndex >= Weapons.Count)
            {
                weaponIndex = 0;
            }
        }

        /// <summary>
        /// Выполнение выстрела игроком или врагом игрока
        /// </summary>
        public void makeShot()
        {
            if (Bullets > 0 && Player.shotTime >= selectedWeapon.MillisecondsBetweenShot && Player.GameObject.CanMove)
            {
                Shot shot = new Shot(Player.Parent, Player.GameObject.ShotPoint, 80, Player.GameObject.Angle);
                shot.GameArea = Player.Parent.Level.Position;
                shot.DrawOrder = 2;
                shot.UpdateOrder = 2;
                shot.Parent = Player.Parent;
                shot.Damage = selectedWeapon.Damage;

                Player.Parent.Components.Add(shot);
                if (!Player.Parent.mute)
                {
                    Player.Parent.sound.PlayCue("aShot");
                }
                
                bullets--;
                Player.shotTime = 0;
            }
        }

        /// <summary>
        /// Задать стартовую позицию "вылета" пули
        /// </summary>
        public void setShotPosition()
        {
            Player.GameObject.ShotPoint = new Point(Player.GameObject.Position.Center.X - 10, Player.GameObject.Position.Center.Y - 3);
        }

        /// <summary>
        /// Добавление пуль
        /// </summary>
        /// <param name="Count">Количество пуль</param>
        public void addBullets(int Count)
        {
            this.bullets += Count;
        }
    }
}
