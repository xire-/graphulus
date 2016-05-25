using System;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    private Dictionary<string, GameObject> nodes = new Dictionary<string, GameObject>();
    Dictionary<GameObject, Vector3> moveto = new Dictionary<GameObject, Vector3>();

    void Start()
    {
        var nconf = System.IO.File.ReadAllText("Examples/nodes.txt");
        var nprefab = Resources.Load("Node");
        foreach (var line in nconf.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
        {
            var node = (GameObject)Instantiate(nprefab);
            node.transform.position = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-1f, 0f));
            node.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            node.transform.parent = gameObject.transform;
            node.GetComponent<Node>().Value = line;
            nodes.Add(line, node);
            moveto.Add(node, node.transform.position);
        }

        var links = new List<Node>();
        var lconf = System.IO.File.ReadAllText("Examples/links.txt");
        var lprefab = Resources.Load("Link");
        foreach (var line in lconf.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
        {
            var start = line.Split(' ')[0];
            var end = line.Split(' ')[1];
            var startnode = nodes[start];
            var endnode = nodes[end];

            var link = (GameObject)Instantiate(lprefab);
            link.transform.parent = gameObject.transform;
            link.GetComponent<Link>().node1 = startnode;
            link.GetComponent<Link>().node2 = endnode;
        }
    }

    void Update()
    {
        // randomly move nodes
        var keys = new List<GameObject>(moveto.Keys);
        foreach (var node in keys)
        {
            var to = moveto[node];
            if (node.transform.position == to)
            {
                moveto[node] = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-1f, 0f));
            }
            else
            {
                node.transform.position = Vector3.MoveTowards(node.transform.position, to, 0.2f * Time.deltaTime);
            }
        }
    }
}
