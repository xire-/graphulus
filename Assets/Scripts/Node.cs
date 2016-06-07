using System.Collections.Generic;

using UnityEngine;

public class Node : MonoBehaviour
{
    public Springy.Node springyNode;
    private const float fadeTime = 1f;
    private bool renderEnabled;
    private float renderTimeLeft;

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