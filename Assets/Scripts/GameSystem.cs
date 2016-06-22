using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour
{
    // to be set in editor
    public GameObject graphObject;

    private static GameSystem _instance;
    private Dictionary<KeyCode, Action> _keyToActionMap = new Dictionary<KeyCode, Action>();
    private Settings _settings = new Settings();

    public static GameSystem Instance {
        get { return _instance; }
    }

    public bool EdgesActive {
        get { return _settings.edgesActive; }
        set {
            _settings.edgesActive = value;
            graphObject.GetComponent<Graph>().EdgesActive = _settings.edgesActive;
        }
    }

    public bool TextsActive {
        get { return _settings.textsActive; }
        set {
            _settings.textsActive = value;
            graphObject.GetComponent<Graph>().TextsActive = _settings.textsActive;
        }
    }

    private Theme Theme {
        get {
            return new Theme {
                skyboxColor = Camera.main.backgroundColor,
                nodeColor = graphObject.GetComponent<Graph>().NodesColor,
                textColor = graphObject.GetComponent<Graph>().TextsColor,
                edgeColor = graphObject.GetComponent<Graph>().EdgesColor
            };
        }
    }

    public void Animate(Animation animation) {
        StartCoroutine("AnimateCoroutine", animation);
    }

    public void ChangeRotationSpeed() {
        var value = GameObject.Find("SliderRotation").GetComponent<Slider>().value;
        _settings.autoRotationSpeed = value;
    }

    public void ToggleAutoRotation() {
        _settings.autoRotationEnabled = !_settings.autoRotationEnabled;
        GameObject.Find("SliderRotation").GetComponent<Slider>().interactable = _settings.autoRotationEnabled;
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

    private void ChangeTheme(Theme newTheme) {
        var startTheme = Theme;
        Animate(new Animation {
            Update = t => {
                Camera.main.backgroundColor = Color.Lerp(startTheme.skyboxColor, newTheme.skyboxColor, t);
                graphObject.GetComponent<Graph>().NodesColor = Color.Lerp(startTheme.nodeColor, newTheme.nodeColor, t);
                graphObject.GetComponent<Graph>().TextsColor = Color.Lerp(startTheme.textColor, newTheme.textColor, t);
                graphObject.GetComponent<Graph>().EdgesColor = Color.Lerp(startTheme.edgeColor, newTheme.edgeColor, t);
            },
            duration = 1.5f,
            Ease = Easing.EaseOutCubic
        });
    }

    private void OnDestroy() {
        _instance = null;
    }

    private void OnGUI() {
        // draw debug menu
        var text =
            String.Format("FPS: {0:f} [{1:f}ms]\n", (int)(1.0f / Time.smoothDeltaTime), Time.smoothDeltaTime * 1000f) +
            "\n" +
            String.Format("Total energy: {0:f} [{1:f}]\n", graphObject.GetComponent<Graph>().TotalKineticEnergy, graphObject.GetComponent<Graph>().EnergyThreshold) +
            "\n" +
            String.Format("Text rendering: {0}\n", _settings.textsActive ? "ON" : "OFF") +
            String.Format("Edge rendering: {0}\n", _settings.edgesActive ? "ON" : "OFF") +
            String.Format("_rotationSpeed: {0:f}\n", _settings.autoRotationSpeed);
        GUI.TextArea(new Rect(Screen.width - 250 - 10, 10, 250, Screen.height - 20), text);
    }

    private void SetupKeymap() {
        // switch themes
        _keyToActionMap.Add(KeyCode.L, () => ChangeTheme(_settings.lightTheme));
        _keyToActionMap.Add(KeyCode.K, () => ChangeTheme(_settings.darkTheme));

        // toggle edges active
        _keyToActionMap.Add(KeyCode.E, () => {
            _settings.edgesActive = !_settings.edgesActive;
            graphObject.GetComponent<Graph>().EdgesActive = _settings.edgesActive;
        });

        // toggle texts active
        _keyToActionMap.Add(KeyCode.T, () => {
            _settings.textsActive = !_settings.textsActive;
            graphObject.GetComponent<Graph>().TextsActive = _settings.textsActive;
        });

        // adjust rotation velocity
        _keyToActionMap.Add(KeyCode.B, () => {
            if (_settings.autoRotationSpeed < _settings.autoRotationSpeedMax) {
                _settings.autoRotationSpeed += 10f;
            }
        });
        _keyToActionMap.Add(KeyCode.V, () => {
            if (_settings.autoRotationSpeed > -_settings.autoRotationSpeedMax) {
                _settings.autoRotationSpeed -= 10f;
            }
        });
    }

    private void Start() {
        SetupKeymap();

        graphObject.GetComponent<Graph>().PopulateFrom("Assets/Graphs/miserables.json");

        ChangeTheme(_settings.darkTheme);
    }

    private void Update() {
        // continuously rotate graph
        if (_settings.autoRotationEnabled) {
            graphObject.transform.Rotate(Vector3.up, Time.deltaTime * _settings.autoRotationSpeed);
        }

        // update input
        foreach (KeyCode keyCode in _keyToActionMap.Keys) {
            if (Input.GetKeyDown(keyCode)) {
                _keyToActionMap[keyCode]();
            }
        }
    }

    private class Settings
    {
        public readonly float autoRotationSpeedMax = 100f;

        public readonly Theme darkTheme = new Theme() {
            skyboxColor = new Color32(0x10, 0x0F, 0x0F, 0xFF),
            nodeColor = new Color32(0x17, 0xE2, 0xDA, 0xA1),
            textColor = new Color32(0xE3, 0xE3, 0xE3, 0xFF),
            edgeColor = new Color32(0xF3, 0xF3, 0xF3, 0x64)
        };

        public readonly Theme lightTheme = new Theme() {
            skyboxColor = new Color32(0x02, 0x44, 0x5F, 0xFF),
            nodeColor = new Color32(0x10, 0xAA, 0x51, 0xD2),
            textColor = new Color32(0x9E, 0xCC, 0xC7, 0xFF),
            edgeColor = new Color32(0xD9, 0x68, 0x3E, 0xC6)
        };

        public bool autoRotationEnabled = false;
        public float autoRotationSpeed = 10f;
        public bool edgesActive = true;
        public bool textsActive = true;
    }
}