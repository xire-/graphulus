using UnityEngine;

public class Node : MonoBehaviour
{
    public Springy.Node springyNode;
    private const float fadeTime = 1f;
    private readonly float _positionScale = 20f;
    private Color _color;
    private bool _selected;
    private bool renderEnabled;
    private float renderTimeLeft, selectionTimeLeft;

    public bool Pinched
    {
        get; set;
    }

    public bool Selected
    {
        get
        {
            return _selected;
        }
        set
        {
            if (value && !_selected)
            {
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

    private void LateUpdate()
    {
        if (Pinched)
            springyNode.Position = transform.localPosition * _positionScale;
        else
            transform.localPosition = springyNode.Position / _positionScale;
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