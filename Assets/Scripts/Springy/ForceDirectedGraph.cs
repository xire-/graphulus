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
        public Vector3 forcesAccumulator { get; private set; }

        public Node(int id, float mass, float randRange = 100f)
        {
            this.id = id;

            this.pos = new Vector3(
                UnityEngine.Random.Range(-randRange, randRange),
                UnityEngine.Random.Range(-randRange, randRange),
                UnityEngine.Random.Range(-randRange, randRange)
                );
            this.vel = new Vector3();
            this.acc = new Vector3();

            this.mass = mass;
            this.forcesAccumulator = new Vector3();
        }

        public void addForce(Vector3 f)
        {
            forcesAccumulator += f;
        }

        public void computeAcceleration()
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
        private List<Node> nodes;
        private List<Edge> edges;

        // TODO struttura per ottenere adiacenza

        public float stiffness { get; set; }
        public float repulsion { get; set; }
        public float damping { get; set; }
        public float minEnergyThreshold { get; set; }

        public bool running { get; set; }
        public bool inEquilibrium { get; set; }

        public ForceDirectedGraph()
        {
            nodes = new List<Node>();
            edges = new List<Edge>();

            // set some default values
            stiffness = 300f;
            repulsion = 400f;
            damping = 0.5f;
            minEnergyThreshold = 0.05f;
        }

        public Node newNode(float mass = 1)
        {
            if (mass < 0)
                throw new ArgumentException("Cannot have negative mass");

            Node node = new Node(nodes.Count, mass);
            nodes.Add(node);
            return node;
        }

        public Edge newEdge(int source, int target, float length)
        {
            if (source == target)
                throw new ArgumentException("Cannot link a node with itself");
            if (source >= nodes.Count || target >= nodes.Count)
                throw new ArgumentException("Source or destination non existant");
            if (length < 0)
                throw new ArgumentException("Cannot have negative length");

            Edge edge = new Edge(edges.Count, nodes[source], nodes[target], length);
            edges.Add(edge);
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
            for (int i = 0; i < nodes.Count; i++)
            {
                Node n1 = nodes[i];
                for (int j = i + 1; j < nodes.Count; j++)
                {
                    Node n2 = nodes[j];

                    Vector3 delta = n1.pos - n2.pos;
                    float sqrDistance = Math.Max(0.1f, delta.sqrMagnitude);
                    Vector3 direction = delta.normalized;

                    n1.addForce((direction * repulsion) / (sqrDistance * 0.5f));
                    n2.addForce((direction * repulsion) / (sqrDistance * -0.5f));
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
            foreach (Node n in nodes)
            {
                Vector3 direction = n.pos * -1;
                n.addForce(direction * (repulsion / 50f));
            }
        }

        private void physicStep(float timestep)
        {
            foreach (Node n in nodes)
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
            foreach (Node n in nodes)
            {
                float speed = n.vel.magnitude;
                energy += 0.5f * n.mass * speed * speed;
            }
            return energy;
        }

    }
}
