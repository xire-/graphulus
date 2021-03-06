﻿using System.Collections;
using UnityEngine;

public class GameSystem : MonoBehaviour {

    // to be set in editor
    public GameObject graphObject;

    private static GameSystem _instance;

    private Coroutine _changeThemeCoroutine;
    private Settings _settings = new Settings();

    public static GameSystem Instance { get { return _instance; } }

    public bool AutoRotationEnabled {
        get { return _settings.autoRotationEnabled; }
        set { _settings.autoRotationEnabled = value; }
    }

    public float AutoRotationSpeed {
        get { return _settings.autoRotationSpeed; }
        set { _settings.autoRotationSpeed = value; }
    }

    public bool EdgesActive {
        get { return _settings.edgesActive; }
        set {
            Graph.EdgesActive = value;
            _settings.edgesActive = value;
        }
    }

    public Graph Graph { get { return graphObject.GetComponent<Graph>(); } }

    public bool LabelsActive {
        get { return _settings.labelsActive; }
        set {
            Graph.LabelsActive = value;
            _settings.labelsActive = value;
        }
    }

    public Theme Theme {
        get { return Settings.themes[_settings.themeIndex]; }
    }

    public Coroutine Execute(Job job, float duration = float.PositiveInfinity) {
        return StartCoroutine(ExecuteCoroutine(job, duration));
    }

    public void ResetAndLoadGraph(string graphName) {
        _settings.graphName = graphName;

        Random.seed = 1337;
        Graph.PopulateFrom(string.Format("Assets/Graphs/{0}.json", _settings.graphName));

        AutoRotationEnabled = _settings.autoRotationEnabled;
        AutoRotationSpeed = _settings.autoRotationSpeed;
        EdgesActive = _settings.edgesActive;
        LabelsActive = _settings.labelsActive;

        ChangeThemeAnim(Theme);
    }

    public void SwitchTheme() {
        // set the new theme
        int newThemeIndex = (_settings.themeIndex + 1) % Settings.themes.Length;
        _settings.themeIndex = newThemeIndex;

        // animate transition to the new theme
        var newTheme = Settings.themes[newThemeIndex];
        ChangeThemeAnim(newTheme);
    }

    private void Awake() {
        _instance = this;
    }

    private void ChangeThemeAnim(Theme endTheme) {
        if (_changeThemeCoroutine != null) {
            StopCoroutine(_changeThemeCoroutine);
        }

        var startTheme = new Theme {
            skyboxColor = Camera.main.backgroundColor,
            nodeColor = Graph.NodesColor,
            labelColor = Graph.LabelsColor,
            edgeColor = Graph.EdgesColor
        };

        _changeThemeCoroutine = Execute(new Job {
            Update = (deltaTime, t) => {
                t = Easing.EaseOutCubic(t);

                Camera.main.backgroundColor = Color.Lerp(startTheme.skyboxColor, endTheme.skyboxColor, t);
                Graph.NodesColor = Color.Lerp(startTheme.nodeColor, endTheme.nodeColor, t);
                Graph.LabelsColor = Color.Lerp(startTheme.labelColor, endTheme.labelColor, t);
                Graph.EdgesColor = Color.Lerp(startTheme.edgeColor, endTheme.edgeColor, t);
                return true;
            },
        }, 1.5f);
    }

    private IEnumerator ExecuteCoroutine(Job job, float duration) {
        if (job.OnStart != null) {
            job.OnStart();
        }

        float startTime = Time.realtimeSinceStartup;
        float endTime = startTime + duration;

        do {
            yield return null;

            float t = Time.realtimeSinceStartup > endTime ? 1f : (Time.realtimeSinceStartup - startTime) / duration;
            if (!job.Update(Time.deltaTime, t)) {
                break;
            }
        } while (Time.realtimeSinceStartup <= endTime);

        if (job.OnEnd != null) {
            job.OnEnd();
        }
    }

    private void Start() {
        ResetAndLoadGraph(_settings.graphName);
    }

    private void Update() {
        if (AutoRotationEnabled) {
            Graph.transform.RotateAround(Graph.transform.position, Vector3.up, Time.deltaTime * AutoRotationSpeed);
        }
    }
}
