using UnityEngine;


public class Edge : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public GameObject node1, node2;

    public void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void LateUpdate()
    {
        lineRenderer.SetPosition(0, node1.transform.position);
        lineRenderer.SetPosition(1, node2.transform.position);
    }
}
