using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading;
using System.Xml;

namespace _2DGame.Logic.Agents
{
    public class GameAgent : CommunicativeAgent
    {
        /// <summary>
        /// "Владелец" агента
        /// </summary>
        public object Owner
        { get; set; }

        public AgentLevel LEVEL
        { get; set; }

        /// <summary>
        /// Область "зрения"
        /// </summary>
        public Rectangle LOOK_AREA
        { get; set; }

        /// <summary>
        /// Поток отправки сообщений
        /// </summary>
        private Thread SENDER_THREAD;
        private bool run = true;

        /// <summary>
        /// "Оглянутся" вокруг себя
        /// </summary>
        public void Look()
        { }
        
        /// <summary>
        /// Отправка сообщения
        /// </summary>
        public override void SendMessage()
        { }

        /// <summary>
        /// Получение сообщения
        /// </summary>
        public override void RecieveMessage()
        { }

        /// <summary>
        /// Отправка сообщений в отдельном потоке
        /// </summary>
        public override void Sender()
        {
            while (run)
            {
                Player owner = (Player)Owner;
                string MSG = String.Empty;
                if (owner != null)
                {
                    updateLookArea();
                    List<GameBlock> Blocks = getBlocksNearAgent(owner);
                    List<GameBonus> Bonuses = getBonusesNearAgent(owner);
                    addObjectsToLevel(Blocks, Bonuses);
                    bool HasPlayer = checkPlayerNearAgent(owner);
                    MSG = EncodeMessage(Blocks, Bonuses);

                }
                try
                {
                    try
                    {
                        foreach (GameAgent agent in ENVIRONMENT.AGENTS)
                        {
                            agent.DecodeMessage(MSG);
                        }
                    }
                    catch (NullReferenceException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine(e.Message);
                }
                Thread.Sleep(5000+rand.Next(-500,500));
            }
        }

        public void updateLookArea()
        {
            Enemy owner = (Enemy)Owner;
            LOOK_AREA = new Rectangle(owner.GameObject.Position.X - LOOK_AREA.Width / 2, owner.GameObject.Position.Y - LOOK_AREA.Height / 2, LOOK_AREA.Width, LOOK_AREA.Height);
        }

        /// <summary>
        /// Зашифровать сообщение в нужном формате
        /// </summary>
        public override string EncodeMessage()
        {
            return String.Empty;
        }

        public string EncodeMessage(List<GameBlock> Blocks, List<GameBonus> Bonuses)
        {
            string XML = string.Empty;
            XmlDocument xml = new XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", null));
            XmlNode root = xml.AppendChild(xml.CreateElement("objects"));
            foreach (GameBlock obj in Blocks)
            {
                XmlNode node = root.AppendChild(xml.CreateElement("object"));
                XmlAttribute oType = node.Attributes.Append(xml.CreateAttribute("type"));
                oType.Value = "block";
                XmlAttribute oX = node.Attributes.Append(xml.CreateAttribute("x"));
                oX.Value = obj.Position.X.ToString();
                XmlAttribute oY = node.Attributes.Append(xml.CreateAttribute("y"));
                oY.Value = obj.Position.Y.ToString();
                XmlAttribute oWidth = node.Attributes.Append(xml.CreateAttribute("width"));
                oWidth.Value = obj.Position.Width.ToString();
                XmlAttribute oHeight = node.Attributes.Append(xml.CreateAttribute("height"));
                oHeight.Value = obj.Position.Height.ToString();
                XmlAttribute oAngle = node.Attributes.Append(xml.CreateAttribute("angle"));
                oAngle.Value = obj.Angle.ToString();
                XmlAttribute oSprite = node.Attributes.Append(xml.CreateAttribute("sprite"));
                oSprite.Value = obj.spriteName.ToString();
            }

            foreach (GameBonus obj in Bonuses)
            {
                XmlNode node = root.AppendChild(xml.CreateElement("object"));
                XmlAttribute oType = node.Attributes.Append(xml.CreateAttribute("type"));
                oType.Value = obj.Type;
                XmlAttribute oX = node.Attributes.Append(xml.CreateAttribute("x"));
                oX.Value = obj.Position.X.ToString();
                XmlAttribute oY = node.Attributes.Append(xml.CreateAttribute("y"));
                oY.Value = obj.Position.Y.ToString();
                XmlAttribute oWidth = node.Attributes.Append(xml.CreateAttribute("width"));
                oWidth.Value = obj.Position.Width.ToString();
                XmlAttribute oHeight = node.Attributes.Append(xml.CreateAttribute("height"));
                oHeight.Value = obj.Position.Height.ToString();
                XmlAttribute oAngle = node.Attributes.Append(xml.CreateAttribute("angle"));
                oAngle.Value = obj.Angle.ToString();
                XmlAttribute oValue = node.Attributes.Append(xml.CreateAttribute("value"));
                oValue.Value = obj.Value.ToString();
                XmlAttribute oSprite = node.Attributes.Append(xml.CreateAttribute("sprite"));
                oSprite.Value = obj.spriteName.ToString();
            }
            XML = xml.InnerXml;
            return XML;
        }

        public void addObjectsToLevel(List<GameBlock> Blocks, List<GameBonus> Bonuses)
        {
            foreach (GameBlock obj in Blocks)
            {
                bool exist = false;
                foreach (GameBlock block in LEVEL.Blocks)
                {
                    if(block.Position.X == obj.Position.X && block.Position.Y == obj.Position.Y && block.Position.Width == obj.Position.Width && block.Position.Height == obj.Position.Height)
                    {
                        exist = true;
                        break;
                    }
                }
                if (!exist)
                {
                    LEVEL.Blocks.Add(obj);
                }
            }

            foreach (GameBonus obj in Bonuses)
            {
                bool exist = false;
                foreach (GameBlock block in LEVEL.Blocks)
                {
                    if (block.Position.X == obj.Position.X && block.Position.Y == obj.Position.Y && block.Position.Width == obj.Position.Width && block.Position.Height == obj.Position.Height)
                    {
                        exist = true;
                        break;
                    }
                }
                if (!exist)
                {
                    LEVEL.Bonuses.Add(obj);
                }
            }
        }

        /// <summary>
        /// Расшифровать сообщение
        /// </summary>
        public override void DecodeMessage()
        { }

        public void DecodeMessage(string MSG)
        {
            XmlDocument xml = new XmlDocument();
            List<GameBlock> Blocks = new List<GameBlock>() { };
            List<GameBonus> Bonuses = new List<GameBonus>() { };
            try
            {
                xml.LoadXml(MSG);

                foreach (XmlNode obj in xml.DocumentElement.ChildNodes)
                {
                    int X = 0, Y = 0, Width = 0, Height = 0, Angle = 0;
                    string Type = String.Empty, Value = String.Empty, Sprite = String.Empty;
                    foreach (XmlAttribute attr in obj.Attributes)
                    {
                        switch (attr.Name)
                        {
                            case "type":
                                Type = attr.Value;
                                break;
                            case "x":
                                X = Int32.Parse(attr.Value);
                                break;
                            case "y":
                                Y = Int32.Parse(attr.Value);
                                break;
                            case "width":
                                Width = Int32.Parse(attr.Value);
                                break;
                            case "height":
                                Height = Int32.Parse(attr.Value);
                                break;
                            case "angle":
                                Angle = Int32.Parse(attr.Value);
                                break;
                            case "value":
                                Value = attr.Value;
                                break;
                            case "sprite":
                                Sprite = attr.Value;
                                break;
                        }
                    }

                    Enemy enemy = (Enemy)Owner;
                    switch (Type)
                    {
                        case "block":
                            GameBlock bl = new GameBlock((Game)enemy.Parent, Sprite, new Rectangle(X, Y, Width, Height));
                            bl.Angle = Angle;
                            bl.GameArea = enemy.GameObject.GameArea;
                            bl.Parent = enemy.Parent;
                            bl.getLocalPosition();
                            Blocks.Add(bl);
                            break;
                        case "weapon":
                            GameBonus bo = new GameBonus((Game)enemy.Parent, enemy.Parent, Sprite, new Rectangle(X, Y, Width, Height), Type, Int32.Parse(Value));
                            bo.Angle = Angle;
                            bo.GameArea = enemy.GameObject.GameArea;
                            bo.Parent = enemy.Parent;
                            bo.getLocalPosition();
                            Bonuses.Add(bo);
                            break;
                        case "bullet":
                            GameBonus bu = new GameBonus((Game)enemy.Parent, enemy.Parent, Sprite, new Rectangle(X, Y, Width, Height), Type, Int32.Parse(Value));
                            bu.Angle = Angle;
                            bu.GameArea = enemy.GameObject.GameArea;
                            bu.Parent = enemy.Parent;
                            bu.getLocalPosition();
                            Bonuses.Add(bu);
                            break;
                    }
                }

            }
            catch (XmlException e)
            {
                Console.WriteLine(e.Message);
            }
            addObjectsToLevel(Blocks, Bonuses);
        }

        Random rand;
        int prevDistance = int.MaxValue;

        public GameAgent()
            :this(0,"NULL")
        {}

        public GameAgent(int ID, string AID)
            :base(ID, AID)
        {
            this.SENDER_THREAD = new Thread(Sender);
            this.SENDER_THREAD.Start();
            LOOK_AREA = new Rectangle(0, 0, 300, 300);
            LEVEL = new AgentLevel();
            rand = new Random();
        }

        /// <summary>
        /// Удаление потока отправки сообщений
        /// </summary>
        public void Die()
        {
            this.run = false;
            Console.WriteLine("<"+AID + "> I`m die");
            this.SENDER_THREAD.Abort();
        }

        private GameBonus findNearentBonus()
        {
            GameBonus Bonus = null;
            int min = Int32.MaxValue;
            Enemy e = (Enemy) Owner;
            foreach (GameBonus bonus in LEVEL.Bonuses)
            {
                int dis = e.GameObject.getDistanceBetweenObjects(bonus);
                if (min > dis)
                {
                    min = dis;
                    Bonus = bonus;
                }
            }
            return Bonus;
        }

        /// <summary>
        /// Принятие решения
        /// </summary>
        public void makeDecision()
        {
            Enemy e = (Enemy)Owner;
            syncObjects();
            updateLookArea();
            if (e.Movements.Count == 0)
            {
                // смена решения если достигли предыдущей цели
                GameBonus nearestBonus = findNearentBonus();
                if (nearestBonus != null)
                {
                    prevDistance = e.GameObject.getDistanceBetweenObjects(nearestBonus);
                    e.setNewTarget(nearestBonus.Position);
                    LEVEL.Bonuses.Remove(nearestBonus);
                    e.Target = "Bonus";
                }
                else
                {
                    e.generateNewTarget();
                    prevDistance = e.GameObject.getDistanceBetweenObjects(e.Movements[e.Movements.Count - 1].Location);
                    e.Target = "Random";
                }

            }
            else
            { 
                // Попытка найти более удачное решение
                int distToBonus = int.MaxValue;
                GameBonus nearestBonus = findNearentBonus();
                if (nearestBonus != null)
                {
                    distToBonus = e.GameObject.getDistanceBetweenObjects(nearestBonus);
                }
                int distToPlayer = e.GameObject.getDistanceBetweenObjects(ENVIRONMENT.LEVEL.Parent.Player.GameObject);
                if (prevDistance > distToBonus || prevDistance > distToPlayer)
                {
                    if (distToPlayer < 500)
                    {
                        e.setNewTarget(ENVIRONMENT.LEVEL.Parent.Player.GameObject.Position);
                        prevDistance = e.GameObject.getDistanceBetweenObjects(ENVIRONMENT.LEVEL.Parent.Player.GameObject);
                        e.Target = "Player";
                    }
                    else
                    if (distToPlayer < distToBonus && distToPlayer < 1000)
                    {
                        e.setNewTarget(ENVIRONMENT.LEVEL.Parent.Player.GameObject.Position);
                        prevDistance = e.GameObject.getDistanceBetweenObjects(ENVIRONMENT.LEVEL.Parent.Player.GameObject);
                        e.Target = "Player";
                    }
                    else
                    if (distToBonus > 1000)
                    {
                        e.generateNewTarget();
                        prevDistance = e.GameObject.getDistanceBetweenObjects(e.Movements[e.Movements.Count - 1].Location);
                    }
                    else
                    {
                        if (nearestBonus != null)
                        {
                            e.setNewTarget(nearestBonus.Position);
                            prevDistance = e.GameObject.getDistanceBetweenObjects(nearestBonus);
                            LEVEL.Bonuses.Remove(nearestBonus);
                            e.Target = "Bonus";
                        }
                        else
                        {
                            e.generateNewTarget();
                            prevDistance = e.GameObject.getDistanceBetweenObjects(e.Movements[e.Movements.Count - 1].Location);
                        }
                    }
                    
                }
            }
        }

        public void syncObjects()
        { 
            List<GameBonus> removeBonuses = new List<GameBonus>(){};
            List<GameBlock> removeBlocks = new List<GameBlock>(){};
            try
            {
                foreach (GameBonus bonus in LEVEL.Bonuses)
                {
                    bool exist = false;
                    foreach (GameBonus srcBonus in ENVIRONMENT.LEVEL.GameBonuses)
                    {
                        if (bonus.Equals(srcBonus))
                        {
                            exist = true;
                            break;
                        }
                    }
                    if (!exist)
                    {
                        removeBonuses.Add(bonus);
                    }
                }
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
            }
            try
            {
                foreach (GameBlock block in LEVEL.Blocks)
                {
                    bool exist = false;
                    foreach (GameBlock srcBlock in ENVIRONMENT.LEVEL.GameObjects)
                    {
                        if (block.Equals(srcBlock))
                        {
                            exist = true;
                            break;
                        }
                    }
                    if (!exist)
                    {
                        removeBlocks.Add(block);
                    }
                }
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
            }

            foreach (GameBonus bonus in removeBonuses)
            {
                LEVEL.Bonuses.Remove(bonus);
            }

            foreach (GameBlock bonus in removeBlocks)
            {
                LEVEL.Blocks.Remove(bonus);
            }
        }

        public List<GameBlock> getBlocksNearAgent(Player owner)
        {
            List<GameBlock> Blocks = new List<GameBlock>(){};
            foreach (GameBlock obj in ENVIRONMENT.LEVEL.GameObjects)
            {
                if (obj.Position.Intersects(LOOK_AREA) || LOOK_AREA.Contains(obj.Position))
                {
                    Blocks.Add(obj);
                }
            }
            return Blocks;
        }

        public List<GameBonus> getBonusesNearAgent(Player owner)
        { 
            List<GameBonus> Bonuses = new List<GameBonus>(){};
            foreach (GameBonus obj in ENVIRONMENT.LEVEL.GameBonuses)
            {
                if (obj.Position.Intersects(LOOK_AREA) || LOOK_AREA.Contains(obj.Position))
                {
                    Bonuses.Add(obj);
                }
            }
            return Bonuses;
        }

        public bool checkPlayerNearAgent(Player owner)
        {
            if (owner.GameObject.Position.Intersects(LOOK_AREA) || LOOK_AREA.Contains(owner.GameObject.Position))
            {
                return true;
            }
            return false;
        }

        public void Update()
        {

        }
    }
}
