using System.Collections.Generic;

using UnityEngine;


public class World : MonoBehaviour
{
    Springy.ForceDirectedGraph forceDirectedGraph { get; set; }


    void Start()
    {
        forceDirectedGraph = new Springy.ForceDirectedGraph();
        forceDirectedGraph.stiffness = 300f;
        forceDirectedGraph.repulsion = 400f;
        forceDirectedGraph.damping = 0.5f;
        forceDirectedGraph.running = true;

        var nodes = new List<GameObject>();
        var edges = new List<GameObject>();
        JsonLoader.Deserialize("Examples/miserables.json", nodes, edges);

        foreach (var node in nodes)
        {
            node.transform.parent = gameObject.transform;
            node.GetComponent<Node>().SpringyNode = forceDirectedGraph.newNode(node.GetComponent<Node>().Text);
        }
        foreach (var edge in edges)
        {
            edge.transform.parent = gameObject.transform;
            forceDirectedGraph.newEdge(
                edge.GetComponent<Edge>().Source.GetComponent<Node>().Id,
                edge.GetComponent<Edge>().Target.GetComponent<Node>().Id,
                edge.GetComponent<Edge>().Length
            );
        }
    }


    void Update()
    {
        // update simulation
        forceDirectedGraph.tick(Time.deltaTime);

        // enable/disable text rendering of nodes
        if (Input.GetKeyDown(KeyCode.R))
        {
            var nodes = GameObject.FindGameObjectsWithTag("Node");
            foreach (var node in nodes)
            {
                node.transform.Find("Text").GetComponent<Renderer>().enabled = !node.transform.Find("Text").GetComponent<Renderer>().enabled;
            }
        }
    }
}
