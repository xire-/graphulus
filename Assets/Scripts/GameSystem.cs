using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour {

    [HideInInspector]
    public Graph graph;

    private static GameSystem _instance;
    private Settings _settings = new Settings();

    public static GameSystem Instance {
        get { return _instance; }
    }

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
            graph.EdgesActive = value;
            _settings.edgesActive = value;
        }
    }

    public bool TextsActive {
        get { return _settings.textsActive; }
        set {
            graph.TextsActive = value;
            _settings.textsActive = value;
        }
    }

    public Theme Theme {
        get { return Settings.themes[_settings.themeIndex]; }
    }

    public void Animate(Animation animation) {
        StartCoroutine("AnimateCoroutine", animation);
    }

    public void AnimateConditional(AnimationConditional animation) {
        StartCoroutine("AnimateConditionalCoroutine", animation);
    }

    public void ChangeRotationSpeed() {
        var value = GameObject.Find("SliderRotation").GetComponent<Slider>().value; // TODO
        AutoRotationSpeed = value;
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

    public void ToggleAutoRotation() {
        AutoRotationEnabled = !AutoRotationEnabled;
        GameObject.Find("SliderRotation").GetComponent<Slider>().interactable = _settings.autoRotationEnabled; // TODO
    }

    public void ToggleEdgesActive() {
        EdgesActive = !EdgesActive;
    }

    public void ToggleTextsActive() {
        TextsActive = !TextsActive;
    }

    private IEnumerator AnimateConditionalCoroutine(AnimationConditional animation) {
        if (animation.OnStart != null) {
            animation.OnStart();
        }

        while (true) {
            if (!animation.Update(Time.deltaTime)) {
                break;
            }
            yield return null;
        }

        if (animation.OnEnd != null) {
            animation.OnEnd();
        }
    }

    private IEnumerator AnimateCoroutine(Animation animation) {
        if (animation.OnStart != null) {
            animation.OnStart();
        }

        float startTime = Time.realtimeSinceStartup;
        float endTime = startTime + animation.duration;

        while (Time.realtimeSinceStartup < endTime) {
            float t = (Time.realtimeSinceStartup - startTime) / animation.duration;
            if (animation.Ease != null) {
                t = animation.Ease(t);
            }
            if (animation.Update != null) {
                animation.Update(t);
            }
            yield return null;
        }

        if (animation.Update != null) {
            animation.Update(1f);
        }

        if (animation.OnEnd != null) {
            animation.OnEnd();
        }
    }

    private void Awake() {
        _instance = this;

        UnityEngine.Random.seed = 1337;
    }

    private void ChangeThemeAnim(Theme startTheme, Theme endTheme) {
        Animate(new Animation {
            Update = t => {
                Camera.main.backgroundColor = Color.Lerp(startTheme.skyboxColor, endTheme.skyboxColor, t);
                graph.NodesColor = Color.Lerp(startTheme.nodeColor, endTheme.nodeColor, t);
                graph.TextsColor = Color.Lerp(startTheme.textColor, endTheme.textColor, t);
                graph.EdgesColor = Color.Lerp(startTheme.edgeColor, endTheme.edgeColor, t);
            },
            duration = 1.5f,
            Ease = Easing.EaseOutCubic
        });
    }

    private void OnDestroy() {
        _instance = null;
    }

    private void Start() {
        var graphObject = new GameObject("Graph");
        graph = graphObject.AddComponent<Graph>();
        graph.PopulateFrom(string.Format("Assets/Graphs/{0}.json", _settings.graph));

        // animate the transition from editor colors to the default theme
        var currentTheme = new Theme {
            skyboxColor = Camera.main.backgroundColor,
            nodeColor = graph.NodesColor,
            textColor = graph.TextsColor,
            edgeColor = graph.EdgesColor
        };
        var newTheme = Theme;
        ChangeThemeAnim(currentTheme, newTheme);
    }

    private void Update() {
        // continuously rotate graph
        if (_settings.autoRotationEnabled) {
            graph.transform.Rotate(Vector3.up, Time.deltaTime * _settings.autoRotationSpeed);
        }
    }

    private class Settings {

        public static readonly Theme[] themes = new Theme[] {
            new Theme {
                name = "Dark",
                skyboxColor = new Color32(0x10, 0x0F, 0x0F, 0xFF),
                nodeColor = new Color32(0x17, 0xE2, 0xDA, 0xA1),
                nodeSelectedColor = new Color32(0xD7, 0x8F, 0x32, 0xD9),
                textColor = new Color32(0xE3, 0xE3, 0xE3, 0xFF),
                edgeColor = new Color32(0xF3, 0xF3, 0xF3, 0x64)
            },
            new Theme {
                name = "Light",
                skyboxColor = new Color32(0x02, 0x44, 0x5F, 0xFF),
                nodeColor = new Color32(0x10, 0xAA, 0x51, 0xD2),
                nodeSelectedColor = new Color32(0xE4, 0xE6, 0x2D, 0xD4),
                textColor = new Color32(0x9E, 0xCC, 0xC7, 0xFF),
                edgeColor = new Color32(0xD9, 0x68, 0x3E, 0xC6)
            }
        };

        public bool autoRotationEnabled = false;
        public float autoRotationSpeed = 15f;
        public bool edgesActive = true;
        public string graph = "Miserables";
        public bool textsActive = true;
        public int themeIndex = 0;
    }
}
