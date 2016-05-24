using UnityEngine;
using System.Collections;

public class Link : MonoBehaviour {
    public Node node1, node2;

	void LateUpdate () {
        float width = 0.003f;
        var offset = node2.transform.position - node1.transform.position;
        var scale = new Vector3 (width, offset.magnitude / 2f, width);
        transform.position = node1.transform.position + (offset / 2f);
        transform.up = offset;
        transform.localScale = scale;
	}
}
