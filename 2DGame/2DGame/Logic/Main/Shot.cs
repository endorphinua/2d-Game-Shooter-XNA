using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DGame.Logic
{
    class Shot : DrawableGameObject
    {
        /// <summary>
        /// Количество оставшихся "движений"
        /// </summary>
        public int MovesLeft
        { get; set; }

        /// <summary>
        /// Скорость по оси X
        /// </summary>
        public double speedX
        { get; set; }

        /// <summary>
        /// Скорость по оси Y
        /// </summary>
        public double speedY
        { get; set; }

        /// <summary>
        /// Игровая зона
        /// </summary>
        public Rectangle GameArea
        { get; set; }

        /// <summary>
        /// Количество наносимого урона
        /// </summary>
        public int Damage
        { get; set; }

        /// <summary>
        /// Количество миллисекунд в течении которого пуля не наносит урона
        /// </summary>
        public int NonDamageTime = 20;

        /// <summary>
        /// Время нанесения урона
        /// </summary>
        public int DamageTime = 0;

        SpriteBatch spriteBatch;

        /// <summary>
        /// Количество пройденных "шагов"
        /// </summary>
        private int timeElapsed = 0;

        /// <summary>
        /// Время "шага"
        /// </summary>
        private int tickTime;

        public Shot(Game game)
            : base(game)
        { 
            this.Initialize();
            this.LoadContent();
        }

        public Shot(Game game,Point startPosition, int MovesLeft, int Angle)
            : this(game)
        {
            Position = new Rectangle(startPosition.X, startPosition.Y, 5, 5);
            this.Angle = Angle;
            this.MovesLeft = MovesLeft;
        }

        /// <summary>
        /// Инициализация
        /// </summary>
        public override void Initialize()
        {
            Random rand = new Random();
            speedX = 11.5;
            speedY = speedX;
            tickTime = 5;
            //Damage = 10;
            base.Initialize();
        }

        /// <summary>
        /// Загрузка содержимого
        /// </summary>
        protected override void LoadContent()
        {
            Sprite = Game.Content.Load<Texture2D>("shot");
            if (Position.X == 0)
            {
                Position = new Rectangle(50, 50, 5, 5);
            }

            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            base.LoadContent();
        }

        /// <summary>
        /// Обновление
        /// </summary>
        /// <param name="gameTime">Время</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            timeElapsed += gameTime.ElapsedGameTime.Milliseconds;
            if (timeElapsed >= tickTime)
            {
                int X = Position.Location.X - (int)(speedX * Math.Cos(getRadians()));
                int Y = Position.Location.Y - (int)(speedY * Math.Sin(getRadians()));
                Position = new Rectangle(X, Y, 10, 3);
                timeElapsed = 0;
                MovesLeft--;
                if (!GameArea.Contains(Position))
                {
                    this.Dispose();
                }
                speedX -= Parent.rand.Next(100,1000) / Parent.rand.Next(1000,5000);
                if (speedX < 0)
                {
                    this.Dispose();
                }

                if (DamageTime > NonDamageTime)
                {
                    foreach (GameBlock block in Parent.Level.GameObjects)
                    {
                        if (block.Position.Intersects(Position))
                        {
                            block.DamageObject(Damage);
                            this.Dispose();
                            break;
                        }
                    }

                    if (Parent.Player.GameObject.Position.Intersects(Position))
                    {
                        Parent.Player.GameObject.DamageObject(Damage);
                    }

                    foreach (Enemy enemy in Parent.Level.Enemies)
                    {
                        if (enemy.GameObject.Position.Intersects(Position))
                        {
                            enemy.GameObject.DamageObject(Damage);
                            this.Dispose();
                            break;
                        }
                    }
                }
            }
            DamageTime += gameTime.ElapsedGameTime.Milliseconds;
            
        }

        /// <summary>
        /// Отрисовка "выстрела"
        /// </summary>
        /// <param name="gameTime">Время</param>
        public override void Draw(GameTime gameTime)
        {
            getLocalPosition();

            spriteBatch.Begin();
            spriteBatch.Draw(Sprite, new Rectangle(LocalPosition.Location.X + (LocalPosition.Width / 2), LocalPosition.Location.Y + (LocalPosition.Height / 2), LocalPosition.Width, LocalPosition.Height), null, Color.White, getRadians(), new Vector2(Position.Width / 2, Position.Height / 2), SpriteEffects.None, 1);
            spriteBatch.End();
        }

        /// <summary>
        /// Получение угла в радианах
        /// </summary>
        /// <returns>Угол в радианах</returns>
        private float getRadians()
        {
            return (float)(Angle * (Math.PI / 180));
        }
    }
}
