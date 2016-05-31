using System;
using System.Collections.Generic;

using UnityEngine;

public class World : MonoBehaviour
{
    private Rect crosshairPosition;
    private Texture2D crosshairTexture;
    private bool debugModeEnabled;
    private Springy.ForceDirectedGraph forceDirectedGraph;
    private float fps, avgDeltaTime, timeElapsed;
    private int frameCount;
    private List<GameObject> nodes, edges;
    private bool textRenderingEnabled, edgeRenderingEnabled;

    private void Awake()
    {
        UnityEngine.Random.seed = 1337;

        nodes = new List<GameObject>();
        edges = new List<GameObject>();
        forceDirectedGraph = new Springy.ForceDirectedGraph();

        debugModeEnabled = true;
        textRenderingEnabled = true;
        edgeRenderingEnabled = true;

        crosshairTexture = (Texture2D)Resources.Load("Crosshair");
        crosshairPosition = new Rect((Screen.width - crosshairTexture.width) / 2, (Screen.height - crosshairTexture.height) / 2, crosshairTexture.width, crosshairTexture.height);
    }

    private void FixedUpdate()
    {
        forceDirectedGraph.tick(Time.fixedDeltaTime);
    }

    private void OnGUI()
    {
        if (debugModeEnabled)
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

            // draw crosshair
            GUI.DrawTexture(crosshairPosition, crosshairTexture);
        }
    }

    private void Start()
    {
        StartNodesAndEdges();
        StartForceDirectedGraph();

        // temp - disable text rendering on start
        foreach (var text in GameObject.FindGameObjectsWithTag("Text"))
        {
            text.GetComponent<Renderer>().enabled = !text.GetComponent<Renderer>().enabled;
        }
    }

    private void StartForceDirectedGraph()
    {
        foreach (var node in nodes)
        {
            node.transform.parent = gameObject.transform;
            node.GetComponent<Node>().springyNode = forceDirectedGraph.newNode();
        }

        foreach (var edge in edges)
        {
            edge.transform.parent = gameObject.transform;
            forceDirectedGraph.newEdge(
                edge.GetComponent<Edge>().source.GetComponent<Node>().id,
                edge.GetComponent<Edge>().target.GetComponent<Node>().id,
                edge.GetComponent<Edge>().length
            );

            edge.GetComponent<Edge>().source.GetComponent<Node>().connectedTo.Add(edge.GetComponent<Edge>().target);
            edge.GetComponent<Edge>().target.GetComponent<Node>().connectedTo.Add(edge.GetComponent<Edge>().source);
        }

        // set node size based on the number of connections
        int min = 1, max = 1;
        foreach (var node in nodes)
        {
            int curr = node.GetComponent<Node>().connectedTo.Count;
            if (curr < min)
                min = curr;
            if (curr > max)
                max = curr;
        }
        foreach (var node in nodes)
        {
            node.transform.localScale += node.transform.localScale * (node.GetComponent<Node>().connectedTo.Count - min) / (max - min);
        }

        forceDirectedGraph.running = true;
    }

    private void StartNodesAndEdges()
    {
        // parse JSON graph
        var jsonRoot = JsonLoader.Deserialize("Examples/miserables.json");

        // create nodes
        var nodePrefab = Resources.Load("Node");
        foreach (var jsonNode in jsonRoot.nodes)
        {
            var node = (GameObject)Instantiate(nodePrefab);
            node.name = String.Format("Node-{0}", jsonNode.name);
            node.GetComponent<Node>().Text = jsonNode.name;
            node.GetComponent<Node>().Group = jsonNode.group;
            nodes.Add(node);
        }
        for (int i = 0; i < nodes.Count; ++i)
        {
            var node = nodes[i];
            node.GetComponent<Node>().id = i;
        }

        // create edges
        var edgePrefab = Resources.Load("Edge");
        foreach (var jsonEdge in jsonRoot.links)
        {
            var edge = (GameObject)UnityEngine.Object.Instantiate(edgePrefab);
            var sourceNode = nodes[jsonEdge.source];
            var targetNode = nodes[jsonEdge.target];
            edge.name = String.Format("Edge-{0}-{1}", sourceNode.name, targetNode.name);
            edge.GetComponent<Edge>().source = sourceNode;
            edge.GetComponent<Edge>().target = targetNode;
            edge.GetComponent<Edge>().length = jsonEdge.value;
            edges.Add(edge);
        }
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

        // show text of nodes pointed by the camera
        RaycastHit hit;
        if (Physics.SphereCast(Camera.main.transform.position, 0.4f, Camera.main.transform.forward, out hit))
        {
            var gameObject = hit.transform.gameObject;
            if (gameObject.tag == "Node")
                gameObject.GetComponent<Node>().Render();
        }

        // enable/disable debug menu
        if (Input.GetKeyDown(KeyCode.Space))
            debugModeEnabled = !debugModeEnabled;
        if (debugModeEnabled)
            UpdateDebug();
    }

    private void UpdateDebug()
    {
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
}