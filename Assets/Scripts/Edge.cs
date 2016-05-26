using UnityEngine;


public class Edge : MonoBehaviour
{
    public GameObject Source { get; set; }

    public GameObject Target { get; set; }

    public int Length { get; set; }


    public void LateUpdate()
    {
        GetComponent<LineRenderer>().SetPosition(0, Source.transform.position);
        GetComponent<LineRenderer>().SetPosition(1, Target.transform.position);

        // not perfect but enough for now
        var startWidth = Vector3.Distance(GameObject.Find("Main Camera").transform.position, Source.transform.position) / 300f;
        var endWidth = Vector3.Distance(GameObject.Find("Main Camera").transform.position, Target.transform.position) / 300f;
        GetComponent<LineRenderer>().SetWidth(startWidth, endWidth);
    }
}
