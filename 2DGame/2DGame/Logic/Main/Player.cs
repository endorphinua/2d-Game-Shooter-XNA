using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace _2DGame.Logic
{
    public class Player
    {
        /// <summary>
        /// Ссылка на объект с игрой
        /// </summary>
        public Game1 Parent
        { get; set; }

        /// <summary>
        /// Набранные очки в течении игры
        /// </summary>
        public int Points
        { get; set; }

        /// <summary>
        /// Время выстрела
        /// </summary>
        public int shotTime = 0;

        /// <summary>
        /// Отрисовываемый игровой объект
        /// </summary>
        public DrawableGameObject GameObject
        { get; set; }

        /// <summary>
        /// Набор оружия у игрока
        /// </summary>
        public WeaponSet Weapon
        { get; set; }

        public Player(Game game)
        {
            this.GameObject = new DrawableGameObject(game);
            this.GameObject.Player = this;
            this.Initialize();
            this.LoadContent();
            this.Points = 0;
        }

        /// <summary>
        /// Задать начальную позицию игрока
        /// </summary>
        /// <param name="position">Позиция</param>
        public void setStartPosition(Point position)
        {
            GameObject.Position = new Rectangle(position.X, position.Y, GameObject.Position.Width, GameObject.Position.Height);
            Weapon.setShotPosition();
        }

        /// <summary>
        /// Сброс игрока на начальную позицию
        /// </summary>
        public void Reset()
        {
            GameObject.Position = new Rectangle(50, 50, GameObject.Position.Width, GameObject.Position.Height);
            GameObject.Angle = 180;
        }

        /// <summary>
        /// Инициализация игрока
        /// </summary>
        public void Initialize()
        {
            this.Weapon = new WeaponSet();
            GameObject.setSpeed(2.5, 2.5);
            Weapon.Weapons.Add(new Weapon("Pistol", 500, 10));
            Weapon.Player = this;
        }

        public String baseSpriteName;

        /// <summary>
        /// Загрузка содержимого
        /// </summary>
        protected void LoadContent()
        {
            GameObject.changeSprite("player");
            baseSpriteName = "player";
            GameObject.Position = new Rectangle(50, 50, GameObject.Sprite.Width, GameObject.Sprite.Height);
            Weapon.setShotPosition();
        }

        /// <summary>
        /// Обновление
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            shotTime += gameTime.ElapsedGameTime.Milliseconds;
            Weapon.setShotPosition();
        }

        /// <summary>
        /// Отрисовка игрока
        /// </summary>
        /// <param name="gameTime">Время</param>
        public void Draw(GameTime gameTime)
        {
            if (GameObject.LiveCount < 100 && GameObject.LiveCount > 75)
            {
                if (GameObject.spriteName != baseSpriteName + "_damaged1")
                {
                    GameObject.spriteName = baseSpriteName + "_damaged1";
                }
            }
            if (GameObject.LiveCount < 75 && GameObject.LiveCount > 50)
            {
                if (GameObject.spriteName != baseSpriteName + "_damaged2")
                {
                    GameObject.spriteName = baseSpriteName + "_damaged2";
                }
            }
            if (GameObject.LiveCount < 50 && GameObject.LiveCount > 0)
            {
                if (GameObject.spriteName != baseSpriteName + "_damaged3")
                {
                    GameObject.spriteName = baseSpriteName + "_damaged3";
                }
            }
            GameObject.changeSprite(GameObject.spriteName);
            GameObject.getLocalPosition();
            GameObject.Draw(gameTime);
        }

        public override string ToString()
        {
            return "{ Posision: [" + GameObject.Position.ToString() + "], SpeedX: [" + GameObject.SpeedX.ToString() + "] SpeedY: [" + GameObject.SpeedY.ToString() + "] }";
        }
    }
}
