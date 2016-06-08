﻿using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class World : MonoBehaviour
{
    private Springy.ForceDirectedGraph forceDirectedGraph;
    private float fps, avgDeltaTime, timeElapsed;
    private int frameCount;
    private List<GameObject> nodes, edges;
    private bool textRenderingEnabled, edgeRenderingEnabled;
    private AnimationManager animationManager;

    private void Awake()
    {
        UnityEngine.Random.seed = 1337;

        nodes = new List<GameObject>();
        edges = new List<GameObject>();
        forceDirectedGraph = new Springy.ForceDirectedGraph();

        textRenderingEnabled = true;
        edgeRenderingEnabled = true;

        animationManager = new AnimationManager();
    }

    private void FixedUpdate()
    {
        forceDirectedGraph.tick(Time.fixedDeltaTime);
    }

    private void OnGUI()
    {
        // draw debug menu
        var text =
            String.Format("FPS: {0:f} [{1:f} ms]\n", fps, avgDeltaTime * 1000f) +
            "\n" +
            String.Format("Total energy: {0:f} [{1:f}]\n", forceDirectedGraph.totalKineticEnergy(), forceDirectedGraph.minEnergyThreshold) +
            "\n" +
            String.Format("Text rendering: {0}\n", textRenderingEnabled ? "ON" : "OFF") +
            String.Format("Edge rendering: {0}\n", edgeRenderingEnabled ? "ON" : "OFF");
        GUI.TextArea(new Rect(Screen.width - 250 - 10, 10, 250, Screen.height - 20), text);
    }

    private void Start()
    {
        CreateNodesAndEdges();
        CreateSpringyNodesAndEdges();

        // count the number of connections
        var connectionsCount = new Dictionary<GameObject, int>();
        foreach (var node in nodes)
            connectionsCount[node] = 0;
        foreach (var edge in edges)
        {
            connectionsCount[edge.GetComponent<Edge>().source]++;
            connectionsCount[edge.GetComponent<Edge>().target]++;
        }

        AdjustNodes(connectionsCount);

//        SetTheme(lightTheme);
    }

    private void AdjustNodes(Dictionary<GameObject, int> connectionsCount)
    {
        foreach (var node in nodes)
            node.transform.localScale *= 1.5f - Mathf.Pow(1.2f, -connectionsCount[node]);
    }

    private void CreateNodesAndEdges()
    {
        // create nodes and edges from JSON graph
        var jsonRoot = JsonLoader.Deserialize("Examples/miserables.json");
        nodes = (from jsonNode in jsonRoot.nodes
                       select CreateNode(jsonNode.name)).ToList();
        edges = (from jsonEdge in jsonRoot.links
                       select CreateEdge(jsonEdge.source, jsonEdge.target, jsonEdge.value)).ToList();
    }

    private void CreateSpringyNodesAndEdges()
    {
        // create springy nodes
        foreach (var node in nodes)
            node.GetComponent<Node>().springyNode = forceDirectedGraph.newNode();

        // create springy edges
        foreach (var edge in edges)
        {
            var sourceNode = edge.GetComponent<Edge>().source;
            var targetNode = edge.GetComponent<Edge>().target;
            forceDirectedGraph.newEdge(sourceNode.GetComponent<Node>().springyNode, targetNode.GetComponent<Node>().springyNode, edge.GetComponent<Edge>().length);
        }

        forceDirectedGraph.enabled = true;
    }

    private GameObject CreateNode(string text)
    {
        var node = (GameObject)Instantiate(Resources.Load("Node"));
        node.transform.parent = transform;
        node.name = String.Format("Node-{0}", text);
        node.GetComponent<Node>().Text = text;
        node.transform.Find("Text").GetComponent<Renderer>().enabled = false;
        return node;
    }

    private GameObject CreateEdge(int source, int target, int length)
    {
        var edge = (GameObject)Instantiate(Resources.Load("Edge"));
        var sourceNode = nodes[source];
        var targetNode = nodes[target];
        edge.transform.parent = transform;
        edge.name = String.Format("Edge-{0}-{1}", sourceNode.name, targetNode.name);
        edge.GetComponent<Edge>().source = sourceNode;
        edge.GetComponent<Edge>().target = targetNode;
        edge.GetComponent<Edge>().length = length;
        return edge;
    }

    private void Update()
    {
        // keep track of stats
        frameCount++;
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= 1f)
        {
            fps = frameCount;
            avgDeltaTime = timeElapsed / frameCount;
            frameCount = 0;
            timeElapsed = 0f;
        }

        animationManager.Update();

        // show text of nodes pointed by the camera
        RaycastHit hit;
        if (Physics.SphereCast(Camera.main.transform.position, 0.4f, Camera.main.transform.forward, out hit))
        {
            var gameObject = hit.transform.gameObject;
            if (gameObject.tag == "Node")
            {
                gameObject.GetComponent<Node>().Render();

                if (Input.GetKeyUp(KeyCode.X))
                {
                    gameObject.GetComponent<Node>().Select();
                }
            }
        }

        // set themes
        if (Input.GetKeyUp(KeyCode.L))
        {
            animationManager.StartAnimation(t => ChangeTheme(darkTheme, lightTheme, t), 0.7f);
        }
        if (Input.GetKeyUp(KeyCode.K))
        {
            animationManager.StartAnimation(t => ChangeTheme(lightTheme, darkTheme, t), 0.7f);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            // enable/disable text rendering of nodes
            foreach (var text in GameObject.FindGameObjectsWithTag("Text"))
            {
                text.GetComponent<Renderer>().enabled = !text.GetComponent<Renderer>().enabled;
            }
            textRenderingEnabled = !textRenderingEnabled;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            // enable/disable edge rendering
            foreach (var edge in GameObject.FindGameObjectsWithTag("Edge"))
            {
                edge.GetComponent<Renderer>().enabled = !edge.GetComponent<Renderer>().enabled;
            }
            edgeRenderingEnabled = !edgeRenderingEnabled;
        }
    }

    private void ChangeTheme(Theme startTheme, Theme endTheme, float t)
    {
        // set skybox color
        Camera.main.backgroundColor = Color.Lerp(startTheme.skyboxColor, endTheme.skyboxColor, t);

        // set nodes color
        foreach (var node in GameObject.FindGameObjectsWithTag("Node"))
            node.GetComponent<Renderer>().material.color = Color.Lerp(startTheme.nodeColor, endTheme.nodeColor, t);

        // set text color
        foreach (var text in GameObject.FindGameObjectsWithTag("Text"))
            text.GetComponent<TextMesh>().color = Color.Lerp(startTheme.textColor, endTheme.textColor, t);
        
        // set edges color
        foreach (var edge in GameObject.FindGameObjectsWithTag("Edge"))
            edge.GetComponent<Renderer>().material.color = Color.Lerp(startTheme.edgeColor, endTheme.edgeColor, t);
    }

    private readonly Theme lightTheme = new Theme()
    { 
        skyboxColor = new Color32(0xF3, 0xF3, 0xF3, 0xFF),
        nodeColor = new Color32(0xEA, 0x24, 0x7A, 0xA1),
        textColor = new Color32(0x51, 0x4B, 0x4B, 0xFF),
        edgeColor = new Color32(0x9F, 0x9D, 0x9D, 0x64)
    };
    private readonly Theme darkTheme = new Theme()
    { 
        skyboxColor = new Color32(0x10, 0x0F, 0x0F, 0xFF),
        nodeColor = new Color32(0x17, 0xE2, 0xDA, 0xA1),
        textColor = new Color32(0xE3, 0xE3, 0xE3, 0xFF),
        edgeColor = new Color32(0xF3, 0xF3, 0xF3, 0x64)
    };
}