using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using _2DGame.Logic.Agents;
using _2DGame.Logic.Graph;

namespace _2DGame.Logic
{
    public class Enemy : Player
    {
        public String EnemyType
        { get; set; }

        public Enemy(Game game)
            : base(game)
        { }

        public GameAgent Agent
        { get; set; }

        public List<Vertex> Movements
        { get; set; }

        public String Target
        {
            get;
            set;
        }

        public String State = "search";
        public SearchAlgorithm Algorithm;
        public bool inited = false;
        public Texture2D VertexSprite;
        SpriteBatch spriteBatch;

        int mustBeAngle = 0;

        public Enemy(Game game,Game1 Parent, Rectangle Position, String spriteName, String EnemyType)
            : this(game)
        {
            this.Parent = Parent;
            this.GameObject.Parent = Parent;
            GameObject.changeSprite(spriteName);
            GameObject.Position = Position;
            baseSpriteName = spriteName;
            this.EnemyType = EnemyType;
            GameObject.setSpeed(2.5, 2.5);
            Weapon.setShotPosition();
            spriteBatch = new SpriteBatch(Parent.GraphicsDevice);
            this.VertexSprite = Parent.Content.Load<Texture2D>("vertex");
        }

        protected void LoadContent()
        {
            GameObject.Position = new Rectangle(50, 50, 25, 25);
            Weapon.setShotPosition();
        }

        public void setSearchAlgorithm(String name)
        {
            switch (name)
            { 
                case "dijkstra":
                    Algorithm = new DijkstraAlgorithm(Agent.ENVIRONMENT.GRAPH);
                    break;
                case "a-star":
                    Algorithm = new AStarAlgorithm(Agent.ENVIRONMENT.GRAPH);
                    break;
            }
        }

        public void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            bool first = true;

            if (!inited)
            {
                InitSearchAlgorithm(first);
            }

            int shot = GameObject.Parent.rand.Next(100, 10000);
            if(shot%5 == 0 || shot%3 == 0)
            {
                switch (EnemyType)
                { 
                    case "easy":
                        EasyMove(gameTime);
                        break;
                }
            }
            shotTime += gameTime.ElapsedGameTime.Milliseconds;
            this.Agent.makeDecision();
            Agent.Update();
        }

        public void generateNewTarget()
        {
            InitSearchAlgorithm();
        }

        public void setNewTarget(Rectangle Position)
        {
            Vertex currentVertex = new Vertex(new Point(0, 0));
            Vertex targetVertex = new Vertex(new Point(0, 0));

            int mivDistance = int.MaxValue;

            foreach (Vertex v in Algorithm.Graph.Vertices)
            {
                Rectangle rect = new Rectangle(v.Location.X, v.Location.Y, 75, 75);
                if (mivDistance > GameObject.getDistanceBetweenObjects(rect))
                {
                    currentVertex = v;
                    mivDistance = GameObject.getDistanceBetweenObjects(rect);
                }
            }

            mivDistance = int.MaxValue;
            foreach (Vertex v in Algorithm.Graph.Vertices)
            {
                Rectangle rect = new Rectangle(v.Location.X, v.Location.Y, 75, 75);
                if (mivDistance > GameObject.getDistanceBetweenObjects(rect))
                {
                    targetVertex = v;
                    mivDistance = GameObject.getDistanceBetweenObjects(rect);
                }
            }
            if (Movements.Count > 0)
            {
                if (!targetVertex.Equals(Movements[Movements.Count - 1]))
                {
                    Movements = Algorithm.Search(currentVertex, targetVertex);
                }
            }
            else
                Movements = Algorithm.Search(currentVertex, targetVertex);
        }

