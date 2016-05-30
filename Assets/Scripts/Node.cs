using System.Collections.Generic;

using UnityEngine;


public class Node : MonoBehaviour
{
    public int Id { get; set; }

    public string Text
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

    public int Group
    { 
        set
        { 
            var index = value % colors.Count;
            transform.Find("Sphere").GetComponent<Renderer>().material.color = colors[index];
            transform.Find("Text").GetComponent<Renderer>().material.color = colors[index];
        } 
    }

    public Springy.Node SpringyNode { get; set; }

    public List<GameObject> ConnectedTo { get; set; }

    private static readonly List<Color> colors = new List<Color>() { Color.black, Color.blue, Color.cyan, Color.gray, Color.green, Color.magenta, Color.red, Color.white, Color.yellow };


    public Node()
    {
        ConnectedTo = new List<GameObject>();
    }

    void LateUpdate()
    {
        transform.position = SpringyNode.pos;
    }
}
