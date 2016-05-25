using System;
using System.Collections.Generic;

namespace Springy
{
    public class Graph
    {
        public HashSet<Node> nodeSet;
        public List<Node> nodes;
        public List<Edge> edges;
        // TODO implementare
        // public adjacency = {};

        public int nextNodeId;
        public int nextEdgeId;

        // TODO implementare
        // public eventListeners = [];

        public Graph() {
            nodeSet = HashSet<>();
        }

        public Node addNode(Node node) {

            return node;
        }
        public Edge addEdge(Edge edge) {

            return edge;
        }


        public Node newNode(Node node) {

            return node;
        }
        public Edge newEdge(Edge edge) {

            return edge;
        }

        public void initFromFile() {
            // read from json
        }

        public Edge getEdge(Node n1, Node n2){
            // get edge from two nodes if any
            return null;
        }

        // TODO removeNode remove a node and it's associated edges from the graph
        // TODO detachNode removes edges associated with a given node
        // TODO removeEdge remove an edge from the graph

    }

    public class Node
    {
        public int id;
        // TODO cambiare tipo
        public Object data;
    }

    public class Edge
    {
        public int id;
        public Node source;
        public Node target;
        // TODO cambiare tipo
        public Object data;
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("asd");
        }
    }
}
