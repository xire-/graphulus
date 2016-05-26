﻿using System.Collections.Generic;

using UnityEngine;


public class Node : MonoBehaviour
{
    public string Text
    {
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
        } 
    }

    private static readonly List<Color> colors = new List<Color>() { Color.black, Color.blue, Color.cyan, Color.gray, Color.green, Color.magenta, Color.red, Color.white, Color.yellow };
}
