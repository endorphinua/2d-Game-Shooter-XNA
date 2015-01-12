using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace _2DGame.Logic
{
    public class GameObject : DrawableGameComponent
    {
        public bool CanMove
        { get; set; }

        public String ObjectType
        { get; set; }

        /// <summary>
        /// Позиция объекта в игровом мире
        /// </summary>
        public Rectangle Position
        { get; set; }

        /// <summary>
        /// Позиция объекта для отрисовки "Камерой"
        /// </summary>
        public Rectangle LocalPosition
        { get; set; }

        /// <summary>
        /// Игровой объект
        /// </summary>
        public Game1 Parent
        { get; set; }

        /// <summary>
        /// Количество жизней
        /// </summary>
        public int LiveCount
        { get; set; }

        /// <summary>
        /// Игрок
        /// </summary>
        public Player Player
        { get; set; }

        /// <summary>
        /// Угол поворота объекта
        /// </summary>
        public int Angle
        { get; set; }

        /// <summary>
        /// Скорость по оси X
        /// </summary>
        public double SpeedX
        { get; set; }

        /// <summary>
        /// Скорость по оси Y
        /// </summary>
        public double SpeedY
        { get; set; }

        /// <summary>
        /// Позиция для выстрела (Только для Игроков и Врагов)
        /// </summary>
        public Point ShotPoint
        { get; set; }

        /// <summary>
        /// Игровая зона
        /// </summary>
        public Rectangle GameArea
        { get; set; }

        public GameObject(Game game)
            : base(game)
        {
            CanMove = true;
            LiveCount = 100;
        }

        public int getDistanceBetweenObjects(GameObject obj)
        {
            return (int)Math.Sqrt(Math.Pow((double)(this.Position.X - obj.Position.X), 2) + Math.Pow((double)(this.Position.Y - obj.Position.Y), 2));
        }

        public int getDistanceBetweenObjects(Rectangle obj)
        {
            return (int)Math.Sqrt(Math.Pow((double)(this.Position.X - obj.X), 2) + Math.Pow((double)(this.Position.Y - obj.Y), 2));
        }

        public int getDistanceBetweenObjects(Point obj)
        {
            return (int)Math.Sqrt(Math.Pow((double)(this.Position.X - obj.X), 2) + Math.Pow((double)(this.Position.Y - obj.Y), 2));
        }

        /// <summary>
        /// Получить позицию для отрисовки "Камерой"
        /// </summary>
        public void getLocalPosition()
        {
            LocalPosition = new Rectangle(Position.X - Parent.Camera.X, Position.Y - Parent.Camera.Y, Position.Width, Position.Height);
        }

        /// <summary>
        /// Нанести урон объекту
        /// </summary>
        /// <param name="Damage">Количество урона</param>
        public void DamageObject(int Damage)
        {
            LiveCount -= Damage;
            checkAlive();
        }

        /// <summary>
        /// Проверить "жив" ли объект
        /// </summary>
        private void checkAlive()
        {
            if (LiveCount <= 0)
            { 
                if(this.GetType() == typeof(GameBlock))
                {
                    Parent.Level.GameObjects.Remove((GameBlock)this);
                }

                if (this.Player != null && this.Player.GetType() == typeof(Enemy))
                {
                    Enemy e = (Enemy)this.Player;
                    e.Movements[0].Death += 1;
                    e.Parent.Player.Points += 100;
                    e.Die();
                    Parent.Level.Enemies.Remove(e);
                }

                if (this.Player != null && this.Player.GetType() == typeof(Player))
                {
                    Parent.Window.Title = "GAME OVER";
                    Parent.Player.GameObject.Dispose();
                    Parent.isRunning = false;
                }
                CanMove = false;
            }
        }


        /// <summary>
        /// Задать скорость передвижения объекта
        /// </summary>
        /// <param name="speedX">Скорость по оси X</param>
        /// <param name="speedY">Скорость по оси Y</param>
        public void setSpeed(double speedX, double speedY)
        {
            this.SpeedX = speedX;
            this.SpeedY = speedY;
        }

        /// <summary>
        /// Движение объекта в заданном направлении
        /// </summary>
        /// <param name="Direction">Направление (forward|backward)</param>
        public void Move(String Direction)
        {
            if (CanMove)
            {
                if (ObjectType == "Player" || ObjectType == "Enemy")
                {
                    // проверка наличия бонуса
                    this.checkBonus();
                }

                //регулировка скорости
                bool isInArea = false;
                foreach (GameArea area in Parent.Level.GameAreas)
                {
                    if (area.Position.Contains(Position))
                    {
                        SpeedX = area.SpeedX;
                        SpeedY = area.SpeedY;
                        isInArea = true;
                        break;
                    }
                }
                if (!isInArea)
                {
                    SpeedX = Parent.Level.SpeedX;
                    SpeedY = Parent.Level.SpeedY;
                }

                System.Drawing.PointF Value = new System.Drawing.PointF((int)(SpeedX * Math.Cos(getRadians())), (int)(SpeedY * Math.Sin(getRadians())));
                Rectangle prevPosition = Position;

                // проверка ударения по оси x
                switch (Direction)
                {
                    case "forward":
                        Position = new Rectangle((int)(Position.X - Value.X), Position.Y, Position.Width, Position.Height);
                        break;
                    case "backward":
                        Position = new Rectangle((int)(Position.X + Value.X), Position.Y, Position.Width, Position.Height);
                        break;
                }


                foreach (GameBlock block in Parent.Level.GameObjects)
                {
                    if (block.Position.Intersects(Position))
                    {
                        switch (Direction)
                        {
                            case "forward":
                                Position = new Rectangle((int)(Position.X + Value.X), Position.Y, Position.Width, Position.Height);
                                break;
                            case "backward":
                                Position = new Rectangle((int)(Position.X - Value.X), Position.Y, Position.Width, Position.Height);
                                break;
                        }
                        break;
                    }
                }

                // проверка на вхождение в игровую зону
                if (GameArea.Contains(this.Position))
                {

                }
                else
                {
                    switch (Direction)
                    {
                        case "forward":
                            Position = new Rectangle((int)(Position.X + Value.X), Position.Y, Position.Width, Position.Height);
                            break;
                        case "backward":
                            Position = new Rectangle((int)(Position.X - Value.X), Position.Y, Position.Width, Position.Height);
                            break;
                    }
                }

                // проверка ударения по оси y
                switch (Direction)
                {
                    case "forward":
                        Position = new Rectangle(Position.X, (int)(Position.Y - Value.Y), Position.Width, Position.Height);
                        break;
                    case "backward":
                        Position = new Rectangle(Position.X, (int)(Position.Y + Value.Y), Position.Width, Position.Height);
                        break;
                }

                foreach (GameBlock block in Parent.Level.GameObjects)
                {
                    if (block.Position.Intersects(Position))
                    {
                        switch (Direction)
                        {
                            case "forward":
                                Position = new Rectangle(Position.X, (int)(Position.Y + Value.Y), Position.Width, Position.Height);
                                break;
                            case "backward":
                                Position = new Rectangle(Position.X, (int)(Position.Y - Value.Y), Position.Width, Position.Height);
                                break;
                        }
                        break;
                    }
                }
                // проверка на вхождение в игровую зону
                if (GameArea.Contains(this.Position))
                {

                }
                else
                {
                    switch (Direction)
                    {
                        case "forward":
                            Position = new Rectangle(Position.X, (int)(Position.Y + Value.Y), Position.Width, Position.Height);
                            break;
                        case "backward":
                            Position = new Rectangle(Position.X, (int)(Position.Y - Value.Y), Position.Width, Position.Height);
                            break;
                    }
                }
                checkBonus();
            }
        }

        /// <summary>
        /// Проверка на наличие бонусов в позиции в которой находится объект
        /// </summary>
        public void checkBonus()
        {
            if (Player != null)
            {
                foreach (GameBonus bonus in Parent.Level.GameBonuses)
                {
                    if (bonus.Position.Intersects(Position))
                    {
                        switch (bonus.Type)
                        {
                            case "bullet":
                                this.Player.Weapon.addBullets((int)bonus.Value);
                                Player.Points += 5;
                                break;
                            case "weapon":
                                switch ((int)bonus.Value)
                                {
                                    case 2:
                                        this.Player.Weapon.Weapons.Add(new Weapon("AK 47", 200, 25));                                      
                                        break;
                                }
                                Player.Points += 25;
                                break;
                            case "health":
                                this.Player.GameObject.LiveCount += (int)bonus.Value;
                                Player.Points += 10;
                                break;
                        }
                        bonus.Dispose(); 
                        Parent.Level.GameBonuses.Remove(bonus);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Проверка угла
        /// </summary>
        public void checkAngle()
        {
            if (Angle > 360)
            {
                Angle = Math.Abs(Angle) - 360;
            }

            if (Angle < 0)
            {
                Angle = 360 - Math.Abs(Angle);
            }
        }


        /// <summary>
        /// Поворот игрока влево
        /// </summary>
        public void turnLeft()
        {
            if (CanMove)
            {
                Angle -= 2;
                checkAngle();
            }
        }

        /// <summary>
        /// Поворот игрока вправо
        /// </summary>
        public void turnRight()
        {
            if (CanMove)
            {
                Angle += 2;
                checkAngle();
            }
        }

        public override string ToString()
        {
            return "{<type:" + this.GetType().ToString() + ">, <x:" + Position.X.ToString() + ">, <y: " + Position.Y.ToString() + "> <lives: " + LiveCount.ToString() + ">}";
        }


        /// <summary>
        /// Получение угла в радианах
        /// </summary>
        /// <param name="Angle">Угол в градусах</param>
        /// <returns>Угол в радианах</returns>
        public float getRadians(int Angle = int.MaxValue)
        {
            if (Angle == int.MaxValue)
            {
                Angle = this.Angle;
            }
            return (float)(Angle * (Math.PI / 180));
        }
    }
}
