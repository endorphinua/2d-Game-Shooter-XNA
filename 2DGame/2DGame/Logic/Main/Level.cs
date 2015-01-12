using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DGame.Logic
{
    public class Level : GameObject
    {
        public List<GameBlock> GameObjects
        { get; set; }

        public List<GameArea> GameAreas
        { get; set; }

        public List<GameBonus> GameBonuses
        { get; set; }

        public List<Enemy> Enemies
        { get; set; }

        public Texture2D Background
        { get; set; }

        public Texture2D Point
        { get; set; }

        public Texture2D PointRed
        { get; set; }

        public SpriteFont Font
        { get; set; }

        public Level(Game game)
            : base(game)
        {
            this.Initialize();
        }

        public Point Camera
        { get; set; }

        int updateTime = 5;
        int lastUpdateTime = 0;

        public override void Initialize()
        {
            GameObjects = new List<GameBlock>() { };
            GameAreas = new List<GameArea>() { };
            GameBonuses = new List<GameBonus>() { };
            Enemies = new List<Enemy>() { };
            base.Initialize();
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            Point = Game.Content.Load<Texture2D>("vertex");
            PointRed = Game.Content.Load<Texture2D>("vertex_red");
            Font = Game.Content.Load<SpriteFont>("Script MT Bold");
        }

        public Agents.Environment Environment
        { get; set; }

        public void loadLevel(String Path)
        {
            GameObjects.Clear();
            XmlDocument xml = new XmlDocument();
            xml.Load("Levels\\" + Path + ".xml");

            int width = Int32.Parse(xml.DocumentElement.Attributes["width"].Value);
            int height = Int32.Parse(xml.DocumentElement.Attributes["height"].Value);
            SpeedX = Double.Parse(xml.DocumentElement.Attributes["speed-x"].Value);
            SpeedY = Double.Parse(xml.DocumentElement.Attributes["speed-y"].Value);
            this.GameArea = new Rectangle(0, 0, width, height);
            Environment = new Agents.Environment(new List<Agents.Agent>() { },this);

            this.Environment.GRAPH.loadGraph("graph1");

            int AgentID = 0;

            Position = new Rectangle(0, 0, width, height);
            
            foreach (XmlNode gameObject in xml.DocumentElement.ChildNodes)
            {
                String Type = gameObject.Attributes["type"].Value;
                this.Background = Game.Content.Load<Texture2D>(Path + "\\bg");
                switch (Type)
                {
                    case "gameBlock":
                        GameBlock block = new GameBlock(Game, Path + "\\" + gameObject.Attributes["sprite"].Value, new Rectangle(Int32.Parse(gameObject.Attributes["x"].Value), Int32.Parse(gameObject.Attributes["y"].Value), Int32.Parse(gameObject.Attributes["width"].Value), Int32.Parse(gameObject.Attributes["height"].Value)));
                        if (gameObject.Attributes["lives-count"] != null)
                        {
                            block.LiveCount = Int32.Parse(gameObject.Attributes["lives-count"].Value);
                        }
                        else
                        {
                            block.LiveCount = 10;
                        }
                        block.Parent = this.Parent;
                        GameObjects.Add(block);
                        break;
                    case "gameArea":
                        int Width_ = Int32.Parse(gameObject.Attributes["width"].Value);
                        int Height_ = Int32.Parse(gameObject.Attributes["height"].Value);
                        int X_ = Int32.Parse(gameObject.Attributes["x"].Value);
                        int Y_ = Int32.Parse(gameObject.Attributes["y"].Value);
                        double speedX = Double.Parse(gameObject.Attributes["speed-x"].Value);
                        double speedY = Double.Parse(gameObject.Attributes["speed-y"].Value);
                        GameArea area = new GameArea(Game, new Rectangle(X_, Y_, Width_, Height_), speedX, speedY);
                        GameAreas.Add(area);
                        break;
                    case "gameBonus":
                        String Sprite_ = Path + "\\" + gameObject.Attributes["sprite"].Value;
                        int Width2 = Int32.Parse(gameObject.Attributes["width"].Value);
                        int Height2 = Int32.Parse(gameObject.Attributes["height"].Value);
                        int X2 = Int32.Parse(gameObject.Attributes["x"].Value);
                        int Y2 = Int32.Parse(gameObject.Attributes["y"].Value);
                        String BonusType = gameObject.Attributes["bonus-type"].Value;
                        double BonusValue = Double.Parse(gameObject.Attributes["bonus-value"].Value);
                        GameBonus bonus = new GameBonus(Game, Parent, Sprite_, new Rectangle(X2, Y2, Width2, Height2),BonusType,BonusValue);
                        GameBonuses.Add(bonus);
                        break;
                    case "gameEnemy":
                        String Sprite2 = Path + "\\" + gameObject.Attributes["sprite"].Value;
                        String algorithm = gameObject.Attributes["algorithm"].Value;
                        int Width3 = Int32.Parse(gameObject.Attributes["width"].Value);
                        int Height3 = Int32.Parse(gameObject.Attributes["height"].Value);
                        int X3 = Int32.Parse(gameObject.Attributes["x"].Value);
                        int Y3 = Int32.Parse(gameObject.Attributes["y"].Value);
                        Enemy enemy = new Enemy(Game, Parent,new Rectangle(X3, Y3, Width3, Height3), Sprite2, gameObject.Attributes["enemy-type"].Value.ToString());
                        enemy.GameObject.GameArea = this.Position;
                        enemy.Weapon.addBullets(100);
                        enemy.GameObject.DrawOrder = 3;
                        enemy.GameObject.UpdateOrder = 3;
                        enemy.Agent = new Agents.GameAgent(AgentID, "agent-" + gameObject.Attributes["enemy-type"].Value.ToString() + "-" + AgentID.ToString());
                        enemy.Agent.ENVIRONMENT = Environment;
                        enemy.Agent.Owner = enemy;
                        enemy.setSearchAlgorithm(algorithm);
                        Environment.AGENTS.Add(enemy.Agent);
                        Enemies.Add(enemy);
                        AgentID++;
                        break;
                }
            }
        }

        SpriteBatch spriteBatch;

        public override void Update(GameTime gameTime)
        {
            if (lastUpdateTime >= updateTime && Parent.Player.GameObject.LiveCount > 0)
            {
                foreach (Enemy enemy in Enemies)
                {
                    enemy.Update(gameTime);
                }
                lastUpdateTime = 0;
            }
            lastUpdateTime += gameTime.ElapsedGameTime.Milliseconds;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            getLocalPosition();
            spriteBatch.Begin();
            spriteBatch.Draw(Background, LocalPosition, Color.White);
            spriteBatch.End();

            //DrawGraph(gameTime);

            if (Parent.drawType == "all")
            {
                foreach (GameBlock gameObject in GameObjects)
                {
                    gameObject.Draw(gameTime);
                }

                foreach (GameBonus gameBonus in GameBonuses)
                {
                    gameBonus.Draw(gameTime);
                }

                foreach (Enemy enemy in Enemies)
                {
                    enemy.Draw(gameTime);
                }
            }
            else
            {
                AgentLevel Level = new AgentLevel();

                switch (Parent.drawType)
                {
                    case "1":
                        Level = Enemies[0].Agent.LEVEL;
                        break;
                    case "2":
                        Level = Enemies[1].Agent.LEVEL;
                        break;
                    case "3":
                        Level = Enemies[2].Agent.LEVEL;
                        break;
                    case "4":
                        Level = Enemies[3].Agent.LEVEL;
                        break;
                    case "5":
                        Level = Enemies[4].Agent.LEVEL;
                        break;
                }

                foreach (GameBlock gameObject in Level.Blocks)
                {
                    gameObject.Draw(gameTime);
                }

                foreach (GameBonus gameBonus in Level.Bonuses)
                {
                    gameBonus.Draw(gameTime);
                }

                foreach (Enemy enemy in Enemies)
                {
                    enemy.Draw(gameTime);
                }
            }
            
           
            base.Draw(gameTime);
        }

        private void DrawGraph(GameTime gameTime)
        {
            spriteBatch.Begin();
            int i = 1;
            foreach (Graph.Vertex vertex in this.Environment.GRAPH.Vertices)
            {
                bool exist = false;
                if (Enemies.Count > 0)
                {
                    foreach (Enemy enemy in Enemies)
                    {
                        if (enemy.Movements != null)
                        {
                            foreach (Graph.Vertex v in enemy.Movements)
                            {
                                if (v.Equals(vertex))
                                {
                                    exist = true;
                                }
                            }
                        }
                    }
                }
                if (!exist)
                {
                    spriteBatch.Draw(Point, new Rectangle(vertex.Location.X - Parent.Camera.X, vertex.Location.Y - Parent.Camera.Y, 20, 20), Color.White);
                }
                else
                    spriteBatch.Draw(PointRed, new Rectangle(vertex.Location.X - Parent.Camera.X, vertex.Location.Y - Parent.Camera.Y, 20, 20), Color.White);
                //spriteBatch.DrawString(Font, vertex.Value.ToString(), new Vector2((vertex.Location.X - Parent.Camera.X) + 4, (vertex.Location.Y - Parent.Camera.Y) + 4), Color.White);
                spriteBatch.DrawString(Font, i.ToString(), new Vector2((vertex.Location.X - Parent.Camera.X) + 4, (vertex.Location.Y - Parent.Camera.Y) + 4), Color.White);
                i++;
            }
            spriteBatch.End();
        }
    }
}