        public void EasyMove(GameTime gameTime)
        {
            Vertex targetVertex = getTargetVertex();
            if (targetVertex != null)
            {
                switch (State)
                {
                    case "search":
                        setMustBeAngle(targetVertex.Location);
                        turn(gameTime);
                        if (GameObject.Angle == mustBeAngle)
                        {
                            GameObject.Move("forward");
                        }
                        Weapon.setShotPosition();
                        break;
                    case "shot":
                        Rectangle barrier = getBarrier();
                        setMustBeAngle(new Point(barrier.Center.X, barrier.Center.Y));
                        turn(gameTime);
                        if (GameObject.Angle == mustBeAngle)
                        {
                            Weapon.makeShot();
                        }
                        checkShot(gameTime);
                        break;
                    case "shot player":
                        setMustBeAngle(new Point(Parent.Player.GameObject.Position.X,Parent.Player.GameObject.Position.Y));
                        turn(gameTime);
                        Weapon.makeShot();
                        checkShot(gameTime);
                        break;
                }
            }
        }

        private Vertex getCurrentVertex()
        {
            Vertex currentVertex = new Vertex(new Point(0, 0));

            foreach (Vertex v in Agent.ENVIRONMENT.GRAPH.Vertices)
            {
                Rectangle rect = new Rectangle(v.Location.X, v.Location.Y, 3, 3);
                Rectangle eRect = new Rectangle(GameObject.Position.X, GameObject.Position.Y, 3, 3);
                if (eRect.Contains(rect) || rect.Intersects(eRect))
                {
                    currentVertex = v;
                    break;
                }
            }
            return currentVertex;
        }

        private Vertex getTargetVertex()
        {
            Vertex currentVertex = getCurrentVertex();
            if (Movements.Count > 0)
            {
                Vertex targetVertex = Movements[0];

                Rectangle cRect = new Rectangle(currentVertex.Location.X, currentVertex.Location.Y, 1, 1);
                Rectangle tRect = new Rectangle(targetVertex.Location.X, targetVertex.Location.Y, 1, 1);
                if (cRect.Contains(tRect) || tRect.Intersects(cRect))
                {
                    Movements.Remove(targetVertex);
                    targetVertex.Visits += 1;
                    if (Movements.Count > 0)
                    {
                        targetVertex = Movements[0];
                    }
                    else
                        return null;
                }

                return targetVertex;
            }
            else
                return null;
        }

        public int getAngles(double radians)
        { return (int) (radians / (Math.PI / 180)); }

        public void setMustBeAngle(Point Position)
        {
            int X = GameObject.Position.X, Y = GameObject.Position.Y;

            int PlayerX = Position.X, PlayerY = Position.Y;

            
            if (X > PlayerX)
                X -= PlayerX;
            else
                X = PlayerX - X;

            if (Y > PlayerY)
                Y -= PlayerY;
            else
                Y = PlayerY - Y;

            mustBeAngle = getAngles(Math.Atan2(Y, X));
            X = GameObject.Position.X; Y = GameObject.Position.Y;

            if (PlayerX > X && PlayerY < Y)
                mustBeAngle = 180 - mustBeAngle;

            if (PlayerX < X && PlayerY < Y)
                mustBeAngle = 0 + mustBeAngle;

            if (PlayerX > X && PlayerY > Y)
                mustBeAngle = 180 + mustBeAngle;

            if (PlayerX < X && PlayerY > Y)
                mustBeAngle = 360 - mustBeAngle;
        }

        private void checkShot(GameTime gameTime)
        {
            if(checkShotPlayer(gameTime))
            {
                changeState("shot player");
            }
            else
            if (hasBarrier())
            {
                changeState("shot");
            }
            else
            if (hasBarrier("right"))
            {
                GameObject.Angle += 45;
                changeState("shot");
            }
            else
            if (hasBarrier("left"))
            {
                GameObject.Angle -= 45;
                changeState("shot");
            }
            else
                changeState("search");

            Weapon.setShotPosition();
        }

        public bool hasBarrier(String Direction="forward")
        {
            foreach (GameBlock block in Parent.Level.GameObjects)
            {
                Point Value = new Point(0,0);
                int Angle = 0;
                switch(Direction)
                {
                    case "forward":
                        Angle = this.GameObject.Angle;
                        break;
                    case "left":
                        Angle = this.GameObject.Angle - 10;
                        break;
                    case "right":
                        Angle = this.GameObject.Angle + 10;
                        break;
                }
                Value = new Point((int)(GameObject.SpeedX * Math.Cos(GameObject.getRadians(GameObject.Angle))), (int)(GameObject.SpeedY * Math.Sin(GameObject.getRadians(GameObject.Angle))));

                Rectangle NewPosition = new Rectangle((int)(GameObject.Position.X - Value.X), (int)(GameObject.Position.Y - Value.Y), GameObject.Position.Width, GameObject.Position.Height);
                if (block.Position.Intersects(NewPosition))
                {
                    return true;
                }
            }
            return false;
        }

