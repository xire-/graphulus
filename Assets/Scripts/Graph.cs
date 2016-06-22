using System;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour {
    private List<GameObject> _edgeObjects = new List<GameObject>();
    private Springy.ForceDirectedGraph _forceDirectedGraph;
    private List<GameObject> _nodeObjects = new List<GameObject>();
    private List<GameObject> _textObjects = new List<GameObject>();

    public bool EdgesActive {
        set {
            foreach (var edge in _edgeObjects) {
                edge.SetActive(value);
            }
        }
    }

    public Color EdgesColor {
        get { return _edgeObjects.Count > 0 ? _edgeObjects[0].GetComponent<Renderer>().material.color : Color.black; }
        set {
            foreach (var edge in _edgeObjects) {
                edge.GetComponent<Renderer>().material.color = value;
            }
        }
    }

    public float EnergyThreshold {
        get { return _forceDirectedGraph.EnergyThreshold; }
    }

    public Color NodesColor {
        get { return _nodeObjects.Count > 0 ? _nodeObjects[0].GetComponent<Renderer>().material.color : Color.black; }
        set {
            foreach (var node in _nodeObjects) {
                node.GetComponent<Renderer>().material.color = value;
            }
        }
    }

    public bool TextsActive {
        set {
            foreach (var text in _textObjects) {
                text.SetActive(value);
            }
        }
    }

    public Color TextsColor {
        get { return _textObjects.Count > 0 ? _textObjects[0].GetComponent<Renderer>().material.color : Color.black; }
        set {
            foreach (var text in _textObjects) {
                text.GetComponent<TextMesh>().color = value;
            }
        }
    }

    public float TotalKineticEnergy {
        get { return _forceDirectedGraph.TotalKineticEnergy(); }
    }

    public Node GetClosestNodeOrNull(Vector3 point, float maxDistance) {
        GameObject closestObject = null;
        float closestDistance = float.MaxValue;
        foreach (var nodeObject in _nodeObjects) {
            var nodeDistance = Vector3.Distance(point, nodeObject.transform.position);
            if (nodeDistance < closestDistance && nodeDistance <= maxDistance) {
                closestObject = nodeObject;
                closestDistance = nodeDistance;
            }
        }
        return closestObject.GetComponent<Node>();
    }

    public void PopulateFrom(string jsonPath, bool adjustNodesSize = true) {
        var jsonRoot = JsonLoader.Deserialize(jsonPath);

        _forceDirectedGraph = new Springy.ForceDirectedGraph {
            Stiffness = jsonRoot.parameters.stiffness,
            Repulsion = jsonRoot.parameters.repulsion,
            Convergence = jsonRoot.parameters.convergence,
            Damping = jsonRoot.parameters.damping,
            EnergyThreshold = -1f,
            SimulationEnabled = true,
        };

        AddNodes(jsonRoot);
        AddEdges(jsonRoot);

        if (adjustNodesSize) {
            AdjustNodesSize();
        }
    }

    private static GameObject CreateEdge(Springy.Edge springyEdge, GameObject sourceNode, GameObject targetNode) {
        var edge = (GameObject)Instantiate(Resources.Load("Edge"));
        edge.name = String.Format("E.{0}.{1}", sourceNode.name, targetNode.name);
        edge.GetComponent<Edge>().springyEdge = springyEdge;
        edge.GetComponent<Edge>().source = sourceNode;
        edge.GetComponent<Edge>().target = targetNode;
        return edge;
    }

    private static GameObject CreateNode(Springy.Node springyNode, string text) {
        var node = (GameObject)Instantiate(Resources.Load("Node"));
        node.name = String.Format("N.{0}", text);
        node.GetComponent<Node>().springyNode = springyNode;
        node.GetComponent<Node>().Text = text;
        return node;
    }

    private void AddEdges(JsonLoader.JsonRoot jsonRoot) {
        _edgeObjects.Clear();

        foreach (var jsonEdge in jsonRoot.edges) {
            var sourceNode = _nodeObjects[jsonEdge.source];
            var targetNode = _nodeObjects[jsonEdge.target];
            var springyEdge = _forceDirectedGraph.CreateNewEdge(sourceNode.GetComponent<Node>().springyNode, targetNode.GetComponent<Node>().springyNode, jsonEdge.value);

            var edge = CreateEdge(springyEdge, sourceNode, targetNode);
            edge.transform.parent = transform;
            _edgeObjects.Add(edge);
        }
    }

    private void AddNodes(JsonLoader.JsonRoot jsonRoot) {
        _nodeObjects.Clear();
        _textObjects.Clear();

        foreach (var jsonNode in jsonRoot.nodes) {
            var springyNode = _forceDirectedGraph.CreateNewNode();

            var node = CreateNode(springyNode, jsonNode.name);
            node.transform.parent = transform;
            _nodeObjects.Add(node);

            var text = node.transform.Find("Text").gameObject;
            _textObjects.Add(text);
        }
    }

    private void AdjustNodesSize() {
        // count the number of connections
        var connectionsCount = new Dictionary<GameObject, int>();
        foreach (var node in _nodeObjects) {
            connectionsCount[node] = 0;
        }
        foreach (var edge in _edgeObjects) {
            connectionsCount[edge.GetComponent<Edge>().source]++;
            connectionsCount[edge.GetComponent<Edge>().target]++;
        }

        foreach (var node in _nodeObjects) {
            node.transform.localScale *= 1.5f - Mathf.Pow(1.2f, -connectionsCount[node]);
        }
    }

    private void FixedUpdate() {
        _forceDirectedGraph.Tick(Time.fixedDeltaTime);
    }
}
