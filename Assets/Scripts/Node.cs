using UnityEngine;

public class Node : MonoBehaviour
{
    public Springy.Node springyNode;
    private bool renderEnabled;
    private const float fadeTime = 1f;

    private float renderTimeLeft, selectionTimeLeft;

    private Color _color;

    private bool _selected;
    public bool Selected
    {
        get
        {
            return _selected;
        }
        set
        {
            if (value && !_selected) {
                _color = GetComponent<Renderer>().material.color;
                GetComponent<Renderer>().material.color = Color.white;
            }
            else if (!value && _selected)
                GetComponent<Renderer>().material.color = _color;

            _selected = value;
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

    public void RenderText(float howMuchTime = 3)
    {
        renderEnabled = true;
        renderTimeLeft = howMuchTime;

        // reset alpha
        Color color = transform.Find("Text").GetComponent<Renderer>().material.color;
        color.a = 1f;
        transform.Find("Text").GetComponent<Renderer>().material.color = color;

        transform.Find("Text").GetComponent<Renderer>().enabled = true;
    }

    public bool Pinched {
        get; set;
    }

    private void LateUpdate()
    {
        if (!Pinched)
            transform.localPosition = springyNode.Position / 20f;
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