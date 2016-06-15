using UnityEngine;
using UnityEngine.Assertions;

public class Edge : MonoBehaviour
{
    public float length;
    public GameObject source, target;

    private void Awake()
    {
        // draw edges before nodes
        GetComponent<Renderer>().material.renderQueue = 0;
    }

    private void LateUpdate()
    {
        Assert.IsNotNull(source);
        GetComponent<LineRenderer>().SetPosition(0, source.transform.position);
        Assert.IsNotNull(target);
        GetComponent<LineRenderer>().SetPosition(1, target.transform.position);
    }
}