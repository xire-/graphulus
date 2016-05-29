using System;
using System.Collections.Generic;
using UnityEngine;

namespace Springy
{

    public class Node
    {
        public int id { get; private set; }

        public Vector3 pos { get; set; }
        public Vector3 vel { get; set; }
        public Vector3 acc { get; set; }

        public float mass { get; set; }
        public Vector3 forcesAccumulator { get; set; }

        public Node(int id, float mass = 1)
        {
            this.id = id;

            // TODO random position???
            this.pos = new Vector3(UnityEngine.Random.Range(-70f, 70f), UnityEngine.Random.Range(-70f, 70f), UnityEngine.Random.Range(-40f, 40f));
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
        private Dictionary<int, Node> nodes;
        private Dictionary<int, Edge> edges;

        // TODO struttura per ottenere adiacenza

        // edge and node id counter
        public int nextNodeId { get; private set; }
        public int nextEdgeId { get; private set; }

        public float stiffness { get; set; }
        public float repulsion { get; set; }
        public float damping { get; set; }
        public float minEnergyThreshold { get; set; }

        public bool running { get; set; }
        public bool inEquilibrium { get; set; }

        public ForceDirectedGraph()
        {
            nodes = new Dictionary<int, Node>();
            edges = new Dictionary<int, Edge>();

            // ids starts from 0
            nextNodeId = 0;
            nextEdgeId = 0;

            // set some default values
            stiffness = 300f;
            repulsion = 400f;
            damping = 0.5f;
            minEnergyThreshold = 0.01f;
        }

        public Node newNode(float mass = 1)
        {
            Node node = new Node(nextNodeId);
            nextNodeId += 1;
            nodes[node.id] = node;
            return node;
        }

        public Edge newEdge(int source, int target, float length)
        {
            if (source == target)
                throw new ArgumentException("Cannot link a node with itself");
            if (!nodes.ContainsKey(source) || !nodes.ContainsKey(target))
                throw new ArgumentException("Source or destination non existant");

            Edge edge = new Edge(nextEdgeId, nodes[source], nodes[target], length);
            nextEdgeId += 1;
            edges[edge.id] = edge;
            return edge;
        }
        
        // TODO removeNode remove a node and it's associated edges from the graph
        // TODO detachNode removes edges associated with a given node
        // TODO removeEdge remove an edge from the graph

        public void tick(float timestep)
        {
            if (running && !inEquilibrium)
            {
                applyCoulombsLaw();
                applyHookesLaw();
                attractToCenter();
                physicStep(timestep);

                if (totalKineticEnergy() < minEnergyThreshold)
                {
                    inEquilibrium = true;
                }
            }
        }

        private void applyCoulombsLaw()
        {
            foreach (Node n1 in nodes.Values)
            {
                foreach (Node n2 in nodes.Values)
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
            foreach (Edge edge in edges.Values)
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
            foreach (Node n in nodes.Values)
            {
                Vector3 direction = n.pos * -1;
                n.addForce(direction * (repulsion / 50f));
            }
        }

        private void physicStep(float timestep)
        {
            foreach (Node n in nodes.Values)
            {
                // calculate acceleration from forces
                n.computeAcceleration();

                // TODO integratore scrauso, rifare
                n.vel = (n.vel + n.acc * timestep) * damping;
                n.pos += n.vel * timestep;
            }
        }

        private float totalKineticEnergy()
        {
            float energy = 0.0f;
            foreach (Node n in nodes.Values)
            {
                float speed = n.vel.magnitude;
                energy += 0.5f * n.mass * speed * speed;
            }
            return energy;
        }

    }
}
