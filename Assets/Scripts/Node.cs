﻿using System;
using System.Collections.Generic;

using UnityEngine;

public class Node : MonoBehaviour
{
    public Springy.Node springyNode;
    private const float fadeTime = 1f;
    private bool renderEnabled, selected;
    private float renderTimeLeft, selectionTimeLeft;
    const float totaltimeselection = 0.3f;

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
        if (selected)
        {
            selectionTimeLeft -= Time.deltaTime;
            if (selectionTimeLeft >= 0)
            {
                Color color = GetComponent<Renderer>().material.color;
                float f = (totaltimeselection - selectionTimeLeft) / totaltimeselection;
                Color newcolor = Color.Lerp(color, Color.white, f);
                newcolor.a = color.a;
                GetComponent<Renderer>().material.color = newcolor;
            }
        }

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

    public void Select()
    {
        selected = true;
        selectionTimeLeft = totaltimeselection;
    }
}