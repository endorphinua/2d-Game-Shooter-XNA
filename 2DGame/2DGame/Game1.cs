using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using _2DGame.Logic;

namespace _2DGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        AudioEngine audio;
        public SoundBank sound;
        WaveBank wave;
        public Cue mainTheme;
        GamePanel gamePanel;
        Rectangle gameArea;
        public String drawType = "all";
        bool godMode = false;
        int prevLives;
        int prevBullets;

        /// <summary>
        /// Генератор случайных чисел
        /// </summary>
        public Random rand = new Random();

        /// <summary>
        /// Игровой уровень
        /// </summary>
        public Level Level
        { get; set; }

        /// <summary>
        /// Камера
        /// </summary>
        public Point Camera;

        /// <summary>
        /// Запускать игру в полноэкранном режиме или нет
        /// </summary>
        public bool fullScreen = false;

        /// <summary>
        /// Игрок
        /// </summary>
        public Player Player;

        /// <summary>
        /// Время между выстрелами
        /// </summary>
        int shotTime;

        /// <summary>
        /// Время смены оружия
        /// </summary>
        int changeWeaponTime;

        /// <summary>
        /// ВРемя выключения звука
        /// </summary>
        int muteTime;

        /// <summary>
        /// Игровое окно
        /// </summary>
        public Rectangle GameWindow;

        /// <summary>
        /// Запускать игру без звука или нет
        /// </summary>
        public bool mute = true;

        /// <summary>
        /// Запущена игра или нет
        /// </summary>
        public bool isRunning = true;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.GameWindow = new Rectangle(0, 0, 600, 480);
            Camera = new Point(GameWindow.X, GameWindow.Y);
            if (!fullScreen)
            {
                graphics.PreferredBackBufferWidth = GameWindow.Width;
                graphics.PreferredBackBufferHeight = GameWindow.Height;
            }
            else
            {
                graphics.IsFullScreen = true;
            }
            Console.WriteLine(graphics.PreferredBackBufferWidth + " " + graphics.PreferredBackBufferHeight);
        }

        /// <summary>
        /// Инициализация
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();

            this.Window.Title = "Шутер";

            if (fullScreen)
            {
                this.GameWindow = new Rectangle(0, 0, this.GraphicsDevice.DisplayMode.Width, this.GraphicsDevice.DisplayMode.Height);
            }
            Level = new Level(this);
            Level.Camera = Camera;
            Level.Parent = this;
            Level.loadLevel("level1");
            Level.DrawOrder = 1;
            Level.UpdateOrder = 1;
            gameArea = Level.Position;
            Level.GameArea = gameArea;

            Player = new Player(this);
            Player.setStartPosition(new Point(25, 125));
            Player.Weapon.addBullets(100);
            prevLives = Player.GameObject.LiveCount;
            prevBullets = Player.Weapon.Bullets;
            Player.GameObject.Parent = this;
            Player.GameObject.GameArea = gameArea;
            Player.GameObject.DrawOrder = 3;
            Player.GameObject.UpdateOrder = 3;
            Player.Parent = this;
            Player.GameObject.LiveCount = 250;

            gamePanel = new GamePanel(this,this);
            gamePanel.Player = Player;
            gamePanel.TextColor = Color.White;
            gamePanel.GameWindow = GameWindow;
            gamePanel.DrawOrder = 4;
            gamePanel.UpdateOrder = 4;
            gamePanel.Parent = this;

            Components.Add(Level);
            Components.Add(Player.GameObject);
            Components.Add(gamePanel);

            audio = new AudioEngine("Content\\audio.xgs");
            wave = new WaveBank(audio, "Content\\Wave Bank.xwb");
            sound = new SoundBank(audio, "Content\\Sound Bank.xsb");
            mainTheme = sound.GetCue("mainTheme");
            if (!mute)
            {
                mainTheme.Play();
            }
        }

        /// <summary>
        /// Сдвинуть камеру
        /// </summary>
        public void moveCamera()
        {
            Camera.X = Player.GameObject.Position.Center.X - GameWindow.Width / 2;
            Camera.Y = Player.GameObject.Position.Center.Y - GameWindow.Height / 2;
            if (Camera.X < 0)
                Camera.X = 0;
            if (Camera.Y < 0)
                Camera.Y = 0;
            if (Camera.X > Level.Position.Width - GameWindow.Width + 140)
                Camera.X = Level.Position.Width - GameWindow.Width + 140;
            if (Camera.Y > Level.Position.Height - GameWindow.Height)
                Camera.Y = Level.Position.Height - GameWindow.Height;
        }

        /// <summary>
        /// Загрузка содержимого
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("Segoe UI");
            
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// Выгрузка содержимого
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Обновление
        /// </summary>
        /// <param name="gameTime">Время</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            audio.Update();

            // выход из игры
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                this.Level.Environment.GRAPH.saveGraph("graph1");
                foreach (Enemy e in this.Level.Enemies)
                {
                    e.Die();
                }
                this.Exit();
            }

            if (isRunning)
            {
                // застревание
                if (keyboardState.IsKeyDown(Keys.F10))
                {
                    if (shotTime > Player.Weapon.selectedWeapon.MillisecondsBetweenShot)
                    {
                        Player.setStartPosition(new Point(330, 280));
                        Camera.X = GameWindow.X;
                        Camera.Y = GameWindow.Y;
                        shotTime = 0;
                    }
                }

                // звук
                if (keyboardState.IsKeyDown(Keys.M))
                {
                    changeMute();
                }

                // переключение оружия
                if (keyboardState.IsKeyDown(Keys.R))
                {
                    if (changeWeaponTime > 800)
                    {
                        Player.Weapon.changeWeapon();
                        changeWeaponTime = 0;
                    }
                }

                // поворот влево
                if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
                    Player.GameObject.turnLeft();
                // поворот вправо
                if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
                    Player.GameObject.turnRight();
                // движение вперед
                if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
                    Player.GameObject.Move("forward");
                // движение назад
                if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
                    Player.GameObject.Move("backward");

                // выстрел
                if (keyboardState.IsKeyDown(Keys.Space))
                {
                    makeShot();
                }

                // смена отрисовки
                if (keyboardState.IsKeyDown(Keys.F1))
                {
                    drawType = "all";
                }

                // смена отрисовки
                if (keyboardState.IsKeyDown(Keys.F2))
                {
                    drawType = "1";
                }
                // смена отрисовки
                if (keyboardState.IsKeyDown(Keys.F3))
                {
                    drawType = "2";
                }
                // смена отрисовки
                if (keyboardState.IsKeyDown(Keys.F4))
                {
                    drawType = "3";
                }
                // смена отрисовки
                if (keyboardState.IsKeyDown(Keys.F5))
                {
                    drawType = "4";
                }
                // смена отрисовки
                if (keyboardState.IsKeyDown(Keys.F6))
                {
                    drawType = "5";
                }

                moveCamera();

                changeWeaponTime += gameTime.ElapsedGameTime.Milliseconds;
                muteTime += gameTime.ElapsedGameTime.Milliseconds;
                Player.Update(gameTime);
            }
            base.Update(gameTime);
            if (godMode)
            {
                if (Player.Weapon.Bullets > prevBullets)
                {
                    prevBullets = Player.Weapon.Bullets;
                }
                if (Player.GameObject.LiveCount > prevLives)
                {
                    prevLives = Player.GameObject.LiveCount;
                }
                Player.Weapon.addBullets(prevBullets - Player.Weapon.Bullets);
                Player.GameObject.LiveCount = prevLives;
            }
        }

        /// <summary>
        /// Включение / Выключение звука
        /// </summary>
        public void changeMute()
        {
            if (muteTime > 1000)
            {
                if (!mute)
                {
                    if (mainTheme.IsPlaying)
                    {
                        mainTheme.Pause();
                    }
                    mute = true;
                }
                else
                {
                    if (mainTheme.IsPaused)
                    {
                        mainTheme.Resume();
                    }
                    else
                    {
                        mainTheme.Play();
                    }
                    mute = false;
                }
                muteTime = 0;
            }
        }

        /// <summary>
        /// Выполнения выстрела
        /// </summary>
        public void makeShot()
        {
            Player.Weapon.makeShot();
        }

        /// <summary>
        /// Отрисовка
        /// </summary>
        /// <param name="gameTime">Время</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
