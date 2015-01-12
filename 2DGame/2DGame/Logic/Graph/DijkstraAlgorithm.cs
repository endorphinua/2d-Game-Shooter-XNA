using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DGame.Logic.Graph
{
    public class DijkstraAlgorithm : SearchAlgorithm
    {
        public DijkstraAlgorithm(Graph Graph)
            :base(Graph)
        { }

        List<Vertex> Visited;
        Dictionary<Vertex, int> Values;
        Vertex To;
        Vertex From;

        public override List<Vertex> Search(Vertex From, Vertex To)
        {
            this.To = To;
            this.From = From;
            Visited = new List<Vertex>() { };
            Values = new Dictionary<Vertex, int>() { };
            foreach (Vertex v in Graph.Vertices)
            {
                Values.Add(v, Int32.MaxValue);
            }

            Vertex currentVertex = From;
            Values[currentVertex] = 0;
            // Добавляем начальную вершину в путь
            Step(currentVertex);
            Visited.Add(currentVertex);
            foreach (Vertex v in Graph.Vertices)
            {
                currentVertex = getNextUnchectedVertex(currentVertex);
                if (currentVertex != null)
                {
                    if (currentVertex.Equals(To))
                    {
                        break;
                    }
                    Step(currentVertex);
                }
                else
                    break;
            }

            return getPath();
        }

        public Vertex getNextUnchectedVertex(Vertex currentVertex)
        { 
            List<Vertex> Unchected = new List<Vertex>(){};

            foreach (Vertex v in Graph.Vertices)
            {
                //Vertex v = e.ConnectWith;
                if (!existInVisited(v))
                {
                    Unchected.Add(v);
                }
            }
            if (Unchected.Count != 0)
            {
                double minValue = Values[Unchected[0]];
                Vertex minVertex = Unchected[0];

                foreach (Vertex v in Unchected)
                {
                    if (Values[v] < minValue)
                    {
                        minValue = Values[v];
                        minVertex = v;
                    }
                }

                Visited.Add(minVertex);
                return minVertex;
            }
            else
                return null;
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

        /// <summary>
        /// Один шаг алгоритма
        /// </summary>
        /// <param name="Vertex"></param>
        private void Step(Vertex Vertex)
        {
            foreach (Edge Edge in Vertex.Edges)
            {
                Vertex otherVertex = Edge.ConnectWith;
                if(!existInVisited(otherVertex))
                {
                    if (Values[Vertex] != Int32.MaxValue)
                    {
                        int basicValue = (int)(Values[Vertex] + Edge.Cost);
                        if (Values[otherVertex] > basicValue)
                        {
                            //Console.WriteLine(Values[otherVertex].ToString() + " > " + otherVertex.Value.ToString());
                            Values[otherVertex] = basicValue;
                        }
                    }
                }
            }
            Visited.Add(Vertex);
        }

        public List<Vertex> getPath()
        {
            List<Vertex> Path = new List<Vertex>() { };
            Path.Add(To);

            Vertex currentVertex = To;
            foreach (Vertex Vertex in Visited)
            {
                Path.Add(currentVertex);
                int min = int.MaxValue;
                try
                {
                    min = Values[currentVertex];
                }
                catch (KeyNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                }
                Vertex v = currentVertex;
                foreach (Edge edge in currentVertex.Edges)
                {
                    Vertex otherVertex = edge.ConnectWith;
                    if (Values[otherVertex] < min)
                    {
                        min = Values[otherVertex];
                        v = otherVertex;
                    }
                }
                currentVertex = v;
                if(currentVertex.Equals(From))
                {
                    break;
                }
            }

            Path.Reverse();

            return Path;
        }
    }

}
