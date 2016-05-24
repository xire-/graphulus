using System;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    private Dictionary<string, Node> nodes = new Dictionary<string, Node> ();
    Dictionary<Node, Vector3> moveto = new Dictionary<Node, Vector3> ();

    void Start ()
    {
        // load nodes to experiment on (will be removed)
        string nconf = System.IO.File.ReadAllText ("Examples/nodes.txt");
        foreach (var line in nconf.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)) {
            GameObject o = (GameObject)Instantiate (Resources.Load ("Node"));
            o.transform.position = new Vector3 (UnityEngine.Random.Range (-0.5f, 0.5f), UnityEngine.Random.Range (-0.5f, 0.5f), UnityEngine.Random.Range (-1f, 0f));
            o.transform.localScale = new Vector3 (0.02f, 0.02f, 0.02f);
            o.transform.parent = gameObject.transform;
            Node node = o.GetComponent<Node> ();
            node.Value = line;
            nodes.Add (line, node);
            moveto.Add (node, node.transform.position);
        }

        List<Node> links = new List<Node> ();
        string lconf = System.IO.File.ReadAllText ("Examples/links.txt");
        foreach (var line in lconf.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)) {
            string start = line.Split (' ') [0];
            string end = line.Split (' ') [1];
            Node startnode = nodes [start];
            Node endnode = nodes [end];

            GameObject o = (GameObject)Instantiate (Resources.Load ("Link"));
            o.transform.parent = gameObject.transform;

            Link link = o.GetComponent<Link> ();
            link.node1 = startnode;
            link.node2 = endnode;
        }
    }

    void Update ()
    {
        // randomly move nodes
        List<Node> keys = new List<Node> (moveto.Keys);
        foreach (Node node in keys) {
            var to = moveto [node];
            if (node.transform.position == to) {
                moveto [node] = new Vector3 (UnityEngine.Random.Range (-0.5f, 0.5f), UnityEngine.Random.Range (-0.5f, 0.5f), UnityEngine.Random.Range (-1f, 0f));
            } else {
                node.transform.position = Vector3.MoveTowards (node.transform.position, to, 0.2f * Time.deltaTime);
            }
        }        
    }
}
