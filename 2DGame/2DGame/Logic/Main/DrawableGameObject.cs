using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace _2DGame.Logic
{
    public class DrawableGameObject : GameObject
    {
        /// <summary>
        /// Срайт
        /// </summary>
        SpriteBatch spriteBatch;

        /// <summary>
        /// Спрайт
        /// </summary>
        public Texture2D Sprite
        { get; set; }

        /// <summary>
        /// Имя спрайта
        /// </summary>
        public String spriteName;

        public DrawableGameObject(Game game)
            : base(game)
        {
            this.Initialize();
        }

        /// <summary>
        /// Загрузка содержимого
        /// </summary>
        protected override void LoadContent()
        {
            try
            {
                spriteBatch = new SpriteBatch(Game.GraphicsDevice);
                base.LoadContent();
            }
            catch (OutOfMemoryException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Отрисовка игрового объекта
        /// </summary>
        /// <param name="gameTime">Время</param>
        public override void Draw(GameTime gameTime)
        {
            getLocalPosition();
            try
            {
                spriteBatch.Begin();
                spriteBatch.Draw(Sprite, new Rectangle(LocalPosition.Location.X + (LocalPosition.Width / 2), LocalPosition.Location.Y + (LocalPosition.Height / 2), LocalPosition.Width, LocalPosition.Height), null, Color.White, getRadians(), new Vector2(Position.Width / 2, Position.Height / 2), SpriteEffects.None, 1);
                spriteBatch.End();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
            }
            base.Draw(gameTime);
        }

        /// <summary>
        /// "Уничтожение" игрового объекта
        /// </summary>
        public void Destroy()
        {
            this.Dispose();
        }

        /// <summary>
        /// Смена спрайта
        /// </summary>
        /// <param name="spriteName">Имя картинки</param>
        public void changeSprite(String spriteName)
        {
            if (spriteName != null)
            {
                this.spriteName = spriteName;
                Sprite = Game.Content.Load<Texture2D>(spriteName);
            }
        }
    }
}
