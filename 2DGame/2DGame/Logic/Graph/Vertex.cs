using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DGame.Logic.Graph
{
    /// <summary>
    /// Вершина графа
    /// </summary>
    public class Vertex
    {
        /// <summary>
        /// Расположение
        /// </summary>
        public Point Location
        {
            get;
            set;
        }

        /// <summary>
        /// Ребра вершины
        /// </summary>
        private List<Edge> _edges;

        /// <summary>
        /// Ребра вершины
        /// </summary>
        public List<Edge> Edges
        { get{ return this._edges;}}

        /// <summary>
        /// Количество смертей врагов в этой вершине
        /// </summary>
        public int Death
        { get; set;}

        /// <summary>
        /// Количество посещений данной вершины
        /// </summary>
        public int Visits
        { get; set; }

        public Vertex(Point Location)
        {
            this.Location = Location;
            this._edges = new List<Edge>() { };
        }

        /// <summary>
        /// Добавить связь между вершинами
        /// </summary>
        /// <param name="Vertex">Вершина графа</param>
        public void addConnection(Vertex Vertex)
        {
            Edge edge = new Edge(this, Vertex, getDistanceBetweenVertex(Vertex),getAngleBetweenVertex(Vertex));
            this._edges.Add(edge);
        }

        public int getDistanceBetweenVertex(Vertex vertex)
        {
            return (int)Math.Sqrt(Math.Pow((double)(this.Location.X - vertex.Location.X), 2) + Math.Pow((double)(this.Location.Y - vertex.Location.Y), 2));
        }

        public int getAngleBetweenVertex(Vertex vertex)
        {
            int Angle = 0;
            int X = Location.X, Y = Location.Y;

            int vertexX = vertex.Location.X, vertexY = vertex.Location.Y;

            if (X > vertexX)
                X -= vertexX;
            else
                X = vertexX - X;

            if (Y > vertexY)
                Y -= vertexY;
            else
                Y = vertexY - Y;

            Angle = getAngles(Math.Atan2(Y, X));
            X = Location.X; Y = Location.Y;

            if (vertexX > X && vertexY < Y)
                Angle = 180 - Angle;

            if (vertexX < X && vertexY < Y)
                Angle = 0 + Angle;

            if (vertexX > X && vertexY > Y)
                Angle = 180 + Angle;

            if (vertexX < X && vertexY > Y)
                Angle = 360 - Angle;

            return Angle;
        }

        public int getAngles(double radians)
        { return (int)(radians / (Math.PI / 180)); }
    }
}
