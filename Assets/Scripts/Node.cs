using UnityEngine;


public class Node : MonoBehaviour
{
    public string Value
    {
        get
        {
            return transform.Find("Text").GetComponent<TextMesh>().text;
        }
        set
        {
            transform.Find("Text").GetComponent<TextMesh>().text = value;
        }
    }

    public int Group { get; set; }
}
