using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DGame.Logic.Graph
{
    class AStarAlgorithm : SearchAlgorithm
    {
        /// <summary>
        /// Список посещенных вершин
        /// </summary>
        List<Vertex> Visited;
        /// <summary>
        /// Список Ожидающих рассмотрения вершин
        /// </summary>
        List<Vertex> Waited;

        /// <summary>
        /// Значение G для вершины
        /// </summary>
        Dictionary<Vertex, int> G;

        /// <summary>
        /// Значение H для вершины
        /// </summary>
        Dictionary<Vertex, int> H;

        Dictionary<Vertex, int> Values;

        /// <summary>
        /// Ссылка на предыдущую вершину
        /// </summary>
        Dictionary<Vertex, Vertex> Prev;

        /// <summary>
        /// Начальная вершина
        /// </summary>
        Vertex From;

        /// <summary>
        /// Цель
        /// </summary>
        Vertex To;

        /// <summary>
        /// Помечаем вершину как "посещенную"
        /// </summary>
        /// <param name="V">Вершина</param>
        private void markVertexAsVisited(Vertex V)
        {
            Visited.Add(V);
            Waited.Remove(V);
        }

        private int getG(Edge Edge)
        {
            return G[Edge.Owner] + Edge.Cost;
        }

        private int getH(Edge Edge)
        {
            return Edge.ConnectWith.getDistanceBetweenVertex(To);
        }

        private int getF(Edge Edge)
        {
            int G = getG(Edge);
            int H = getH(Edge);
            return G + H;
        }

        public AStarAlgorithm(Graph Graph)
            : base(Graph)
        { }

        public override List<Vertex> Search(Vertex From, Vertex To)
        {
            this.To = To;
            this.From = From;
            this.Visited = new List<Vertex>() { };
            this.Waited = new List<Vertex>() { };
            this.G = new Dictionary<Vertex, int>() { };
            this.H = new Dictionary<Vertex, int>() { };
            this.Values = new Dictionary<Vertex, int>() { };

            this.Prev = new Dictionary<Vertex, Vertex>() { };
            foreach (Vertex v in Graph.Vertices)
            {
                Values.Add(v, 0);
                H.Add(v, 0);
                G.Add(v, 0);
                Prev.Add(v, null);
            }
            Waited.Add(From);

            while (Waited.Count != 0)
            {
                Vertex currentVertex = getMinVertex();
                markVertexAsVisited(currentVertex);
                if (currentVertex.Equals(To))
                {
                    break;
                }
                Step(currentVertex);
            }

            return getPath();
        }

        public List<Vertex> getPath()
        {
            List<Vertex> Path = new List<Vertex>() { };
            Path.Add(To);

            Vertex currentVertex = To;
            foreach (Vertex Vertex in Visited)
            {
                Path.Add(currentVertex);
                Vertex prev = Visited[0];
                try
                {
                    prev = this.Prev[currentVertex];
                }
                catch (KeyNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                }
                if (prev != null)
                {
                    currentVertex = prev;
                    if (currentVertex.Equals(From))
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }

            }

            Path.Reverse();

            return Path;
        }

        public void Step(Vertex Vertex)
        {
            foreach (Edge Edge in Vertex.Edges)
            {
                Vertex neighbour = Edge.ConnectWith;
                if (!existInVisited(neighbour))
                {
                    if (!existInWaited(neighbour))
                    {
                        Waited.Add(neighbour);
                        Values[neighbour] = getF(Edge);
                        Prev[neighbour] = Vertex;
                    }
                    else
                    {
                        int g = getG(Edge);
                        if (g < G[neighbour])
                        {
                            G[neighbour] = g;
                            Prev[neighbour] = Vertex;
                            Values[neighbour] = getF(Edge);
                        }
                    }
                }
            }
        }

        private bool existInVisited(Vertex v)
        {
            foreach (Vertex vertex in Visited)
            {
                if (vertex.Equals(v))
                {
                    return true;
                }
            }
            return false;
        }

        private bool existInWaited(Vertex v)
        {
            foreach (Vertex vertex in Waited)
            {
                if (vertex.Equals(v))
                {
                    return true;
                }
            }
            return false;
        }

        public Vertex getMinVertex()
        {
            Vertex minVertex = Waited[0];
            int minValue = int.MaxValue;
            try
            {
                minValue = Values[Waited[0]];
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            foreach (Vertex v in Waited)
            {
                try
                {
                    if (minValue > Values[v])
                    {
                        minValue = Values[v];
                        minVertex = v;
                    }
                }
                catch (KeyNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return minVertex;
        }
    }
}