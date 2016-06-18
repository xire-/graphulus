using System;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public AnimationManager animationManager;
    private const float _rotationSpeedMax = 100f;

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

    private bool _rotationEnabled = true;
    private float _rotationSpeed = 10f;
    private bool _textsActive = true, _edgesActive = true;
    private GameObject graph;

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

        _textsActive = true;
        _edgesActive = true;
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

    private void OnGUI()
    {
        // draw debug menu
        var text =
            String.Format("FPS: {0:f} [{1:f}ms]\n", (int)(1.0f / Time.smoothDeltaTime), Time.smoothDeltaTime * 1000f) +
            "\n" +
            String.Format("Total energy: {0:f} [{1:f}]\n", graph.GetComponent<Graph>().forceDirectedGraph.TotalKineticEnergy(), graph.GetComponent<Graph>().forceDirectedGraph.EnergyThreshold) +
            "\n" +
            String.Format("Text rendering: {0}\n", _textsActive ? "ON" : "OFF") +
            String.Format("Edge rendering: {0}\n", _edgesActive ? "ON" : "OFF") +
            String.Format("_rotationSpeed: {0:f}\n", _rotationSpeed);
        GUI.TextArea(new Rect(Screen.width - 250 - 10, 10, 250, Screen.height - 20), text);
    }

    private void Start()
    {
        graph = GameObject.Find("Graph");
        graph.GetComponent<Graph>().PopulateFrom("Assets/Graphs/miserables.json");

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

        if (_rotationEnabled)
            graph.transform.Rotate(Vector3.up, Time.deltaTime * _rotationSpeed);

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
            if (_textsActive)
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

        // toggle edges active
        if (Input.GetKeyDown(KeyCode.E))
        {
            _edgesActive = !_edgesActive;
            graph.GetComponent<Graph>().EdgesActive = _edgesActive;
        }

        // toggle texts active
        if (Input.GetKeyDown(KeyCode.T))
        {
            _textsActive = !_textsActive;
            graph.GetComponent<Graph>().TextsActive = _textsActive;
        }

        // adjust rotation velocity
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (_rotationSpeed < _rotationSpeedMax)
                _rotationSpeed += 10f;
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            if (_rotationSpeed > -_rotationSpeedMax)
                _rotationSpeed -= 10f;
        }
    }
}