        public Rectangle getBarrier(String Direction = "forward")
        {
            foreach (GameBlock block in Parent.Level.GameObjects)
            {
                Point Value = new Point(0, 0);
                int Angle = 0;
                switch (Direction)
                {
                    case "forward":
                        Angle = this.GameObject.Angle;
                        break;
                    case "left":
                        Angle = this.GameObject.Angle - 10;
                        break;
                    case "right":
                        Angle = this.GameObject.Angle + 10;
                        break;
                }
                Value = new Point((int)(GameObject.SpeedX * Math.Cos(GameObject.getRadians(GameObject.Angle))), (int)(GameObject.SpeedY * Math.Sin(GameObject.getRadians(GameObject.Angle))));

                Rectangle NewPosition = new Rectangle((int)(GameObject.Position.X - Value.X), (int)(GameObject.Position.Y - Value.Y), GameObject.Position.Width, GameObject.Position.Height);
                if (block.Position.Intersects(NewPosition))
                {
                    return block.Position;
                }
            }
            return new Rectangle(0,0,0,0);
        }

        public void changeState(String State)
        {
            this.State = State;
        }

        public bool checkShotPlayer(GameTime gameTime)
        {
            Point Value = new Point(0, 0);
            Rectangle NewPosition = GameObject.Position;
            for (int i = 0; i < 50; i++)
            {
                Value = new Point((int)(11.5 * Math.Cos(GameObject.getRadians(GameObject.Angle))), (int)(11.5 * Math.Sin(GameObject.getRadians(GameObject.Angle))));

                NewPosition = new Rectangle((int)(NewPosition.Center.X - Value.X), (int)(NewPosition.Center.Y - Value.Y), 10, 3);
                if (Parent.Player.GameObject.Position.Intersects(NewPosition))
                {
                    return true;
                }
            }
            return false;
        }

        public void Die()
        {
            this.Agent.Die();
        }

        private void InitSearchAlgorithm(bool first = true)
        {
            Random rand = new Random();

            Vertex random = Algorithm.Graph.Vertices[rand.Next(0, Algorithm.Graph.Vertices.Count - 1)];
            //Vertex random = Algorithm.Graph.Vertices[37];
            Vertex currentVertex = new Vertex(new Point(0, 0));

            foreach (Vertex v in Algorithm.Graph.Vertices)
            {
                Rectangle rect = new Rectangle(v.Location.X, v.Location.Y, 25, 25);
                if (rect.Intersects(GameObject.Position) || GameObject.Position.Contains(rect))
                {
                    currentVertex = v;
                    break;
                }
            }
            Movements = Algorithm.Search(currentVertex, random);
            inited = true;
        }

        /// <summary>
        /// Отрисовка игрока
        /// </summary>
        /// <param name="gameTime">Время</param>
        public void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            if (Movements != null && Movements.Count != 0)
            {
                /*Vertex target = getTargetVertex();
                spriteBatch.Begin();
                foreach (Edge edge in target.Edges)
                {
                    Vertex vertex = edge.ConnectWith;
                    spriteBatch.Draw(VertexSprite, new Rectangle(vertex.Location.X - Parent.Camera.X, vertex.Location.Y - Parent.Camera.Y, 20, 20), Color.White);
                }
                spriteBatch.End();*/
            }
        }

        private void turn(GameTime gameTime)
        {
            for (int i = 0; i < 3; i++)
            {
                if (GameObject.Angle != mustBeAngle)
                {
                    if (GameObject.Angle > mustBeAngle)
                    {
                        GameObject.Angle -= 1;
                    }
                    else
                        GameObject.Angle += 1;
                }
                else
                {
                    checkShot(gameTime);
                    break;
                }
            }
        }
    }
}
