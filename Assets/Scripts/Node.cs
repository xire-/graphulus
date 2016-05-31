using System.Collections.Generic;

using UnityEngine;

public class Node : MonoBehaviour
{
    public List<GameObject> connectedTo;
    public int id;
    public Springy.Node springyNode;
    private const float fadeTime = 1f;
    private static readonly List<Color> colors = new List<Color>() { Color.blue, Color.cyan, Color.gray, Color.green, Color.magenta, Color.red, Color.white, Color.yellow };
    private bool renderEnabled;
    private float renderTimeLeft;

    public int Group
    {
        set
        {
            var index = value % colors.Count;
            var alphaColor = new Color(colors[index].r, colors[index].g, colors[index].b, .8f);
            GetComponent<Renderer>().material.color = alphaColor;
            transform.Find("Text").GetComponent<Renderer>().material.color = colors[index];
        }
    }

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

    public void Render(float howMuchTime = 3)
    {
        renderEnabled = true;
        renderTimeLeft = howMuchTime;

        // reset alpha
        Color color = transform.Find("Text").GetComponent<Renderer>().material.color;
        color.a = 1f;
        transform.Find("Text").GetComponent<Renderer>().material.color = color;

        transform.Find("Text").GetComponent<Renderer>().enabled = true;
    }

    private void Awake()
    {
        connectedTo = new List<GameObject>();
    }

    private void LateUpdate()
    {
        transform.position = springyNode.pos;
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