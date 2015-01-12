using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace _2DGame.Logic.Graph
{
    /// <summary>
    /// Граф
    /// </summary>
    public class Graph
    {
        /// <summary>
        /// Вершины графа
        /// </summary>
        public List<Vertex> Vertices
        { get { return this._vertices; } }

        private List<Vertex> _vertices;

        public Graph()
        {
            this._vertices = new List<Vertex>() { };
        }

        /// <summary>
        /// Добавить вершину графа
        /// </summary>
        /// <param name="Vertex"></param>
        public void addVertex(Vertex Vertex)
        {
            this._vertices.Add(Vertex);
        }

        /// <summary>
        /// Добавить вершину графа
        /// </summary>
        /// <param name="Location"></param>
        public void addVertex(Point Location)
        {
            this._vertices.Add(new Vertex(Location));
        }

        public void loadGraph(String Path)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load("Levels\\" + Path + ".xml");

            //Загрузка всех вершин
            foreach (XmlNode vertex in xml.DocumentElement.ChildNodes)
            {
                int X = 0, Y=0, ID=0, death = 0, visits = 0;
                foreach (XmlAttribute attr in vertex.Attributes)
                {
                    switch (attr.Name)
                    { 
                        case "x":
                            X = Int32.Parse(attr.Value);
                            break;
                        case "y":
                            Y = Int32.Parse(attr.Value);
                            break;
                        case "death":
                            death = Int32.Parse(attr.Value);
                            break;
                        case "visits":
                            visits = Int32.Parse(attr.Value);
                            break;
                    }
                }
                Vertex v = new Vertex(new Point(X, Y));
                v.Death = death;
                v.Visits = visits;
                Vertices.Add(v);
            }

            foreach (XmlNode vertex in xml.DocumentElement.ChildNodes)
            {
                int ID = Int32.Parse(vertex.Attributes["id"].Value)-1;
                foreach (XmlNode edge in vertex.ChildNodes)
                {
                    int With = Int32.Parse(edge.Attributes["with"].Value)-1;
                    Vertices[ID].addConnection(Vertices[With]);
                }
            }
        }

        public void saveGraph(String Path)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load("Levels\\" + Path + ".xml");

            //Загрузка всех вершин

            foreach (XmlNode vertex in xml.DocumentElement.ChildNodes)
            {
                int ID = Int32.Parse(vertex.Attributes["id"].Value) - 1;
                XmlAttribute death = vertex.Attributes["death"];
                death.Value = Vertices[ID].Death.ToString();
                vertex.Attributes.Append(death);
                XmlAttribute visits = vertex.Attributes["visits"];
                visits.Value = Vertices[ID].Visits.ToString();
                vertex.Attributes.Append(visits);
            }
            xml.Save("Levels\\" + Path + ".xml");
        }

        /// <summary>
        /// Сгенерировать граф
        /// </summary>
        /// <param name="Level"></param>
        public void generateGraph(Level Level)
        {
            Random rand = new Random();
            Rectangle GameArea = Level.GameArea;
            List<Vertex> toDelete = new List<Vertex>() { };

            // Добавление случайных вершин
            for (int X = 5; X < GameArea.Width; X += 0)
            {
                for (int Y = 5; Y < GameArea.Height; Y += 0)
                {
                    addVertex(new Point(X, Y));
                    Y += rand.Next(30, 50);
                }
                X += rand.Next(30, 50);
            }

            // Нахождение вершин которые пересекаются с блоками
            foreach (Vertex v in _vertices)
            { 
                foreach(GameBlock block in Level.GameObjects)
                {
                    Rectangle vertex = new Rectangle(v.Location.X,v.Location.Y,5,5);
                    if (block.Position.Contains(vertex) || block.Position.Intersects(vertex))
                    {
                        toDelete.Add(v);
                    }
                }
            }

            // Удаление пересекающихся вершин
            foreach (Vertex v in toDelete)
            {
                _vertices.Remove(v);
            }

            // Поск соседних вершин
            foreach (Vertex v in _vertices)
            {
                Rectangle nearestArea = new Rectangle(v.Location.X - 51, v.Location.Y - 51, 105, 105);
                foreach (Vertex v2 in _vertices)
                {
                    if (!v.Equals(v2))
                    {
                        Rectangle rect = new Rectangle(v2.Location.X, v2.Location.Y, 5, 5);
                        if (nearestArea.Intersects(rect) || nearestArea.Contains(rect))
                        {
                            v.addConnection(v2);
                        }
                    }
                }
            }
        }
    }
}
