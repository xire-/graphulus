using System.Collections.Generic;

using UnityEngine;

public class Node : MonoBehaviour
{
    private const float fadeTime = 1f;
    private static readonly List<Color> colors = new List<Color>() { Color.black, Color.blue, Color.cyan, Color.gray, Color.green, Color.magenta, Color.red, Color.white, Color.yellow };
    private bool renderEnabled;
    private float renderTimeLeft;
    public Node()
    {
        ConnectedTo = new List<GameObject>();
    }

    public List<GameObject> ConnectedTo { get; set; }
    public int Group
    {
        set
        {
            var index = value % colors.Count;
            var alphaColor = new Color(colors[index].r, colors[index].g, colors[index].b, .8f);
            transform.Find("Circle").GetComponent<Renderer>().material.color = alphaColor;
            transform.Find("Text").GetComponent<Renderer>().material.color = colors[index];
        }
    }

    public int Id { get; set; }

    public Springy.Node SpringyNode { get; set; }

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
    public void Render(float howMuchTime)
    {
        renderEnabled = true;
        renderTimeLeft = howMuchTime;

        // reset alpha
        Color color = transform.Find("Text").GetComponent<Renderer>().material.color;
        color.a = 1f;
        transform.Find("Text").GetComponent<Renderer>().material.color = color;

        transform.Find("Text").GetComponent<Renderer>().enabled = true;
    }

    private void LateUpdate()
    {
        transform.position = SpringyNode.pos;
    }

    private void Start()
    {
        transform.Find("Text").GetComponent<Renderer>().enabled = false;
    }

    private void Update()
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
}