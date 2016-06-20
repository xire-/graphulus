﻿using UnityEngine;

public class Node : MonoBehaviour
{
    public Springy.Node springyNode;
    private Color _color;
    private bool _selected;
    private TextMesh _textMesh;

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
            return _textMesh.text;
        }
        set
        {
            _textMesh.text = value;
        }
    }

    private void Awake()
    {
        _textMesh = transform.Find("Text").GetComponent<TextMesh>();
    }

    private void LateUpdate()
    {
        const float scale = 20f;
        if (Pinched)
            springyNode.Position = transform.localPosition * scale;
        else
            transform.localPosition = springyNode.Position / scale;
    }
}