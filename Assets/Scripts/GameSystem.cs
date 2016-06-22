using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour
{
    // to be set in editor
    public GameObject graphObject;

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
            graphObject.GetComponent<Graph>().EdgesActive = value;
            _settings.edgesActive = value;
        }
    }

    public bool TextsActive {
        get { return _settings.textsActive; }
        set {
            graphObject.GetComponent<Graph>().TextsActive = value;
            _settings.textsActive = value;
        }
    }

    public Theme Theme {
        get { return Settings.themes[_settings.themeIndex]; }
    }

    public void Animate(Animation animation) {
        StartCoroutine("AnimateCoroutine", animation);
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
                graphObject.GetComponent<Graph>().NodesColor = Color.Lerp(startTheme.nodeColor, endTheme.nodeColor, t);
                graphObject.GetComponent<Graph>().TextsColor = Color.Lerp(startTheme.textColor, endTheme.textColor, t);
                graphObject.GetComponent<Graph>().EdgesColor = Color.Lerp(startTheme.edgeColor, endTheme.edgeColor, t);
            },
            duration = 1.5f,
            Ease = Easing.EaseOutCubic
        });
    }

    private void OnDestroy() {
        _instance = null;
    }

    private void Start() {
        graphObject.GetComponent<Graph>().PopulateFrom("Assets/Graphs/miserables.json");

        // animate the transition from editor colors to the default theme
        var currentTheme = new Theme {
            skyboxColor = Camera.main.backgroundColor,
            nodeColor = graphObject.GetComponent<Graph>().NodesColor,
            textColor = graphObject.GetComponent<Graph>().TextsColor,
            edgeColor = graphObject.GetComponent<Graph>().EdgesColor
        };
        var newTheme = Theme;
        ChangeThemeAnim(currentTheme, newTheme);
    }

    private void Update() {
        // continuously rotate graph
        if (_settings.autoRotationEnabled) {
            graphObject.transform.Rotate(Vector3.up, Time.deltaTime * _settings.autoRotationSpeed);
        }
    }

    private class Settings
    {
        public static readonly Theme[] themes = new Theme[] {
            new Theme {
                name = "Dark",
                skyboxColor = new Color32(0x10, 0x0F, 0x0F, 0xFF),
                nodeColor = new Color32(0x17, 0xE2, 0xDA, 0xA1),
                textColor = new Color32(0xE3, 0xE3, 0xE3, 0xFF),
                edgeColor = new Color32(0xF3, 0xF3, 0xF3, 0x64)
            },
            new Theme {
                name = "Light",
                skyboxColor = new Color32(0x02, 0x44, 0x5F, 0xFF),
                nodeColor = new Color32(0x10, 0xAA, 0x51, 0xD2),
                textColor = new Color32(0x9E, 0xCC, 0xC7, 0xFF),
                edgeColor = new Color32(0xD9, 0x68, 0x3E, 0xC6)
            }
        };

        public bool autoRotationEnabled = false;
        public float autoRotationSpeed = 15f;
        public bool edgesActive = true;
        public bool textsActive = true;
        public int themeIndex = 0;
    }
}
