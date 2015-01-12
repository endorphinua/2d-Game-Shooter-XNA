using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Globalization;

namespace _2DGame.Logic
{
    class GamePanel : DrawableGameComponent
    {
        public Game1 Parent
        { get; set; }

        public Player Player
        { get; set; }

        private Texture2D Sidebar
        { get; set; }

        private Texture2D Pistol
        { get; set; }

        private Texture2D Ak47
        { get; set; }

        private Texture2D Minigun
        { get; set; }

        private Texture2D PistolSidebar
        { get; set; }

        private Texture2D Ak47Sidebar
        { get; set; }

        private Texture2D MinigunSidebar
        { get; set; }

        private Texture2D WinTexture
        { get; set; }

        private Texture2D GameOverTexture
        { get; set; }

        public Color TextColor
        { get; set; }

        public Texture2D WeaponTexture
        { get; set; }

        public Rectangle GameWindow
        { get; set; }

        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        SpriteFont gameOverFont;
        SpriteFont bulletsFont;

        Texture2D point;

        public GamePanel(Game game,Game1 Parent)
            : base(game)
        {
            this.Initialize();
            this.Parent = Parent;
            this.LoadContent();
        }

        public override void Initialize()
        {
            base.Initialize();
            this.TextColor = Color.Yellow;
        }

        protected override void LoadContent()
        {
            Sidebar = Game.Content.Load<Texture2D>("sidebar");
            Pistol = Game.Content.Load<Texture2D>("pistol");
            Ak47 = Game.Content.Load<Texture2D>("ak47");
            Minigun = Game.Content.Load<Texture2D>("minigun");
            WeaponTexture = Game.Content.Load<Texture2D>("weaponTexture");
            spriteFont = Game.Content.Load<SpriteFont>("Script MT Bold");
            gameOverFont = Game.Content.Load<SpriteFont>("GameOver");
            bulletsFont = Game.Content.Load<SpriteFont>("BulletsFont");
            point = Game.Content.Load<Texture2D>("point");
            if(Parent != null && Parent.fullScreen)
            {
                WinTexture = Game.Content.Load<Texture2D>("win_full");
                GameOverTexture = Game.Content.Load<Texture2D>("game_over_full");
            }
            else
            {
                WinTexture = Game.Content.Load<Texture2D>("win");
                GameOverTexture = Game.Content.Load<Texture2D>("game_over");
            }
            PistolSidebar = Game.Content.Load<Texture2D>("pistol_sidebar");
            Ak47Sidebar = Game.Content.Load<Texture2D>("ak47_sidebar");
            MinigunSidebar = Game.Content.Load<Texture2D>("minigun_sidebar");

            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            spriteBatch.Begin();
            if (Player.GameObject.LiveCount <= 0)
            {
                spriteBatch.Draw(GameOverTexture, new Vector2(0, 0), Color.White);
                spriteBatch.DrawString(gameOverFont, "Points: " + Player.Points.ToString(), new Vector2((int)(GameWindow.Width / 2) - 130, (int)(GameWindow.Height / 2) + 50), TextColor);
                Parent.isRunning = false;
            }
            else
                if (Parent.Level.Enemies.Count == 0)
                {
                    spriteBatch.Draw(WinTexture, new Vector2(0, 0), Color.White);
                    spriteBatch.DrawString(gameOverFont, "Points: " + Player.Points.ToString(), new Vector2((int)(GameWindow.Width / 2) - 130, (int)(GameWindow.Height / 2) + 50), TextColor);
                    Parent.isRunning = false;
                }
                else
                {
                    // левая панель
                    spriteBatch.Draw(Sidebar, new Vector2(GameWindow.Width - 140, 0), Color.White);

                    spriteBatch.Draw(WeaponTexture, new Vector2(GameWindow.Width - 120, 22), Color.White);
                    // отрисовка оружия
                    switch (Player.Weapon.selectedWeapon.Name)
                    {
                        case "Pistol":
                            spriteBatch.Draw(Pistol, new Vector2(GameWindow.Width - 120, 20), Color.White);
                            break;
                        case "AK 47":
                            spriteBatch.Draw(Ak47, new Vector2(GameWindow.Width - 120, 20), Color.White);
                            break;
                        case "Minigun":
                            spriteBatch.Draw(Minigun, new Vector2(GameWindow.Width - 120, 20), Color.White);
                            break;
                    }
                    foreach (Weapon weapon in Player.Weapon.Weapons)
                    {
                        switch (weapon.Name)
                        {
                            case "Pistol":
                                spriteBatch.Draw(PistolSidebar, new Vector2(GameWindow.Width - 140, 310), Color.White);
                                break;
                            case "AK 47":
                                spriteBatch.Draw(Ak47Sidebar, new Vector2(GameWindow.Width - 140, 360), Color.White);
                                break;
                            case "Minigun":
                                spriteBatch.Draw(Minigun, new Vector2(GameWindow.Width - 120, 20), Color.White);
                                break;         
                        }
                    }
                    String time = gameTime.TotalGameTime.Minutes.ToString() + ":" + gameTime.TotalGameTime.Seconds.ToString();
                    spriteBatch.DrawString(bulletsFont, Player.Weapon.Bullets.ToString(), new Vector2(GameWindow.Width - 90, 132), Color.White);
                    spriteBatch.DrawString(bulletsFont, time, new Vector2(GameWindow.Width - 90, 167), TextColor);
                    spriteBatch.DrawString(bulletsFont, Player.GameObject.LiveCount.ToString(), new Vector2(GameWindow.Width - 90, 202), TextColor);
                    spriteBatch.DrawString(bulletsFont, Player.Points.ToString(), new Vector2(GameWindow.Width - 90, 237), TextColor);
                    //spriteBatch.DrawString(bulletsFont, Player.GameObject.Position.X.ToString() + " " + Player.GameObject.Position.Y.ToString(), new Vector2(GameWindow.Width - 120, GameWindow.Height - 40), TextColor);
                }
            spriteBatch.End();
        }
    }
}
