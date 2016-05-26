using System;
using System.Collections.Generic;

using UnityEngine;

using Newtonsoft.Json;


public class JsonLoader
{
    public static void Deserialize(string path, List<GameObject> nodes, List<GameObject> edges)
    {
        var json = System.IO.File.ReadAllText(path);
        var root = JsonConvert.DeserializeObject<RootObject>(json);

        var nodePrefab = Resources.Load("Node");
        foreach (var jsonNode in root.nodes)
        {
            var node = (GameObject)UnityEngine.Object.Instantiate(nodePrefab);
            node.name = String.Format("Node-{0}", jsonNode.name);
            node.GetComponent<Node>().Text = jsonNode.name;
            node.GetComponent<Node>().Group = jsonNode.group;
            nodes.Add(node);
        }

        var edgePrefab = Resources.Load("Edge");
        foreach (var jsonEdge in root.links)
        {
            var edge = (GameObject)UnityEngine.Object.Instantiate(edgePrefab);
            var sourceNode = nodes[jsonEdge.source];
            var targetNode = nodes[jsonEdge.target];
            edge.name = String.Format("Edge-{0}-{1}", sourceNode.name, targetNode.name);
            edge.GetComponent<Edge>().source = sourceNode;
            edge.GetComponent<Edge>().target = targetNode;
            edges.Add(edge);
        }
    }

    private struct RootObject
    {
        public List<JsonNode> nodes;
        public List<JsonEdge> links;
    }

    private struct JsonNode
    {
        public string name;
        public int group;
    }

    private struct JsonEdge
    {
        public int source;
        public int target;
        public int value;
    }
}
