using System;
using System.Collections.Generic;

using UnityEngine;

public class World : MonoBehaviour
{
    private Rect crosshairPosition;
    private Texture2D crosshairTexture;
    private float fps, avgDeltaTime, timeElapsed;
    private int frameCount;
    private bool textRenderingEnabled, edgeRenderingEnabled;
    public bool debugModeEnabled { get; set; }

    private Springy.ForceDirectedGraph forceDirectedGraph { get; set; }

    private void FixedUpdate()
    {
        // update simulation
        forceDirectedGraph.tick(Time.fixedDeltaTime);
    }

    private void OnGUI()
    {
        if (debugModeEnabled)
        {
            var debug =
                String.Format("FPS: {0:f} [{1:f} ms]\n", fps, avgDeltaTime * 1000f) +
                "\n" +
                String.Format("Total energy: {0:f} [{1:f}]\n", forceDirectedGraph.totalKineticEnergy(), forceDirectedGraph.minEnergyThreshold) +
                "\n" +
                String.Format("Text rendering: {0}\n", textRenderingEnabled ? "ON" : "OFF") +
                String.Format("Edge rendering: {0}\n", edgeRenderingEnabled ? "ON" : "OFF");

            GUI.TextArea(new Rect(Screen.width - 250 - 10, 10, 250, Screen.height - 20), debug);

            GUI.DrawTexture(crosshairPosition, crosshairTexture);
        }
    }

    private void Start()
    {
        UnityEngine.Random.seed = 1337;

        debugModeEnabled = true;

        forceDirectedGraph = new Springy.ForceDirectedGraph();
        forceDirectedGraph.running = true;

        var nodes = new List<GameObject>();
        var edges = new List<GameObject>();
        JsonLoader.Deserialize("Examples/miserables.json", nodes, edges);

        foreach (var node in nodes)
        {
            node.transform.parent = gameObject.transform;
            node.GetComponent<Node>().SpringyNode = forceDirectedGraph.newNode();
        }
        foreach (var edge in edges)
        {
            edge.transform.parent = gameObject.transform;
            forceDirectedGraph.newEdge(
                edge.GetComponent<Edge>().Source.GetComponent<Node>().Id,
                edge.GetComponent<Edge>().Target.GetComponent<Node>().Id,
                edge.GetComponent<Edge>().Length
            );

            edge.GetComponent<Edge>().Source.GetComponent<Node>().ConnectedTo.Add(edge.GetComponent<Edge>().Target);
            edge.GetComponent<Edge>().Target.GetComponent<Node>().ConnectedTo.Add(edge.GetComponent<Edge>().Source);
        }

        // set node size based on the number of connections
        int min = 1, max = 1;
        foreach (var node in nodes)
        {
            int curr = node.GetComponent<Node>().ConnectedTo.Count;
            if (curr < min)
                min = curr;
            if (curr > max)
                max = curr;
        }
        foreach (var node in nodes)
        {
            node.transform.localScale += node.transform.localScale * (node.GetComponent<Node>().ConnectedTo.Count - min) / (max - min);
        }

        textRenderingEnabled = true;
        edgeRenderingEnabled = true;

        crosshairTexture = (Texture2D)Resources.Load("Crosshair");
        crosshairPosition = new Rect((Screen.width - crosshairTexture.width) / 2, (Screen.height - crosshairTexture.height) / 2, crosshairTexture.width, crosshairTexture.height);
    }

    private void Update()
    {
        // find objects pointed by the camera
        var camera = GameObject.Find("Main Camera");
        var radius = 1f;
        RaycastHit hit;
        if (Physics.SphereCast(camera.transform.position, radius, camera.transform.forward, out hit))
        {
            var o = hit.transform.gameObject;
            var parent = o.transform.parent.gameObject;
            if (parent && parent.tag == "Node")
            {
                parent.GetComponent<Node>().Render(3f);
            }
        }

        // enable/disable debug menu
        if (Input.GetKeyDown(KeyCode.Space))
        {
            debugModeEnabled = !debugModeEnabled;
        }
        if (debugModeEnabled)
        {
            updateDebug();
        }

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
    }

    private void updateDebug()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            // enable/disable text rendering of nodes
            var nodes = GameObject.FindGameObjectsWithTag("Node");
            foreach (var node in nodes)
            {
                node.transform.Find("Text").GetComponent<Renderer>().enabled = !node.transform.Find("Text").GetComponent<Renderer>().enabled;
            }
            textRenderingEnabled = !textRenderingEnabled;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            // enable/disable edge rendering
            var edges = GameObject.FindGameObjectsWithTag("Edge");
            foreach (var edge in edges)
            {
                edge.GetComponent<Renderer>().enabled = !edge.GetComponent<Renderer>().enabled;
            }
            edgeRenderingEnabled = !edgeRenderingEnabled;
        }
    }
}