using System;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public Springy.ForceDirectedGraph forceDirectedGraph;
    public List<GameObject> nodes, texts, edges;

    public bool EdgesActive
    {
        set
        {
            foreach (var edge in edges)
                edge.SetActive(value);
        }
    }

    public bool TextsActive
    {
        set
        {
            foreach (var text in texts)
                text.SetActive(value);
        }
    }

    public void PopulateFrom(string jsonPath)
    {
        nodes = new List<GameObject>();
        texts = new List<GameObject>();
        edges = new List<GameObject>();

        var jsonRoot = JsonLoader.Deserialize(jsonPath);

        forceDirectedGraph = new Springy.ForceDirectedGraph()
        {
            Stiffness = jsonRoot.parameters.stiffness,
            Repulsion = jsonRoot.parameters.repulsion,
            Convergence = jsonRoot.parameters.convergence,
            Damping = jsonRoot.parameters.damping,
        };
        forceDirectedGraph.SimulationEnabled = true;

        AddNodes(jsonRoot);
        AddEdges(jsonRoot);
    }

    private static GameObject CreateEdge(Springy.Edge springyEdge, GameObject sourceNode, GameObject targetNode)
    {
        var edge = (GameObject)Instantiate(Resources.Load("Edge"));
        edge.name = String.Format("E.{0}.{1}", sourceNode.name, targetNode.name);
        edge.GetComponent<Edge>().springyEdge = springyEdge;
        edge.GetComponent<Edge>().source = sourceNode;
        edge.GetComponent<Edge>().target = targetNode;
        return edge;
    }

    private static GameObject CreateNode(Springy.Node springyNode, string text)
    {
        var node = (GameObject)Instantiate(Resources.Load("Node"));
        node.name = String.Format("N.{0}", text);
        node.GetComponent<Node>().springyNode = springyNode;
        node.GetComponent<Node>().Text = text;
        return node;
    }

    private void AddEdges(JsonLoader.JsonRoot jsonRoot)
    {
        foreach (var jsonEdge in jsonRoot.links)
        {
            var sourceNode = nodes[jsonEdge.source];
            var targetNode = nodes[jsonEdge.target];
            var springyEdge = forceDirectedGraph.CreateNewEdge(sourceNode.GetComponent<Node>().springyNode, targetNode.GetComponent<Node>().springyNode, jsonEdge.value);

            var edge = CreateEdge(springyEdge, sourceNode, targetNode);
            edge.transform.parent = transform;
            edges.Add(edge);
        }
    }

    private void AddNodes(JsonLoader.JsonRoot jsonRoot)
    {
        foreach (var jsonNode in jsonRoot.nodes)
        {
            var springyNode = forceDirectedGraph.CreateNewNode();

            var node = CreateNode(springyNode, jsonNode.name);
            node.transform.parent = transform;
            nodes.Add(node);

            var text = node.transform.Find("Text").gameObject;
            texts.Add(text);
        }
    }

    private void FixedUpdate()
    {
        forceDirectedGraph.Tick(Time.fixedDeltaTime);
    }
}