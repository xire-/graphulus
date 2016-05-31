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

    private bool renderEnabled;
    private float renderTimeLeft;
    private const float fadeTime = 1f;


    public Node()
    {
        ConnectedTo = new List<GameObject>();
    }

    void Start()
    {
        transform.Find("Text").GetComponent<Renderer>().enabled = false;
    }

    void Update()
    {

        if (renderEnabled)
        {
            renderTimeLeft -= Time.deltaTime;
            if (renderTimeLeft <= 0)
            {
                renderEnabled = false;
                renderTimeLeft = 0;
                transform.Find("Text").GetComponent<Renderer>().enabled = false;
            }
            else if (renderTimeLeft <= fadeTime)
            {
                Color color = transform.Find("Text").GetComponent<Renderer>().material.color;
                color.a = Mathf.Lerp(0, fadeTime, renderTimeLeft);
                transform.Find("Text").GetComponent<Renderer>().material.color = color;
            }
        }
    }

    void LateUpdate()
    {
        transform.position = SpringyNode.pos;
    }

    public void Render(float howMuchTime)
    {
        renderEnabled = true;
        renderTimeLeft = howMuchTime;
        transform.Find("Text").GetComponent<Renderer>().enabled = true;
    }
}
