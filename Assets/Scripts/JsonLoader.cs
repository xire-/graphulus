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
        foreach (var jsonNode in root.Nodes)
        {
            var node = (GameObject)UnityEngine.Object.Instantiate(nodePrefab);
            node.name = String.Format("Node-{0}", jsonNode.Name);
            node.GetComponent<Node>().Text = jsonNode.Name;
            node.GetComponent<Node>().Group = jsonNode.Group;
            nodes.Add(node);
        }
        for (int i = 0; i < nodes.Count; ++i)
        {
            var node = nodes[i];
            node.GetComponent<Node>().Id = i;
        }

        var edgePrefab = Resources.Load("Edge");
        foreach (var jsonEdge in root.Links)
        {
            var edge = (GameObject)UnityEngine.Object.Instantiate(edgePrefab);
            var sourceNode = nodes[jsonEdge.Source];
            var targetNode = nodes[jsonEdge.Target];
            edge.name = String.Format("Edge-{0}-{1}", sourceNode.name, targetNode.name);
            edge.GetComponent<Edge>().Source = sourceNode;
            edge.GetComponent<Edge>().Target = targetNode;
            edge.GetComponent<Edge>().Length = jsonEdge.Value;
            edges.Add(edge);
        }
    }

    struct RootObject
    {
        public List<JsonNode> Nodes { get; set; }

        public List<JsonEdge> Links { get; set; }
    }

    struct JsonNode
    {
        public string Name { get; set; }

        public int Group { get; set; }
    }

    struct JsonEdge
    {
        public int Source { get; set; }

        public int Target { get; set; }

        public int Value { get; set; }
    }
}
