using UnityEngine;


public class Edge : MonoBehaviour
{
    public GameObject source, target;

    public void LateUpdate()
    {
        GetComponent<LineRenderer>().SetPosition(0, source.transform.position);
        GetComponent<LineRenderer>().SetPosition(1, target.transform.position);
    }
}
