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
    }
}
