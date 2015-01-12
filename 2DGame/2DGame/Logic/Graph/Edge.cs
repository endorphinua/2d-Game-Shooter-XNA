using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DGame.Logic.Graph
{
    public class Edge
    {
        /// <summary>
        /// Первая вершина
        /// </summary>
        public Vertex Owner
        { get; set; }

        /// <summary>
        /// "Цена" перехода
        /// </summary>
        public int Cost
        { get; set; }

        public int Angle
        { get; set; }

        /// <summary>
        /// Вторая вершина
        /// </summary>
        public Vertex ConnectWith
        { get; set; }

        public Edge(Vertex Owner, Vertex ConnectWith, int Cost,int Angle)
        {
            this.Owner = Owner;
            this.ConnectWith = ConnectWith;
            this.Cost = Cost;
            this.Angle = Angle;
        }
    }
}
