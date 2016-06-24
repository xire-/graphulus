using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour {
    private static Object _edgeResource, _nodeResource;

    private Color _edgesColor, _nodesColor, _textsColor;
    private Springy.ForceDirectedGraph _forceDirectedGraph;
    private GameObject _nodesObject, _edgesObject;

    public bool EdgesActive {
        set { _edgesObject.SetActive(value); }
    }

    public Color EdgesColor {
        get { return _edgesColor; }
        set {
            foreach (Transform edge in _edgesObject.transform) {
                edge.GetComponent<Renderer>().material.color = value;
            }
            _edgesColor = value;
        }
    }

    public float EnergyThreshold {
        get { return _forceDirectedGraph.EnergyThreshold; }
    }

    public Color NodesColor {
        get { return _nodesColor; }
        set {
            foreach (Transform node in _nodesObject.transform) {
                node.GetComponent<Renderer>().material.color = value;
            }
            _nodesColor = value;
        }
    }

    public float Scale { get; set; }

    public bool TextsActive {
        set {
            foreach (Transform node in _nodesObject.transform) {
                node.Find("Text").gameObject.SetActive(value);
            }
        }
    }

    public Color TextsColor {
        get { return _textsColor; }
        set {
            foreach (Transform node in _nodesObject.transform) {
                node.Find("Text").GetComponent<TextMesh>().color = value;
            }
            _textsColor = value;
        }
    }

    public float TotalKineticEnergy {
        get { return _forceDirectedGraph.TotalKineticEnergy(); }
    }

    public Node GetClosestNode(Vector3 point) {
        GameObject closestNodeObject = null;
        float closestDistance = float.MaxValue;
        foreach (Transform nodeTransform in _nodesObject.transform) {
            var nodeDistance = Vector3.Distance(point, nodeTransform.position);
            if (nodeDistance < closestDistance) {
                closestNodeObject = nodeTransform.gameObject;
                closestDistance = nodeDistance;
            }
        }
        return closestNodeObject != null ? closestNodeObject.GetComponent<Node>() : null;
    }

    public void PopulateFrom(string jsonPath) {
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

        AdjustNodesSize();
    }

    private static GameObject CreateEdge(Springy.Edge springyEdge, GameObject sourceNode, GameObject targetNode) {
        var edgeObject = (GameObject)Instantiate(_edgeResource);
        edgeObject.name = string.Format("{0}---{1}", sourceNode.name, targetNode.name);
        var edge = edgeObject.GetComponent<Edge>();
        edge.springyEdge = springyEdge;
        edge.source = sourceNode;
        edge.target = targetNode;
        return edgeObject;
    }

    private static GameObject CreateNode(Springy.Node springyNode, string text) {
        var nodeObject = (GameObject)Instantiate(_nodeResource);
        nodeObject.name = text;
        var node = nodeObject.GetComponent<Node>();
        node.springyNode = springyNode;
        node.Text = text;
        return nodeObject;
    }

    private void AddEdges(JsonLoader.JsonRoot jsonRoot) {
        Destroy(_edgesObject);
        _edgesObject = new GameObject("Edges");
        _edgesObject.transform.parent = transform;

        foreach (var jsonEdge in jsonRoot.edges) {
            var sourceNode = _nodesObject.transform.GetChild(jsonEdge.source).gameObject;
            var targetNode = _nodesObject.transform.GetChild(jsonEdge.target).gameObject;
            var springyEdge = _forceDirectedGraph.CreateNewEdge(sourceNode.GetComponent<Node>().springyNode, targetNode.GetComponent<Node>().springyNode, jsonEdge.value);

            var edgeObject = CreateEdge(springyEdge, sourceNode, targetNode);
            edgeObject.transform.parent = _edgesObject.transform;
        }
    }

    private void AddNodes(JsonLoader.JsonRoot jsonRoot) {
        Destroy(_nodesObject);
        _nodesObject = new GameObject("Nodes");
        _nodesObject.transform.parent = transform;

        foreach (var jsonNode in jsonRoot.nodes) {
            var springyNode = _forceDirectedGraph.CreateNewNode();

            var nodeObject = CreateNode(springyNode, jsonNode.name);
            nodeObject.transform.parent = _nodesObject.transform;
        }
    }

    private void AdjustNodesSize() {
        // count the number of connections
        var connectionsCount = new Dictionary<GameObject, int>();
        foreach (Transform nodeTransform in _nodesObject.transform) {
            connectionsCount[nodeTransform.gameObject] = 0;
        }
        foreach (Transform edgeTransform in _edgesObject.transform) {
            connectionsCount[edgeTransform.GetComponent<Edge>().source]++;
            connectionsCount[edgeTransform.GetComponent<Edge>().target]++;
        }

        // adjust node size
        foreach (Transform nodeTransform in _nodesObject.transform) {
            nodeTransform.localScale *= 1.5f - Mathf.Pow(1.2f, -connectionsCount[nodeTransform.gameObject]);
        }
    }

    private void Awake() {
        _edgeResource = Resources.Load("Edge");
        _nodeResource = Resources.Load("Node");

        Scale = 30f;
    }

    private void FixedUpdate() {
        _forceDirectedGraph.Tick(Time.fixedDeltaTime);
    }
}
