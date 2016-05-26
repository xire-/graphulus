using System.Collections.Generic;

using UnityEngine;


public class World : MonoBehaviour
{
    void Start()
    {
        var nodes = new List<GameObject>();
        var edges = new List<GameObject>();
        JsonLoader.Deserialize("Examples/miserables.json", nodes, edges);

        foreach (var node in nodes)
        {
            node.transform.parent = gameObject.transform;
            node.transform.position = new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), 20f);
        }
        foreach (var edge in edges)
        {
            edge.transform.parent = gameObject.transform;
        }
    }
}
