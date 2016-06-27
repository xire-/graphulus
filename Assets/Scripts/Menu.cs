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
        GameSystem.Instance.Graph.Scale = Mathf.Exp(_panelTransform.Find("SliderGraphScale").GetComponent<Slider>().value);
    }

    public void ChangeLabelsActive() {
        GameSystem.Instance.LabelsActive = _panelTransform.Find("ToggleLabelsActive").GetComponent<Toggle>().isOn;
    }

    public void RecenterGraph() {
        var pinchController = GameSystem.Instance.Graph.GetComponent<GraphPinchController>().pinchControllerObject;
        var pinchControllerInitialPosition = pinchController.transform.position;
        var pinchControllerInitialScale = pinchController.transform.localScale;

        var graphInitialPosition = GameSystem.Instance.Graph.transform.position;
        var graphInitialScale = GameSystem.Instance.Graph.transform.localScale;
        var graphInitialRotation = GameSystem.Instance.Graph.transform.rotation;

        GameSystem.Instance.Execute(new Job {
            Update = (_, t) => {
                t = Easing.EaseOutQuart(t);

                pinchController.transform.position = Vector3.Lerp(pinchControllerInitialPosition, Vector3.zero, t);
                pinchController.transform.localScale = Vector3.Lerp(pinchControllerInitialScale, Vector3.one, t);

                GameSystem.Instance.Graph.transform.position = Vector3.Lerp(graphInitialPosition, Vector3.zero, t);
                GameSystem.Instance.Graph.transform.localScale = Vector3.Lerp(graphInitialScale, Vector3.one, t);
                GameSystem.Instance.Graph.transform.rotation = Quaternion.Lerp(graphInitialRotation, Quaternion.Euler(0f, 0f, 0f), t);
                return true;
            }
        }, 2f);
    }

    public void SwitchGraph() {
        var options = _panelTransform.Find("DropdownSwitchGraph").GetComponent<Dropdown>().options;
        var graphName = options[_panelTransform.Find("DropdownSwitchGraph").GetComponent<Dropdown>().value].text;
        GameSystem.Instance.ResetAndLoadGraph(graphName);

        Reset();
    }

    public void SwitchTheme() {
        GameSystem.Instance.SwitchTheme();
    }

    private void Reset() {
        _panelTransform = transform.Find("Panel");

        _panelTransform.Find("ToggleEdgesActive").GetComponent<Toggle>().isOn = GameSystem.Instance.EdgesActive;

        _panelTransform.Find("ToggleLabelsActive").GetComponent<Toggle>().isOn = GameSystem.Instance.LabelsActive;

        _panelTransform.Find("ToggleAutoRotationEnabled").GetComponent<Toggle>().isOn = GameSystem.Instance.AutoRotationEnabled;

        _panelTransform.Find("SliderAutoRotationSpeed").GetComponent<Slider>().interactable = GameSystem.Instance.AutoRotationEnabled;
        _panelTransform.Find("SliderAutoRotationSpeed").GetComponent<Slider>().value = GameSystem.Instance.AutoRotationSpeed;

        _panelTransform.Find("SliderGraphScale").GetComponent<Slider>().value = Mathf.Log(GameSystem.Instance.Graph.Scale);
    }

    private void Start() {
        Reset();
    }

    private void Update() {
        var angle = Camera.main.transform.eulerAngles.x;
        if (angle >= 30f && angle <= 90f) {
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

            GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0, 1, (angle - 30f) / (5f));
        }
        else {
            if (_panelTransform.gameObject.activeSelf) {
                _panelTransform.gameObject.SetActive(false);
            }
        }
    }
}
