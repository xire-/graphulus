using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
    private Transform _panelTransform;

    public void ChangeAutoRotationEnabled() {
        GameSystem.Instance.AutoRotationEnabled = _panelTransform.Find("ToggleAutoRotationEnabled").GetComponent<Toggle>().isOn;
        _panelTransform.Find("SliderAutoRotationSpeed").GetComponent<Slider>().interactable = GameSystem.Instance.AutoRotationEnabled;
    }

    public void ChangeAutoRotationSpeed() {
        GameSystem.Instance.AutoRotationSpeed = _panelTransform.Find("SliderAutoRotationSpeed").GetComponent<Slider>().value;
    }

    public void ChangeEdgesActive() {
        GameSystem.Instance.EdgesActive = _panelTransform.Find("ToggleEdgesActive").GetComponent<Toggle>().isOn;
    }

    public void ChangeGraphScale() {
        GameSystem.Instance.graph.Scale = _panelTransform.Find("SliderGraphScale").GetComponent<Slider>().value;
    }

    public void ChangeTextsActive() {
        GameSystem.Instance.TextsActive = _panelTransform.Find("ToggleTextsActive").GetComponent<Toggle>().isOn;
    }

    public void ResetGraphTransform() {
        var pinchController = GameObject.Find("PinchController");
        if (pinchController != null) {
            var pinchControllerInitialPosition = pinchController.transform.position;
            var pinchControllerInitialScale = pinchController.transform.localScale;
            var graphInitialPosition = GameSystem.Instance.graph.transform.position;
            var graphInitialScale = GameSystem.Instance.graph.transform.localScale;
            var graphInitialRotation = GameSystem.Instance.graph.transform.rotation;

            GameSystem.Instance.Animate(new Animation {
                Update = t => {
                    pinchController.transform.position = Vector3.Lerp(pinchControllerInitialPosition, Vector3.zero, t);
                    pinchController.transform.localScale = Vector3.Lerp(pinchControllerInitialScale, Vector3.one, t);

                    GameSystem.Instance.graph.transform.position = Vector3.Lerp(graphInitialPosition, Vector3.zero, t);
                    GameSystem.Instance.graph.transform.localScale = Vector3.Lerp(graphInitialScale, Vector3.one, t);
                    GameSystem.Instance.graph.transform.rotation = Quaternion.Lerp(graphInitialRotation, Quaternion.Euler(0f, 0f, 0f), t);
                },
                duration = 2f,
                Ease = Easing.EaseOutQuart,
            });
        }
        else {
            var graphInitialPosition = GameSystem.Instance.graph.transform.position;
            var graphInitialScale = GameSystem.Instance.graph.transform.localScale;
            var graphInitialRotation = GameSystem.Instance.graph.transform.rotation;

            GameSystem.Instance.Animate(new Animation {
                Update = t => {
                    GameSystem.Instance.graph.transform.position = Vector3.Lerp(graphInitialPosition, Vector3.zero, t);
                    GameSystem.Instance.graph.transform.localScale = Vector3.Lerp(graphInitialScale, Vector3.one, t);
                    GameSystem.Instance.graph.transform.rotation = Quaternion.Lerp(graphInitialRotation, Quaternion.Euler(0f, 0f, 0f), t);
                },
                duration = 2f,
                Ease = Easing.EaseOutQuart,
            });
        }
    }

    public void SwitchGraph(System.Int32 asd) {
        var options = _panelTransform.Find("DropdownSwitchGraph").GetComponent<Dropdown>().options;
        var graphName = options[_panelTransform.Find("DropdownSwitchGraph").GetComponent<Dropdown>().value].text;

        GameSystem.Instance.ResetGraph(graphName);
    }

    public void SwitchTheme() {
        GameSystem.Instance.SwitchTheme();
    }

    private void Start() {
        _panelTransform = transform.Find("Panel");

        _panelTransform.Find("ToggleEdgesActive").GetComponent<Toggle>().isOn = GameSystem.Instance.EdgesActive;

        _panelTransform.Find("ToggleTextsActive").GetComponent<Toggle>().isOn = GameSystem.Instance.TextsActive;

        _panelTransform.Find("ToggleAutoRotationEnabled").GetComponent<Toggle>().isOn = GameSystem.Instance.AutoRotationEnabled;

        _panelTransform.Find("SliderAutoRotationSpeed").GetComponent<Slider>().interactable = GameSystem.Instance.AutoRotationEnabled;
        _panelTransform.Find("SliderAutoRotationSpeed").GetComponent<Slider>().value = GameSystem.Instance.AutoRotationSpeed;

        _panelTransform.Find("SliderGraphScale").GetComponent<Slider>().value = GameSystem.Instance.graph.Scale;
    }

    private void Update() {
        var angle = Camera.main.transform.eulerAngles.x;
        if (angle >= 25 && angle <= 90) {
            if (!_panelTransform.gameObject.activeSelf) {
                _panelTransform.gameObject.SetActive(true);

                // move the menu in front of the camera
                var vec = Camera.main.transform.forward;
                vec.y = 0;
                vec.Normalize();
                vec.y = -1f;
                transform.position = Camera.main.transform.position + vec;
                transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward);
            }

            GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0, 1, (angle - 25f) / (5f));
        }
        else {
            if (_panelTransform.gameObject.activeSelf) {
                _panelTransform.gameObject.SetActive(false);
            }
        }
    }
}
