using System;
using System.Collections.Generic;
using UnityEngine;

namespace Springy
{

    public class Node
    {
        public int id { get; private set; }
        public string label { get; private set; }

        public Vector3 pos { get; set; }
        public Vector3 vel { get; set; }
        public Vector3 acc { get; set; }

        public float mass { get; set; }
        public Vector3 forcesAccumulator { get; set; }

        public Node(int id, string label, float mass = 1)
        {
            this.id = id;
            this.label = label;

            // TODO random position???
            this.pos = new Vector3();
            this.vel = new Vector3();
            this.acc = new Vector3();

            this.mass = mass;
            this.forcesAccumulator = new Vector3();
        }

        internal void addForce(Vector3 f)
        {
            forcesAccumulator += f;
        }

        internal void computeAcceleration()
        {
            acc = forcesAccumulator / mass;
            forcesAccumulator = Vector3.zero;
        }
    }

    public class Edge
    {
        public int id { get; private set; }
        public Node source { get; private set; }
        public Node target { get; private set; }
        public float length { get; set; }


        public Edge(int id, Node source, Node target, float length)
        {
            this.id = id;
            this.source = source;
            this.target = target;
            this.length = length;
        }
    }

    public class ForceDirectedGraph
    {
        public Dictionary<int, Node> nodeMap;
        public List<Edge> edges;
        // TODO implementare
        // public adjacency = {};
        public int nextNodeId;
        public int nextEdgeId;

        public float stiffness { get; private set; }
        public float repulsion { get; private set; }
        public float damping { get; private set; }
        public float minEnergyThreshold { get; private set; }
        public bool running { get; set; }

        public ForceDirectedGraph()
        {
            nodeMap = new Dictionary<int, Node>();
            edges = new List<Edge>();

            nextNodeId = 0;
            nextEdgeId = 0;
        }
        public Node newNode(string label, float mass = 1)
        {
            Node node = new Node(nextNodeId, label, mass);
            nextNodeId += 1;
            nodeMap[node.id] = node;
            return node;
        }
        public Edge newEdge(int source, int target, float length)
        {
            if (source == target)
            {
                throw new ArgumentException("Cannot link a node with itself");
            }
            if (!nodeMap.ContainsKey(source) || !nodeMap.ContainsKey(target))
            {
                throw new ArgumentException("Source or destination non existant");
            }
            Edge edge = new Edge(nextEdgeId, nodeMap[source], nodeMap[target], length);
            nextEdgeId += 1;
            return edge;
        }

        public Edge getEdge(Node n1, Node n2)
        {
            throw new NotImplementedException();
        }

        // TODO removeNode remove a node and it's associated edges from the graph
        // TODO detachNode removes edges associated with a given node
        // TODO removeEdge remove an edge from the graph

        public void tick(float timestep)
        {
            if (running)
            {
                applyCoulombsLaw();
                applyHookesLaw();
                attractToCenter();
                physicStep(timestep);

                if (totalEnergy() < minEnergyThreshold)
                {
                    running = false;
                }
            }
        }

        private void applyCoulombsLaw()
        {
            foreach (Node n1 in nodeMap.Values)
            {
                foreach (Node n2 in nodeMap.Values)
                {
                    // TODO fix: viene applicata due volte per coppia
                    if (n1.id != n2.id)
                    {
                        Vector3 delta = n1.pos - n2.pos;
                        float sqrDistance = Math.Max(0.1f, delta.sqrMagnitude);
                        Vector3 direction = delta.normalized;

                        n1.addForce((direction * repulsion) / (sqrDistance * 0.5f));
                        n2.addForce((direction * repulsion) / (sqrDistance * -0.5f));
                    }
                }
            }
        }

        private void applyHookesLaw()
        {
            foreach (Edge edge in edges)
            {
                Vector3 delta = edge.target.pos - edge.source.pos;
                float displacement = edge.length - delta.magnitude;
                Vector3 direction = delta.normalized;

                edge.source.addForce(direction * (stiffness * displacement * -0.5f));
                edge.target.addForce(direction * (stiffness * displacement * 0.5f));
            }
        }

        private void attractToCenter()
        {
            foreach (Node n in nodeMap.Values)
            {
                Vector3 direction = n.pos * -1;
                n.addForce(direction * (repulsion / 50f));
            }
        }

        private void physicStep(float timestep)
        {
            foreach (Node n in nodeMap.Values)
            {
                // calculate acceleration from forces
                n.computeAcceleration();

                // TODO integratore scrauso, rifare
                n.vel = (n.vel + n.acc * timestep) * damping;
                n.pos += n.vel * timestep;
            }
        }

        private float totalEnergy()
        {
            float energy = 0.0f;
            foreach (Node n in nodeMap.Values)
            {
                float speed = n.vel.magnitude;
                energy += 0.5f * n.mass * speed * speed;
            }
            return energy;
        }

    }
}
