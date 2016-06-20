using System;
using System.Collections;
using UnityEngine;

public class World : MonoBehaviour
{
    // to be set in editor
    public GameObject graphObject;

    private Settings _settings = new Settings();

    private Theme Theme
    {
        get
        {
            return new Theme
            {
                skyboxColor = Camera.main.backgroundColor,
                nodeColor = graphObject.GetComponent<Graph>().NodesColor,
                textColor = graphObject.GetComponent<Graph>().TextsColor,
                edgeColor = graphObject.GetComponent<Graph>().EdgesColor
            };
        }
    }

    public void StartAnimation(Animation animation)
    {
        StartCoroutine(Animate(animation));
    }

    private IEnumerator Animate(Animation animation)
    {
        if (animation.OnStart != null)
            animation.OnStart();

        float startTime = Time.realtimeSinceStartup;
        float endTime = startTime + animation.duration;

        while (Time.realtimeSinceStartup < endTime)
        {
            float t = (Time.realtimeSinceStartup - startTime) / animation.duration;
            if (animation.Ease != null)
                t = animation.Ease(t);
            animation.Update(t);
            yield return null;
        }

        animation.Update(1f);

        if (animation.OnEnd != null)
            animation.OnEnd();
    }

    private void Awake()
    {
        UnityEngine.Random.seed = 1337;
    }

    private void ChangeTheme(Theme newTheme)
    {
        var startTheme = Theme;
        StartAnimation(new Animation
        {
            Update = t =>
            {
                Camera.main.backgroundColor = Color.Lerp(startTheme.skyboxColor, newTheme.skyboxColor, t);
                graphObject.GetComponent<Graph>().NodesColor = Color.Lerp(startTheme.nodeColor, newTheme.nodeColor, t);
                graphObject.GetComponent<Graph>().TextsColor = Color.Lerp(startTheme.textColor, newTheme.textColor, t);
                graphObject.GetComponent<Graph>().EdgesColor = Color.Lerp(startTheme.edgeColor, newTheme.edgeColor, t);
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
            String.Format("Total energy: {0:f} [{1:f}]\n", graphObject.GetComponent<Graph>().TotalKineticEnergy, graphObject.GetComponent<Graph>().EnergyThreshold) +
            "\n" +
            String.Format("Text rendering: {0}\n", _settings.textsActive ? "ON" : "OFF") +
            String.Format("Edge rendering: {0}\n", _settings.edgesActive ? "ON" : "OFF") +
            String.Format("_rotationSpeed: {0:f}\n", _settings.rotationSpeed);
        GUI.TextArea(new Rect(Screen.width - 250 - 10, 10, 250, Screen.height - 20), text);
    }

    private void Start()
    {
        graphObject.GetComponent<Graph>().PopulateFrom("Assets/Graphs/miserables.json");

        ChangeTheme(_settings.darkTheme);
    }

    private void Update()
    {
        // continuously rotate graph
        if (_settings.rotationEnabled)
            graphObject.transform.Rotate(Vector3.up, Time.deltaTime * _settings.rotationSpeed);

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
            if (_settings.textsActive)
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
                lookedNode.GetComponent<Node>().Selected = true;
        }

        // set themes
        if (Input.GetKeyUp(KeyCode.L))
            ChangeTheme(_settings.lightTheme);
        if (Input.GetKeyUp(KeyCode.K))
            ChangeTheme(_settings.darkTheme);

        // toggle edges active
        if (Input.GetKeyDown(KeyCode.E))
        {
            _settings.edgesActive = !_settings.edgesActive;
            graphObject.GetComponent<Graph>().EdgesActive = _settings.edgesActive;
        }

        // toggle texts active
        if (Input.GetKeyDown(KeyCode.T))
        {
            _settings.textsActive = !_settings.textsActive;
            graphObject.GetComponent<Graph>().TextsActive = _settings.textsActive;
        }

        // adjust rotation velocity
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (_settings.rotationSpeed < _settings.rotationSpeedMax)
                _settings.rotationSpeed += 10f;
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            if (_settings.rotationSpeed > -_settings.rotationSpeedMax)
                _settings.rotationSpeed -= 10f;
        }
    }

    private class Settings
    {
        public readonly Theme darkTheme = new Theme()
        {
            skyboxColor = new Color32(0x10, 0x0F, 0x0F, 0xFF),
            nodeColor = new Color32(0x17, 0xE2, 0xDA, 0xA1),
            textColor = new Color32(0xE3, 0xE3, 0xE3, 0xFF),
            edgeColor = new Color32(0xF3, 0xF3, 0xF3, 0x64)
        };

        public readonly Theme lightTheme = new Theme()
        {
            skyboxColor = new Color32(0x02, 0x44, 0x5F, 0xFF),
            nodeColor = new Color32(0x10, 0xAA, 0x51, 0xD2),
            textColor = new Color32(0x9E, 0xCC, 0xC7, 0xFF),
            edgeColor = new Color32(0xD9, 0x68, 0x3E, 0xC6)
        };

        public readonly float rotationSpeedMax = 100f;
        public bool edgesActive = true;
        public bool rotationEnabled = true;
        public float rotationSpeed = 10f;
        public bool textsActive = true;
    }
}