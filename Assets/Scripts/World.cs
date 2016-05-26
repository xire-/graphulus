using System.Collections.Generic;

using UnityEngine;


public class World : MonoBehaviour
{
    void Start()
    {
        var jsonWorld = JsonLoader.Load("Examples/miserables.min.json");

        var nodes = new List<GameObject>();
        var nodePrefab = Resources.Load("Node");
        foreach (var jsonNode in jsonWorld.nodes)
        {
            var node = (GameObject)Instantiate(nodePrefab);
            node.transform.parent = gameObject.transform;
            node.transform.position = new Vector3(Random.Range(-6f, 6f), Random.Range(-6f, 6f), 5f);
            node.GetComponent<Node>().Value = jsonNode.name;
            node.GetComponent<Node>().Text = jsonNode.name;
            node.GetComponent<Node>().Group = jsonNode.group;
            nodes.Add(node);
        }

        var edgePrefab = Resources.Load("Edge");
        foreach (var jsonEdge in jsonWorld.links)
        {
            var edge = (GameObject)Instantiate(edgePrefab);
            edge.transform.parent = gameObject.transform;
            edge.GetComponent<Edge>().node1 = nodes[jsonEdge.source];
            edge.GetComponent<Edge>().node2 = nodes[jsonEdge.target];
        }
    }
}
