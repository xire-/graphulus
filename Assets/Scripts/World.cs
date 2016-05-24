using System;
using UnityEngine;

public class World : MonoBehaviour
{
    void Start ()
    {
        // draw grid of nodes to experiments on (will be removed)
        for (float x = -1; x < 2; x += 0.5f) {
            for (float y = -1; y < 2; y += 0.5f) {
                GameObject o = (GameObject)Instantiate (Resources.Load ("Node"), new Vector3 (x, y, 0f), Quaternion.identity);
                Node node = o.GetComponent<Node> ();
                node.transform.localScale = new Vector3 (0.02f, 0.02f, 0.02f);
                node.transform.parent = gameObject.transform;
                node.Value = String.Format ("{0:f}/{1:f}/0", x, y);
            }
        }
        for (float z = -3; z < 0; z += 0.5f) {
            for (float y = -1; y < 2; y += 0.5f) {
                GameObject o = (GameObject)Instantiate (Resources.Load ("Node"), new Vector3 (-1f, y, z), Quaternion.identity);
                Node node = o.GetComponent<Node> ();
                node.transform.localScale = new Vector3 (0.02f, 0.02f, 0.02f);
                node.transform.parent = gameObject.transform;
                node.Value = String.Format ("-1/{0:f}/{1:f}", y, z);
            }
        }
    }
}
