using System.Collections;
using UnityEngine;

public class GameSystem : MonoBehaviour {

    // to be set in editor
    public GameObject graphObject;

    private static GameSystem _instance;

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

    public void Execute(Job job, float duration = 0f) {
        StartCoroutine(ExecuteCoroutine(job, duration));
    }

    public void ResetAndLoadGraph(string graphName) {
        Random.seed = 1337;

        Graph.PopulateFrom(string.Format("Assets/Graphs/{0}.json", graphName));

        var currentTheme = new Theme {
            skyboxColor = Camera.main.backgroundColor,
            nodeColor = Graph.NodesColor,
            labelColor = Graph.LabelsColor,
            edgeColor = Graph.EdgesColor
        };
        var newTheme = Theme;
        ChangeThemeAnim(currentTheme, newTheme);
    }

    public void SwitchTheme() {
        var currentTheme = Theme;

        // set the new theme
        int newThemeIndex = (_settings.themeIndex + 1) % Settings.themes.Length;
        _settings.themeIndex = newThemeIndex;

        // animate transition to the new theme
        var newTheme = Settings.themes[newThemeIndex];
        ChangeThemeAnim(currentTheme, newTheme);
    }

    private void Awake() {
        _instance = this;
    }

    private void ChangeThemeAnim(Theme startTheme, Theme endTheme) {
        Execute(new Job {
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
        float endTime = startTime + (duration > 0f ? duration : float.PositiveInfinity);

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
        ResetAndLoadGraph(_settings.graph);
    }

    private void Update() {
        if (AutoRotationEnabled) {
            Graph.transform.RotateAround(Graph.transform.position, Vector3.up, Time.deltaTime * AutoRotationSpeed);
        }
    }

    private class Settings {

        public static readonly Theme[] themes = new Theme[] {
            new Theme {
                name = "Dark",
                skyboxColor = new Color32(0x10, 0x0F, 0x0F, 0xFF),
                nodeColor = new Color32(0x17, 0xE2, 0xDA, 0xA1),
                nodeSelectedColor = new Color32(0xD7, 0x8F, 0x32, 0xD9),
                labelColor = new Color32(0xE3, 0xE3, 0xE3, 0xFF),
                edgeColor = new Color32(0xF3, 0xF3, 0xF3, 0x64)
            },
            new Theme {
                name = "Light",
                skyboxColor = new Color32(0x02, 0x44, 0x5F, 0xFF),
                nodeColor = new Color32(0x10, 0xAA, 0x51, 0xD2),
                nodeSelectedColor = new Color32(0xE4, 0xE6, 0x2D, 0xD4),
                labelColor = new Color32(0x9E, 0xCC, 0xC7, 0xFF),
                edgeColor = new Color32(0xD9, 0x68, 0x3E, 0xC6)
            }
        };

        public bool autoRotationEnabled = false;
        public float autoRotationSpeed = 15f;
        public bool edgesActive = true;
        public string graph = "Miserables";
        public bool labelsActive = true;
        public int themeIndex = 0;
    }
}
