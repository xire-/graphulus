using System;
using System.Collections.Generic;
using UnityEngine;

namespace Springy
{
    public class ForceDirectedGraph
    {
        private List<Node> nodes;
        private List<Edge> edges;

        public float Stiffness { get; set; }
        public float Repulsion { get; set; }
        public float Convergence { get; set; }
        public float Damping { get; set; }
        public float EnergyThreshold { get; set; }

        public bool SimulationEnabled { get; set; }
        public bool SpringsEnabled { get; set; }
        public bool RepulsionEnabled { get; set; }
        public bool InEquilibrium { get; set; }

        public ForceDirectedGraph() {
            nodes = new List<Node>();
            edges = new List<Edge>();

            // set some default values
            Stiffness = 300f;
            SpringsEnabled = true;

            Repulsion = 400f;
            RepulsionEnabled = true;

            Convergence = 0.7f;
            Damping = 0.5f;
            EnergyThreshold = 0.05f;
        }

        public Node CreateNewNode(float mass = 1) {
            if (mass < 0) {
                throw new ArgumentException("Cannot have negative mass");
            }

            Node node = new Node(nodes.Count, mass);
            nodes.Add(node);
            return node;
        }

        public Edge CreateNewEdge(Node source, Node target, float length) {
            if (source.Id == target.Id) {
                throw new ArgumentException("Cannot link a node with itself");
            }
            if (length < 0) {
                throw new ArgumentException("Cannot have negative length");
            }

            Edge edge = new Edge(edges.Count, source, target, length);
            edges.Add(edge);
            return edge;
        }

        // TODO removeNode remove a node and it's associated edges from the graph
        // TODO detachNode removes edges associated with a given node
        // TODO removeEdge remove an edge from the graph

        public void Tick(float timestep) {
            if (SimulationEnabled && !InEquilibrium) {
                if (RepulsionEnabled) {
                    //applyCoulombsLaw();
                    applyCoulombsLaw_BarnesHut();
                }
                if (SpringsEnabled) {
                    applyHookesLaw();
                }
                attractToCenter();
                physicStep(timestep);

                if (TotalKineticEnergy() < EnergyThreshold) {
                    InEquilibrium = true;
                }
            }
        }

        private void applyCoulombsLaw() {
            for (int i = 0; i < nodes.Count; i++) {
                Node n1 = nodes[i];
                for (int j = i + 1; j < nodes.Count; j++) {
                    Node n2 = nodes[j];

                    Vector3 delta = n1.Position - n2.Position;
                    float sqrDistance = Math.Max(0.1f, delta.sqrMagnitude);
                    Vector3 direction = delta.normalized;

                    n1.AddForce((direction * Repulsion) / (sqrDistance * 0.5f));
                    n2.AddForce((direction * Repulsion) / (sqrDistance * -0.5f));
                }
            }
        }

        private void applyCoulombsLaw_BarnesHut() {
            // first find the bounds of the nodes
            Vector3 minVector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 maxVector = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (var node in nodes) {
                minVector.x = Mathf.Min(minVector.x, node.Position.x);
                minVector.y = Mathf.Min(minVector.y, node.Position.y);
                minVector.z = Mathf.Min(minVector.z, node.Position.z);

                maxVector.x = Mathf.Max(maxVector.x, node.Position.x);
                maxVector.y = Mathf.Max(maxVector.y, node.Position.y);
                maxVector.z = Mathf.Max(maxVector.z, node.Position.z);
            }
            float total_width = Mathf.Max(maxVector.x - minVector.x, maxVector.y - minVector.y, maxVector.z - minVector.z);

            BarnesHutOctree<Node> bh_tree = new BarnesHutOctree<Node>((maxVector + minVector) / 2, total_width / 2, 0.5f);
            foreach (var node in nodes) {
                bh_tree.AddObject(node);
            }

            // start iteration over nodes to apply forces
            foreach (var node in nodes) {
                foreach (var body in bh_tree.GetNearBodies(node.Position)) {
                    if (body == node) continue;
                    Vector3 delta = node.Position - body.Position;
                    float sqrDistance = Math.Max(0.1f, delta.sqrMagnitude);
                    Vector3 direction = delta.normalized;

                    Vector3 force = (direction * Repulsion * body.Mass) / (sqrDistance * 0.5f);
                    node.AddForce(force);
                }
            }
        }

        private void applyHookesLaw() {
            foreach (Edge edge in edges) {
                Vector3 delta = edge.Target.Position - edge.Source.Position;
                float displacement = edge.Length - delta.magnitude;
                Vector3 direction = delta.normalized;
                edge.Source.AddForce(direction * (Stiffness * displacement * -0.5f));
                edge.Target.AddForce(direction * (Stiffness * displacement * 0.5f));
            }
        }

        private void attractToCenter() {
            foreach (Node n in nodes) {
                Vector3 direction = n.Position * -1;
                n.AddForce(direction * Convergence);
            }
        }

        private void physicStep(float timestep) {
            foreach (Node n in nodes) {
                // calculate acceleration from forces
                n.ComputeAcceleration();

                n.Velocity = (n.Velocity + n.Acceleration * timestep) * Damping;
                n.Position += n.Velocity * timestep;
            }
        }

        public float TotalKineticEnergy() {
            float energy = 0.0f;
            foreach (Node n in nodes) {
                float speed = n.Velocity.magnitude;
                energy += 0.5f * n.Mass * speed * speed;
            }
            return energy;
        }
    }
}