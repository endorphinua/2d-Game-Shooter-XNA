using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DGame.Logic.Graph
{
    public class SearchAlgorithm
    {
        public Graph Graph
        { get; set; }


        public SearchAlgorithm(Graph Graph)
        {
            this.Graph = Graph;
        }

        public virtual List<Vertex> Search(Vertex From, Vertex To)
        {
            List<Vertex> Result = new List<Vertex>() { };
            return Result;
        }
    }
}
