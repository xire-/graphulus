using UnityEngine;
using System.Collections;

public class Edge : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public GameObject node1, node2;

    public void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetWidth(0.003f, 0.003f);
    }

    public void LateUpdate()
    {
        lineRenderer.SetPosition(0, node1.transform.position);
        lineRenderer.SetPosition(1, node2.transform.position);
    }
}
