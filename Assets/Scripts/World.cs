using System;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public AnimationManager animationManager;

    private readonly Theme darkTheme = new Theme()
    {
        skyboxColor = new Color32(0x10, 0x0F, 0x0F, 0xFF),
        nodeColor = new Color32(0x17, 0xE2, 0xDA, 0xA1),
        textColor = new Color32(0xE3, 0xE3, 0xE3, 0xFF),
        edgeColor = new Color32(0xF3, 0xF3, 0xF3, 0x64)
    };

    private readonly Theme lightTheme = new Theme()
    {
        skyboxColor = new Color32(0xF3, 0xF3, 0xF3, 0xFF),
        nodeColor = new Color32(0xEA, 0x24, 0x7A, 0xA1),
        textColor = new Color32(0x85, 0x0F, 0x26, 0xFF),
        edgeColor = new Color32(0x9F, 0x9D, 0x9D, 0x64)
    };

    private GameObject graph;
    private bool textRenderingEnabled, edgeRenderingEnabled;

    private Theme CurrentTheme
    {
        get
        {
            return new Theme
            {
                skyboxColor = Camera.main.backgroundColor,
                nodeColor = graph.GetComponent<Graph>().nodes[0].GetComponent<Renderer>().material.color,
                textColor = graph.GetComponent<Graph>().texts[0].GetComponent<Renderer>().material.color,
                edgeColor = graph.GetComponent<Graph>().edges[0].GetComponent<Renderer>().material.color
            };
        }
    }

    private void AdjustNodes(Dictionary<GameObject, int> connectionsCount)
    {
        foreach (var node in graph.GetComponent<Graph>().nodes)
            node.transform.localScale *= 1.5f - Mathf.Pow(1.2f, -connectionsCount[node]);
    }

    private void Awake()
    {
        UnityEngine.Random.seed = 1337;

        animationManager = new AnimationManager();

        textRenderingEnabled = true;
        edgeRenderingEnabled = true;
    }

    private void ChangeTheme(Theme newTheme)
    {
        var startTheme = CurrentTheme;
        animationManager.Add(new Animation
        {
            Update = t =>
            {
                Camera.main.backgroundColor = Color.Lerp(startTheme.skyboxColor, newTheme.skyboxColor, t);
                foreach (var node in graph.GetComponent<Graph>().nodes)
                    node.GetComponent<Renderer>().material.color = Color.Lerp(startTheme.nodeColor, newTheme.nodeColor, t);
                foreach (var text in graph.GetComponent<Graph>().texts)
                    text.GetComponent<TextMesh>().color = Color.Lerp(startTheme.textColor, newTheme.textColor, t);
                foreach (var edge in graph.GetComponent<Graph>().edges)
                    edge.GetComponent<Renderer>().material.color = Color.Lerp(startTheme.edgeColor, newTheme.edgeColor, t);
            },
            duration = 1.5f,
            Ease = Easing.EaseOutCubic
        });
    }

    private GameObject CreateGraphFrom(string path)
    {
        GameObject graph = (GameObject)Instantiate(Resources.Load("Graph"));
        graph.name = "Graph";
        graph.transform.parent = transform;
        graph.GetComponent<Graph>().PopulateFrom(path);
        return graph;
    }

    private void OnGUI()
    {
        // draw debug menu
        var text =
            String.Format("FPS: {0:f} [{1:f}ms]\n", (int)(1.0f / Time.smoothDeltaTime), Time.smoothDeltaTime * 1000f) +
            "\n" +
            String.Format("Total energy: {0:f} [{1:f}]\n", graph.GetComponent<Graph>().forceDirectedGraph.totalKineticEnergy(), graph.GetComponent<Graph>().forceDirectedGraph.minEnergyThreshold) +
            "\n" +
            String.Format("Text rendering: {0}\n", textRenderingEnabled ? "ON" : "OFF") +
            String.Format("Edge rendering: {0}\n", edgeRenderingEnabled ? "ON" : "OFF");
        GUI.TextArea(new Rect(Screen.width - 250 - 10, 10, 250, Screen.height - 20), text);
    }

    private void SetEdgesActive(bool active)
    {
        foreach (var edge in graph.GetComponent<Graph>().edges)
            edge.SetActive(active);
        edgeRenderingEnabled = active;
    }

    private void SetTextsActive(bool active)
    {
        foreach (var text in graph.GetComponent<Graph>().texts)
            text.SetActive(active);
        textRenderingEnabled = active;
    }

    private void Start()
    {
        graph = CreateGraphFrom("Examples/miserables.json");

        // count the number of connections
        var connectionsCount = new Dictionary<GameObject, int>();
        foreach (var node in graph.GetComponent<Graph>().nodes)
            connectionsCount[node] = 0;
        foreach (var edge in graph.GetComponent<Graph>().edges)
        {
            connectionsCount[edge.GetComponent<Edge>().source]++;
            connectionsCount[edge.GetComponent<Edge>().target]++;
        }

        AdjustNodes(connectionsCount);

        ChangeTheme(darkTheme);
    }

    private void Update()
    {
        animationManager.Update();

        graph.transform.Rotate(Vector3.up, -Time.deltaTime * 5);

        // check if a node is pointed by the camera
        GameObject lookedNode = null;
        RaycastHit hit;
        if (Physics.SphereCast(Camera.main.transform.position, 0.4f, Camera.main.transform.forward, out hit))
        {
            var gameObject = hit.transform.gameObject;
            if (gameObject.tag == "Node")
                lookedNode = gameObject;
        }

        if (lookedNode != null)
        {
            // render the text of the looked at node
            if (textRenderingEnabled)
                lookedNode.GetComponent<Node>().RenderText();
        }

        // parse input from keyboard
        UpdateInput(lookedNode);
    }

    private void UpdateInput(GameObject lookedNode)
    {
        if (lookedNode != null)
        {
            // selection
            if (Input.GetKeyUp(KeyCode.X))
                lookedNode.GetComponent<Node>().Select();
        }

        // set themes
        if (Input.GetKeyUp(KeyCode.L))
            ChangeTheme(lightTheme);
        if (Input.GetKeyUp(KeyCode.K))
            ChangeTheme(darkTheme);

        // enable/disable text rendering
        if (Input.GetKeyDown(KeyCode.N))
        {
            SetTextsActive(!textRenderingEnabled);
        }

        // enable/disable edge rendering
        if (Input.GetKeyDown(KeyCode.M))
        {
            SetEdgesActive(!edgeRenderingEnabled);
        }
    }
}